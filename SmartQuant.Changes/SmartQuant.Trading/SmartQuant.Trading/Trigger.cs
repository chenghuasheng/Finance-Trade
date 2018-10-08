using SmartQuant.Charting;
using SmartQuant.Series;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace SmartQuant.Trading
{
	public class Trigger : IDrawable, IZoomable
	{
		private Strategy fStrategy;

		private TriggerType fType;

		private TriggerStatus fStatus;

		private DoubleSeries fSeries;

		private double fLevel;

		private Signal fSignal;

		private DateTime fCreationTime;

		private DateTime fCompletionTime;

		private Color activeColor = Color.Brown;

		private Color executedColor = Color.Green;

		private Color canceledColor = Color.DarkGray;

		private bool textEnabled = true;

		private bool toolTipEnabled = true;

		private string toolTipFormat = "{0} {1} {2}{3}{4} {5} {6} {7}";

		public TriggerType Type
		{
			get
			{
				return this.fType;
			}
		}

		public TriggerStatus Status
		{
			get
			{
				return this.fStatus;
			}
		}

		public DoubleSeries Series
		{
			get
			{
				return this.fSeries;
			}
		}

		public double Level
		{
			get
			{
				return this.fLevel;
			}
		}

		public Signal Signal
		{
			get
			{
				return this.fSignal;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.fCreationTime;
			}
		}

		public DateTime CompletionTime
		{
			get
			{
				return this.fCompletionTime;
			}
		}

		[Category("Drawing Style")]
		public Color ExecutedColor
		{
			get
			{
				return this.executedColor;
			}
			set
			{
				this.executedColor = value;
			}
		}

		[Category("Drawing Style")]
		public Color ActiveColor
		{
			get
			{
				return this.activeColor;
			}
			set
			{
				this.activeColor = value;
			}
		}

		[Category("Drawing Style")]
		public Color CanceledColor
		{
			get
			{
				return this.canceledColor;
			}
			set
			{
				this.canceledColor = value;
			}
		}

		[Category("Drawing Style")]
		public bool TextEnabled
		{
			get
			{
				return this.textEnabled;
			}
			set
			{
				this.textEnabled = value;
			}
		}

		[Category("ToolTip"), Description("Enable or disable tooltip appearance for this marker.")]
		public bool ToolTipEnabled
		{
			get
			{
				return this.toolTipEnabled;
			}
			set
			{
				this.toolTipEnabled = value;
			}
		}

		[Category("ToolTip"), Description("Tooltip format string. {1} - X coordinate, {2} - Y coordinte.")]
		public string ToolTipFormat
		{
			get
			{
				return this.toolTipFormat;
			}
			set
			{
				this.toolTipFormat = value;
			}
		}

		public Trigger(Strategy strategy, TriggerType type, DoubleSeries series, double level, Signal signal)
		{
			this.fStrategy = strategy;
			this.fType = type;
			this.fSeries = series;
			this.fLevel = level;
			this.fSignal = signal;
			this.fStatus = TriggerStatus.Active;
			this.fCreationTime = Clock.Now;
			this.fCompletionTime = DateTime.MinValue;
			this.fStrategy.AddTrigger(this);
			this.Connect();
		}

		internal void Disconnect()
		{
			this.fSeries.ItemAdded -= new ItemAddedEventHandler(this.OnItemAdded);
		}

		private void Connect()
		{
			this.fSeries.ItemAdded += new ItemAddedEventHandler(this.OnItemAdded);
		}

		private void OnItemAdded(object sender, DateTimeEventArgs args)
		{
			DateTime dateTime = args.DateTime;
			switch (this.fType)
			{
			case TriggerType.Above:
				if (this.fSeries[dateTime] > this.fLevel)
				{
					this.Disconnect();
					this.fSignal = new Signal(Clock.Now, this.fSignal.Sender, this.fSignal.Type, this.fSignal.Side, this.fSignal.Qty, this.fSignal.Price, this.fSignal.Instrument, this.fSignal.Text);
					this.fStrategy.EmitSignal(this.fSignal);
					this.Complete(TriggerStatus.Executed);
					return;
				}
				break;
			case TriggerType.Below:
				if (this.fSeries[dateTime] < this.fLevel)
				{
					this.Disconnect();
					this.fSignal = new Signal(Clock.Now, this.fSignal.Sender, this.fSignal.Type, this.fSignal.Side, this.fSignal.Qty, this.fSignal.Price, this.fSignal.Instrument, this.fSignal.Text);
					this.fStrategy.EmitSignal(this.fSignal);
					this.Complete(TriggerStatus.Executed);
				}
				break;
			default:
				return;
			}
		}

		private void Complete(TriggerStatus status)
		{
			this.fStatus = status;
			this.fCompletionTime = Clock.Now;
			this.fStrategy.EmitTriggerStatusChanged(this);
		}

		public void Paint(Pad Pad, double MinX, double MaxX, double MinY, double MaxY)
		{
			double worldY = this.fLevel;
			int num = Pad.ClientX((double)this.CreationTime.Ticks);
			int num2 = Pad.ClientY(worldY);
			string text = string.Concat(new string[]
			{
				"Trigger - ",
				worldY.ToString("F4"),
				" (",
				this.Status.ToString(),
				")"
			});
			Font font = new Font("Arial", 8f);
			Color color = this.canceledColor;
			switch (this.Status)
			{
			case TriggerStatus.Active:
				color = this.activeColor;
				break;
			case TriggerStatus.Executed:
				color = this.executedColor;
				break;
			}
			Pen pen = new Pen(color, 1f);
			pen.DashStyle = DashStyle.DashDot;
			double val = (double)Pad.ClientX((double)Clock.Now.Ticks);
			double val2 = (double)Pad.ClientX(MinX);
			double val3 = (double)Pad.ClientX(MaxX);
			if (this.Status != TriggerStatus.Active)
			{
				val = (double)Pad.ClientX((double)this.CompletionTime.Ticks);
			}
			float num3 = (float)Math.Max(val2, (double)num);
			float num4 = (float)Math.Min(val3, val);
			if (num3 <= num4)
			{
				Pad.Graphics.DrawLine(pen, num3, (float)num2, num4, (float)num2);
				if (this.textEnabled)
				{
					double num5 = (double)(num + 2);
					double num6;
					if (this.fType == TriggerType.Above)
					{
						num6 = (double)(num2 - 2 - (int)Pad.Graphics.MeasureString(text, font).Height);
					}
					else
					{
						num6 = (double)(num2 + 2);
					}
					Pad.Graphics.DrawString(text, font, Brushes.Black, (float)num5, (float)num6);
				}
			}
		}

		public TDistance Distance(double X, double Y)
		{
			TDistance tDistance = new TDistance();
			double y = this.fLevel;
			tDistance.X = X;
			tDistance.Y = y;
			if (X >= (double)this.CreationTime.Ticks && ((this.Status == TriggerStatus.Active && X <= (double)Clock.Now.Ticks) || X <= (double)this.CompletionTime.Ticks))
			{
				tDistance.dX = 0.0;
			}
			else
			{
				tDistance.dX = 1.7976931348623157E+308;
			}
			tDistance.dY = Math.Abs(Y - tDistance.Y);
			StringBuilder stringBuilder = new StringBuilder();
			if (this.CreationTime.Second != 0 || this.CreationTime.Minute != 0 || this.CreationTime.Hour != 0)
			{
				stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
					this.fStatus.ToString(),
					this.fType.ToString(),
					" Trigger (",
					this.fLevel.ToString(this.Signal.Instrument.PriceDisplay),
					"): ",
					this.fSeries.Name,
					y.ToString(this.Signal.Instrument.PriceDisplay),
					this.CreationTime
				});
			}
			else
			{
				stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
					this.fStatus.ToString(),
					this.fType.ToString(),
					" Trigger (",
					this.fLevel.ToString(this.Signal.Instrument.PriceDisplay),
					"): ",
					this.fSeries.Name,
					y.ToString(this.Signal.Instrument.PriceDisplay),
					this.CreationTime.ToShortDateString()
				});
			}
			tDistance.ToolTipText = stringBuilder.ToString();
			return tDistance;
		}

		public void Draw()
		{
			Chart.Pad.Add(this);
		}

		public bool IsPadRangeY()
		{
			return true;
		}

		public PadRange GetPadRangeY(Pad pad)
		{
			DateTime t = new DateTime((long)pad.XMin);
			DateTime t2 = new DateTime((long)pad.XMax);
			if (t <= this.CompletionTime && t2 >= this.CreationTime)
			{
				double num = this.fLevel;
				return new PadRange(num - 4.94065645841247E-324, num + 4.94065645841247E-324);
			}
			return new PadRange(0.0, 0.0);
		}

		public bool IsPadRangeX()
		{
			return false;
		}

		public PadRange GetPadRangeX(Pad pad)
		{
			return null;
		}
	}
}
