using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	public interface IStrategyComponent
	{
		StrategyBase StrategyBase
		{
			get;
		}

		Portfolio Portfolio
		{
			get;
		}
	}
}
