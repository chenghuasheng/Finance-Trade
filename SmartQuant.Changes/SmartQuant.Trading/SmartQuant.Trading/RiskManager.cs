using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{BE0176A8-3BBD-407c-814A-D5A3E3437899}", ComponentType.RiskManager, Name = "Default_RiskManager", Description = "")]
	public class RiskManager : StrategySingleComponent
	{
		public const string GUID = "{BE0176A8-3BBD-407c-814A-D5A3E3437899}";

		public virtual double GetPositionRisk()
		{
			return 1.0;
		}

		public virtual bool Validate(Signal signal)
		{
			return true;
		}

		public virtual Stop AddStop(Position position, double level, StopType type, StopMode mode)
		{
			if (!base.Strategy.IsInstrumentActive(position.Instrument))
			{
				return null;
			}
			return new Stop(base.Strategy, position, level, type, mode);
		}

		public virtual Stop AddStop(double level, StopType type, StopMode mode)
		{
			if (!base.Strategy.IsInstrumentActive(base.Position.Instrument))
			{
				return null;
			}
			return new Stop(base.Strategy, base.Position, level, type, mode);
		}

		public virtual Stop AddStop(Position position, DateTime time)
		{
			if (!base.Strategy.IsInstrumentActive(position.Instrument))
			{
				return null;
			}
			return new Stop(base.Strategy, position, time);
		}

		public virtual Stop AddStop(DateTime time)
		{
			if (!base.Strategy.IsInstrumentActive(base.Position.Instrument))
			{
				return null;
			}
			return new Stop(base.Strategy, base.Position, time);
		}
	}
}
