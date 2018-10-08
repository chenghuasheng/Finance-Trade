using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Series;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartQuant.Trading
{
	public class ATSStop : StopBase
	{
		private bool connected;

		private DoubleSeries series;

        public event StopEventHandler StatusChanged;

		[Browsable(false)]
		public DoubleSeries Series
		{
			get
			{
				return this.series;
			}
		}

		[Browsable(false)]
		public PositionSide Side
		{
			get
			{
				return this.fSide;
			}
		}

		[Browsable(false)]
		public override Instrument Instrument
		{
			get
			{
				return this.fInstrument;
			}
		}

		[Browsable(false)]
		public double FillPrice
		{
			get
			{
				return this.fFillPrice;
			}
		}

		[Browsable(false)]
		public double StopPrice
		{
			get
			{
				return this.fStopPrice;
			}
		}

		[Browsable(false)]
		public bool Connected
		{
			get
			{
				return this.connected;
			}
		}

		public StopFillMode FillMode
		{
			get
			{
				return this.fFillMode;
			}
			set
			{
				this.fFillMode = value;
			}
		}

		public ATSStop(Position position, double level, StopType type, StopMode mode)
		{
			this.fPosition = position;
			this.fInstrument = position.Instrument;
			this.fQty = position.Qty;
			this.fSide = position.Side;
			this.fLevel = level;
			this.fType = type;
			this.fMode = mode;
			this.fCurrPrice = this.fInstrument.Price();
			this.fTrailPrice = this.fCurrPrice;
			this.fStopPrice = this.GetStopPrice();
			this.fCreationTime = Clock.Now;
			this.fCompletionTime = DateTime.MinValue;
			this.series = new DoubleSeries();
			if (this.fType == StopType.Trailing)
			{
				this.series.Add(this.fCreationTime, this.GetStopPrice());
			}
			this.Connect();
		}

		public ATSStop(Position position, DateTime time)
		{
			this.fPosition = position;
			this.fInstrument = position.Instrument;
			this.fQty = position.Qty;
			this.fSide = position.Side;
			this.fType = StopType.Time;
			this.fCreationTime = Clock.Now;
			this.fCompletionTime = time;
			this.fStopPrice = this.fInstrument.Price();
			this.series = new DoubleSeries();
			if (this.fType == StopType.Trailing)
			{
				this.series.Add(this.fCreationTime, this.GetStopPrice());
			}
			if (this.fCompletionTime > this.fCreationTime)
			{
				Clock.AddReminder(new ReminderEventHandler(this.OnClock), this.fCompletionTime, null);
			}
		}

		public void Cancel()
		{
			if (this.fStatus != StopStatus.Active)
			{
				return;
			}
			if (this.fType == StopType.Time)
			{
				Clock.RemoveReminder(new ReminderEventHandler(this.OnClock));
			}
			else
			{
				this.Disconnect();
			}
			this.Complete(StopStatus.Canceled);
		}

		private double GetStopPrice()
		{
			this.fInitPrice = this.fTrailPrice;
			switch (this.fMode)
			{
			case StopMode.Absolute:
				switch (this.fSide)
				{
				case PositionSide.Long:
					return this.fTrailPrice - Math.Abs(this.fLevel);
				case PositionSide.Short:
					return this.fTrailPrice + Math.Abs(this.fLevel);
				default:
					throw new ArgumentException("Unknown position side : " + this.fPosition.Side);
				}
				break;
			case StopMode.Percent:
				switch (this.fPosition.Side)
				{
				case PositionSide.Long:
					return this.fTrailPrice - Math.Abs(this.fTrailPrice * this.fLevel);
				case PositionSide.Short:
					return this.fTrailPrice + Math.Abs(this.fTrailPrice * this.fLevel);
				default:
					throw new ArgumentException("Unknown position side : " + this.fPosition.Side);
				}
				break;
			default:
				throw new ArgumentException("Unknown stop mode : " + this.fMode);
			}
		}

		private void Connect()
		{
			this.connected = true;
		}

		public override void Disconnect()
		{
			if (base.Type == StopType.Time)
			{
				Clock.RemoveReminder(new ReminderEventHandler(this.OnClock), this.fCompletionTime);
				return;
			}
			this.connected = false;
		}

		private void CheckStop()
		{
			if (this.fCurrPrice == 0.0)
			{
				return;
			}
			lock (this)
			{
				switch (this.fSide)
				{
				case PositionSide.Long:
					if (this.fCurrPrice <= this.fStopPrice)
					{
						this.Disconnect();
						this.Complete(StopStatus.Executed);
					}
					else if (this.fType == StopType.Trailing && this.fTrailPrice > this.fInitPrice)
					{
						this.fStopPrice = this.GetStopPrice();
					}
					break;
				case PositionSide.Short:
					if (this.fCurrPrice >= this.fStopPrice)
					{
						this.Disconnect();
						this.Complete(StopStatus.Executed);
					}
					else if (this.fType == StopType.Trailing && this.fTrailPrice < this.fInitPrice)
					{
						this.fStopPrice = this.GetStopPrice();
					}
					break;
				}
			}
		}

		public void OnPositionClosed(Position position)
		{
			if (position == this.fPosition)
			{
				this.Disconnect();
				this.Complete(StopStatus.Canceled);
			}
		}

		public void OnNewBar(Bar bar)
		{
			if (this.fTraceOnBar && (this.fFilterBarSize < 0L || (this.fFilterBarSize == bar.Size && this.fFilterBarType == BarType.Time)))
			{
				this.fTrailPrice = bar.Close;
				switch (this.fSide)
				{
				case PositionSide.Long:
					this.fCurrPrice = bar.Low;
					this.fFillPrice = bar.Low;
					if (this.fTrailOnHighLow)
					{
						this.fTrailPrice = bar.High;
					}
					break;
				case PositionSide.Short:
					this.fCurrPrice = bar.High;
					this.fFillPrice = bar.High;
					if (this.fTrailOnHighLow)
					{
						this.fTrailPrice = bar.Low;
					}
					break;
				}
				switch (this.fFillMode)
				{
				case StopFillMode.Close:
					this.fFillPrice = bar.Close;
					break;
				case StopFillMode.Stop:
					this.fFillPrice = this.fStopPrice;
					break;
				}
				this.CheckStop();
			}
			if (this.fType == StopType.Trailing)
			{
				this.series.Add(bar.DateTime, this.fStopPrice);
			}
		}

		public void OnNewBarOpen(Bar bar)
		{
			if (this.fTraceOnBar && this.fTraceOnBarOpen && (this.fFilterBarSize < 0L || (this.fFilterBarSize == bar.Size && this.fFilterBarType == BarType.Time)))
			{
				this.fCurrPrice = bar.Open;
				this.fFillPrice = bar.Open;
				if (this.fTrailOnOpen)
				{
					this.fTrailPrice = bar.Open;
				}
				this.CheckStop();
			}
		}

		public void OnNewTrade(Trade trade)
		{
			if (this.fTraceOnTrade)
			{
				this.fCurrPrice = trade.Price;
				this.fFillPrice = trade.Price;
				this.fTrailPrice = trade.Price;
				this.CheckStop();
			}
		}

		public void OnNewQuote(Quote quote)
		{
			if (this.fTraceOnQuote)
			{
				switch (this.fSide)
				{
				case PositionSide.Long:
					this.fCurrPrice = quote.Ask;
					break;
				case PositionSide.Short:
					this.fCurrPrice = quote.Bid;
					break;
				}
				this.fFillPrice = this.fCurrPrice;
				this.fTrailPrice = this.fCurrPrice;
				this.CheckStop();
			}
		}

		private void Complete(StopStatus status)
		{
			this.fStatus = status;
			this.fCompletionTime = Clock.Now;
			if (this.fType == StopType.Trailing)
			{
				this.series.Add(this.fCompletionTime, this.fStopPrice);
			}
			this.EmitStatusChanged();
		}

		private void OnClock(ReminderEventArgs args)
		{
			this.fStopPrice = this.fInstrument.Price();
			this.Complete(StopStatus.Executed);
		}

		private void EmitStatusChanged()
		{
			if (this.StatusChanged != null)
			{
				this.StatusChanged(new StopEventArgs(this));
			}
		}
	}
}
