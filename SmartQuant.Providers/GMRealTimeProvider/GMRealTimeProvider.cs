using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using HuaQuant.Data.Sina;

namespace HuaQuant.Data.GM
{
    public class GMRealTimeProvider : IProvider, IMarketDataProvider
    {

        private  GMSDK.MdApi _md = GMSDK.MdApi.Instance;
        public GMSDK.MdApi MdApi
        {
            get { return this._md; }
        }
        private System.Timers.Timer _Timer = null;
        private Thread _mdThread = null;
        public GMRealTimeProvider()
        {
            this.barItems = new BarFactoryItemList();
            this.barItems.Add(new BarFactoryItem(BarType.Time, 60, true));
            this.BarFactory = new BarFactory(false);
            ProviderManager.Add(this);
        }
        //登录用户名
        private string username;
        [Category("登录设置"), Description("用户名"), DefaultValue(@"demo@myquant.cn")]
        public string Username
        {
            get { return this.username; }
            set { this.username = value.Trim(); }
        }
        //登录密码
        private string password;
        [Category("登录设置"), Description("密码"), DefaultValue(@"123456"),PasswordPropertyText(true)]
        public string Password
        {
            get { return this.password; }
            set { this.password = value.Trim(); }
        }

        #region IProvider
        
        [Category("信息")]
        public byte Id
        {
            get
            {
                return 188;
            }
        }
        private bool isConnected = false;
        [Category("状态")]
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }
        [Category("信息")]
        public string Name
        {
            get
            {
                return "GMRealTimeProvider";
            }
        }

        [Category("状态")]
        public ProviderStatus Status
        {
            get
            {
                if (!IsConnected)
                    return ProviderStatus.Disconnected;
                else
                    return ProviderStatus.Connected;
            }
        }
        [Category("信息")]
        public string Title
        {
            get
            {
                return "掘金量化实时数据提供者";
            }
        }
        [Category("信息")]
        public string URL
        {
            get
            {
                return "www.huaquant.com";
            }
        }
        private bool subscribeBar = false;
        [Category("数据"), Description("是否订阅Bar数据"), DefaultValue(false)]
        public bool SubscribeBar
        {
            get
            {
                return this.subscribeBar;
            }
            set
            {
                this.subscribeBar = value;
            }
        }
        private BarFactoryItemList barItems;
        [Category("数据"), Description("Bar数据时间周期")]
        public BarFactoryItemList BarItems
        {
            get { return this.barItems; }
        }
        
        private int refreshInterval = 1000;
        [Category("辅助数据"), Description("刷新时间间隔，单位毫秒")]
        public int RefreshInterval
        {
            get { return this.refreshInterval; }
            set { this.refreshInterval = value; }
        }
        private int requestNumPerBatch = 50;
        [Category("辅助数据"), Description("每次批量请求数据个数"), DefaultValue(50)]
        public int RequestNumPerBatch
        {
            get { return this.requestNumPerBatch; }
            set { this.requestNumPerBatch = value; }
        }
        private DateTime beginTime = DateTime.Today.Add(new TimeSpan(9, 15, 0));
        [Category("辅助数据"), Description("开始刷新时间")]
        public TimeSpan BeginTimeSpan
        {
            get { return this.beginTime.TimeOfDay; }
            set { this.beginTime= DateTime.Today.Add(value); }
        }
        private DateTime endTime = DateTime.Today.Add(new TimeSpan(15, 2, 0));
        [Category("辅助数据"), Description("结束刷新时间")]
        public TimeSpan EndTimeSpan
        {
            get { return this.endTime.TimeOfDay; }
            set { this.endTime = DateTime.Today.Add(value); }
        }
        private bool usingSina = false;

        public void onTick(GMSDK.Tick tick) {
            lock (this.newTickLock)
            {
                this.setNewTick(tick, true);
            }
        }
        public void onBar(GMSDK.Bar gskBar) {
            this.setNewBar(gskBar);
        }
        public void onLogin() {
            Console.WriteLine("GM real time provider logined.");
            if (!this.isConnected)
            {
                this.isConnected = true;
                this.EmitConnectedEvent();
                this.EmitStatusChangedEvent();
            }
            if (this.usingSina)
            {
                this.usingSina = false;
            }
        }
        public void onError(int errorCode, string errorMsg) {
            this.EmitError(errorCode, errorMsg);
            //连接失败或连接断开时
            if (errorCode== GMSDK.ErrorCode.ERR_MD_CONNECT || errorCode == GMSDK.ErrorCode.ERR_MD_CONNECT_CLOSE)
            {
                if (this.isConnected)
                {
                    this.isConnected = false;
                    this.EmitDisconnectedEvent();
                    this.EmitStatusChangedEvent();              
                }
                if (!this.usingSina)
                {
                    this.usingSina = true;         
                }
            }
        }
        public void onMd(GMSDK.MDEvent e) {
            Console.WriteLine("event:{0} on time :{1}",e.event_type,e.utc_time);
        }
        public void mdRun()
        {
            this._md.Run();
        }
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        public event EventHandler StatusChanged;
        private void EmitConnectedEvent()
        {
            if (Connected != null)
            {
                this.Connected(this, EventArgs.Empty);
            }
        }
        private void EmitStatusChangedEvent()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, EventArgs.Empty);
            }
        }
        private void EmitDisconnectedEvent()
        {
            if (Disconnected != null)
            {
                this.Disconnected(this, EventArgs.Empty);
            }
        }
        private void EmitError(int code, string message)
        {
            if (this.Error != null)
            {
                this.Error(new ProviderErrorEventArgs(new ProviderError(Clock.Now, this, this.Id, code, message)));
            }
        }
        public void Connect()
        {
            if (!isConnected)
            {
                int ret;              
                ret = this._md.Init(this.username, this.password, GMSDK.MDMode.MD_MODE_LIVE);
                if (ret != 0)
                {
                    string msg = this._md.StrError(ret);
                    this.EmitError(ret, msg);
                }
                else
                {
                    this._md.LoginEvent += this.onLogin;
                    this._md.ErrorEvent += this.onError;
                    this._md.TickEvent += this.onTick;
                    this._md.BarEvent += this.onBar;
                    this._md.MdEvent += this.onMd;

                    if (this.subscribedSymbols.Count > 0)
                    {
                        foreach (string symbol in this.subscribedSymbols.ToArray())
                        {
                            this.SubscribeSymbol(symbol);
                        }
                    }
                    this.symbolsOfPush.Clear();

                    if (this._mdThread == null)
                    {
                        this._mdThread = new Thread(new ThreadStart(this.mdRun));
                        if (this._mdThread.ThreadState == ThreadState.Unstarted)
                        {
                            this._mdThread.Start();
                        }
                    }
                    if (this._Timer == null)
                    {
                        this._Timer = new System.Timers.Timer();
                        this._Timer.Interval = this.refreshInterval;
                        this._Timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                    }
                    this._Timer.Start();
                } 
            }
        }

        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                this._md.Close(); 
                if (this._mdThread != null)
                {
                    if (this._mdThread.ThreadState == ThreadState.Running) this._mdThread.Abort();
                    this._mdThread = null;
                }
                this._md.TickEvent -= this.onTick;
                this._md.BarEvent -= this.onBar;
                this._md.MdEvent -= this.onMd;
                this._md.LoginEvent -= this.onLogin;
                this._md.ErrorEvent -= this.onError;

                this.isConnected = false;
                this.EmitDisconnectedEvent();
                this.EmitStatusChangedEvent();
            }
            if (this._Timer != null)
            {
                this._Timer.Stop();
                this._Timer = null;
            }
            this.usingSina = false;
        }
        public void Shutdown()
        {
            this.Disconnect();
        }
        #endregion

        #region IMarketDataProvider
        private IBarFactory factory = null;
        public IBarFactory BarFactory
        {
            get
            {
                return this.factory;
            }
            set
            {
                if (this.factory != null)
                {
                    this.factory.NewBar -= new BarEventHandler(this.OnNewBar);
                    this.factory.NewBarOpen -= new BarEventHandler(this.OnNewBarOpen);
                    this.factory.NewBarSlice -= new BarSliceEventHandler(this.OnNewBarSlice);
                }
                this.factory = (IBarFactory)value;
                if (this.factory != null)
                {
                    this.factory.NewBar += new BarEventHandler(this.OnNewBar);
                    this.factory.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
                    this.factory.NewBarSlice += new BarSliceEventHandler(this.OnNewBarSlice);
                }
            }
        }
        public event MarketDataRequestRejectEventHandler MarketDataRequestReject;
        public event MarketDataSnapshotEventHandler MarketDataSnapshot;
        public event BarEventHandler NewBar;
        public event BarEventHandler NewBarOpen;
        public event BarSliceEventHandler NewBarSlice;
        public event CorporateActionEventHandler NewCorporateAction;
        public event FundamentalEventHandler NewFundamental;
        public event BarEventHandler NewMarketBar;
        public event MarketDataEventHandler NewMarketData;
        public event MarketDepthEventHandler NewMarketDepth;
        public event QuoteEventHandler NewQuote;
        public event TradeEventHandler NewTrade;

        private void OnNewBar(object sender, SmartQuant.Providers.BarEventArgs args)
        {
            if (this.NewBar != null)
            {
                this.NewBar(this, new SmartQuant.Providers.BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarOpen(object sender, SmartQuant.Providers.BarEventArgs args)
        {
            if (this.NewBarOpen != null)
            {
                this.NewBarOpen(this, new SmartQuant.Providers.BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarSlice(object sender, SmartQuant.Providers.BarSliceEventArgs args)
        {
            if (this.NewBarSlice != null)
            {
                this.NewBarSlice(this, new SmartQuant.Providers.BarSliceEventArgs(args.BarSize, this));
            }
        }
        private void EmitNewQuote(Quote quote, Instrument instrument)
        {
            if (this.NewQuote != null)
            {
                this.NewQuote(this, new QuoteEventArgs(quote, instrument, this));
            }
            if (this.factory != null && this.factory.Enabled)
            {
                this.factory.OnNewQuote(instrument, quote);
            }
        }
        private void EmitNewTrade(Trade trade, Instrument instrument)
        {
            if (this.NewTrade != null)
            {
                this.NewTrade(this, new TradeEventArgs(trade, instrument, this));
            }

            if (this.factory != null && this.factory.Enabled)
            {
                this.factory.OnNewTrade(instrument, trade);
            }
        }
        
        private List<string> subscribedSymbols = new List<string>();
        /*订阅数据*/
        private void SubscribeSymbol(string symbol)
        {
            lock (this._md)
            {
                this._md.Subscribe(symbol + ".tick");
            }
            if (this.subscribeBar)
            {
                foreach (BarFactoryItem barItem in this.barItems)
                {
                    if (barItem.Enabled && barItem.BarType == BarType.Time)
                    {
                        lock (this._md)
                        {
                            this._md.Subscribe(symbol + ".bar." + barItem.BarSize.ToString());
                        }
                    }
                }
            }        
        }
        /*取消订阅数据*/
        private void UnsubscribeSymbol(string symbol)
        {
            lock (this._md)
            {
                this._md.Unsubscribe(symbol + ".tick");
            }
            if (this.subscribeBar)
            {
                foreach (BarFactoryItem barItem in this.barItems)
                {
                    if (barItem.Enabled && barItem.BarType == BarType.Time)
                    {
                        lock (this._md)
                        {
                            this._md.Unsubscribe(symbol + ".bar." + barItem.BarSize.ToString());
                        }
                    }
                }
            }
        }
        public void SendMarketDataRequest(FIXMarketDataRequest request)
        {
            lock (this)
            {
                if (!IsConnected)
                {
                    EmitError(1, "The current market data provider not connected.");
                    return;
                }
                switch (request.SubscriptionRequestType)
                {
                    case DataManager.MARKET_DATA_SUBSCRIBE:
                        for (int i = 0; i < request.NoRelatedSym; i++)
                        {
                            FIXRelatedSymGroup group = request.GetRelatedSymGroup(i);
                            if (!this.subscribedSymbols.Contains(group.Symbol))
                            {
                                this.SubscribeSymbol(group.Symbol);
                                this.subscribedSymbols.Add(group.Symbol);
                            }
                        }
                        break;
                    case DataManager.MARKET_DATA_UNSUBSCRIBE:
                        for (int i = 0; i < request.NoRelatedSym; i++)
                        {
                            FIXRelatedSymGroup group = request.GetRelatedSymGroup(i);
                            if (this.subscribedSymbols.Contains(group.Symbol))
                            {
                                this.UnsubscribeSymbol(group.Symbol);
                                this.subscribedSymbols.Remove(group.Symbol);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentException("Unknown subscription type: " + request.SubscriptionRequestType.ToString());
                }
            }
        }
        private List<string> symbolsOfPush = new List<string>();//推送行情的证券
        private object newTickLock =new object();
        private Dictionary<string, GMSDK.Tick> tickCache = new Dictionary<string, GMSDK.Tick>();
        private void setNewTick(GMSDK.Tick tick,bool isPush=false)
        {
            string symbol = tick.exchange + "." + tick.sec_id;
            if (isPush) this.symbolsOfPush.Add(symbol);
            
            if (!subscribedSymbols.Contains(symbol)) return;
            Instrument curInstrument = InstrumentManager.Instruments[symbol];
            if (curInstrument == null)
            {
                this.EmitError(2, "Symbol " + symbol + " was not found in InstrumentManager.");
                return;
            }
            GMSDK.Tick prevTick = null;
            if (!this.tickCache.TryGetValue(symbol, out prevTick))
            {
                this.EmitNewTrade(GSKToGM.ConvertTrade(tick), curInstrument);
                this.EmitNewQuote(GSKToGM.ConvertQuote(tick), curInstrument);
            }
            else
            {
                if (tick.utc_time > prevTick.utc_time)
                {
                    if (tick.last_volume > 0 || tick.last_price != prevTick.last_price)
                    {
                        this.EmitNewTrade(GSKToGM.ConvertTrade(tick), curInstrument);
                    }
                    this.EmitNewQuote(GSKToGM.ConvertQuote(tick), curInstrument);
                }
            }
            this.tickCache[symbol] = tick;
        }
        private void setNewBar(GMSDK.Bar gskBar)
        {
            string symbol = gskBar.exchange + "." + gskBar.sec_id;
            Instrument curInstrument = InstrumentManager.Instruments[symbol];
            if (curInstrument == null)
            {
                this.EmitError(2, "Symbol " + symbol + " was not found in InstrumentManager.");
                return;
            }
            GMBar gmBar = GSKToGM.ConvertBar(gskBar);
            this.OnNewBar(this, new BarEventArgs(gmBar, curInstrument, this));
        }
        private int inTimer = 0;//防止计时器事件重入
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now< this.beginTime || DateTime.Now > this.endTime) return;
            if (Interlocked.Exchange(ref this.inTimer,1)==0)
            {
                if (this.usingSina)
                {
                    SinaQuoter sq = SinaQuoter.Instance;
                    List<GMSDK.Tick> ticks = sq.GetQuotes(this.subscribedSymbols);
                    foreach (GMSDK.Tick tick in ticks)
                    {
                        lock (this.newTickLock)
                        {
                            this.setNewTick(tick);
                        }
                    }
                }
                else
                {
                    List<string> symbolsOfNotPush = new List<string>();
                    foreach (string symbol in this.subscribedSymbols.ToArray())
                    {
                        if (!this.symbolsOfPush.Contains(symbol)) symbolsOfNotPush.Add(symbol);
                    }
                    List<GMSDK.Tick> ticks = this.GetLastTicks(symbolsOfNotPush);

                    foreach (GMSDK.Tick tick in ticks)
                    {
                        lock (this.newTickLock)
                        {
                            this.setNewTick(tick);
                        }
                    }
                }
                Interlocked.Exchange(ref this.inTimer, 0);
            }
        }      
        protected List<GMSDK.Tick> GetLastTicks(List<string> symbols)
        {
            List<GMSDK.Tick> gskTicks = new List<GMSDK.Tick>();
            int i = 0;
            int j = 0;
            string symbolList = "";
            int num = symbols.Count;
            foreach (string symbol in symbols)
            {
                j++;
                i++;
                symbolList += symbol + ",";
                if (i >= this.requestNumPerBatch || j >= num)
                {
                    lock (this._md)
                    {
                        gskTicks.AddRange(this._md.GetLastTicks(symbolList));
                    }
                    i = 0;
                    symbolList = "";
                }
            }
            return gskTicks;
        }
        #endregion

        //以下为新增
        #region public method
        public List<string> GetSymbols(string market, int securityType, int active)
        {
            List<GMSDK.Instrument> gskInsts = new List<GMSDK.Instrument>();
            lock (this._md)
            {
                gskInsts = this._md.GetInstruments(market, securityType, active);
            }
            List<string> symbols = new List<string>();
            foreach (GMSDK.Instrument gskInst in gskInsts)
            {
                symbols.Add(gskInst.symbol);
            }
            return symbols;
        }
        public List<Trade> GetLastNTrades(string symbol,string lastTimeString,int n)
        {
            List<GMSDK.Tick> gskTicks;
            lock (this._md)
            {
                gskTicks = this._md.GetLastNTicks(symbol, n, lastTimeString);
            }
            gskTicks.Reverse();
            List<Trade> trades = new List<Trade>();
            foreach(GMSDK.Tick gskTick in gskTicks)
            {
                trades.Add(GSKToGM.ConvertTrade(gskTick));
            }
            return trades;
        }
        public List<Trade> GetTrades(string symbol,string beginTimeString,string endTimeString)
        {
            List<GMSDK.Tick> gskTicks;
            lock (this._md)
            {
                gskTicks = this._md.GetTicks(symbol, beginTimeString, endTimeString);
            }
            List<Trade> trades = new List<Trade>();
            foreach (GMSDK.Tick gskTick in gskTicks)
            {
                trades.Add(GSKToGM.ConvertTrade(gskTick));
            }
            return trades;
        }
        public Dictionary<string, Trade> GetLastTrades(List<string> symbols,bool useCached=true)
        {
            List<GMSDK.Tick> gskTicks=new List<GMSDK.Tick>();
            if (useCached)
            {
                foreach (string symbol in symbols.ToArray())
                {
                    GMSDK.Tick gskTick;
                    if (this.tickCache.TryGetValue(symbol,out gskTick))
                    {
                        gskTicks.Add(gskTick);
                        symbols.Remove(symbol);
                    }
                }
            }
            if (symbols.Count > 0)
            {
                gskTicks.AddRange(this.GetLastTicks(symbols));
            }
            Dictionary<string, Trade> tradeDict = new Dictionary<string, Trade>();
            foreach (GMSDK.Tick gskTick in gskTicks)
            {
                string symbol = gskTick.exchange + "." + gskTick.sec_id;
                tradeDict.Add(symbol, GSKToGM.ConvertTrade(gskTick));
            }
            return tradeDict;
        }

        public List<Daily> GetLastNDailys(string symbol, int n, string lastDateString)
        {
            List<GMSDK.DailyBar> gskDailys;
            lock (this._md)
            {
                gskDailys = this._md.GetLastNDailyBars(symbol, n, lastDateString);
            }
            gskDailys.Reverse();
            List<Daily> dailys = new List<Daily>();
            foreach (GMSDK.DailyBar gskDaily in gskDailys)
            {
                dailys.Add(GSKToGM.ConvertDaily(gskDaily));
            }
            return dailys;
        }
        public List<Daily> GetDailys(string symbol,string beginDateString,string endDateString)
        {
            List<GMSDK.DailyBar> gskDailys;
            lock (this._md)
            {
                gskDailys = this._md.GetDailyBars(symbol, beginDateString, endDateString);
            }
            List<Daily> dailys = new List<Daily>();
            foreach (GMSDK.DailyBar gskDaily in gskDailys)
            {
                dailys.Add(GSKToGM.ConvertDaily(gskDaily));
            }
            return dailys;
        }
        public List<Bar> GetLastNBars(string symbol,int barSize,int n,string lastTimeString)
        {
            List<GMSDK.Bar> gskBars;
            lock (this._md)
            {
                gskBars = this._md.GetLastNBars(symbol, barSize, n, lastTimeString);
            }
            gskBars.Reverse();
            List<Bar> bars = new List<Bar>();
            foreach (GMSDK.Bar gskBar in gskBars)
            {
                bars.Add(GSKToGM.ConvertBar(gskBar));
            }
            return bars;
        }
        public List<Bar> GetBars(string symbol, int barSize, string beginTimeString, string endTimeString)
        {
            List<GMSDK.Bar> gskBars;
            lock (this._md)
            {
                gskBars = this._md.GetBars(symbol, barSize, beginTimeString, endTimeString);
            }
            List<Bar> bars = new List<Bar>();
            foreach (GMSDK.Bar gskBar in gskBars)
            {
                bars.Add(GSKToGM.ConvertBar(gskBar));
            }
            return bars;
        }
        #endregion
    }
}
