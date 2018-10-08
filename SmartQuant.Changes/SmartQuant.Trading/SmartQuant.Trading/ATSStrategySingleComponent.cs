using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class ATSStrategySingleComponent : StrategyBaseSingleComponent
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
