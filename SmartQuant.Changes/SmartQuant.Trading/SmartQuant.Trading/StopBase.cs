using SmartQuant.Charting;
using SmartQuant.Data;
using SmartQuant.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace SmartQuant.Trading
{
	public abstract class StopBase : IStop, IDrawable, IZoomable
	{
		protected internal StopType fType = StopType.Trailing;

		protected internal StopMode fMode = StopMode.Percent;

		protected internal StopStatus fStatus;

		protected internal Position fPosition;

		protected internal Instrument fInstrument;

		protected internal double fLevel;

		protected internal double fInitPrice;

		protected internal double fCurrPrice;

		protected internal double fStopPrice;

		protected internal double fFillPrice;

		protected internal double fTrailPrice;

		protected internal double fQty;

		protected internal PositionSide fSide;

		protected internal DateTime fCreationTime;

		protected internal DateTime fCompletionTime;

		protected internal bool fTraceOnQuote = true;

		protected internal bool fTraceOnTrade = true;

		protected internal bool fTraceOnBar = true;

		protected internal bool fTraceOnBarOpen = true;

		protected internal bool fTrailOnOpen = true;

		protected internal bool fTrailOnHighLow;

		protected internal long fFilterBarSize = -1L;

		protected internal BarType fFilterBarType = BarType.Time;

		protected internal StopFillMode fFillMode = StopFillMode.Stop;

		protected internal bool textEnabled = true;

		protected internal bool toolTipEnabled = true;

		protected internal string toolTipFormat = "{0} {1} {2}{3}{4} {5} {6} {7}";

		protected internal Color activeColor = Color.Purple;

		protected internal Color executedColor = Color.RoyalBlue;

		protected internal Color canceledColor = Color.Gray;

		public abstract Instrument Instrument
		{
			get;
		}

		public bool TraceOnBar
		{
			get
			{
				return this.fTraceOnBar;
			}
			set
			{
				this.fTraceOnBar = value;
			}
		}

		public bool TraceOnBarOpen
		{
			get
			{
				return this.fTraceOnBarOpen;
			}
			set
			{
				this.fTraceOnBarOpen = value;
			}
		}

		public bool TraceOnTrade
		{
			get
			{
				return this.fTraceOnTrade;
			}
			set
			{
				this.fTraceOnTrade = value;
			}
		}

		public bool TraceOnQuote
		{
			get
			{
				return this.fTraceOnQuote;
			}
			set
			{
				this.fTraceOnQuote = value;
			}
		}

		public bool TrailOnOpen
		{
			get
			{
				return this.fTrailOnOpen;
			}
			set
			{
				this.fTrailOnOpen = value;
			}
		}

		public bool TrailOnHighLow
		{
			get
			{
				return this.fTrailOnHighLow;
			}
			set
			{
				this.fTrailOnHighLow = value;
			}
		}

		public double Level
		{
			get
			{
				return this.fLevel;
			}
			set
			{
				this.fLevel = value;
			}
		}

		public StopType Type
		{
			get
			{
				return this.fType;
			}
		}

		public StopMode Mode
		{
			get
			{
				return this.fMode;
			}
		}

		public StopStatus Status
		{
			get
			{
				return this.fStatus;
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

		public Position Position
		{
			get
			{
				return this.fPosition;
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

		public StopBase()
		{
		}

		public abstract void Disconnect();

		public void SetBarFilter(long barSize, BarType barType)
		{
			this.fFilterBarSize = barSize;
			this.fFilterBarType = barType;
		}

		public void SetBarFilter(long barSize)
		{
			this.SetBarFilter(barSize, BarType.Time);
		}

		public void Paint(Pad Pad, double MinX, double MaxX, double MinY, double MaxY)
		{
			double worldY;
			if (this.fStatus == StopStatus.Executed)
			{
				worldY = Math.Abs(this.fFillPrice);
			}
			else
			{
				worldY = Math.Abs(this.fStopPrice);
			}
			if (this.fType == StopType.Time)
			{
				worldY = this.fStopPrice;
			}
			int num = Pad.ClientX((double)this.fCreationTime.Ticks);
			int num2 = Pad.ClientY(worldY);
			string text = string.Concat(new string[]
			{
				"Stop at ",
				worldY.ToString(this.fInstrument.PriceDisplay),
				" (",
				this.fStatus.ToString(),
				")"
			});
			Font font = new Font("Arial", 8f);
			Color color = this.canceledColor;
			switch (this.fStatus)
			{
			case StopStatus.Active:
				color = this.activeColor;
				break;
			case StopStatus.Executed:
				color = this.executedColor;
				break;
			case StopStatus.Canceled:
				color = this.canceledColor;
				break;
			}
			Pen pen = new Pen(color, 2f);
			pen.DashStyle = DashStyle.Dash;
			double val = (double)Pad.ClientX((double)Clock.Now.Ticks);
			double val2 = (double)Pad.ClientX(MinX);
			double val3 = (double)Pad.ClientX(MaxX);
			if (this.fStatus != StopStatus.Active)
			{
				val = (double)Pad.ClientX((double)this.fCompletionTime.Ticks);
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
					if (this.fSide == PositionSide.Long)
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
			double y = Math.Abs(this.fStopPrice);
			tDistance.X = X;
			tDistance.Y = y;
			if (X >= (double)this.fCreationTime.Ticks && ((this.fStatus == StopStatus.Active && X <= (double)Clock.Now.Ticks) || X <= (double)this.fCompletionTime.Ticks))
			{
				tDistance.dX = 0.0;
			}
			else
			{
				tDistance.dX = 1.7976931348623157E+308;
			}
			tDistance.dY = Math.Abs(Y - tDistance.Y);
			StringBuilder stringBuilder = new StringBuilder();
			if (this.fCreationTime.Second != 0 || this.fCreationTime.Minute != 0 || this.fCreationTime.Hour != 0)
			{
				stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
					this.fStatus.ToString(),
					this.fType.ToString(),
					" Stop (",
					this.fLevel.ToString(this.fPosition.Instrument.PriceDisplay),
					"): ",
					this.fPosition.Instrument.Symbol,
					y.ToString(this.fPosition.Instrument.PriceDisplay),
					this.fCreationTime
				});
			}
			else
			{
				stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
					this.fStatus.ToString(),
					this.fType.ToString(),
					" Stop (",
					this.fLevel.ToString(this.fPosition.Instrument.PriceDisplay),
					"): ",
					this.fPosition.Instrument.Symbol,
					y.ToString(this.fPosition.Instrument.PriceDisplay),
					this.fCreationTime.ToShortDateString()
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
			if (t <= this.fCompletionTime && t2 >= this.fCreationTime && this.fStatus != StopStatus.Canceled)
			{
				double num = Math.Abs(this.fStopPrice);
				return new PadRange(num - 1E-10, num + 1E-10);
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
