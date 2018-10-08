using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{2DBD0B38-8399-4d0b-9FAA-7C29FC1462BC}", ComponentType.MetaExposureManager, Name = "Default_MetaExposureManager", Description = "")]
	public class MetaExposureManager : MetaStrategyComponent
	{
		public const string GUID = "{2DBD0B38-8399-4d0b-9FAA-7C29FC1462BC}";

		public virtual bool Validate(Signal signal)
		{
			return true;
		}
	}
}
