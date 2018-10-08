using SmartQuant.Charting;
using SmartQuant.Instruments;
using System;
using System.Collections;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class StrategyBaseMultiComponent : MultiInstrumentComponent, IStrategyComponent
	{
		private StrategyBase strategyBase;

		[Browsable(false)]
		public StrategyBase StrategyBase
		{
			get
			{
				return this.strategyBase;
			}
			internal set
			{
				if (this.strategyBase != null)
				{
					this.Disconnect();
				}
				this.strategyBase = value;
				if (this.strategyBase != null)
				{
					this.Connect();
				}
			}
		}

		[Browsable(false)]
		public Portfolio Portfolio
		{
			get
			{
				return this.strategyBase.Portfolio;
			}
		}

		[Browsable(false)]
		public BarSeriesList Bars
		{
			get
			{
				return this.StrategyBase.Bars;
			}
		}

		[Browsable(false)]
		public Hashtable Global
		{
			get
			{
				return this.strategyBase.Global;
			}
		}

		public virtual void OnPortfolioValueChanged(Position position)
		{
		}

		public virtual void OnBarSlice(long size)
		{
		}

		public void Draw(Instrument instrument, IDrawable primitive, int padNumber)
		{
			this.strategyBase.MetaStrategyBase.DrawPrimitive(instrument, primitive, padNumber);
		}
	}
}
