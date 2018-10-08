using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	public interface IMetaStrategyComponent
	{
		MetaStrategyBase MetaStrategyBase
		{
			get;
		}

		Portfolio Portfolio
		{
			get;
		}
	}
}
