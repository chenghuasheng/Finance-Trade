using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Execution;
using SmartQuant.Testing;
using SmartQuant.Simulation;

public class Strategy{
	//需要订阅数据的证券工具
    protected InstrumentList activeInstruments;
	//执行提供者
	protected IExecutionProvider executionProvider;
	public IExecutionProvider ExecutionProvider
	{
		get{ return this.executionProvider; }
		set{ this.executionProvider = value; }
	}
    //市场数据提供者
	protected IMarketDataProvider marketDataProvider;
	public IMarketDataProvider MarketDataProvider
	{
		get{ return this.marketDataProvider; }
		set{ this.marketDataProvider = value; }
	}
	//策略名称
	protected string name;
	public string Name
	{
		get{return this.name;}
	}
	//策略描述
	protected string description;
	//投资组合
	protected Portfolio portfolio;
	public Portfolio Portfolio
	{
		get{ return this.portfolio; }
		set{ this.portfolio = value; }
	}
	//测试者
	protected LiveTester tester;
	public LiveTester Tester
	{
		get{ return this.tester; }
	}
	//模拟者
	protected Simulator simulator;
	//模拟采用的费用提供者
	public ICommissionProvider CommissionProvider
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).CommissionProvider;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).CommissionProvider = value;
		}
	}
	//模拟采用的滑价提供者
	public ISlippageProvider SlippageProvider
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).SlippageProvider;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).SlippageProvider = value;
		}
	}
	//可以在Quote上成交
	public bool FillOnQuote
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuote;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuote = value;
		}
	}
	//可以在Trade上成交
	public bool FillOnTrade
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTrade;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTrade = value;
		}
	}
	//可以在Bar上成交
	public bool FillOnBar
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBar;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBar = value;
		}
	}
	//Trade成交模式
	public FillOnTradeMode FillOnTradeMode
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTradeMode;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTradeMode = value;
		}
	}
	//Quote成交模式
	public FillOnQuoteMode FillOnQuoteMode
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuoteMode;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuoteMode = value;
		}
	}
	//Bar成交模式
	public FillOnBarMode FillOnBarMode
	{
		get
		{
			return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBarMode;
		}
		set
		{
			(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBarMode = value;
		}
	}
	//是否重置投资组合
	protected bool resetPortfolio;
	public bool ResetPortfolio
	{
		get{ return this.resetPortfolio;}
		set{ this.resetPortfolio = value;}
	}
	
	//请求的数据名称串
    protected List<string> requests;
	//策略执行模式
    protected StrategyMode strategyMode = StrategyMode.Simulation;
	public StrategyMode StrategyMode
	{
		get{ return this.strategyMode;}
		set{ this.strategyMode = value;}
	}
	//模拟开始时间
	protected DateTime simulationEntryDate=new DateTime(1970,1,1);
	public DateTime SimulationEntryDate {
		get { return this.simulationEntryDate; }
		set { this.simulationEntryDate=value; }
	}
	//模拟结束时间
	protected DateTime simulationExitDate=DateTime.Today;
	public DateTime SimulationExitDate {
		get { return this.simulationExitDate; }
		set { this.simulationExitDate=value; }
	}
	//模拟方式
	protected SimulationMode simulationMode=SimulationMode.MaxSpeed;
	public SimulationMode SimulationMode {
		get {return this.simulationMode;}
		set {this.simulationMode=value;}
	}
	//模拟倍速
	protected double simulationSpeedMultiplier=1;
	//模拟每步骤的跨越时间（秒）
	protected int simulationStep=86400;
	//模拟初始资金
	protected double simulationCash=20000;
	public double SimulationCash{
		get { return this.simulationCash; }
		set { this.simulationCash=value; }
	}
	//模拟货币
	protected Currency simulationCurrency=Currency.USD;	
	
	protected Hashtable modes;
	//记录订单
	protected List<SingleOrder> orders;
	//记录买卖信号
	protected SignalList signals;
	//是否保存订单
	protected bool saveOrders=false;
	public bool SaveOrders
	{
		get{ return this.saveOrders;}
		set{ this.saveOrders = value;}
	}
	//全局变量集合
	public Hashtable Global;
	//连接最大超时时间
	protected int maxConnectionTime=10;
	public int MaxConnectionTime
	{
		get{return this.maxConnectionTime;}
		set{
			if ((value < 1) || (value > 60))
			{
				throw new ArgumentOutOfRangeException("MaxConnectionTime", "The value must be in range 1..60");
			}
			this.maxConnectionTime = value;
		}
	}
    //所有订阅证券的数据Bar集合
	public BarSeriesList Bars
	{
		get {return DataManager.Bars;}
	}
	public TradeArrayList Trades
	{
		get { return DataManager.Trades; }
	}
	
	public QuoteArrayList Quotes
	{
		get { return DataManager.Quotes; }
	}
	//指示当天是否开市
	protected bool marketOpen=true;
	public bool MarketOpen{
		get { return this.marketOpen;}
		set { this.marketOpen=value;}
	}
	//指示是否每天改变市场
	protected bool changeMarketOnDay=false;
	public bool ChangeMarketOnDay{
		get { return this.changeMarketOnDay;}
		set { this.changeMarketOnDay=value;}
	}
	private ReportManager reportManager;
	//指示在模拟的时候是否检查资金购买能力
	private bool checkBuyPower=false;
	public bool CheckBuyPower{
		get { return this.checkBuyPower;}
		set { this.checkBuyPower=value;}
	}
	/*仓位水平*/
	protected double positionLevel=1.0;
	public double PositionLevel{
		get {return this.positionLevel;}
		set {this.positionLevel=value;}
	}
	protected bool isRunning;
	//构造函数
	public Strategy(string name, string description){
		this.name = name;
		this.description = description;
		this.portfolio = PortfolioManager.Portfolios[name];
		if (this.portfolio == null) this.portfolio = new Portfolio(name);
		this.tester = new LiveTester(this.portfolio);
		this.tester.FollowChanges = true;
		this.marketDataProvider = null;
		this.executionProvider = null;
		this.activeInstruments = new InstrumentList();
		this.resetPortfolio = true;
		this.simulator = (ProviderManager.MarketDataSimulator as SimulationDataProvider).Simulator;
		this.simulator.StateChanged += new EventHandler(this.OnSimulatorStateChanged);
		this.reportManager=new ReportManager();
		this.isRunning = false;
		this.requests = new List<string>();
		this.signals = new SignalList();
		this.orders=new List<SingleOrder>();
		this.modes = new Hashtable();
		this.modes.Add(StrategyMode.Simulation, 'S');
		this.modes.Add(StrategyMode.Live, 'L');
		this.Global=new Hashtable();
    }
    //策略开始执行
	public void Start(){
		this.orders.Clear();
		this.providers.Clear();
		this.orderProcessors.Clear();
		this.Global.Clear();
		/*检测数据提供者与执行提供者*/
		if (this.strategyMode==StrategyMode.Simulation){
			this.marketDataProvider=ProviderManager.MarketDataSimulator;
			this.executionProvider=ProviderManager.ExecutionSimulator;		
		}
		if (this.marketDataProvider==null) {
			this.EmitError(new Exception("MarketDataProvider is NULL."));
			return;
		}else {
			this.AddProviderDispatcher(this.marketDataProvider);
		}
		if (this.executionProvider==null){
			this.EmitError(new Exception("ExecutionProvider is NULL"));
			return;
		}else {
			this.AddProviderDispatcher(this.executionProvider);
			this.AddOrderProcessor(this.executionProvider);
		}
		/*提供者上线*/
		this.ProviderDispatcherOnline(this.maxConnectionTime * 0x3e8);//超时等待N秒	
		/*模拟器初始化*/
		if (this.strategyMode == StrategyMode.Simulation)
		{
			this.requests.Clear();
			this.SimulationInit();
			Clock.ClockMode = ClockMode.Simulation;
			Clock.SetDateTime(this.simulationEntryDate);
			this.simulator.SimulationMode = this.simulationMode;
			this.simulator.SpeedMultiplier = this.simulationSpeedMultiplier;
			this.simulator.Step = this.simulationStep;		
			OrderManager.RemoveOrders(11104, this.modes[this.strategyMode]);
			OrderManager.OCA.Clear();
			OrderManager.SellSide.RemoveOrders(11104, this.modes[this.strategyMode]);
		}
		/*为模拟执行提供者设置费用、滑价、成交模式*/
		if (this.executionProvider is SimulationExecutionProvider)
		{
			CommissionProvider commissionor=this.CommissionProvider as CommissionProvider;//费用提供者
			commissionor.Commission=0.00025;//万分之2.5
			commissionor.CommType=CommType.Percent;
			commissionor.MinCommission=5;//每笔最小费用
			(this.SlippageProvider as SlippageProvider).Slippage=0.002;//滑价比率
			this.FillOnBar=false;
			this.FillOnQuote=false;
			this.FillOnTrade=true;
			this.FillOnTradeMode=FillOnTradeMode.NextTrade;	
		}
		
		/*投资组合初始化和上线*/
        if (this.resetPortfolio) this.portfolio.Clear();
		this.tester.Disconnect();
		this.tester.Connect();
		this.tester.Reset();
		bool flag = this.portfolio.Account.Transactions.Count == 0;
		if (flag&&(this.executionProvider is SimulationExecutionProvider))
		{
			this.portfolio.Account.Deposit(this.simulationCash, this.simulationCurrency, Clock.Now, "Initial Cash Allocation");
		}
        this.portfolio.TransactionAdded += new TransactionEventHandler(this.portfolio_TransactionAdded);
        this.portfolio.PositionOpened += new PositionEventHandler(this.portfolio_PositionOpened);
        this.portfolio.PositionChanged += new PositionEventHandler(this.portfolio_PositionChanged);
        this.portfolio.PositionClosed += new PositionEventHandler(this.portfolio_PositionClosed);
        this.portfolio.ValueChanged += new PositionEventHandler(this.portfolio_ValueChanged);
        this.portfolio.Monitored = true;
		this.reportManager.Tester = this.tester;
		this.reportManager.Init();
		
		//调用初始化事件
        this.OnInit();
		/*订单处理上线*/
		this.OrderProcessorOnline();
		//开始执行
		this.isRunning = true;
		if (this.changeMarketOnDay){
			DateTime curDate;
			switch (this.strategyMode)
			{	
				case StrategyMode.Simulation:
					curDate=this.simulationEntryDate.Date;
					this.simulator.Intervals.Clear();
					while (curDate<this.simulationExitDate){						
						DateTime nextDate=curDate.AddDays(1);		
						this.simulator.Intervals.Add(curDate, nextDate);
						curDate=nextDate;						
					}
					this.simulator.EnterInterval+=new IntervalEventHandler(this.OnEnterInterval);
					this.simulator.LeaveInterval+=new IntervalEventHandler(this.OnLeaveInterval);
					this.simulator.Start(true);
					return;
				case StrategyMode.Live:
					curDate=Clock.Now.Date;
					this.ConnectMarketAndBehavior();
					this.EmitStarted();
					while (this.isRunning)
					{
						Thread.Sleep(1);
						if (this.isRunning&&curDate<Clock.Now.Date){
							this.DisconnectMarketAndBehavior();
							this.ConnectMarketAndBehavior();
							curDate=Clock.Now.Date;
						}
					}
					return;
			}
		}else {
			//发出市场数据请求
			this.ConnectMarketAndBehavior();		
			//等待事件监听		
			switch (this.strategyMode)
			{
				case StrategyMode.Simulation:
					this.simulator.Intervals.Clear();
					this.simulator.Intervals.Add(this.simulationEntryDate, this.simulationExitDate);
					this.simulator.Start(true);
					return;
				case StrategyMode.Live:
					this.EmitStarted();
					while (this.isRunning)
					{
						Thread.Sleep(1);
					}
					return;
			}
		}
    }
	//连接市场数据
	private void OnEnterInterval(IntervalEventArgs args){
		this.tester.WaitingForStart=true;//当simulator调用了FireAllReminder后，必须重设tester的WaitingForStart属性才能继续测试统计
		this.ConnectMarketAndBehavior();
	}
	private void OnLeaveInterval(IntervalEventArgs args){	
		this.DisconnectMarketAndBehavior();
	}
	private void ConnectMarketAndBehavior(){
		this.MarketInit();
		this.BehaviorInit();
		foreach(Position pos in this.portfolio.Positions){
			this.AddInstrument(pos.Instrument);
		}
		foreach(Instrument inst in this.activeInstruments){
			switch (this.strategyMode)
			{
				case StrategyMode.Simulation:
					foreach (string str2 in this.requests)
					{
						this.AddMarketDataDispatcher(ProviderManager.MarketDataSimulator, inst, str2);
					}
					break;
				case StrategyMode.Live:
				{
					IMarketDataProvider provider2 = this.marketDataProvider;
					this.AddMarketDataDispatcher(provider2, inst, null);
					break;
				}
			}
		}	
		this.MarketDataDispatcherOnline();
	}
	private void DisconnectMarketAndBehavior(){
		this.MarketDataDispatcherOffline();
		this.requestDict.Clear();
		//新加，调用每一个行为的Close方法
		foreach(KeyValuePair<Instrument,List<Behavior>> kvp in this.behaviorDict){
			foreach(Behavior behavior in kvp.Value) behavior.Close();
		}
		//--------------------------------
		this.behaviorDict.Clear();
		DataManager.ClearDataArrays();//清理上一周期的数据
		this.activeInstruments.Clear();
	}
	/*订阅市场数据的分发器操作*/
	private Dictionary<IMarketDataProvider, Dictionary<Instrument, List<string>>> requestDict=new Dictionary<IMarketDataProvider, Dictionary<Instrument, List<string>>>();
	private void AddMarketDataDispatcher(IMarketDataProvider provider, Instrument instrument, string seriesSuffix)
	{
		Dictionary<Instrument, List<string>> dictionary = null;
		if (!this.requestDict.TryGetValue(provider, out dictionary))
		{
			dictionary = new Dictionary<Instrument, List<string>>();
			this.requestDict.Add(provider, dictionary);
		}
		List<string> list = null;
		if (!dictionary.TryGetValue(instrument, out list))
		{
			list = new List<string>();
			dictionary.Add(instrument, list);
		}
		if (!list.Contains(seriesSuffix))
		{
			list.Add(seriesSuffix);
		}
	}
	private void MarketDataDispatcherOnline(){
		foreach (KeyValuePair<IMarketDataProvider, Dictionary<Instrument, List<string>>> pair in this.requestDict)
		{
			IMarketDataProvider key = pair.Key;
			foreach (KeyValuePair<Instrument, List<string>> pair2 in pair.Value)
			{
				Instrument instrument = pair2.Key;
				foreach (string str in pair2.Value)
				{
					if (str == null)
					{
						instrument.RequestMarketData(key, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade);
					}
					else
					{
						instrument.RequestMarketData(key, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade, str);
					}
				}
			}
		}
		ProviderManager.NewBar += new BarEventHandler(this.SetNewBar);
		ProviderManager.NewBarOpen += new BarEventHandler(this.SetNewBarOpen);
		ProviderManager.NewTrade += new TradeEventHandler(this.SetNewTrade);
		ProviderManager.NewQuote += new QuoteEventHandler(this.SetNewQuote);
		ProviderManager.NewMarketDepth += new MarketDepthEventHandler(this.SetNewMarketDepth);
		ProviderManager.NewFundamental += new FundamentalEventHandler(this.SetNewFundamental);
		ProviderManager.NewCorporateAction += new CorporateActionEventHandler(this.SetNewCorporateAction);
		ProviderManager.NewBarSlice += new BarSliceEventHandler(this.SetNewBarSlice);
	}
	private void MarketDataDispatcherOffline(){
		ProviderManager.NewBar -= new BarEventHandler(this.SetNewBar);
		ProviderManager.NewBarOpen -= new BarEventHandler(this.SetNewBarOpen);
		ProviderManager.NewTrade -= new TradeEventHandler(this.SetNewTrade);
		ProviderManager.NewQuote -= new QuoteEventHandler(this.SetNewQuote);
		ProviderManager.NewMarketDepth -= new MarketDepthEventHandler(this.SetNewMarketDepth);
		ProviderManager.NewFundamental -= new FundamentalEventHandler(this.SetNewFundamental);
		ProviderManager.NewCorporateAction -= new CorporateActionEventHandler(this.SetNewCorporateAction);
		ProviderManager.NewBarSlice -= new BarSliceEventHandler(this.SetNewBarSlice);
		foreach (KeyValuePair<IMarketDataProvider, Dictionary<Instrument, List<string>>> pair in this.requestDict)
		{
			IMarketDataProvider key = pair.Key;
			if (key.BarFactory != null)
			{
				key.BarFactory.Reset();
			}
			foreach (KeyValuePair<Instrument, List<string>> pair2 in pair.Value)
			{
				Instrument instrument = pair2.Key;
				foreach (string str in pair2.Value)
				{
					if (str == null)
					{
						instrument.CancelMarketData(key, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade);
					}
					else
					{
						instrument.CancelMarketData(key, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade, str);
					}
				}
			}
		}
	}
	private bool HandleMarketData(IMarketDataProvider provider, IFIXInstrument instrument)
	{
		Dictionary<Instrument, List<string>> dictionary = null;
		return (this.requestDict.TryGetValue(provider, out dictionary) && dictionary.ContainsKey(instrument as Instrument));
	}
	private bool HandleMarketData(IMarketDataProvider provider)
	{
		return this.requestDict.ContainsKey(provider);
	}
    /*订单执行回报的操作*/
	private List<IExecutionProvider> orderProcessors=new List<IExecutionProvider>();
	private void AddOrderProcessor(IExecutionProvider provider){
		if (!this.orderProcessors.Contains(provider))
		{
			this.orderProcessors.Add(provider);
		}
	}
	private void OrderProcessorOnline(){
		OrderManager.NewOrder += new OrderEventHandler(this.SetNewOrder);
		OrderManager.ExecutionReport += new ExecutionReportEventHandler(this.SetExecutionReport);
		OrderManager.OrderStatusChanged += new OrderEventHandler(this.SetOrderStatusChanged);
		OrderManager.OrderDone += new OrderEventHandler(this.SetOrderDone);
	}
	private void OrderProcessorOffline(){
		OrderManager.NewOrder -= new OrderEventHandler(this.SetNewOrder);
		OrderManager.ExecutionReport -= new ExecutionReportEventHandler(this.SetExecutionReport);
		OrderManager.OrderStatusChanged -= new OrderEventHandler(this.SetOrderStatusChanged);
		OrderManager.OrderDone -= new OrderEventHandler(this.SetOrderDone);
	}
    /*提供者分发器*/
	private List<IProvider> providers=new List<IProvider>();
	private void AddProviderDispatcher(IProvider provider)
	{
		if (!this.providers.Contains(provider))
		{
			this.providers.Add(provider);
		}
	}
	private void ProviderDispatcherOnline(int timeout){
		ProviderManager.Connected += new ProviderEventHandler(this.SetProviderConnected);
		ProviderManager.Disconnected += new ProviderEventHandler(this.SetProviderDisconnected);
		ProviderManager.Error += new ProviderErrorEventHandler(this.SetProviderError);
		foreach (IProvider provider in this.providers)
		{
			if (!provider.IsConnected)
			{
				provider.Connect(timeout);
			}
		}
	}
	private void ProviderDispatcherOffline(){
		ProviderManager.Connected -= new ProviderEventHandler(this.SetProviderConnected);
		ProviderManager.Disconnected -= new ProviderEventHandler(this.SetProviderDisconnected);
		ProviderManager.Error -= new ProviderErrorEventHandler(this.SetProviderError);
		foreach (IProvider provider in this.providers)
		{
			if (provider.IsConnected)
			{
				provider.Disconnect();
			}
		}
	}
	//策略结束执行
	public void Stop()
	{
		if (this.strategyMode == StrategyMode.Simulation)
		{
			this.simulator.Stop(true);
		}
		else
		{
			this.StrategyStop();
		}
	}
	protected void StrategyStop(){
		this.isRunning = false;
		this.DisconnectMarketAndBehavior();
		this.OrderProcessorOffline();
		this.ProviderDispatcherOffline();
		this.OnStrategyStop();	
		this.portfolio.TransactionAdded -= new TransactionEventHandler(this.portfolio_TransactionAdded);
		this.portfolio.PositionOpened -= new PositionEventHandler(this.portfolio_PositionOpened);
		this.portfolio.PositionChanged -= new PositionEventHandler(this.portfolio_PositionChanged);
		this.portfolio.PositionClosed -= new PositionEventHandler(this.portfolio_PositionClosed);
		this.portfolio.ValueChanged -= new PositionEventHandler(this.portfolio_ValueChanged);
		this.portfolio.Monitored = false;
		this.ShowPerformance(this.portfolio,this.tester);
		//this.ShowPerformanceAnalysis(this.tester);
		this.ShowRoundTripsStatistics(this.tester);
		this.tester.Disconnect();
		if (this.changeMarketOnDay){
			this.simulator.EnterInterval-=new IntervalEventHandler(this.OnEnterInterval);
			this.simulator.LeaveInterval-=new IntervalEventHandler(this.OnLeaveInterval);
		}
		if (this.strategyMode==StrategyMode.Simulation){
			this.simulator.StateChanged -= new EventHandler(this.OnSimulatorStateChanged);
		}
		this.portfolio = null;
	}
	protected virtual void MarketInit(){
	}
	protected virtual void BehaviorInit()
	{
	}
    protected virtual void SimulationInit()
    {
    }
	protected Dictionary<Instrument,List<Behavior>> behaviorDict=new Dictionary<Instrument,List<Behavior>>();	
	public void AddBehavior(Instrument instrument,Behavior behavior){
		List<Behavior> list=null;
		if (!this.behaviorDict.TryGetValue(instrument,out list)){
			list=new List<Behavior> ();
			this.behaviorDict.Add(instrument,list);
		}
		if (!list.Contains(behavior)){
			list.Add(behavior);
		}
	}
    protected void SendMarketDataRequest(string request)
    {
        if (!this.requests.Contains(request))
        {
            this.requests.Add(request);
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
	protected void RegisterOrder(SingleOrder order)
	{
		order.Portfolio = this.portfolio;
		switch (this.strategyMode)
		{
			case StrategyMode.Simulation:
				order.Provider = ProviderManager.ExecutionSimulator;
				break;

			case StrategyMode.Live:
				order.Provider = this.executionProvider;
				if (this.saveOrders)
				{
					order.Persistent = true;
				}
				break;
		}
		order.StrategyMode = (char)this.modes[this.strategyMode];
		this.orders.Add(order);
	}
	protected virtual SingleOrder Execute(Signal signal)
	{
		SingleOrder singleOrder = new SingleOrder();
		singleOrder.Instrument = signal.Instrument;
		singleOrder.Strategy = signal.Strategy.Name;
		singleOrder.StrategyFill = signal.StrategyFill;
		singleOrder.StrategyPrice = signal.StrategyPrice;
		singleOrder.ForceMarketOrder = signal.ForceMarketOrder;
		if (signal.ForceFillOnBarMode)
		{
			singleOrder.FillOnBarMode = (int)signal.FillOnBarMode;
		}
		switch (signal.Side)
		{
			case SignalSide.Buy:
				singleOrder.Side = Side.Buy;
				break;
			case SignalSide.BuyCover:
				singleOrder.Side = Side.Buy;
				break;
			case SignalSide.Sell:
				singleOrder.Side = Side.Sell;
				break;
			case SignalSide.SellShort:
				singleOrder.Side = Side.SellShort;
				break;
			default:
				throw new NotSupportedException();
		}
		switch (signal.Type)
		{
			case SignalType.Market:
				singleOrder.OrdType = OrdType.Market;
				break;
			case SignalType.Limit:
				singleOrder.OrdType = OrdType.Limit;
				singleOrder.Price = signal.LimitPrice;
				break;
			case SignalType.Stop:
				singleOrder.OrdType = OrdType.Stop;
				singleOrder.StopPx = signal.StopPrice;
				break;
			case SignalType.StopLimit:
				singleOrder.OrdType = OrdType.StopLimit;
				singleOrder.StopPx = signal.StopPrice;
				singleOrder.Price = signal.LimitPrice;
				break;
			case SignalType.TrailingStop:
				singleOrder.OrdType = OrdType.TrailingStop;
				singleOrder.StopPx = signal.StopPrice;
				break;
			default:
				throw new NotSupportedException();
		}
		singleOrder.OrderQty = signal.Qty;
		singleOrder.TimeInForce = signal.TimeInForce;
		singleOrder.Text = signal.Text;
		this.RegisterOrder(singleOrder);
		singleOrder.Send();
		return singleOrder;
	}
	public SingleOrder EmitSignal(Signal signal)
	{
		signal.Strategy=this;		
		bool flag = false;
		if (signal.Status == SignalStatus.New)
		{
			if (this.Validate(signal))
			{
				signal.Status = SignalStatus.Accepted;
				flag = true;
			}
			else
			{
				signal.Status = SignalStatus.Rejected;
			}
		}
		this.signals.Add(signal);
		if (this.SignalAdded != null)
		{
			this.SignalAdded(new SignalEventArgs(signal));
		}
		if (flag)
		{
			return this.Execute(signal);
		}
		return null;
	}
	protected void EmitError(Exception exception)
	{
		/*if (Trace.IsLevelEnabled(TraceLevel.Error))
		{
			Trace.WriteLine(exception.ToString());
		}*/
		Console.WriteLine(exception.ToString());
	}
	/*投资组合的事件*/
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
		try
		{
			this.OnPortfolioValueChanged(args.Position);
		}
		catch (Exception exception)
		{
			this.EmitError(exception);
		}
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
	/*模拟器事件*/
	private void OnSimulatorStateChanged(object sender, EventArgs e)
	{
		if (this.isRunning&&(this.strategyMode == StrategyMode.Simulation))
		{
			switch (this.simulator.CurrentState)
			{
				case SimulatorState.Stopped:						
					this.StrategyStop();
					this.EmitStopped();
					return;
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
	private void EmitPaused()
	{
		if (this.Paused != null)
		{
			this.Paused(this, EventArgs.Empty);
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
	public event EventHandler Paused;
	public event EventHandler Started;
	public event EventHandler Stopped;
	/*数据订阅事件*/
	private void SetNewBar(object sender, BarEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Bar bar = args.Bar;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewBar(bar);
					}
				}
				this.OnNewBar(instrument, bar);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewBarOpen(object sender, BarEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Bar bar = args.Bar;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewBarOpen(bar);
					}
				}
				this.OnNewBarOpen(instrument, bar);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewBarSlice(object sender, BarSliceEventArgs args)
	{
		if (this.HandleMarketData(args.Provider))
		{
			try
			{
				long barSize = args.BarSize;
				this.OnNewBarSlice(barSize);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewCorporateAction(object sender, CorporateActionEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				CorporateAction corporateAction = args.CorporateAction;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewCorporateAction(corporateAction);
					}
				}
				this.OnNewCorporateAction(instrument, corporateAction);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewFundamental(object sender, FundamentalEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Fundamental fundamental = args.Fundamental;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewFundamental(fundamental);
					}
				}
				this.OnNewFundamental(instrument, fundamental);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewMarketDepth(object sender, MarketDepthEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				MarketDepth marketDepth = args.MarketDepth;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewMarketDepth(marketDepth);
					}
				}
				this.OnNewMarketDepth(instrument, marketDepth);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetNewQuote(object sender, QuoteEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Quote quote = args.Quote;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewQuote(quote);
					}
				}
				this.OnNewQuote(instrument, quote);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}
	}
	private void SetNewTrade(object sender, TradeEventArgs args)
	{
		if (this.HandleMarketData(args.Provider, args.Instrument))
		{
			try
			{
				Instrument instrument = args.Instrument as Instrument;
				Trade trade = args.Trade;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewTrade(trade);
					}
				}
				this.OnNewTrade(instrument, trade);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}
		}
	}
	protected virtual void OnNewBar(Instrument instrument, Bar bar)
	{
	}
	protected virtual void OnNewBarOpen(Instrument instrument, Bar bar)
	{
	}
	protected virtual void OnNewBarSlice(long barSize)
	{
	}
	protected virtual void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
	{
	}
	protected virtual void OnNewFundamental(Instrument instrument, Fundamental fundamental)
	{
	}
	protected virtual void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
	{
	}
	protected virtual void OnNewQuote(Instrument instrument, Quote quote)
	{
	}
	protected virtual void OnNewTrade(Instrument instrument, Trade trade)
	{
	}
	//订单事件
	private void SetNewOrder(OrderEventArgs args)
	{
		try
		{
			SingleOrder key = args.Order;
			if (this.orders.Contains(key))
			{	
				Instrument instrument=key.Instrument;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnNewOrder(key);
					}
				}
				this.OnNewOrder(key);
			}
		}
		catch (Exception exception)
		{
			this.EmitError(exception);
		}
	}
	private void SetExecutionReport(object sender, ExecutionReportEventArgs args)
	{
		try
		{
			ExecutionReport executionReport = args.ExecutionReport;
			SingleOrder key = null;
			switch (executionReport.OrdStatus)
			{
				case OrdStatus.Cancelled:
				case OrdStatus.PendingCancel:
				case OrdStatus.PendingReplace:
					key = OrderManager.Orders.All[executionReport.OrigClOrdID] as SingleOrder;
					break;

				default:
					key = OrderManager.Orders.All[executionReport.ClOrdID] as SingleOrder;
					break;
			}
			if (key != null)
			{
				if (this.orders.Contains(key))
				{
					Instrument instrument=key.Instrument;
					List<Behavior> list=null;
					if (this.behaviorDict.TryGetValue(instrument,out list)){
						foreach(Behavior behavior in list){
							behavior.OnExecutionReport(key, executionReport);
						}
					}
					this.OnExecutionReport(key, executionReport);
					if (key.OrdStatus == OrdStatus.PartiallyFilled)
					{
						if (list!=null){
							foreach(Behavior behavior in list){
								behavior.OnOrderPartiallyFilled(key);
							}
						}
						this.OnOrderPartiallyFilled(key);
					}
				}
			}
		}
		catch (Exception exception)
		{
			this.EmitError(exception);
		}
	}
	private void SetOrderStatusChanged(OrderEventArgs args)
	{
		try
		{
			SingleOrder key = args.Order;
			if (this.orders.Contains(key))
			{
				Instrument instrument=key.Instrument;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						behavior.OnOrderStatusChanged(key);
					}
				}
				this.OnOrderStatusChanged(key);
			}
		}
		catch (Exception exception)
		{
			this.EmitError(exception);
		}
	}
	private void SetOrderDone(OrderEventArgs args)
	{
		try
		{
			SingleOrder key = args.Order;
			if (this.orders.Contains(key))
			{
				Instrument instrument=key.Instrument;
				List<Behavior> list=null;
				if (this.behaviorDict.TryGetValue(instrument,out list)){
					foreach(Behavior behavior in list){
						switch (key.OrdStatus)
						{
							case OrdStatus.Filled:
								behavior.OnOrderFilled(key);
								break;

							case OrdStatus.Cancelled:
								behavior.OnOrderCancelled(key);
								break;

							case OrdStatus.Rejected:
								behavior.OnOrderRejected(key);
								break;
						}
					}
				}
				switch (key.OrdStatus)
				{
					case OrdStatus.Filled:
						this.OnOrderFilled(key);
						break;

					case OrdStatus.Cancelled:
						this.OnOrderCancelled(key);
						break;

					case OrdStatus.Rejected:
						this.OnOrderRejected(key);
						break;
				}
				if (list!=null) {
					foreach(Behavior behavior in list){
						behavior.OnOrderDone(key);
					}
				}
				this.OnOrderDone(key);
			}
		}
		catch (Exception exception)
		{
			this.EmitError(exception);
		}
	}
	protected virtual void OnNewOrder(SingleOrder order)
	{
	}
	protected virtual void OnOrderCancelled(SingleOrder order)
	{
	}
	protected virtual void OnOrderFilled(SingleOrder order)
	{
	}
	protected virtual void OnOrderPartiallyFilled(SingleOrder order)
	{
	}
	protected virtual void OnOrderRejected(SingleOrder order)
	{
	}
	protected virtual void OnOrderStatusChanged(SingleOrder order)
	{
	}
	protected virtual void OnOrderDone(SingleOrder order)
	{
	}
	protected virtual void OnExecutionReport(SingleOrder order, ExecutionReport report)
	{
	}
	//提供者事件
	private void SetProviderConnected(ProviderEventArgs args)
	{
		if (this.providers.Contains(args.Provider))
		{
			try
			{
				IProvider provider = args.Provider;
				this.OnProviderConnected(provider);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetProviderDisconnected(ProviderEventArgs args)
	{
		if (this.providers.Contains(args.Provider))
		{
			try
			{
				IProvider provider = args.Provider;
				this.OnProviderDisconnected(provider);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

		}
	}
	private void SetProviderError(ProviderErrorEventArgs args)
	{
		if (this.providers.Contains(args.Error.Provider))
		{
			try
			{
				IProvider provider = args.Error.Provider;
				int id = args.Error.Id;
				int code = args.Error.Code;
				string message = args.Error.Message;
				this.OnProviderError(provider, id, code, message);
			}
			catch (Exception exception)
			{
				this.EmitError(exception);
			}

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
	//策略流程事件
	protected virtual void OnInit(){
	}
	protected virtual void OnStrategyStop(){
	}
	//信号添加事件
	public event SignalEventHandler SignalAdded;
	//信号校验
	protected virtual bool Validate(Signal signal)
	{
		return true;
	}
		
	protected void ShowPerformance(Portfolio portfolio,LiveTester tester){
		Performance performance=portfolio.Performance;
		Console.WriteLine("策略 {0} ------- [性能]:",this.Name);
		Console.WriteLine("\t权益:{0}",performance.Equity);
		Console.WriteLine("\t当期损益:{0}",portfolio.Performance.PnL);
		Console.WriteLine("\t\t注：当期损益基于报价数据周期，如果用的是日线，则是今天相对昨天的损益，如果用的是TICK，则是当前TICK相对于前一TICK的损益");
		Console.WriteLine("\t回撤:{0}--权益与最高权益的差",performance.Drawdown);
		Console.WriteLine("\t核心权益:{0}--指账户中的资金",performance.CoreEquity);
		Console.WriteLine("\t最低权益:{0}",performance.LowEquity);
		Console.WriteLine("\t最高权益:{0}",performance.HighEquity);		
		Console.WriteLine("\t初始财富:{0}",tester.InitialWealth);
		Console.WriteLine("\t最终财富:{0}",tester.FinalWealth);
		Console.WriteLine("\t统计天数:{0}",tester.TestDays);
		Console.WriteLine("\t交易天数:{0}",tester.TradeDays);
		try {
			double minCoreEquity=performance.CoreEquitySeries.GetMin();
			double maxUsedCoreEquity=tester.InitialWealth-minCoreEquity;		
			double lastPnL=tester.FinalWealth-tester.InitialWealth;		
			double ROI=0;
			if (maxUsedCoreEquity>0){
				ROI=lastPnL/maxUsedCoreEquity*100;
			}
			Console.WriteLine("\t最大使用核心权益(即最大使用资金):{0}",maxUsedCoreEquity);
			Console.WriteLine("\t最终损益(即利润):{0}",lastPnL);
			Console.WriteLine("\t投资回报率(%):{0}--利润/最大已使用核心权益",ROI);
		}
		catch(Exception ex)
		{
			Console.Write(ex.Message);
		}
		
	}
	protected void ShowPerformanceAnalysis(LiveTester tester){
		Console.WriteLine();
		Console.WriteLine("策略 {0} ------- [性能分析]:",this.Name);
		Console.WriteLine("InitialWealth:{0}",tester.Components["InitialWealth"].LastValue);
		Console.WriteLine("FinalWealth:{0}",tester.Components["FinalWealth"].LastValue);
		Console.WriteLine("TotalDays:{0}",tester.Components["TotalDays"].LastValue);
		Console.WriteLine("TradeDays:{0}",tester.Components["TradeDays"].LastValue);
		Console.WriteLine("GainDays:{0}",tester.Components["GainDays"].LastValue);
		Console.WriteLine("LossDays:{0}",tester.Components["LossDays"].LastValue);
		Console.WriteLine("Average Return (%):{0}",tester.Components["Average Return (%)"].LastValue);
		Console.WriteLine("Average Gain (%):{0}",tester.Components["Average Gain (%)"].LastValue);
		Console.WriteLine("Average Loss (%):{0}",tester.Components["Average Loss (%)"].LastValue);
		Console.WriteLine("Drawdown Average:{0}",tester.Components["Drawdown Average"].LastValue);
		Console.WriteLine("Drawdown Median:{0}",tester.Components["Drawdown Median"].LastValue);
		Console.WriteLine("Average Annual Return (%):{0}",tester.Components["Average Annual Return (%)"].LastValue);
		Console.WriteLine("Median Annual Return (%):{0}",tester.Components["Median Annual Return (%)"].LastValue);
		Console.WriteLine("Maximum Annual Return (%):{0}",tester.Components["Maximum Annual Return (%)"].LastValue);
		Console.WriteLine("Minimum Annual Return (%):{0}",tester.Components["Minimum Annual Return (%)"].LastValue);
		Console.WriteLine("Average Monthly Return (%):{0}",tester.Components["Average Monthly Return (%)"].LastValue);
		Console.WriteLine("Median Monthly Return (%):{0}",tester.Components["Median Monthly Return (%)"].LastValue);
		Console.WriteLine("Maximum Monthly Return (%):{0}",tester.Components["Maximum Monthly Return (%)"].LastValue);
		Console.WriteLine("Minimum Monthly Return (%):{0}",tester.Components["Minimum Monthly Return (%)"].LastValue);
		Console.WriteLine();
	}
	protected void ShowRoundTripsStatistics(LiveTester tester){
		Console.WriteLine();
		Console.WriteLine("策略 {0} ------- [回合统计] 此统计和成交量无关，即假设每个证券成交的都是1股:",this.Name);	
		Dictionary<string,string>  dict1 = new Dictionary<string,string>();
		dict1.Add("交易次数","Number Of RoundTrips");
		dict1.Add("盈利交易次数","Number Of Winning RoundTrips");
		dict1.Add("亏损交易次数","Number Of Losing RoundTrips");
		dict1.Add("胜率(%)","Percent Of Profitable (%)");
		dict1.Add("Value Open RoundTrips","Value Open RoundTrips");
		dict1.Add("所有交易损益和","Total PnL Of All RoundTrips");
		dict1.Add("所有盈利交易损益和","Total PnL Of Winning RoundTrips");
		dict1.Add("所有亏损交易损益和","Total PnL Of Losing RoundTrips");
		dict1.Add("每个盈利交易损益","Profit Per Winning Trade");
		dict1.Add("平均交易损益","Average RoundTrip");
		dict1.Add("最大盈利交易损益","Largest Winning RoundTrip");
		dict1.Add("最大亏损交易损益","Largest Losing RoundTrip");
		dict1.Add("平均盈利交易损益","Average Winning RoundTrip");
		dict1.Add("平均亏损交易损益","Average Losing RoundTrip");
		dict1.Add("回报率","Ratio avg. win / avg. loss");
		dict1.Add("盈利因子","Profit Factor");
		dict1.Add("最大连续盈利次数","Maximal Consecutive Winners");
		dict1.Add("最大连续亏损次数","Maximal Consecutive Losers");
		dict1.Add("平均效能","Average Total Efficiency");
		dict1.Add("平均入场效能","Average Entry Efficiency");
		dict1.Add("平均出场效能","Average Exit Efficiency");
		foreach (KeyValuePair<string,string> kvp in dict1)
		{   
			Console.WriteLine("\t{0}:{1}",kvp.Key,tester.Components[kvp.Value].LastValue);
		}
		Console.WriteLine();
	}
	//关闭头寸
	public void ClosePosition(Instrument instrument, double price, string text)
	{
		Position item = this.portfolio.Positions[instrument];
		if (item != null)
		{
			switch (item.Side)
			{
				case PositionSide.Long:
				{
					this.EmitSignal(new Signal(Clock.Now,  SignalType.Market, SignalSide.Sell, item.Qty, price, instrument, text));
					return;
				}
				case PositionSide.Short:
				{
					this.EmitSignal(new Signal(Clock.Now, SignalType.Market, SignalSide.BuyCover, item.Qty, price, instrument, text));
					break;
				}
				default:
				{
					return;
				}
			}
		}
	}

	public void ClosePosition(Instrument instrument, string text)
	{
		this.ClosePosition(instrument, 0, text);
	}

	public void ClosePosition(Instrument instrument)
	{
		this.ClosePosition(instrument, "");
	}
	//关闭投资组合
	public void ClosePortfolio(string text)
	{
		PositionList positionLists = new PositionList();
		foreach (Position position in this.portfolio.Positions)
		{
			positionLists.Add(position);
		}
		foreach (Position positionList in positionLists)
		{
			this.ClosePosition(positionList.Instrument, text);
		}
	}

	public void ClosePortfolio()
	{
		this.ClosePortfolio("");
	}
	//新增，为单个证券添加市场数据请求
	public void AddMarketDataRequest(Instrument inst){
		IMarketDataProvider provider;
		switch (this.strategyMode)
		{	
			case StrategyMode.Simulation:
				provider=ProviderManager.MarketDataSimulator;
				foreach (string str in this.requests)
				{
					this.AddMarketDataDispatcher(provider, inst, str);
					if (str == null)
					{
						inst.RequestMarketData(provider, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade);
					}
					else
					{
						inst.RequestMarketData(provider, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade, str);
					}
				}
				break;
			case StrategyMode.Live:
			{
				provider = this.marketDataProvider;
				this.AddMarketDataDispatcher(provider, inst, null);
				inst.RequestMarketData(provider, MarketDataType.None | MarketDataType.Quote | MarketDataType.Trade);
				break;
			}
		}
		
	}
	
	public List<Behavior> Behaviors{
		get {
			List<Behavior> ret=new List<Behavior>();
			foreach(KeyValuePair<Instrument,List<Behavior>> kvp in this.behaviorDict)
				ret.AddRange(kvp.Value);
			return ret;
		}
	}
	
}