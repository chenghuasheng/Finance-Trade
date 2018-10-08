using SmartQuant.Instruments;
using SmartQuant.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	[StrategyComponent("{849E4CFE-C19E-4d1e-899D-0BB26DB12AAD}", ComponentType.MarketManager, Name = "Default_MarketManager", Description = "")]
	public class MarketManager : StrategyBaseMultiComponent
	{
		public const string GUID = "{849E4CFE-C19E-4d1e-899D-0BB26DB12AAD}";

		private Dictionary<Instrument, IMarketDataProvider> marketDataProviderTable;

		private Dictionary<Instrument, IExecutionProvider> executionProviderTable;

		private InstrumentList instruments;

		protected IMarketDataProvider strategyMarketDataProvider;

		protected IExecutionProvider strategyExecutionProvider;
        /*----------------------------------------------------------------------------*/
        protected bool marketOpen = true;
        /*---------------------------------------------------------------------------------*/
        public bool MarketOpen
        {
            get { return this.marketOpen; }
        }
        [Browsable(false)]
		public InstrumentList Instruments
		{
			get
			{
				return this.instruments;
			}
		}

		[Browsable(false)]
		internal IMarketDataProvider StrategyMarketDataProvider
		{
			get
			{
				return this.strategyMarketDataProvider;
			}
			set
			{
				this.strategyMarketDataProvider = value;
			}
		}

		[Browsable(false)]
		internal IExecutionProvider StrategyExecutionProvider
		{
			get
			{
				return this.strategyExecutionProvider;
			}
			set
			{
				this.strategyExecutionProvider = value;
			}
		}

		[Browsable(false)]
		internal Dictionary<Instrument, IMarketDataProvider> MarketDataProviderTable
		{
			get
			{
				return this.marketDataProviderTable;
			}
		}

		[Browsable(false)]
		internal Dictionary<Instrument, IExecutionProvider> ExecutionProviderTable
		{
			get
			{
				return this.executionProviderTable;
			}
		}

		public MarketManager()
		{
			this.marketDataProviderTable = new Dictionary<Instrument, IMarketDataProvider>();
			this.executionProviderTable = new Dictionary<Instrument, IExecutionProvider>();
			this.instruments = new InstrumentList();
		}

		public void AddInstrument(Instrument instrument, IMarketDataProvider marketDataProvider, IExecutionProvider executionProvider)
		{
            /*-------不开市的时候无法加入证券------*/
            if (!this.marketOpen) return;
            /*-------------*/
            if (!this.instruments.Contains(instrument))
			{
				this.instruments.Add(instrument);
			}
			if (marketDataProvider != null)
			{
				this.marketDataProviderTable[instrument] = marketDataProvider;
			}
			else
			{
				this.marketDataProviderTable[instrument] = this.strategyMarketDataProvider;
			}
			if (executionProvider != null)
			{
				this.executionProviderTable[instrument] = executionProvider;
				return;
			}
			this.executionProviderTable[instrument] = this.strategyExecutionProvider;
		}

		public void AddInstrument(Instrument instrument, string marketDataProviderName, string executionProviderName)
		{
			IMarketDataProvider marketDataProvider = ProviderManager.MarketDataProviders[marketDataProviderName];
			IExecutionProvider executionProvider = ProviderManager.ExecutionProviders[executionProviderName];
			if (marketDataProvider == null && marketDataProviderName != "")
			{
				throw new InvalidOperationException("Specified Market Data Provider for " + instrument.Symbol + " does not exist");
			}
			if (executionProvider == null && executionProviderName != "")
			{
				throw new InvalidOperationException("Specified Execution Provider for " + instrument.Symbol + " does not exist");
			}
			this.AddInstrument(instrument, marketDataProvider, executionProvider);
		}

		public void AddInstrument(string symbol, string marketDataProviderName, string executionProviderName)
		{
			Instrument instrument = InstrumentManager.Instruments[symbol];
			if (instrument != null)
			{
				this.AddInstrument(instrument, marketDataProviderName, executionProviderName);
			}
		}

		public void AddInstrument(Instrument instrument)
		{
			this.AddInstrument(instrument, this.strategyMarketDataProvider, this.strategyExecutionProvider);
		}

		public void AddInstrument(string symbol)
		{
			Instrument instrument = InstrumentManager.Instruments[symbol];
			if (instrument != null)
			{
				this.AddInstrument(instrument);
			}
		}

		public void RemoveInstrument(Instrument instrument)
		{
			this.instruments.Remove(instrument);
			this.marketDataProviderTable.Remove(instrument);
			this.executionProviderTable.Remove(instrument);
		}

		public void RemoveInstrument(string symbol)
		{
			Instrument instrument = InstrumentManager.Instruments[symbol];
			if (instrument != null)
			{
				this.RemoveInstrument(instrument);
			}
		}
    }
}
