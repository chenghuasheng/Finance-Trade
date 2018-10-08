using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{9637DF40-0F84-46e3-AC54-0EC2D2CE2699}", ComponentType.MoneyManager, Name = "Default_MoneyManager", Description = "")]
	public class MoneyManager : StrategySingleComponent
	{
		public const string GUID = "{9637DF40-0F84-46e3-AC54-0EC2D2CE2699}";

		public double GetPositionRisk()
		{
			return base.Strategy.RiskManagers[base.Instrument].GetPositionRisk();
		}

		public virtual double GetPositionSize(Signal signal)
		{
			return 0.0;
		}
	}
}
