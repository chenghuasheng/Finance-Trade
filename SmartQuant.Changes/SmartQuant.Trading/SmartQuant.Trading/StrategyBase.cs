using SmartQuant.Data;
using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Series;
using SmartQuant.Services;
using SmartQuant.Testing;
using SmartQuant.Trading.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SmartQuant.Trading
{
	public abstract class StrategyBase
	{
		protected const string CATEGORY_COMPONENTS = "Components";

		protected const string CATEGORY_PROVIDERS = "Providers";

		protected const string CATEGORY_NAMING = "Naming";

		protected const string CATEGORY_SERVICES = "Services";

		protected MetaStrategyBase metaStrategyBase;

		protected string name;

		protected string description;

		protected ReportManager reportManager;

		protected MarketManager marketManager;

		protected bool isEnabled;

		protected bool isActive;

		protected Portfolio portfolio;

		protected LiveTester tester;

		protected StopList stops;

		protected TriggerList triggers;

		protected IMarketDataProvider marketDataProvider;

		protected IExecutionProvider executionProvider;

		protected INewsProvider newsProvider;

		protected IExecutionService executionService;

		protected OrderTable orders;

		protected Hashtable global;

		protected InstrumentList activeInstruments;

		protected BarSliceManager barSliceManager;

		protected List<ComponentType> componentTypeList;

		protected Dictionary<Instrument, List<StopBase>> activeStops;

		protected Dictionary<Instrument, Portfolio> portfolios;

		protected Dictionary<Instrument, LiveTester> testers;

		protected bool statisticsPerInstrumentEnabled;

        public event EventHandler ProviderChanged;

        public event EventHandler PortfolioChanged;

        public event EventHandler EnabledChanged;

        public event ComponentTypeEventHandler ComponentChanged;

        public event StopEventHandler StopAdded;

        public event StopEventHandler StopStatusChanged;

        public event EventHandler StopListCleared;

        public event TriggerEventHandler TriggerAdded;

        public event TriggerEventHandler TriggerStatusChanged;

        public event EventHandler TriggerListCleared;

		[Category("Components"), Editor(typeof(ReportManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ReportManager ReportManager
		{
			get
			{
				return this.reportManager;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.reportManager != null)
				{
					this.reportManager.Disconnect();
				}
				this.reportManager = value;
				if (this.reportManager != null)
				{
					this.reportManager.Connect();
				}
				this.EmitComponentChanged(ComponentType.ReportManager);
			}
		}

		[Category("Components"), Editor(typeof(MarketManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public MarketManager MarketManager
		{
			get
			{
				return this.marketManager;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.marketManager != null)
				{
					this.marketManager.Disconnect();
					this.marketManager.StrategyBase = null;
				}
				this.marketManager = value;
				if (this.marketManager != null)
				{
					this.marketManager.StrategyBase = this;
					this.marketManager.Connect();
				}
				this.EmitComponentChanged(ComponentType.MarketManager);
			}
		}

		[Browsable(false)]
		public MetaStrategyBase MetaStrategyBase
		{
			get
			{
				return this.metaStrategyBase;
			}
			internal set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.metaStrategyBase = value;
			}
		}

		[Category("Naming")]
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		[Category("Naming")]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.description = value;
			}
		}

		[DefaultValue(true)]
		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.isEnabled = value;
				this.EmitEnabledChanged();
			}
		}

		[DefaultValue(true)]
		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
			set
			{
				this.isActive = value;
			}
		}

		public Hashtable Global
		{
			get
			{
				return this.global;
			}
		}

		[DefaultValue(false), Description("Enable or disable gathering statistics for each instrument")]
		public bool StatisticsPerInstrumentEnabled
		{
			get
			{
				return this.statisticsPerInstrumentEnabled;
			}
			set
			{
				this.statisticsPerInstrumentEnabled = value;
			}
		}

		[Browsable(false)]
		public List<ComponentType> ComponentTypeList
		{
			get
			{
				return this.componentTypeList;
			}
		}

		[Category("Providers"), DefaultValue(null), Editor(typeof(ExecutionProviderTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ProviderTypeConverter))]
		public IExecutionProvider ExecutionProvider
		{
			get
			{
				return this.executionProvider;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.executionProvider = value;
				this.EmitProviderChanged();
			}
		}

		[Category("Providers"), DefaultValue(null), Editor(typeof(MarketDataProviderTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ProviderTypeConverter))]
		public IMarketDataProvider MarketDataProvider
		{
			get
			{
				return this.marketDataProvider;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.marketDataProvider = value;
				this.EmitProviderChanged();
			}
		}

		[Category("Providers"), DefaultValue(null)]
		public INewsProvider NewsProvider
		{
			get
			{
				return this.newsProvider;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.newsProvider = value;
			}
		}

		[Category("Services"), DefaultValue(null), Editor(typeof(ExecutionServiceTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ServiceTypeConverter))]
		public IExecutionService ExecutionService
		{
			get
			{
				return this.executionService;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.executionService = value;
			}
		}

		[Browsable(false)]
		public LiveTester Tester
		{
			get
			{
				return this.tester;
			}
		}
        /*--------------------------------*/
        [Browsable(false)]
        public Dictionary<Instrument, LiveTester> Testers
        {
            get
            {
                return this.testers;
            }
        }
        /*---------------------------------*/
        [Editor(typeof(PortfolioTypeEditor), typeof(UITypeEditor))]
		public Portfolio Portfolio
		{
			get
			{
				return this.portfolio;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.portfolio = value;
				this.EmitPortfolioChanged();
			}
		}

		[Browsable(false)]
		public StopList Stops
		{
			get
			{
				return this.stops;
			}
		}

		[Browsable(false)]
		public TriggerList Triggers
		{
			get
			{
				return this.triggers;
			}
		}

		[Browsable(false)]
		public BarSeriesList Bars
		{
			get
			{
				return this.metaStrategyBase.Bars;
			}
		}

		[Browsable(false)]
		public OrderTable Orders
		{
			get
			{
				return this.orders;
			}
		}

		protected StrategyBase(string name, string description)
		{
			this.metaStrategyBase = null;
			this.name = name;
			this.description = description;
			this.isEnabled = true;
			this.isActive = true;
			this.ReportManager = (StrategyComponentManager.GetComponent("{5E7810DC-C9C1-427f-8CD9-1DFFE26E59B5}", this) as ReportManager);
			this.MarketManager = (StrategyComponentManager.GetComponent("{849E4CFE-C19E-4d1e-899D-0BB26DB12AAD}", this) as MarketManager);
			this.portfolio = PortfolioManager.Portfolios[name];
			if (this.portfolio == null)
			{
				this.portfolio = new Portfolio(name);
			}
			this.tester = new LiveTester(this.portfolio);
			this.stops = new StopList();
			this.triggers = new TriggerList();
			this.marketDataProvider = null;
			this.executionProvider = null;
			this.newsProvider = null;
			this.executionService = null;
			this.orders = new OrderTable();
			this.global = new Hashtable();
			this.activeInstruments = new InstrumentList();
			this.barSliceManager = new BarSliceManager();
			this.componentTypeList = new List<ComponentType>();
			this.componentTypeList.Add(ComponentType.MarketManager);
			this.componentTypeList.Add(ComponentType.ReportManager);
			this.activeStops = new Dictionary<Instrument, List<StopBase>>();
			this.portfolios = new Dictionary<Instrument, Portfolio>();
			this.testers = new Dictionary<Instrument, LiveTester>();
			this.statisticsPerInstrumentEnabled = false;
		}
        
		protected virtual void OnInit()
		{
		}

		protected virtual void OnStrategyStop()
		{
		}
        /**---------自己添加-----------*/
        protected virtual void OnCrossBehaviorInit()
        {
        }
        protected virtual void OnBehaviorInit()
        {
        }
        /**---------------------------------------*/
		internal void Init()
		{
			this.isActive = true;
			this.global.Clear();
			this.stops.Clear();
			this.activeStops.Clear();
			if (this.StopListCleared != null)
			{
				this.StopListCleared(this, EventArgs.Empty);
			}
			this.triggers.Clear();
			if (this.TriggerListCleared != null)
			{
				this.TriggerListCleared(this, EventArgs.Empty);
			}
			
			if (this.metaStrategyBase.MetaStrategyMode == MetaStrategyMode.Simulation || (this.metaStrategyBase.MetaStrategyMode == MetaStrategyMode.Live && this.metaStrategyBase.ResetPortfolio))
			{
				this.portfolio.Clear();
				this.portfolios.Clear();
				this.testers.Clear();
				this.tester.FriendlyTesters.Clear();
			}
			this.portfolio.TransactionAdded += new TransactionEventHandler(this.portfolio_TransactionAdded);
			this.portfolio.PositionOpened += new PositionEventHandler(this.portfolio_PositionOpened);
			this.portfolio.PositionChanged += new PositionEventHandler(this.portfolio_PositionChanged);
			this.portfolio.PositionClosed += new PositionEventHandler(this.portfolio_PositionClosed);
			this.portfolio.ValueChanged += new PositionEventHandler(this.portfolio_ValueChanged);
			this.portfolio.Monitored = true;
			this.orders.Clear();
			
			if (this.metaStrategyBase.MetaStrategyMode == MetaStrategyMode.Live)
			{
				this.marketManager.StrategyMarketDataProvider = this.marketDataProvider;
				this.marketManager.StrategyExecutionProvider = this.executionProvider;
			}
			this.tester.Disconnect();
			this.tester.Connect();
			this.tester.Reset();
			if (this.statisticsPerInstrumentEnabled)
			{
				foreach (LiveTester current in this.testers.Values)
				{
					current.Connect();
					current.Reset();
				}
			}
			this.OnInit();
			this.reportManager.Tester = this.tester;
			if (!this.MetaStrategyBase.IsOptimizing)
			{
				this.reportManager.Init();
			}
		}
        /*------------自己添加-----------*/
        internal void MarketInit()
        {
            this.activeInstruments.Clear();
            this.marketManager.Instruments.Clear();
            this.marketManager.MarketDataProviderTable.Clear();
            this.marketManager.Init();
            foreach (Instrument instrument in this.marketManager.Instruments)
            {
                instrument.Reset();
                BarSeries arg_24E_0 = this.Bars[instrument];
                this.activeStops[instrument] = new List<StopBase>();
                if (this.statisticsPerInstrumentEnabled && !this.portfolios.ContainsKey(instrument))
                {
                    Portfolio portfolio = new Portfolio();
                    portfolio.Name = string.Format("{0} [{1}]", this.name, instrument.Symbol);
                    if (this.metaStrategyBase.MetaStrategyMode == MetaStrategyMode.Live && !this.metaStrategyBase.ResetPortfolio)
                    {
                        portfolio.Account.Deposit(this.portfolio.GetAccountValue(), this.portfolio.Account.Currency, Clock.Now, "Initial Cash Allocation");
                    }
                    LiveTester liveTester = new LiveTester(portfolio);
                    liveTester.FollowChanges = true;
                    this.portfolios.Add(instrument, portfolio);
                    this.testers.Add(instrument, liveTester);
                    this.tester.FriendlyTesters.Add(instrument, liveTester);
                }
            }
            this.barSliceManager.InstrumentsCount = this.marketManager.Instruments.Count;
            this.barSliceManager.Reset();
            this.OnCrossBehaviorInit();
            this.OnBehaviorInit();
        }
        internal void StrategyStop()
		{
			this.OnStrategyStop();
			this.marketManager.OnStrategyStop();
			this.portfolio.TransactionAdded -= new TransactionEventHandler(this.portfolio_TransactionAdded);
			this.portfolio.PositionOpened -= new PositionEventHandler(this.portfolio_PositionOpened);
			this.portfolio.PositionChanged -= new PositionEventHandler(this.portfolio_PositionChanged);
			this.portfolio.PositionClosed -= new PositionEventHandler(this.portfolio_PositionClosed);
			this.portfolio.ValueChanged -= new PositionEventHandler(this.portfolio_ValueChanged);
			this.portfolio.Monitored = false;
			foreach (Portfolio current in this.portfolios.Values)
			{
				current.Monitored = false;
			}
			this.DisconnectAllStops();
			this.DisconnectAllTriggers();
			this.DisconnectTester();
		}

		protected void SetProxyProperties(object obj, object proxy)
		{
			PropertyInfo[] properties = proxy.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				bool flag = false;
				object[] customAttributes = propertyInfo.GetCustomAttributes(true);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					Attribute attribute = (Attribute)customAttributes[j];
					if (attribute.GetType() == typeof(BrowsableAttribute) && !(attribute as BrowsableAttribute).Browsable)
					{
						flag = true;
					}
				}
				if (!flag && propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					obj.GetType().GetProperty(propertyInfo.Name).SetValue(obj, propertyInfo.GetValue(proxy, null), null);
				}
			}
		}

		internal virtual void AddStop(StopBase stop)
		{
			this.stops.Add(stop);
			this.activeStops[stop.Instrument].Add(stop);
			if (this.StopAdded != null)
			{
				this.StopAdded(new StopEventArgs(stop));
			}
		}

		internal void EmitStopStatusChanged(StopBase stop)
		{
			if (stop.Status != StopStatus.Active)
			{
				this.activeStops[stop.Instrument].Remove(stop);
			}
			if (this.StopStatusChanged != null)
			{
				this.StopStatusChanged(new StopEventArgs(stop));
			}
		}

		private void DisconnectAllStops()
		{
			foreach (StopBase stopBase in this.stops)
			{
				stopBase.Disconnect();
			}
		}

		internal void AddTrigger(Trigger trigger)
		{
			this.triggers.Add(trigger);
			if (this.TriggerAdded != null)
			{
				this.TriggerAdded(new TriggerEventArgs(trigger));
			}
		}

		internal void EmitTriggerStatusChanged(Trigger trigger)
		{
			if (this.TriggerStatusChanged != null)
			{
				this.TriggerStatusChanged(new TriggerEventArgs(trigger));
			}
		}

		private void DisconnectAllTriggers()
		{
			foreach (Trigger trigger in this.triggers)
			{
				trigger.Disconnect();
			}
		}

		private void DisconnectTester()
		{
			if (this.statisticsPerInstrumentEnabled)
			{
				foreach (LiveTester current in this.testers.Values)
				{
					current.Disconnect();
				}
			}
			this.tester.Disconnect();
		}

		internal void Close()
		{
			StrategyComponentManager.ClearComponentCache(this);
			this.metaStrategyBase = null;
		}

		internal void PrepareTester()
		{
			this.tester.SaveSettings();
			this.tester.DisableComponents();
			this.tester.FollowChanges = true;
		}

		internal void RestoreTester()
		{
			this.tester.RestoreSettings();
		}

		internal void Deposit(double val, Currency currency, DateTime datetime, string text)
		{
			this.portfolio.Account.Deposit(val, currency, datetime, text);
			foreach (Portfolio current in this.portfolios.Values)
			{
				current.Account.Deposit(val, currency, datetime, text);
			}
		}

		internal void Withdraw(double val, Currency currency, DateTime datetime, string text)
		{
			this.portfolio.Account.Withdraw(val, currency, datetime, text);
			foreach (Portfolio current in this.portfolios.Values)
			{
				current.Account.Withdraw(val, currency, datetime, text);
			}
		}

		public void AddInstrument(Instrument instrument)
		{
			if (!this.activeInstruments.Contains(instrument))
			{
				this.activeInstruments.Add(instrument);
			}
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
			if (this.activeInstruments.Contains(instrument))
			{
				this.activeInstruments.Remove(instrument);
			}
		}

		public void RemoveInstrument(string symbol)
		{
			Instrument instrument = InstrumentManager.Instruments[symbol];
			if (instrument != null)
			{
				this.RemoveInstrument(instrument);
			}
		}

		public bool IsInstrumentActive(Instrument instrument)
		{
			return this.activeInstruments.Contains(instrument);
		}

		protected void EmitProviderChanged()
		{
			if (this.ProviderChanged != null)
			{
				this.ProviderChanged(this, EventArgs.Empty);
			}
		}

		protected void EmitPortfolioChanged()
		{
			if (this.PortfolioChanged != null)
			{
				this.PortfolioChanged(this, EventArgs.Empty);
			}
		}

		protected void EmitEnabledChanged()
		{
			if (this.EnabledChanged != null)
			{
				this.EnabledChanged(this, EventArgs.Empty);
			}
		}

		protected void EmitComponentChanged(ComponentType componentType)
		{
			if (this.ComponentChanged != null)
			{
				this.ComponentChanged(this, new ComponentTypeEventArgs(componentType));
			}
		}

		protected void EmitStopAdded(StopBase stop)
		{
			if (this.StopAdded != null)
			{
				this.StopAdded(new StopEventArgs(stop));
			}
		}

		protected virtual void OnProviderConnected(IProvider provider)
		{
		}

		protected virtual void OnProviderDisconnected(IProvider provider)
		{
		}

		protected virtual void OnProviderError(IProvider provider, int id, int code, string message)
		{
		}

		internal void SetProviderConnected(IProvider provider)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetProviderConnected> {0} {1}", this.name, provider.Name));
			}
			this.OnProviderConnected(provider);
		}

		internal void SetProviderDisconnected(IProvider provider)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetProviderDisconnected> {0} {1}", this.name, provider.Name));
			}
			this.OnProviderDisconnected(provider);
		}

		internal void SetProviderError(IProvider provider, int id, int code, string message)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<SetProviderError> {0} {1} {2} {3} {4}", new object[]
				{
					this.name,
					provider.Name,
					id,
					code,
					message
				}));
			}
			this.OnProviderError(provider, id, code, message);
		}

		protected virtual void OnServiceStatusChanged(IService service)
		{
		}

		protected virtual void OnServiceError(IService service, ServiceErrorType errorType, string text)
		{
		}

		internal void SetServiceStatusChanged(IService service)
		{
			if (!this.isActive)
			{
				return;
			}
			this.OnServiceStatusChanged(service);
		}

		internal void SetServiceError(IService service, ServiceErrorType errorType, string text)
		{
			if (!this.isActive)
			{
				return;
			}
			this.OnServiceError(service, errorType, text);
		}

		protected virtual void OnNewClientOrder(SingleOrder order)
		{
		}

		internal void SetNewClientOrder(SingleOrder order)
		{
			if (!this.isActive)
			{
				return;
			}
			this.OnNewClientOrder(order);
		}

		internal void SendExecutionReport(ExecutionReport report)
		{
			this.MetaStrategyBase.SendExecutionReport(this, report);
		}

		protected virtual void OnNewTrade(Instrument instrument, Trade trade)
		{
		}

		protected virtual void OnNewQuote(Instrument instrument, Quote quote)
		{
		}

		protected virtual void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
		{
		}

		protected virtual void OnNewFundamental(Instrument instrument, Fundamental fundamental)
		{
		}

		protected virtual void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
		{
		}

		protected virtual void OnNewBarOpen(Instrument instrument, Bar bar)
		{
		}

		protected virtual void OnNewBar(Instrument instrument, Bar bar)
		{
		}

		protected virtual void OnNewBarSlice(long barSize)
		{
		}

		internal void SetNewTrade(Instrument instrument, Trade trade)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewTrade> {0} {1} {2}", this.Name, instrument.Symbol, trade));
			}
			this.OnNewTrade(instrument, trade);
		}

		internal void SetNewQuote(Instrument instrument, Quote quote)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewQuote> {0} {1} {2}", this.Name, instrument.Symbol, quote));
			}
			this.OnNewQuote(instrument, quote);
		}

		internal void SetNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewMarketDepth> {0} {1} {2}", this.Name, instrument.Symbol, marketDepth));
			}
			this.OnNewMarketDepth(instrument, marketDepth);
		}

		internal void SetNewFundamental(Instrument instrument, Fundamental fundamental)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewFundamental> {0} {1} {2}", this.Name, instrument.Symbol, fundamental));
			}
			this.OnNewFundamental(instrument, fundamental);
		}

		internal void SetNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewCorporateAction> {0} {1} {2}", this.Name, instrument.Symbol, corporateAction));
			}
			this.OnNewCorporateAction(instrument, corporateAction);
		}

		internal void SetNewBarOpen(Instrument instrument, Bar bar)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewBarOpen> {0} {1} {2}", this.Name, instrument.Symbol, bar));
			}
			this.OnNewBarOpen(instrument, bar);
		}

		internal void SetNewBar(Instrument instrument, Bar bar)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewBar> {0} {1} {2}", this.Name, instrument.Symbol, bar));
			}
			if (bar.BarType == BarType.Time)
			{
				this.barSliceManager.AddBar(instrument, bar);
				return;
			}
			this.OnNewBar(instrument, bar);
		}

		internal void SetNewBarSlice(long barSize)
		{
			if (!this.isActive)
			{
				return;
			}
			if (Trace.IsLevelEnabled(TraceLevel.Verbose))
			{
				Trace.WriteLine(string.Format("<Strategy.SetNewBarSlice> {0} {1}", this.Name, barSize));
			}
			BarSlice slice = this.barSliceManager.GetSlice(barSize);
			if (slice != null)
			{
				foreach (KeyValuePair<Instrument, Bar> current in slice.Table)
				{
					this.OnNewBar(current.Key, current.Value);
				}
				this.OnNewBarSlice(barSize);
				slice.Reset();
			}
		}

		protected virtual void OnNewOrder(SingleOrder order)
		{
		}

		protected virtual void OnExecutionReport(SingleOrder order, ExecutionReport report)
		{
		}

		protected virtual void OnOrderPartiallyFilled(SingleOrder order)
		{
		}

		protected virtual void OnOrderStatusChanged(SingleOrder order)
		{
		}

		protected virtual void OnOrderFilled(SingleOrder order)
		{
		}

		protected virtual void OnOrderCancelled(SingleOrder order)
		{
		}

		protected virtual void OnOrderRejected(SingleOrder order)
		{
		}

		protected virtual void OnOrderDone(SingleOrder order)
		{
		}

		internal void SetNewOrder(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
			if (order.Text != "")
			{
				this.orders.Add(instrument, order.Text, order);
			}
			this.OnNewOrder(order);
		}

		internal void SetExecutionReport(SingleOrder order, ExecutionReport report)
		{
			this.OnExecutionReport(order, report);
		}

		internal void SetOrderPartiallyFilled(SingleOrder order)
		{
			this.OnOrderPartiallyFilled(order);
		}

		internal void SetOrderStatusChanged(SingleOrder order)
		{
			this.OnOrderStatusChanged(order);
		}

		internal void SetOrderDone(SingleOrder order)
		{
			OrdStatus ordStatus = order.OrdStatus;
			switch (ordStatus)
			{
			case OrdStatus.Filled:
				this.OnOrderFilled(order);
				break;
			case OrdStatus.DoneForDay:
				break;
			case OrdStatus.Cancelled:
				this.OnOrderCancelled(order);
				break;
			default:
				if (ordStatus == OrdStatus.Rejected)
				{
					this.OnOrderRejected(order);
				}
				break;
			}
			this.OnOrderDone(order);
		}

		protected virtual void OnTransactionAdded(Transaction transaction)
		{
		}

		protected virtual void OnPositionOpened(Position position)
		{
		}

		protected virtual void OnPositionClosed(Position position)
		{
		}

		protected virtual void OnPositionChanged(Position position)
		{
		}

		protected virtual void OnPortfolioValueChanged(Position position)
		{
		}

		internal void SetTransactionAdded(Transaction transaction)
		{
			if (!this.isActive)
			{
				return;
			}
			this.portfolio.Add(transaction);
			if (this.statisticsPerInstrumentEnabled)
			{
				this.portfolios[transaction.Instrument].Add(transaction);
			}
		}

		private void portfolio_TransactionAdded(object sender, TransactionEventArgs args)
		{
			this.OnTransactionAdded(args.Transaction);
		}

		private void portfolio_PositionOpened(object sender, PositionEventArgs args)
		{
			this.OnPositionOpened(args.Position);
		}

		private void portfolio_PositionChanged(object sender, PositionEventArgs args)
		{
			this.OnPositionChanged(args.Position);
		}

		private void portfolio_PositionClosed(object sender, PositionEventArgs args)
		{
			this.OnPositionClosed(args.Position);
		}

		private void portfolio_ValueChanged(object sender, PositionEventArgs args)
		{
			if (!this.isActive)
			{
				return;
			}
			try
			{
				this.OnPortfolioValueChanged(args.Position);
			}
			catch (Exception exception)
			{
				this.metaStrategyBase.EmitError(exception);
			}
		}

		public abstract void ClosePosition(Instrument instrument, double price, ComponentType component, string text);

		public void ClosePosition(Instrument instrument, string text)
		{
			this.ClosePosition(instrument, 0.0, ComponentType.RiskManager, text);
		}

		public void ClosePosition(Instrument instrument)
		{
			this.ClosePosition(instrument, "");
		}

		public void ClosePortfolio(string text)
		{
			PositionList positionList = new PositionList();
			foreach (Position position in this.portfolio.Positions)
			{
				positionList.Add(position);
			}
			foreach (Position position2 in positionList)
			{
				this.ClosePosition(position2.Instrument, text);
			}
		}

		public void ClosePortfolio()
		{
			this.ClosePortfolio("");
		}

		public abstract IComponentBase GetComponent(ComponentType type);

		public abstract void SetComponent(ComponentType type, IComponentBase component);
	}
}
