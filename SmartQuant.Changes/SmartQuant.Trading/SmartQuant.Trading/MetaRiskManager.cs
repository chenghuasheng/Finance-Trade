using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{521B9C4F-01AE-4488-B4A5-104027D06BB8}", ComponentType.MetaRiskManager, Name = "Default_MetaRiskManager", Description = "")]
	public class MetaRiskManager : MetaStrategyComponent
	{
		public const string GUID = "{521B9C4F-01AE-4488-B4A5-104027D06BB8}";

		public virtual double GetPortfolioRisk()
		{
			return 1.0;
		}

		public virtual void AddStop(Strategy strategy, double level, StopType type, StopMode mode, bool stopStrategy)
		{
			new PortfolioStop(strategy, level, type, mode, stopStrategy);
		}

		public virtual void AddStop(Strategy strategy, DateTime time, bool stopStrategy)
		{
			new PortfolioStop(strategy, time, stopStrategy);
		}

		public virtual void AddStop(Strategy strategy, double level, StopType type, StopMode mode)
		{
			new PortfolioStop(strategy, level, type, mode, true);
		}

		public virtual void AddStop(Strategy strategy, DateTime time)
		{
			new PortfolioStop(strategy, time, true);
		}

		public virtual void OnStrategyPortfolioValueChanged(Strategy strategy)
		{
		}

		public virtual void OnMetaStrategyStarted()
		{
		}
	}
}
