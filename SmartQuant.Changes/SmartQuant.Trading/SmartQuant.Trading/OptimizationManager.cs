using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{A4D510F9-13DB-4b4c-9557-BC6A48A25D0B}", ComponentType.OptimizationManager, Name = "Default_OptimizationManager", Description = "")]
	public class OptimizationManager : MetaStrategyBaseComponent
	{
		public const string GUID = "{A4D510F9-13DB-4b4c-9557-BC6A48A25D0B}";

		public virtual double Objective()
		{
			return 0.0;
		}
	}
}
