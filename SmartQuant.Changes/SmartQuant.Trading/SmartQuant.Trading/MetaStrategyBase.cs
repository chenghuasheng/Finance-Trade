using SmartQuant.Charting;
using SmartQuant.Data;
using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Optimization;
using SmartQuant.Providers;
using SmartQuant.Series;
using SmartQuant.Services;
using SmartQuant.Simulation;
using SmartQuant.Testing;
using SmartQuant.Trading.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SmartQuant.Trading
{
	public abstract class MetaStrategyBase : IOptimizable
	{
		public class FIXModes
		{
			private Hashtable modes;

			public char this[MetaStrategyMode mode]
			{
				get
				{
					return (char)this.modes[mode];
				}
			}

			internal FIXModes()
			{
				this.modes = new Hashtable();
				this.modes.Add(MetaStrategyMode.Simulation, 'S');
				this.modes.Add(MetaStrategyMode.Live, 'L');
			}
		}

		protected const string CATEGORY_NAMING = "Naming";

		protected const string CATEGORY_COMPONENTS = "Components";

		protected const string CATEGORY_PORTFOLIO = "Portfolio";

		protected const string CATEGORY_DATA_PERSISTENT = "Data Persistent";

		public static readonly MetaStrategyBase.FIXModes FIX_MODES = new MetaStrategyBase.FIXModes();

		protected string name;

		protected MetaStrategyMode metaStrategyMode;

		protected SimulationManager simulationManager;

		protected ReportManager reportManager;

		protected OptimizationManager optimizationManager;

		protected MetaMoneyManager metaMoneyManager;

		protected StrategyList strategies;

		protected Portfolio portfolio;

		protected LiveTester tester;

		protected Optimizer optimizer;

		protected EOptimizerType optimizerType;

		protected Simulator simulator;

		protected bool isRunning;

		protected bool isOptimizing;

		protected ArrayList optimizationParemeters;

		protected Hashtable drawingPrimitives;

		protected StopList portfolioStopList;

		protected List<ComponentType> componentTypeList;
        /*--------------------------------------------*/
        protected bool changeMarketOnDay;
        protected bool clearDataArraysOnDay;
        /*--------------------------------------------*/
        protected bool resetPortfolio;

		protected bool saveOrders;

		protected Dictionary<Instrument, Portfolio> portfolios;

		protected Dictionary<Instrument, LiveTester> testers;

		protected bool statisticsPerInstrumentEnabled;

		protected bool executionServicesEnabled;

		private ProviderDispatcher providerDispatcher;

		private MarketDataDispatcher marketDataDispatcher;

		private OrderProcessor orderProcessor;

		private ServiceDispatcher serviceDispatcher;

		private ExecutionServiceDispatcher executionServiceDispatcher;

		private Dictionary<IProvider, List<StrategyBase>> providerDispatcherTable;

		private Dictionary<IMarketDataProvider, Dictionary<Instrument, List<StrategyBase>>> marketDataTable;

		private Dictionary<SingleOrder, StrategyBase> orders;

		private Dictionary<IService, List<StrategyBase>> serviceDispatcherTable;

		private Dictionary<IExecutionService, List<StrategyBase>> executionServiceDispatcherTable;

		private int maxConnectionTime;

        public event EventHandler Changed;

        public event ComponentTypeEventHandler ComponentChanged;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event EventHandler Paused;

        public event EventHandler DesignModeChanged;

        public event MetaStrategyErrorEventHandler Error;

		[Browsable(false)]
		public Dictionary<Instrument, LiveTester> Testers
		{
			get
			{
				return this.testers;
			}
		}

		[Browsable(false)]
		public Dictionary<Instrument, Portfolio> Portfolios
		{
			get
			{
				return this.portfolios;
			}
		}

		[Category("Components"), Editor(typeof(ReportManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ReportManager ReportManager
		{
			get
			{
				return this.reportManager;
			}
			set
			{
				if (!this.DesignMode)
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

		[Category("Components"), Editor(typeof(SimulationManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public SimulationManager SimulationManager
		{
			get
			{
				return this.simulationManager;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.simulationManager != null)
				{
					this.simulationManager.Disconnect();
				}
				this.simulationManager = value;
				if (this.simulationManager != null)
				{
					this.simulationManager.Connect();
				}
				this.EmitComponentChanged(ComponentType.SimulationManager);
			}
		}

		[Category("Components"), Editor(typeof(OptimizationManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public OptimizationManager OptimizationManager
		{
			get
			{
				return this.optimizationManager;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.optimizationManager != null)
				{
					this.optimizationManager.Disconnect();
					this.optimizationManager.MetaStrategyBase = null;
				}
				this.optimizationManager = value;
				if (this.optimizationManager != null)
				{
					this.optimizationManager.MetaStrategyBase = this;
					this.optimizationManager.Connect();
				}
				this.EmitComponentChanged(ComponentType.OptimizationManager);
			}
		}

		[Category("Components"), Editor(typeof(MetaMoneyManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public MetaMoneyManager MetaMoneyManager
		{
			get
			{
				return this.metaMoneyManager;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.metaMoneyManager != null)
				{
					this.metaMoneyManager.Disconnect();
					this.metaMoneyManager.MetaStrategyBase = null;
				}
				this.metaMoneyManager = value;
				if (this.metaMoneyManager != null)
				{
					this.metaMoneyManager.MetaStrategyBase = this;
					this.metaMoneyManager.Connect();
				}
				this.EmitComponentChanged(ComponentType.MetaMoneyManager);
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

		[DefaultValue(10), Description("Maximum connection time to providers in seconds.")]
		public int MaxConnectionTime
		{
			get
			{
				return this.maxConnectionTime;
			}
			set
			{
				if (value < 1 || value > 60)
				{
					throw new ArgumentOutOfRangeException("MaxConnectionTime", "The value must be in range 1..60");
				}
				this.maxConnectionTime = value;
			}
		}

		[DefaultValue(MetaStrategyMode.Simulation)]
		public MetaStrategyMode MetaStrategyMode
		{
			get
			{
				return this.metaStrategyMode;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.metaStrategyMode = value;
				this.EmitChanged();
			}
		}

		[Category("Portfolio"), Editor(typeof(PortfolioTypeEditor), typeof(UITypeEditor))]
		public Portfolio Portfolio
		{
			get
			{
				return this.portfolio;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.portfolio = value;
				this.EmitChanged();
			}
		}
        /**-----自己添加---------------------------------------------------*/
        [Category("Data Persistent"), DefaultValue(false), Description("Change market on every day.")]
        public bool ChangeMarketOnDay
        {
            get
            {
                return this.changeMarketOnDay;
            }
            set
            {
                this.changeMarketOnDay = value;
            }
        }
        [Category("Data Persistent"), DefaultValue(false), Description("Clear data arrays on every day.")]
        public bool ClearDataArraysOnDay
        {
            get
            {
                return this.clearDataArraysOnDay;
            }
            set
            {
                this.clearDataArraysOnDay = value;
            }
        }
        /*-----------------------------------------------------------------*/
        [Category("Data Persistent"), DefaultValue(true), Description("Gets or sets a value indicating whether the portfolio will be cleared when metastrategy start in LIVE mode.")]
		public bool ResetPortfolio
		{
			get
			{
				return this.resetPortfolio;
			}
			set
			{
				this.resetPortfolio = value;
			}
		}

		[Category("Data Persistent"), DefaultValue(false), Description("Gets or sets a value indicating whether orders will be saved in LIVE mode.")]
		public bool SaveOrders
		{
			get
			{
				return this.saveOrders;
			}
			set
			{
				this.saveOrders = value;
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

		[DefaultValue(false)]
		public bool ExecutionServicesEnabled
		{
			get
			{
				return this.executionServicesEnabled;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				this.executionServicesEnabled = value;
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

		[Browsable(false)]
		public Optimizer Optimizer
		{
			get
			{
				return this.optimizer;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.optimizer != value)
				{
					if (this.optimizer != null)
					{
						this.optimizer.BestObjectiveReceived -= new EventHandler(this.optimizer_BestObjectiveReceived);
					}
					this.optimizer = value;
					this.optimizer.BestObjectiveReceived += new EventHandler(this.optimizer_BestObjectiveReceived);
				}
			}
		}

		[DefaultValue(EOptimizerType.BruteForce)]
		public EOptimizerType OptimizerType
		{
			get
			{
				return this.optimizerType;
			}
			set
			{
				if (!this.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (value != this.optimizerType)
				{
					if (this.optimizer != null)
					{
						this.optimizer.BestObjectiveReceived -= new EventHandler(this.optimizer_BestObjectiveReceived);
					}
					this.optimizerType = value;
					switch (this.optimizerType)
					{
					case EOptimizerType.SimulatedAnnealing:
						this.optimizer = new SimulatedAnnealing(this);
						break;
					case EOptimizerType.BruteForce:
						this.optimizer = new BruteForce(this);
						break;
					}
					this.optimizer.BestObjectiveReceived += new EventHandler(this.optimizer_BestObjectiveReceived);
				}
			}
		}

		[Browsable(false)]
		public StrategyList Strategies
		{
			get
			{
				return this.strategies;
			}
		}

		[Browsable(false)]
		public BarSeriesList Bars
		{
			get
			{
				return DataManager.Bars;
			}
		}

		[Browsable(false)]
		public bool DesignMode
		{
			get
			{
				return !this.isRunning && !this.isOptimizing;
			}
		}

		[Browsable(false)]
		internal bool IsRunning
		{
			get
			{
				return this.isRunning;
			}
		}

		[Browsable(false)]
		internal bool IsOptimizing
		{
			get
			{
				return this.isOptimizing;
			}
		}

		[Browsable(false)]
		public Hashtable DrawingPrimitives
		{
			get
			{
				return this.drawingPrimitives;
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
        
		protected MetaStrategyBase(string name)
		{
			this.name = name;
			this.strategies = new StrategyList();
			this.SimulationManager = (StrategyComponentManager.GetComponent("{872476E5-3774-4687-828F-34978288A6E0}", this) as SimulationManager);
			this.OptimizationManager = (StrategyComponentManager.GetComponent("{A4D510F9-13DB-4b4c-9557-BC6A48A25D0B}", this) as OptimizationManager);
			this.ReportManager = (StrategyComponentManager.GetComponent("{5E7810DC-C9C1-427f-8CD9-1DFFE26E59B5}", this) as ReportManager);
			this.MetaMoneyManager = (StrategyComponentManager.GetComponent("{FED5076A-C710-4d3a-B134-3D9D32B8B248}", this) as MetaMoneyManager);
			this.portfolio = PortfolioManager.Portfolios[name];
			if (this.portfolio == null)
			{
				this.portfolio = new Portfolio(name);
			}
            this.resetPortfolio = true;
			this.tester = new LiveTester(this.portfolio);
			this.tester.FollowChanges = true;
			this.optimizer = new BruteForce(this);
			this.optimizer.BestObjectiveReceived += new EventHandler(this.optimizer_BestObjectiveReceived);
			this.optimizerType = EOptimizerType.BruteForce;
			this.providerDispatcher = new ProviderDispatcher(this);
			this.marketDataDispatcher = new MarketDataDispatcher(this);
			this.orderProcessor = new OrderProcessor(this);
			this.serviceDispatcher = new ServiceDispatcher(this);
			this.executionServiceDispatcher = new ExecutionServiceDispatcher(this);
			this.providerDispatcherTable = new Dictionary<IProvider, List<StrategyBase>>();
			this.marketDataTable = new Dictionary<IMarketDataProvider, Dictionary<Instrument, List<StrategyBase>>>();
			this.orders = new Dictionary<SingleOrder, StrategyBase>();
			this.serviceDispatcherTable = new Dictionary<IService, List<StrategyBase>>();
			this.executionServiceDispatcherTable = new Dictionary<IExecutionService, List<StrategyBase>>();
			this.maxConnectionTime = 10;
			this.simulator = (ProviderManager.MarketDataSimulator as SimulationDataProvider).Simulator;
			this.simulator.StateChanged += new EventHandler(this.OnSimulatorStateChanged);
			this.isRunning = false;
			this.isOptimizing = false;
			this.optimizationParemeters = new ArrayList();
			this.drawingPrimitives = new Hashtable();
			this.portfolioStopList = new StopList();
			this.portfolios = new Dictionary<Instrument, Portfolio>();
			this.testers = new Dictionary<Instrument, LiveTester>();
			this.executionServicesEnabled = false;
			this.componentTypeList = new List<ComponentType>();
			this.componentTypeList.Add(ComponentType.SimulationManager);
			this.componentTypeList.Add(ComponentType.MetaMoneyManager);
			this.componentTypeList.Add(ComponentType.OptimizationManager);
			this.componentTypeList.Add(ComponentType.ReportManager);
		}

		public void Add(StrategyBase strategy)
		{
			if (!this.DesignMode)
			{
				throw new InvalidOperationException("MetaStrategy is not in design mode");
			}
			this.strategies.Add(strategy);
			strategy.MetaStrategyBase = this;
		}

		public void Remove(StrategyBase strategy)
		{
			if (!this.DesignMode)
			{
				throw new InvalidOperationException("MetaStrategy is not in design mode");
			}
			this.strategies.Remove(strategy);
			strategy.MetaStrategyBase = null;
		}

		public void Close()
		{
			this.simulator.StateChanged -= new EventHandler(this.OnSimulatorStateChanged);
			foreach (StrategyBase strategyBase in this.strategies)
			{
				strategyBase.Close();
			}
			StrategyComponentManager.ClearComponentCache(this);
			this.Portfolio = null;
		}

		internal void AddPortfolioStop(PortfolioStop stop)
		{
			this.portfolioStopList.Add(stop);
		}

		internal void DisconnectAllPortfolioStops()
		{
			foreach (PortfolioStop portfolioStop in this.portfolioStopList)
			{
				portfolioStop.Disconnect();
			}
		}

		public void StopOptimization()
		{
			this.optimizer.Stop();
		}

		protected virtual void OnInit()
		{
		}

		protected virtual void OnMoneyAllocation()
		{
			this.metaMoneyManager.Allocate();
		}
        /*-----------------------自己添加--------------------------------------*/
        public void Start(bool doStep)
        {
            DataManager.ClearDataArrays();
            this.drawingPrimitives.Clear();
            this.providerDispatcherTable.Clear();
            //this.marketDataTable.Clear();
            this.orders.Clear();
            this.providerDispatcher.Init();
            //this.marketDataDispatcher.Init();
            this.orderProcessor.Init();
            this.serviceDispatcherTable.Clear();
            this.executionServiceDispatcherTable.Clear();
            this.serviceDispatcher.Init();
            this.executionServiceDispatcher.Init();
            if (this.metaStrategyMode == MetaStrategyMode.Simulation)
            {
                this.simulationManager.Requests.Clear();
                this.simulationManager.Init();
                Clock.ClockMode = ClockMode.Simulation;
                Clock.SetDateTime(this.simulationManager.EntryDate);
                foreach (string request in this.simulationManager.StaticRequests)
                {
                    this.simulationManager.SendMarketDataRequest(request);
                }
                //this.simulator.Intervals.Clear();
                //this.simulator.Intervals.Add(this.simulationManager.EntryDate, this.simulationManager.ExitDate);
                this.simulator.SimulationMode = this.simulationManager.Mode;
                this.simulator.SpeedMultiplier = this.simulationManager.SpeedMultiplier;
                this.simulator.Step = this.simulationManager.Step;
                OrderManager.RemoveOrders(11104, MetaStrategyBase.FIX_MODES[this.metaStrategyMode]);
                OrderManager.OCA.Clear();
                OrderManager.SellSide.RemoveOrders(11104, MetaStrategyBase.FIX_MODES[this.metaStrategyMode]);
            }
            if (this.metaStrategyMode == MetaStrategyMode.Simulation || (this.metaStrategyMode == MetaStrategyMode.Live && this.resetPortfolio))
            {
                this.portfolio.Clear();
                this.portfolios.Clear();
                this.testers.Clear();
                this.tester.FriendlyTesters.Clear();
            }
            this.tester.Disconnect();
            this.tester.Connect();
            this.tester.Reset();
            bool flag = this.portfolio.Account.Transactions.Count == 0;
            if (flag)
            {
                this.portfolio.Account.Deposit(this.simulationManager.Cash, this.simulationManager.Currency, Clock.Now, "Initial Cash Allocation");
            }
            this.portfolio.TransactionAdded += new TransactionEventHandler(this.portfolio_TransactionAdded);
            this.portfolio.PositionOpened += new PositionEventHandler(this.portfolio_PositionOpened);
            this.portfolio.PositionChanged += new PositionEventHandler(this.portfolio_PositionChanged);
            this.portfolio.PositionClosed += new PositionEventHandler(this.portfolio_PositionClosed);
            this.portfolio.ValueChanged += new PositionEventHandler(this.portfolio_ValueChanged);
            this.portfolio.Monitored = true;
            this.optimizationManager.Init();
            if (!this.isOptimizing)
            {
                this.reportManager.Tester = this.tester;
                this.reportManager.Init();
            }
            this.metaMoneyManager.Init();
            this.OnInit();
            foreach (StrategyBase strategyBase in this.strategies)
            {
                if (strategyBase.IsEnabled)
                {
                    strategyBase.Init();
                }
            }
            if (flag)
            {
                this.OnMoneyAllocation();
            }
            if (this.statisticsPerInstrumentEnabled)
            {
                foreach (LiveTester current in this.testers.Values)
                {
                    current.Connect();
                    current.Reset();
                }
            }
            foreach (StrategyBase strategyBase3 in this.strategies)
            {
                if (strategyBase3.IsEnabled)
                {
                    switch (this.metaStrategyMode)
                    {
                        case MetaStrategyMode.Simulation:
                            this.orderProcessor.Add(ProviderManager.ExecutionSimulator);
                            this.providerDispatcher.Add(ProviderManager.MarketDataSimulator);
                            this.providerDispatcher.Add(ProviderManager.ExecutionSimulator);
                            this.RegisterStrategy(ProviderManager.MarketDataSimulator, strategyBase3);
                            this.RegisterStrategy(ProviderManager.ExecutionSimulator, strategyBase3);
                            break;
                        case MetaStrategyMode.Live:
                            this.orderProcessor.Add(strategyBase3.ExecutionProvider);
                            this.providerDispatcher.Add(strategyBase3.ExecutionProvider);
                            this.providerDispatcher.Add(strategyBase3.MarketDataProvider);
                            this.RegisterStrategy(strategyBase3.ExecutionProvider, strategyBase3);
                            this.RegisterStrategy(strategyBase3.MarketDataProvider, strategyBase3);
                            break;
                    }
                }
            }
            this.providerDispatcher.Online(this.maxConnectionTime * 1000);
            this.orderProcessor.Online();
            if (this.executionServicesEnabled)
            {
                foreach (StrategyBase strategyBase4 in this.strategies)
                {
                    switch (this.metaStrategyMode)
                    {
                        case MetaStrategyMode.Simulation:
                            this.RegisterStrategy(ServiceManager.ExecutionSimulator, strategyBase4);
                            break;
                        case MetaStrategyMode.Live:
                            if (strategyBase4.ExecutionService != null)
                            {
                                this.RegisterStrategy(strategyBase4.ExecutionService, strategyBase4);
                            }
                            break;
                    }
                }
                this.serviceDispatcher.Online();
                this.executionServiceDispatcher.Online();
            }
            this.isRunning = true;
            if (this.changeMarketOnDay)
            {
                DateTime curDate;
                switch (this.metaStrategyMode)
                {
                    case MetaStrategyMode.Simulation:
                        curDate = this.simulationManager.EntryDate.Date;
                        this.simulator.Intervals.Clear();
                        while (curDate < this.simulationManager.ExitDate)
                        {
                            DateTime nextDate = curDate.AddDays(1);
                            this.simulator.Intervals.Add(curDate, nextDate);
                            curDate = nextDate;
                        }
                        this.simulator.EnterInterval += new IntervalEventHandler(this.OnEnterInterval);
                        this.simulator.LeaveInterval += new IntervalEventHandler(this.OnLeaveInterval);
                        if (doStep)
                        {
                            this.simulator.DoStep(false);
                            while (this.isRunning)
                            {
                                Thread.Sleep(1);
                            }
                            return;
                        }
                        this.simulator.Start(true);
                        return;
                    case MetaStrategyMode.Live:
                        curDate = Clock.Now.Date;
                        this.ConnectMarketAndBehavior();
                        this.EmitStarted();
                        while (this.isRunning)
                        {
                            Thread.Sleep(1);
                            if (this.isRunning && curDate < Clock.Now.Date)
                            {
                                this.DisconnectMarketAndBehavior();
                                this.ConnectMarketAndBehavior();
                                curDate = Clock.Now.Date;
                            }
                        }
                        return;
                }
            }
            else
            {              
                switch (this.metaStrategyMode)
                {
                    case MetaStrategyMode.Simulation:
                        this.simulator.Intervals.Clear();
                        this.simulator.Intervals.Add(this.simulationManager.EntryDate, this.simulationManager.ExitDate);
                        this.simulator.EnterInterval += new IntervalEventHandler(this.OnEnterInterval);
                        this.simulator.LeaveInterval += new IntervalEventHandler(this.OnLeaveInterval);
                        if (doStep)
                        {
                            this.simulator.DoStep(false);
                            while (this.isRunning)
                            {
                                Thread.Sleep(1);
                            }
                            return;
                        }                       
                        this.simulator.Start(true);
                        return;
                    case MetaStrategyMode.Live:
                        this.ConnectMarketAndBehavior();
                        this.EmitStarted();
                        while (this.isRunning)
                        {
                            Thread.Sleep(1);
                        }
                        return;
                    default:
                        return;
                }
            }
        }
        private void OnEnterInterval(IntervalEventArgs args)
        {
            this.tester.WaitingForStart = true;//当simulator调用了FireAllReminder后，必须重设tester的WaitingForStart属性才能继续测试统计
            if (this.statisticsPerInstrumentEnabled)
            {
                foreach (LiveTester tester in this.testers.Values)
                {
                    tester.WaitingForStart = true;
                }
            }
            foreach (StrategyBase strategyBase in this.strategies)
            {
                if (strategyBase.IsEnabled)
                {
                    strategyBase.Tester.WaitingForStart = true;
                }
                if (strategyBase.StatisticsPerInstrumentEnabled)
                {
                    foreach(LiveTester tester in strategyBase.Testers.Values)
                    {
                        tester.WaitingForStart = true;
                    }
                }
            }
            this.ConnectMarketAndBehavior();
        }
        private void OnLeaveInterval(IntervalEventArgs args)
        {
            this.DisconnectMarketAndBehavior();
        }
        protected void ConnectMarketAndBehavior()
        {
            if (this.clearDataArraysOnDay)
            {
                DataManager.ClearDataArrays();
            }
            this.marketDataTable.Clear();
            this.marketDataDispatcher.Init();
            foreach (StrategyBase strategyBase in this.strategies)
            {
                if (strategyBase.IsEnabled)
                {
                    strategyBase.MarketInit();
                    foreach (Instrument instrument in strategyBase.MarketManager.Instruments)
                    {
                        switch (this.metaStrategyMode)
                        {
                            case MetaStrategyMode.Simulation:
                                foreach (string seriesSuffix in this.simulationManager.Requests)
                                {
                                    this.marketDataDispatcher.Add(ProviderManager.MarketDataSimulator, instrument, seriesSuffix);
                                }
                                this.RegisterStrategy(ProviderManager.MarketDataSimulator, instrument, strategyBase);
                                break;
                            case MetaStrategyMode.Live:
                                this.marketDataDispatcher.Add(strategyBase.MarketDataProvider, instrument, null);
                                this.RegisterStrategy(strategyBase.MarketDataProvider, instrument, strategyBase);
                                break;  
                        }
                    }
                }
            }
            
            if (this.statisticsPerInstrumentEnabled)
            {
                foreach (StrategyBase strategyBase2 in this.strategies)
                {
                    if (strategyBase2.IsEnabled)
                    {
                        foreach (Instrument instrument in strategyBase2.MarketManager.Instruments)
                        {
                            if (!this.portfolios.ContainsKey(instrument))
                            {
                                Portfolio portfolio = new Portfolio();
                                portfolio.Name = string.Format("{0} [{1}]", this.name, instrument.Symbol);
                                LiveTester liveTester = new LiveTester(portfolio);
                                liveTester.FollowChanges = true;
                                this.portfolios.Add(instrument, portfolio);
                                this.testers.Add(instrument, liveTester);
                                this.tester.FriendlyTesters.Add(instrument, liveTester);
                                portfolio.Account.Deposit(this.portfolio.GetAccountValue(), this.portfolio.Account.Currency, Clock.Now, "Initial Cash Allocation");
                            }
                        }
                    }
                }
            }
            this.marketDataDispatcher.Online();
        }
        protected void DisconnectMarketAndBehavior()
        {
            this.marketDataDispatcher.Offline();
        }
        /*-----------------------------------------------------------------------*/
        public void Continue()
		{
			if (this.metaStrategyMode == MetaStrategyMode.Simulation)
			{
				this.simulator.Continue();
			}
		}

		public void DoStep()
		{
			if (this.metaStrategyMode == MetaStrategyMode.Simulation)
			{
				this.simulator.DoStep(false);
			}
		}

		public void Stop()
		{
			if (this.metaStrategyMode == MetaStrategyMode.Simulation)
			{
				this.simulator.Stop(false);
				return;
			}
			this.MetaStrategyStop();
		}

		protected virtual void OnMetaStrategyStop()
		{
		}

		private void MetaStrategyStop()
		{
            if (this.DesignMode)
			{
				return;
			}
			this.isRunning = false;
            if (this.metaStrategyMode == MetaStrategyMode.Live)
            {
                this.marketDataDispatcher.Offline();
            }
			this.orderProcessor.Offline();
            this.providerDispatcher.Offline();
            if (this.executionServicesEnabled)
			{
				this.serviceDispatcher.Offline();
				this.executionServiceDispatcher.Offline();
			}
			this.metaMoneyManager.OnStrategyStop();
			this.OnMetaStrategyStop();
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
			this.tester.Disconnect();
			foreach (LiveTester current2 in this.testers.Values)
			{
				current2.Disconnect();
			}
			this.DisconnectAllPortfolioStops();
			foreach (StrategyBase strategyBase in this.strategies)
			{
				if (strategyBase.IsEnabled)
				{
					strategyBase.StrategyStop();
				}
			}
			this.simulationManager.OnStrategyStop();
			this.optimizationManager.OnStrategyStop();
            /*------------------------------*/
            if (this.metaStrategyMode==MetaStrategyMode.Simulation)
            {
                this.simulator.EnterInterval -= new IntervalEventHandler(this.OnEnterInterval);
                this.simulator.LeaveInterval -= new IntervalEventHandler(this.OnLeaveInterval);
            }
            /*------------------------------*/
        }

        public void Pause()
		{
			if (this.metaStrategyMode == MetaStrategyMode.Simulation)
			{
				this.simulator.Pause();
			}
		}

		private void OnSimulatorStateChanged(object sender, EventArgs e)
		{
			if (this.isRunning && this.metaStrategyMode == MetaStrategyMode.Simulation)
			{
				switch (this.simulator.CurrentState)
				{
				case SimulatorState.Stopped:
					this.MetaStrategyStop();
					this.EmitStopped();
					break;
				case SimulatorState.Running:
					this.EmitStarted();
					return;
				case SimulatorState.Paused:
					this.EmitPaused();
					return;
				default:
					return;
				}
			}
		}

		protected void EmitChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this, EventArgs.Empty);
			}
		}

		protected void EmitComponentChanged(ComponentType componentType)
		{
			if (this.ComponentChanged != null)
			{
				this.ComponentChanged(this, new ComponentTypeEventArgs(componentType));
			}
		}

		private void EmitStarted()
		{
			if (this.Started != null)
			{
				this.Started(this, EventArgs.Empty);
			}
		}

		private void EmitStopped()
		{
			if (this.Stopped != null)
			{
				this.Stopped(this, EventArgs.Empty);
			}
		}

		private void EmitPaused()
		{
			if (this.Paused != null)
			{
				this.Paused(this, EventArgs.Empty);
			}
		}

		private void EmitDesignModeChanged()
		{
			if (this.DesignModeChanged != null)
			{
				this.DesignModeChanged(this, EventArgs.Empty);
			}
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

		public void Optimize()
		{
			if (this.isRunning)
			{
				throw new InvalidOperationException("MetaStrategy is not in design mode");
			}
			try
			{
				this.isOptimizing = true;
				this.PrepareTester();
				foreach (StrategyBase strategyBase in this.strategies)
				{
					if (strategyBase.IsEnabled)
					{
						strategyBase.PrepareTester();
					}
				}
				this.optimizer.Clear();
				this.optimizer.Optimize();
			}
			finally
			{
				this.RestoreTester();
				foreach (StrategyBase strategyBase2 in this.strategies)
				{
					if (strategyBase2.IsEnabled)
					{
						strategyBase2.RestoreTester();
					}
				}
				this.isOptimizing = false;
			}
		}

		internal void DrawPrimitive(Instrument instrument, IDrawable primitive, int padNumber)
		{
			if (!this.drawingPrimitives.ContainsKey(instrument))
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(primitive);
				SortedList sortedList = new SortedList();
				sortedList.Add(padNumber, arrayList);
				this.drawingPrimitives.Add(instrument, sortedList);
				return;
			}
			SortedList sortedList2 = this.drawingPrimitives[instrument] as SortedList;
			if (!sortedList2.ContainsKey(padNumber))
			{
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(primitive);
				sortedList2.Add(padNumber, arrayList2);
				return;
			}
			ArrayList arrayList3 = sortedList2[padNumber] as ArrayList;
			bool flag = true;
			if (primitive is TimeSeries)
			{
				for (int i = 0; i < arrayList3.Count; i++)
				{
					if (arrayList3[i] is TimeSeries && (primitive as TimeSeries).Name == (arrayList3[i] as TimeSeries).Name && (primitive as TimeSeries).Name != "")
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				arrayList3.Add(primitive);
			}
		}

		internal void EmitError(Exception exception)
		{
			if (Trace.IsLevelEnabled(TraceLevel.Error))
			{
				Trace.WriteLine(exception.ToString());
			}
			MetaStrategyErrorEventArgs metaStrategyErrorEventArgs = new MetaStrategyErrorEventArgs(exception);
			if (this.Error != null)
			{
				this.Error(metaStrategyErrorEventArgs);
			}
			if (!metaStrategyErrorEventArgs.Ignore)
			{
				this.Stop();
			}
		}

		protected virtual void OnTransactionAdded(Transaction transaction)
		{
		}

		protected virtual void OnPositionOpened(Position position)
		{
		}

		protected virtual void OnPositionChanged(Position position)
		{
		}

		protected virtual void OnPositionClosed(Position position)
		{
		}

		protected virtual void OnPortfolioValueChanged(Position position)
		{
		}

		private void portfolio_TransactionAdded(object sender, TransactionEventArgs args)
		{
			try
			{
				Transaction transaction = args.Transaction;
				if (this.statisticsPerInstrumentEnabled)
				{
					this.portfolios[transaction.Instrument].Add(transaction);
				}
				this.strategies[transaction.Strategy].SetTransactionAdded(transaction);
				this.OnTransactionAdded(transaction);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void portfolio_PositionOpened(object sender, PositionEventArgs args)
		{
			try
			{
				Position position = args.Position;
				this.metaMoneyManager.OnPositionOpened(position);
				this.OnPositionOpened(position);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void portfolio_PositionChanged(object sender, PositionEventArgs args)
		{
			try
			{
				Position position = args.Position;
				this.metaMoneyManager.OnPositionChanged(position);
				this.OnPositionChanged(position);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void portfolio_PositionClosed(object sender, PositionEventArgs args)
		{
			try
			{
				Position position = args.Position;
				this.metaMoneyManager.OnPositionClosed(position);
				this.OnPositionClosed(position);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void portfolio_ValueChanged(object sender, PositionEventArgs args)
		{
			try
			{
				Position position = args.Position;
				this.metaMoneyManager.OnPortfolioValueChanged(position.Portfolio);
				this.metaMoneyManager.OnPositionValueChanged(position);
				this.OnPortfolioValueChanged(position);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
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

		internal void SetNewBarOpen(BarEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Bar bar = args.Bar;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewBarOpen(instrument, bar);
				}
				this.metaMoneyManager.OnBarOpen(instrument, bar);
				this.OnNewBarOpen(instrument, bar);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewBar(BarEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Bar bar = args.Bar;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewBar(instrument, bar);
				}
				this.metaMoneyManager.OnBar(instrument, bar);
				this.OnNewBar(instrument, bar);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewBarSlice(BarSliceEventArgs args)
		{
			try
			{
				long barSize = args.BarSize;
				foreach (StrategyBase strategyBase in this.strategies)
				{
					if (strategyBase.IsEnabled)
					{
						strategyBase.SetNewBarSlice(barSize);
					}
				}
				this.OnNewBarSlice(barSize);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewTrade(TradeEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Trade trade = args.Trade;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewTrade(instrument, trade);
				}
				this.metaMoneyManager.OnTrade(instrument, trade);
				this.OnNewTrade(instrument, trade);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewQuote(QuoteEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Quote quote = args.Quote;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewQuote(instrument, quote);
				}
				this.metaMoneyManager.OnQuote(instrument, quote);
				this.OnNewQuote(instrument, quote);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewMarketDepth(MarketDepthEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				MarketDepth marketDepth = args.MarketDepth;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewMarketDepth(instrument, marketDepth);
				}
				this.metaMoneyManager.OnMarketDepth(instrument, marketDepth);
				this.OnNewMarketDepth(instrument, marketDepth);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewFundamental(FundamentalEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Fundamental fundamental = args.Fundamental;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewFundamental(instrument, fundamental);
				}
				this.metaMoneyManager.OnFundamental(instrument, fundamental);
				this.OnNewFundamental(instrument, fundamental);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetNewCorporateAction(CorporateActionEventArgs args)
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				CorporateAction corporateAction = args.CorporateAction;
				foreach (StrategyBase current in this.GetAffectedStrategies(args.Provider, instrument))
				{
					current.SetNewCorporateAction(instrument, corporateAction);
				}
				this.metaMoneyManager.OnCorporateAction(instrument, corporateAction);
				this.OnNewCorporateAction(instrument, corporateAction);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void RegisterStrategy(IMarketDataProvider provider, Instrument instrument, StrategyBase strategy)
		{
			Dictionary<Instrument, List<StrategyBase>> dictionary = null;
			if (!this.marketDataTable.TryGetValue(provider, out dictionary))
			{
				dictionary = new Dictionary<Instrument, List<StrategyBase>>();
				this.marketDataTable.Add(provider, dictionary);
			}
			List<StrategyBase> list = null;
			if (!dictionary.TryGetValue(instrument, out list))
			{
				list = new List<StrategyBase>();
				dictionary.Add(instrument, list);
			}
			if (!list.Contains(strategy))
			{
				list.Add(strategy);
			}
		}

		private List<StrategyBase> GetAffectedStrategies(IMarketDataProvider provider, Instrument instrument)
		{
			Dictionary<Instrument, List<StrategyBase>> dictionary = null;
			List<StrategyBase> result = null;
			if (this.marketDataTable.TryGetValue(provider, out dictionary))
			{
				if (!dictionary.TryGetValue(instrument, out result))
				{
					result = new List<StrategyBase>();
				}
			}
			else
			{
				result = new List<StrategyBase>();
			}
			return result;
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

		internal void SetNewOrder(OrderEventArgs args)
		{
			try
			{
				SingleOrder order = args.Order;
				StrategyBase strategyBase = null;
				if (this.orders.TryGetValue(order, out strategyBase))
				{
					strategyBase.SetNewOrder(order);
					this.OnNewOrder(order);
				}
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetExecutionReport(ExecutionReportEventArgs args)
		{
			try
			{
				ExecutionReport executionReport = args.ExecutionReport;
				OrdStatus ordStatus = executionReport.OrdStatus;
				switch (ordStatus)
				{
				case OrdStatus.Cancelled:
				case OrdStatus.PendingCancel:
					break;
				case OrdStatus.Replaced:
					goto IL_49;
				default:
					if (ordStatus != OrdStatus.PendingReplace)
					{
						goto IL_49;
					}
					break;
				}
				SingleOrder singleOrder = OrderManager.Orders.All[executionReport.OrigClOrdID] as SingleOrder;
				goto IL_64;
				IL_49:
				singleOrder = (OrderManager.Orders.All[executionReport.ClOrdID] as SingleOrder);
				IL_64:
				if (singleOrder != null)
				{
					StrategyBase strategyBase = null;
					if (this.orders.TryGetValue(singleOrder, out strategyBase))
					{
						strategyBase.SetExecutionReport(singleOrder, executionReport);
						this.OnExecutionReport(singleOrder, executionReport);
						if (singleOrder.OrdStatus == OrdStatus.PartiallyFilled)
						{
							strategyBase.SetOrderPartiallyFilled(singleOrder);
							this.OnOrderPartiallyFilled(singleOrder);
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetOrderStatusChanged(OrderEventArgs args)
		{
			try
			{
				SingleOrder order = args.Order;
				StrategyBase strategyBase = null;
				if (this.orders.TryGetValue(order, out strategyBase))
				{
					strategyBase.SetOrderStatusChanged(order);
					this.OnOrderStatusChanged(order);
				}
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetOrderDone(OrderEventArgs args)
		{
			try
			{
				SingleOrder order = args.Order;
				StrategyBase strategyBase = null;
				if (this.orders.TryGetValue(order, out strategyBase))
				{
					strategyBase.SetOrderDone(order);
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
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
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

		internal void SetProviderConnected(ProviderEventArgs args)
		{
			try
			{
				IProvider provider = args.Provider;
				foreach (StrategyBase current in this.GetAffectedStrategies(provider))
				{
					current.SetProviderConnected(provider);
				}
				this.metaMoneyManager.OnProviderConnected(provider);
				this.OnProviderConnected(provider);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetProviderDisconnected(ProviderEventArgs args)
		{
			try
			{
				IProvider provider = args.Provider;
				foreach (StrategyBase current in this.GetAffectedStrategies(provider))
				{
					current.SetProviderDisconnected(provider);
				}
				this.metaMoneyManager.OnProviderDisconnected(provider);
				this.OnProviderDisconnected(provider);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetProviderError(ProviderErrorEventArgs args)
		{
			try
			{
				IProvider provider = args.Error.Provider;
				int id = args.Error.Id;
				int code = args.Error.Code;
				string message = args.Error.Message;
				foreach (StrategyBase current in this.GetAffectedStrategies(provider))
				{
					current.SetProviderError(provider, id, code, message);
				}
				this.metaMoneyManager.OnProviderError(provider, id, code, message);
				this.OnProviderError(provider, id, code, message);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void RegisterStrategy(IProvider provider, StrategyBase strategy)
		{
			List<StrategyBase> list = null;
			if (!this.providerDispatcherTable.TryGetValue(provider, out list))
			{
				list = new List<StrategyBase>();
				this.providerDispatcherTable.Add(provider, list);
			}
			if (!list.Contains(strategy))
			{
				list.Add(strategy);
			}
		}

		private List<StrategyBase> GetAffectedStrategies(IProvider provider)
		{
			return this.providerDispatcherTable[provider];
		}

		protected virtual void OnServiceStatusChanged(IService service)
		{
		}

		protected virtual void OnServiceError(IService service, ServiceErrorType errorType, string text)
		{
		}

		internal void SetServiceStatusChanged(ServiceEventArgs args)
		{
			try
			{
				IService service = args.Service;
				foreach (StrategyBase current in this.GetAffectedStrategies(service))
				{
					current.SetServiceStatusChanged(service);
				}
				this.OnServiceStatusChanged(service);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		internal void SetServiceError(ServiceErrorEventArgs args)
		{
			try
			{
				IService service = args.Error.Service;
				ServiceErrorType errorType = args.Error.ErrorType;
				string text = args.Error.Text;
				foreach (StrategyBase current in this.GetAffectedStrategies(service))
				{
					current.SetServiceError(service, errorType, text);
				}
				this.OnServiceError(service, errorType, text);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private void RegisterStrategy(IService service, StrategyBase strategy)
		{
			List<StrategyBase> list;
			if (!this.serviceDispatcherTable.TryGetValue(service, out list))
			{
				list = new List<StrategyBase>();
				this.serviceDispatcherTable.Add(service, list);
			}
			if (!list.Contains(strategy))
			{
				list.Add(strategy);
			}
			this.serviceDispatcher.Add(service);
			if (service is IExecutionService)
			{
				IExecutionService executionService = service as IExecutionService;
				if (!this.executionServiceDispatcherTable.TryGetValue(executionService, out list))
				{
					list = new List<StrategyBase>();
					this.executionServiceDispatcherTable.Add(executionService, list);
				}
				if (!list.Contains(strategy))
				{
					list.Add(strategy);
				}
				this.executionServiceDispatcher.Add(executionService);
			}
		}

		private List<StrategyBase> GetAffectedStrategies(IService service)
		{
			return this.serviceDispatcherTable[service];
		}

		protected virtual void OnNewClientOrder(SingleOrder order)
		{
		}

		internal void SetNewClientOrder(NewOrderSingleEventArgs args)
		{
			try
			{
				SingleOrder singleOrder = args.Order as SingleOrder;
				singleOrder.StrategyMode = MetaStrategyBase.FIX_MODES[this.metaStrategyMode];
				IExecutionService service = args.Service;
				foreach (StrategyBase current in this.GetAffectedStrategies(service))
				{
					current.SetNewClientOrder(singleOrder);
				}
				this.OnNewClientOrder(singleOrder);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}

		private List<StrategyBase> GetAffectedStrategies(IExecutionService service)
		{
			return this.executionServiceDispatcherTable[service];
		}

		internal void RegisterOrder(StrategyBase strategy, SingleOrder order)
		{
			order.Portfolio = this.portfolio;
			switch (this.metaStrategyMode)
			{
			case MetaStrategyMode.Simulation:
				order.Provider = ProviderManager.ExecutionSimulator;
				break;
			case MetaStrategyMode.Live:
				order.Provider = strategy.MarketManager.ExecutionProviderTable[order.Instrument];
				if (this.saveOrders)
				{
					order.Persistent = true;
				}
				break;
			}
			order.StrategyMode = MetaStrategyBase.FIX_MODES[this.metaStrategyMode];
			this.orders.Add(order, strategy);
		}

		internal void SendExecutionReport(StrategyBase strategy, ExecutionReport report)
		{
			IExecutionService service = null;
			switch (this.metaStrategyMode)
			{
			case MetaStrategyMode.Simulation:
				service = ServiceManager.ExecutionSimulator;
				break;
			case MetaStrategyMode.Live:
				service = strategy.ExecutionService;
				break;
			}
			OrderManager.SellSide.SendExecutionReport(service, report);
		}

		private void optimizer_BestObjectiveReceived(object sender, EventArgs e)
		{
			this.RestoreTester();
			this.isOptimizing = false;
		}

		public virtual ArrayList GetOptimizationParameters()
		{
			this.optimizationParemeters.Clear();
			int num = 0;
			foreach (ComponentType current in this.componentTypeList)
			{
				IComponentBase component = this.GetComponent(current);
				string str = current + ".";
				PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					object[] customAttributes = propertyInfo.GetCustomAttributes(false);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						Attribute attribute = (Attribute)customAttributes[j];
						if (attribute.GetType() == typeof(OptimizationParameterAttribute))
						{
							this.optimizationParemeters.Add(str + propertyInfo.Name);
							num++;
						}
					}
				}
			}
			foreach (StrategyBase strategyBase in this.strategies)
			{
				if (strategyBase.IsEnabled)
				{
					foreach (ComponentType current2 in strategyBase.ComponentTypeList)
					{
						IComponentBase component = strategyBase.GetComponent(current2);
						string str2 = current2 + ".";
						PropertyInfo[] properties2 = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
						for (int k = 0; k < properties2.Length; k++)
						{
							PropertyInfo propertyInfo2 = properties2[k];
							object[] customAttributes2 = propertyInfo2.GetCustomAttributes(false);
							for (int l = 0; l < customAttributes2.Length; l++)
							{
								Attribute attribute2 = (Attribute)customAttributes2[l];
								if (attribute2.GetType() == typeof(OptimizationParameterAttribute))
								{
									this.optimizationParemeters.Add(str2 + propertyInfo2.Name);
									num++;
								}
							}
						}
					}
				}
			}
			return this.optimizationParemeters;
		}

		public double Objective()
		{
			this.Start(false);
			return -this.optimizationManager.Objective();
		}

		public virtual void Init(ParamSet paramSet)
		{
			int num = 0;
			foreach (ComponentType current in this.componentTypeList)
			{
				IComponentBase component = this.GetComponent(current);
				string str = current + ".";
				PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					if (this.optimizationParemeters.Contains(str + propertyInfo.Name))
					{
						object[] customAttributes = propertyInfo.GetCustomAttributes(false);
						for (int j = 0; j < customAttributes.Length; j++)
						{
							Attribute attribute = (Attribute)customAttributes[j];
							if (attribute.GetType() == typeof(OptimizationParameterAttribute))
							{
								OptimizationParameterAttribute optimizationParameterAttribute = attribute as OptimizationParameterAttribute;
								paramSet.SetParamName(num, str + propertyInfo.Name);
								paramSet.SetParam(num, optimizationParameterAttribute.LowerBound + (optimizationParameterAttribute.UpperBound - optimizationParameterAttribute.LowerBound) / 2.0);
								paramSet.SetLowerBound(num, optimizationParameterAttribute.LowerBound);
								paramSet.SetUpperBound(num, optimizationParameterAttribute.UpperBound);
								paramSet.SetSteps(num, optimizationParameterAttribute.Step);
								if (propertyInfo.PropertyType == typeof(int))
								{
									paramSet.SetParamType(num, EParamType.Int);
								}
								else
								{
									paramSet.SetParamType(num, EParamType.Float);
								}
								num++;
							}
						}
					}
				}
			}
			foreach (StrategyBase strategyBase in this.strategies)
			{
				if (strategyBase.IsEnabled)
				{
					foreach (ComponentType current2 in strategyBase.ComponentTypeList)
					{
						IComponentBase component = strategyBase.GetComponent(current2);
						string str2 = current2 + ".";
						PropertyInfo[] properties2 = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
						for (int k = 0; k < properties2.Length; k++)
						{
							PropertyInfo propertyInfo2 = properties2[k];
							if (this.optimizationParemeters.Contains(str2 + propertyInfo2.Name))
							{
								object[] customAttributes2 = propertyInfo2.GetCustomAttributes(false);
								for (int l = 0; l < customAttributes2.Length; l++)
								{
									Attribute attribute2 = (Attribute)customAttributes2[l];
									if (attribute2.GetType() == typeof(OptimizationParameterAttribute))
									{
										OptimizationParameterAttribute optimizationParameterAttribute2 = attribute2 as OptimizationParameterAttribute;
										paramSet.SetParamName(num, str2 + propertyInfo2.Name);
										paramSet.SetParam(num, optimizationParameterAttribute2.LowerBound + (optimizationParameterAttribute2.UpperBound - optimizationParameterAttribute2.LowerBound) / 2.0);
										paramSet.SetLowerBound(num, optimizationParameterAttribute2.LowerBound);
										paramSet.SetUpperBound(num, optimizationParameterAttribute2.UpperBound);
										paramSet.SetSteps(num, optimizationParameterAttribute2.Step);
										if (propertyInfo2.PropertyType == typeof(int))
										{
											paramSet.SetParamType(num, EParamType.Int);
										}
										else
										{
											paramSet.SetParamType(num, EParamType.Float);
										}
										num++;
									}
								}
							}
						}
					}
				}
			}
		}

		public virtual void Update(ParamSet paramSet)
		{
			int num = 0;
			foreach (ComponentType current in this.componentTypeList)
			{
				IComponentBase component = this.GetComponent(current);
				string str = current + ".";
				PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					if (this.optimizationParemeters.Contains(str + propertyInfo.Name))
					{
						object[] customAttributes = propertyInfo.GetCustomAttributes(false);
						for (int j = 0; j < customAttributes.Length; j++)
						{
							Attribute attribute = (Attribute)customAttributes[j];
							if (attribute.GetType() == typeof(OptimizationParameterAttribute))
							{
								if (propertyInfo.PropertyType == typeof(int))
								{
									propertyInfo.SetValue(component, (int)paramSet.GetParam(num), null);
								}
								if (propertyInfo.PropertyType == typeof(double))
								{
									propertyInfo.SetValue(component, paramSet.GetParam(num), null);
								}
								num++;
							}
						}
					}
				}
			}
			foreach (StrategyBase strategyBase in this.strategies)
			{
				if (strategyBase.IsEnabled)
				{
					foreach (ComponentType current2 in strategyBase.ComponentTypeList)
					{
						IComponentBase component = strategyBase.GetComponent(current2);
						string str2 = current2 + ".";
						PropertyInfo[] properties2 = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
						for (int k = 0; k < properties2.Length; k++)
						{
							PropertyInfo propertyInfo2 = properties2[k];
							if (this.optimizationParemeters.Contains(str2 + propertyInfo2.Name))
							{
								object[] customAttributes2 = propertyInfo2.GetCustomAttributes(false);
								for (int l = 0; l < customAttributes2.Length; l++)
								{
									Attribute attribute2 = (Attribute)customAttributes2[l];
									if (attribute2.GetType() == typeof(OptimizationParameterAttribute))
									{
										if (propertyInfo2.PropertyType == typeof(int))
										{
											propertyInfo2.SetValue(component, (int)paramSet.GetParam(num), null);
										}
										if (propertyInfo2.PropertyType == typeof(double))
										{
											propertyInfo2.SetValue(component, paramSet.GetParam(num), null);
										}
										num++;
									}
								}
							}
						}
					}
				}
			}
		}

		public void OnStep()
		{
		}

		public void OnCircle()
		{
		}

		public virtual IComponentBase GetComponent(ComponentType type)
		{
			if (type <= ComponentType.SimulationManager)
			{
				if (type == ComponentType.OptimizationManager)
				{
					return this.OptimizationManager;
				}
				if (type == ComponentType.SimulationManager)
				{
					return this.SimulationManager;
				}
			}
			else
			{
				if (type == ComponentType.MetaMoneyManager)
				{
					return this.MetaMoneyManager;
				}
				if (type == ComponentType.ReportManager)
				{
					return this.ReportManager;
				}
			}
			throw new InvalidOperationException("Invalid metastrategy component type");
		}

		public virtual void SetComponent(ComponentType type, IComponentBase component)
		{
			if (type <= ComponentType.SimulationManager)
			{
				if (type == ComponentType.OptimizationManager)
				{
					this.OptimizationManager = (component as OptimizationManager);
					return;
				}
				if (type == ComponentType.SimulationManager)
				{
					this.SimulationManager = (component as SimulationManager);
					return;
				}
			}
			else
			{
				if (type == ComponentType.MetaMoneyManager)
				{
					this.MetaMoneyManager = (component as MetaMoneyManager);
					return;
				}
				if (type == ComponentType.ReportManager)
				{
					this.ReportManager = (component as ReportManager);
					return;
				}
			}
			throw new InvalidOperationException("Invalid metastrategy component type");
		}
	}
}
