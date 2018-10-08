using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class StrategyMultiComponent : StrategyBaseMultiComponent
	{
		[Browsable(false)]
		public Strategy Strategy
		{
			get
			{
				return base.StrategyBase as Strategy;
			}
		}
	}
}
