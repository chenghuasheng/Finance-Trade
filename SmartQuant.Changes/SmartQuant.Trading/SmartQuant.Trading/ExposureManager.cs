using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{0449D7E3-2016-47f6-9B80-C787B3E0F18F}", ComponentType.ExposureManager, Name = "Default_ExposureManager", Description = "")]
	public class ExposureManager : StrategyMultiComponent
	{
		public const string GUID = "{0449D7E3-2016-47f6-9B80-C787B3E0F18F}";

		public virtual bool Validate(Signal signal)
		{
			return true;
		}
	}
}
