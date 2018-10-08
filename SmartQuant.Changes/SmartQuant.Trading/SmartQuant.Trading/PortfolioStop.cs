using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	public class PortfolioStop : IStop
	{
		private StrategyBase strategy;

		private Portfolio portfolio;

		private StopType type = StopType.Trailing;

		private StopMode mode = StopMode.Percent;

		private StopStatus status;

		private DateTime creationTime;

		private DateTime completionTime;

		private double level;

		private double initValue;

		private double currValue;

		private double stopValue;

		private double fillValue;

		private double trailValue;

		private StopFillMode fillMode = StopFillMode.Stop;

		private bool stopStrategy;

		public StopFillMode FillMode
		{
			get
			{
				return this.fillMode;
			}
			set
			{
				this.fillMode = value;
			}
		}

		public PortfolioStop(StrategyBase strategy, double level, StopType type, StopMode mode, bool stopStrategy)
		{
			this.strategy = strategy;
			this.portfolio = strategy.Portfolio;
			this.level = level;
			this.type = type;
			this.mode = mode;
			this.stopStrategy = stopStrategy;
			this.currValue = this.portfolio.GetValue();
			this.trailValue = this.currValue;
			this.stopValue = this.GetStopValue();
			this.creationTime = Clock.Now;
			this.completionTime = DateTime.MinValue;
			this.Connect();
			this.fillValue = this.currValue;
			this.trailValue = this.currValue;
			this.CheckStop();
		}

		public PortfolioStop(StrategyBase strategy, DateTime time, bool stopStrategy)
		{
			this.strategy = strategy;
			this.portfolio = strategy.Portfolio;
			this.stopStrategy = stopStrategy;
			this.type = StopType.Time;
			this.creationTime = Clock.Now;
			this.completionTime = time;
			this.stopValue = this.portfolio.GetValue();
			if (this.completionTime > this.creationTime)
			{
				Clock.AddReminder(new ReminderEventHandler(this.OnClock), this.completionTime, null);
			}
		}

		private double GetStopValue()
		{
			this.initValue = this.trailValue;
			switch (this.mode)
			{
			case StopMode.Absolute:
				return this.trailValue - Math.Abs(this.level);
			case StopMode.Percent:
				return this.trailValue - Math.Abs(this.trailValue * this.level);
			default:
				throw new ArgumentException("Unknown stop mode : " + this.mode);
			}
		}

		private void Connect()
		{
			this.portfolio.ValueChanged += new PositionEventHandler(this.portfolio_ValueChanged);
		}

		public void Disconnect()
		{
			this.portfolio.ValueChanged -= new PositionEventHandler(this.portfolio_ValueChanged);
		}

		private void portfolio_ValueChanged(object sender, PositionEventArgs args)
		{
			this.currValue = this.portfolio.GetValue();
			this.fillValue = this.portfolio.GetValue();
			this.trailValue = this.portfolio.GetValue();
			this.CheckStop();
		}

		private void CheckStop()
		{
			if (this.currValue == 0.0)
			{
				return;
			}
			lock (this)
			{
				if (this.currValue <= this.stopValue)
				{
					this.strategy.ClosePortfolio();
					if (this.stopStrategy)
					{
						this.strategy.IsActive = false;
					}
					this.Disconnect();
					this.Complete(StopStatus.Executed);
				}
				else if (this.type == StopType.Trailing && this.currValue > this.initValue)
				{
					this.stopValue = this.GetStopValue();
				}
			}
		}

		private void Complete(StopStatus status)
		{
			this.status = status;
			this.completionTime = Clock.Now;
		}

		private void OnClock(ReminderEventArgs args)
		{
			if (args.SignalTime > Clock.Now)
			{
				return;
			}
			this.stopValue = this.portfolio.GetValue();
			this.strategy.ClosePortfolio();
			if (this.stopStrategy)
			{
				this.strategy.IsActive = false;
			}
			this.Complete(StopStatus.Executed);
		}
	}
}
