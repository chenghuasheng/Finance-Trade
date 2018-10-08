using System;

namespace SmartQuant.Trading
{
	public class StrategyEventArgs : EventArgs
	{
		private StrategyBase strategy;

		public StrategyBase Strategy
		{
			get
			{
				return this.strategy;
			}
		}

		public StrategyEventArgs(StrategyBase strategy)
		{
			this.strategy = strategy;
		}
	}
}
