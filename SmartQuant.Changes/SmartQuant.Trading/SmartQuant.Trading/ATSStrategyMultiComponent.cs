using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class ATSStrategyMultiComponent : StrategyBaseMultiComponent
	{
		[Browsable(false)]
		public ATSStrategy Strategy
		{
			get
			{
				return base.StrategyBase as ATSStrategy;
			}
		}
	}
}
