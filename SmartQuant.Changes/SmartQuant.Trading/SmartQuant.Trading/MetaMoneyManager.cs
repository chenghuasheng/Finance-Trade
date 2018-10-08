using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{FED5076A-C710-4d3a-B134-3D9D32B8B248}", ComponentType.MetaMoneyManager, Name = "Default_MetaMoneyManager", Description = "")]
	public class MetaMoneyManager : MetaStrategyBaseComponent
	{
		public const string GUID = "{FED5076A-C710-4d3a-B134-3D9D32B8B248}";

		public virtual void Allocate()
		{
			foreach (StrategyBase strategy in base.MetaStrategyBase.Strategies)
			{
				this.Deposit(strategy, base.MetaStrategyBase.SimulationManager.Cash / (double)base.MetaStrategyBase.Strategies.Count, Clock.Now, base.MetaStrategyBase.SimulationManager.Currency, "Initial Cash Allocation");
			}
		}

		public virtual void Deposit(StrategyBase strategy, double amount, DateTime datetime, Currency currency, string comment)
		{
			strategy.Deposit(amount, currency, datetime, comment);
		}

		public virtual void Deposit(StrategyBase strategy, double amount, Currency currency, string comment)
		{
			this.Deposit(strategy, amount, Clock.Now, currency, comment);
		}

		public virtual void Deposit(string strategyName, double amount, Currency currency, string comment)
		{
			this.Deposit(base.MetaStrategyBase.Strategies[strategyName], amount, currency, comment);
		}

		public virtual void Deposit(string strategyName, double amount, DateTime datetime, Currency currency, string comment)
		{
			this.Deposit(base.MetaStrategyBase.Strategies[strategyName], amount, datetime, currency, comment);
		}

		public virtual void Withdraw(StrategyBase strategy, double amount, DateTime datetime, Currency currency, string comment)
		{
			strategy.Withdraw(amount, currency, datetime, comment);
		}

		public virtual void Withdraw(StrategyBase strategy, double amount, Currency currency, string comment)
		{
			this.Withdraw(strategy, amount, Clock.Now, currency, comment);
		}

		public virtual void Withdraw(string strategyName, double amount, Currency currency, string comment)
		{
			this.Withdraw(base.MetaStrategyBase.Strategies[strategyName], amount, currency, comment);
		}

		public virtual void Withdraw(string strategyName, double amount, DateTime datetime, Currency currency, string comment)
		{
			this.Withdraw(base.MetaStrategyBase.Strategies[strategyName], amount, datetime, currency, comment);
		}
	}
}
