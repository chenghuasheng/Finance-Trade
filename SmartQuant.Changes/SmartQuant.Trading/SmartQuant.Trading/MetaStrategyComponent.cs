using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class MetaStrategyComponent : MetaStrategyBaseComponent
	{
		[Browsable(false)]
		public MetaStrategy MetaStrategy
		{
			get
			{
				return base.MetaStrategyBase as MetaStrategy;
			}
		}
	}
}
