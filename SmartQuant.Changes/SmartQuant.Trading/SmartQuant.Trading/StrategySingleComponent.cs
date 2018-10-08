using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class StrategySingleComponent : StrategyBaseSingleComponent
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
