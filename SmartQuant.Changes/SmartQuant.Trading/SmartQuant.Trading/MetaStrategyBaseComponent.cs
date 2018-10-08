using SmartQuant.Instruments;
using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class MetaStrategyBaseComponent : MultiInstrumentComponent, IMetaStrategyComponent
	{
		private MetaStrategyBase metaStrategyBase;

		[Browsable(false)]
		public MetaStrategyBase MetaStrategyBase
		{
			get
			{
				return this.metaStrategyBase;
			}
			internal set
			{
				if (this.metaStrategyBase != null)
				{
					this.Disconnect();
				}
				this.metaStrategyBase = value;
				if (this.metaStrategyBase != null)
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
				return this.metaStrategyBase.Portfolio;
			}
		}

		public virtual void OnPortfolioValueChanged(Portfolio portfolio)
		{
		}
	}
}
