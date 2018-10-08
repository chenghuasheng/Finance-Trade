using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using System.ComponentModel;



namespace HuaQuant.Data.GM
{
    public class GMQueryDataProvider : IProvider, IInstrumentProvider,IHistoricalDataProvider
    {
        private GMSDK.MdApi _md;
        public GMSDK.MdApi MdApi
        {
            get { return this._md; }
        }
        public GMQueryDataProvider()
        {
            _md = GMSDK.MdApi.Instance;
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
        [Category("登录设置"), Description("密码"), DefaultValue(@"123456")]
        public string Password
        {
            get { return this.password; }
            set { this.password = value.Trim(); }
        }
        #region IProvider
        private bool isConnected = false;
        [Category("信息")]
        public byte Id
        {
            get
            {
                return 186;
            }
        }
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
                return "GMQueryDataProvider";
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
                return "掘金量化查询数据提供者";
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
                int ret = _md.Init(this.username, this.password);
                if (ret != 0)
                {
                    string msg = _md.StrError(ret);
                    EmitError(ret, msg);
                }
                else
                {
                    isConnected = true;
                    EmitStatusChangedEvent();
                    EmitConnectedEvent();
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
                _md.Close();
                isConnected = false;
                EmitStatusChangedEvent();
                EmitDisconnectedEvent();
            }
        }

        public void Shutdown()
        {
            this.Disconnect();
        }

        #endregion
        #region IInstrumentProvider
        public event SecurityDefinitionEventHandler SecurityDefinition;
        
        private string supportMarkets = "";
        private int securityTypeFilter = 1;
        /// <summary>
        ///上交所，市场代码 SHSE
        ///深交所，市场代码 SZSE
        ///中金所，市场代码 CFFEX
        ///上期所，市场代码 SHFE
        ///大商所，市场代码 DCE
        ///郑商所，市场代码 CZCE
        /// </summary>
        [Category("证券定义设置"), Description("支持的市场"), DefaultValue(@"SHSE,SZSE,CFFEX,SHFE,DCE,CZCE")]
        public string SupportMarkets
        {
            get { return supportMarkets; }
            set { supportMarkets = value; }
        }
        [Category("证券定义设置"), Description("证券类型指定:1=股票,2=共同基金,3=指数,4=期货"), DefaultValue(1)]
        public int SecurityTypeFilter
        {
            get { return securityTypeFilter; }
            set { securityTypeFilter = value; }
        }
       
        public void SendSecurityDefinitionRequest(FIXSecurityDefinitionRequest request)
        {
            lock (this)
            {
                if (!this.IsConnected)
                {
                    this.EmitError(1, "The current instrument provider not connected.");
                    return;
                }
                string[] markets = supportMarkets.Split(',');
                //是否包含交易市场
                if (request.ContainsField(0xcf) && markets.Contains(request.SecurityExchange))
                {
                    List<GMSDK.Instrument> gskInsts = _md.GetInstruments(request.SecurityExchange, this.securityTypeFilter, 0);

                    foreach (GMSDK.Instrument gskInst in gskInsts)
                    {
                        FIXSecurityDefinition definition = new FIXSecurityDefinition(request.SecurityReqID, request.SecurityReqID, 4);
                        string[] ss = gskInst.symbol.Split('.');
                        definition.SecurityExchange = ss[0];//市场
                        definition.SecurityID = ss[1];//代码
                        definition.SecurityIDSource = "8";// 8=Exchange Symbol
                        definition.Symbol = gskInst.symbol;
                        switch (gskInst.sec_type)
                        {
                            case 1:
                                definition.SecurityType = "CS";//股票
                                break;
                            case 2:
                                definition.SecurityType = "MF";//基金（Mutual Fund，共同基金）
                                break;
                            case 3:
                                definition.SecurityType = "IDX";//指数
                                break;
                            case 4:
                                definition.SecurityType = "FUT";//期货
                                break;
                        }
                        //definition.SecuritySubType = symbol.StockType.ToString();
                        definition.SecurityDesc = gskInst.sec_name;
                        if (definition.SecurityType == "FUT")
                        {
                            definition.ContractMultiplier = gskInst.multiplier;//合约乘数
                            definition.PctAtRisk = gskInst.margin_ratio;//保证金比例
                            definition.TickSize = gskInst.price_tick;//价格最小变动单位
                        }
                        definition.TotNoRelatedSym = gskInsts.Count;
                        if (this.SecurityDefinition != null)
                        {
                            this.SecurityDefinition(this, new SecurityDefinitionEventArgs(definition));
                        }
                    }
                }
            }
        }
        #endregion
        #region IHistoricalDataProvider
        //存放历史数据请求
        private Dictionary<string, HistoricalDataRequest> historicalDataIds = new Dictionary<string, HistoricalDataRequest>();
        //存放等待取消的请求
        private Dictionary<string, HistoricalDataRequest> historicalPendingCancels = new Dictionary<string, HistoricalDataRequest>();
        private bool tradeAndQuote = false;
        [Category("历史数据提供者设置"), Description("Trades 和 Quotes 是否一起导入"), DefaultValue(false)]
        public bool TradeAndQuote
        {
            get { return this.tradeAndQuote; }
            set { this.tradeAndQuote = value; }
        }

        public event HistoricalTradeEventHandler NewHistoricalTrade;
        public event HistoricalQuoteEventHandler NewHistoricalQuote;
        public event HistoricalBarEventHandler NewHistoricalBar;
        public event HistoricalMarketDepthEventHandler NewHistoricalMarketDepth;
        public event HistoricalDataEventHandler HistoricalDataRequestCompleted;
        public event HistoricalDataEventHandler HistoricalDataRequestCancelled;
        public event HistoricalDataErrorEventHandler HistoricalDataRequestError;       

        private void EmitHistoricalBar(string requestId, IFIXInstrument instrument, Bar bar)
        {
            if (this.NewHistoricalBar != null)
            {
                this.NewHistoricalBar(this, new HistoricalBarEventArgs(bar, requestId, instrument, this, -1));
            }
        }
        private void EmitHistoricalQuote(string requestID, IFIXInstrument instrument, Quote quote)
        {
            if (this.NewHistoricalQuote != null)
            {
                this.NewHistoricalQuote(this, new HistoricalQuoteEventArgs(quote, requestID, instrument, this, -1));
            }
        }
        private void EmitHistoricalTrade(string requestID, IFIXInstrument instrument, Trade trade)
        {
            if (this.NewHistoricalTrade != null)
            {
                this.NewHistoricalTrade(this, new HistoricalTradeEventArgs(trade, requestID, instrument, this, -1));
            }
        }
        private void EmitHistoricalDataRequestCancelled(string requestId, IFIXInstrument instrument)
        {
            if (this.HistoricalDataRequestCancelled != null)
            {
                this.HistoricalDataRequestCancelled(this, new HistoricalDataEventArgs(requestId, instrument, this, -1));
            }
        }

        private void EmitHistoricalDataRequestCompleted(string requestId, IFIXInstrument instrument)
        {
            if (this.HistoricalDataRequestCompleted != null)
            {
                this.HistoricalDataRequestCompleted(this, new HistoricalDataEventArgs(requestId, instrument, this, -1));
            }
        }

        private void EmitHistoricalDataRequestError(string requestId, IFIXInstrument instrument, string message)
        {
            if (this.HistoricalDataRequestError != null)
            {
                this.HistoricalDataRequestError(this, new HistoricalDataErrorEventArgs(requestId, instrument, this, -1, message));
            }
        }
        [Category("历史数据提供者配置信息")]
        public HistoricalDataType DataType
        {
            get
            {
                return (HistoricalDataType.Bar | HistoricalDataType.Daily | HistoricalDataType.Trade | HistoricalDataType.Quote);
            }
        }
        [Category("历史数据提供者配置信息")]
        public HistoricalDataRange DataRange
        {
            get
            {
                return HistoricalDataRange.DateTimeInterval;
            }
        }
        [Category("历史数据提供者配置信息")]
        public int[] BarSizes
        {
            get
            {
                return new int[] { 60,300,900,1800,3600 };
            }
        }
        [Category("历史数据提供者配置信息")]
        public int MaxConcurrentRequests
        {
            get
            {
                return 1;
            }
        }

        private List<GMSDK.Tick> gskTicksCache = new List<GMSDK.Tick>();//trade和quote都由GMSDK.tick产生,为了历史数据提供者在导入trade和quote时不用读取两遍的tick,故而用此变量缓存tick
        public void SendHistoricalDataRequest(HistoricalDataRequest request)
        {
            if (!this.IsConnected)
            {
                this.EmitError(1, "The current historical data provider not connected.");
                return;
            }
            Instrument instrument = (Instrument)request.Instrument;
            this.historicalDataIds.Add(request.RequestId, request);
            string beginTime = request.BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = request.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
            List<ISeriesObject> list = this.GetHistoricalData(instrument, request.DataType, (int)request.BarSize, beginTime, endTime);
            bool flag = false;
            if ((list.Count < 1) || (flag = this.historicalPendingCancels.ContainsKey(request.RequestId)))
            {
                if (flag)
                {
                    this.historicalPendingCancels.Remove(request.RequestId);
                    this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                }
                else
                {
                    this.historicalDataIds.Remove(request.RequestId);
                    this.EmitHistoricalDataRequestCompleted(request.RequestId, request.Instrument);
                }
            }
            else
            {
                if (request.DataType == HistoricalDataType.Trade)
                {
                    foreach (ISeriesObject obj1 in list)
                    {
                        this.EmitHistoricalTrade(request.RequestId, request.Instrument, obj1 as Trade);
                        if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                        {
                            this.historicalPendingCancels.Remove(request.RequestId);
                            this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                        }
                    }
                    if (this.tradeAndQuote)//Quote与Trade一起导入
                    {
                        List<ISeriesObject> listOther = GSKToGM.ConvertQuotes(this.gskTicksCache);
                        foreach (ISeriesObject obj2 in listOther)
                        {
                            this.EmitHistoricalQuote(request.RequestId, request.Instrument, obj2 as Quote);
                            if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                            {
                                this.historicalPendingCancels.Remove(request.RequestId);
                                this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                            }
                        }
                    }
                }
                else if (request.DataType == HistoricalDataType.Quote)
                {
                    foreach (ISeriesObject obj2 in list)
                    {
                        this.EmitHistoricalQuote(request.RequestId, request.Instrument, obj2 as Quote);
                        if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                        {
                            this.historicalPendingCancels.Remove(request.RequestId);
                            this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                        }
                    }

                    if (this.tradeAndQuote)//Trade与Quote一起导入
                    {
                        List<ISeriesObject> listOther = GSKToGM.ConvertTrades(this.gskTicksCache);
                        foreach (ISeriesObject obj1 in listOther)
                        {
                            this.EmitHistoricalTrade(request.RequestId, request.Instrument, obj1 as Trade);
                            if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                            {
                                this.historicalPendingCancels.Remove(request.RequestId);
                                this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                            }
                        }
                    }
                }
                else
                {
                    foreach (ISeriesObject obj3 in list)
                    {
                        this.EmitHistoricalBar(request.RequestId, request.Instrument, obj3 as Bar);
                        if (this.historicalPendingCancels.ContainsKey(request.RequestId))
                        {
                            this.historicalPendingCancels.Remove(request.RequestId);
                            this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
                        }
                    }
                }
                this.EmitHistoricalDataRequestCompleted(request.RequestId, request.Instrument);
            }
        }
        /// <summary>
        /// 获取某个证券的某段时间的历史数据
        /// </summary>
        /// <param name="instrument"></param>
        /// <param name="dataType"></param>
        /// <param name="barSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<ISeriesObject> GetHistoricalData(Instrument instrument, HistoricalDataType dataType, int barSize, string beginTime, string endTime)
        {
            switch (dataType)
            {
                case HistoricalDataType.Bar:
                    List<GMSDK.Bar> gskBars = new List<GMSDK.Bar>();
                    gskBars = _md.GetBars(instrument.Symbol, barSize, beginTime, endTime);
                    return GSKToGM.ConvertBars(gskBars);

                case HistoricalDataType.Daily:
                    List<GMSDK.DailyBar> gskDailys = new List<GMSDK.DailyBar>();
                    gskDailys = _md.GetDailyBars(instrument.Symbol, beginTime, endTime);
                    return GSKToGM.ConvertDailys(gskDailys);

                case HistoricalDataType.Trade:
                    this.gskTicksCache = _md.GetTicks(instrument.Symbol, beginTime, endTime);
                    return GSKToGM.ConvertTrades(this.gskTicksCache);

                case HistoricalDataType.Quote:
                    this.gskTicksCache = _md.GetTicks(instrument.Symbol, beginTime, endTime);
                    return GSKToGM.ConvertQuotes(this.gskTicksCache);

                default:
                    throw new ArgumentException("Unknown data type: " + dataType.ToString());
            }
        }
        public void CancelHistoricalDataRequest(string requestId)
        {
            HistoricalDataRequest request = null;
            if (this.historicalDataIds.TryGetValue(requestId, out request))
            {
                this.historicalDataIds.Remove(requestId);
                this.historicalPendingCancels.Add(requestId, request);
                this.EmitHistoricalDataRequestCancelled(request.RequestId, request.Instrument);
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 返回的是beginDate到endDate前一天（不包括endDate）的交易日
        /// </summary>
        /// <param name="market"></param>
        /// <param name="beginDate">可以是日期格式，不必带上时间</param>
        /// <param name="endDate">可以是日期格式，不必带上时间</param>
        /// <returns></returns>
        public List<DateTime> GetTradeDates(string market, string beginDate, string endDate)
        {
            List<GMSDK.TradeDate> tradeDates = _md.GetCalendar(market, beginDate, endDate);
            List<DateTime> dates = new List<DateTime>();
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (GMSDK.TradeDate tradeDate in tradeDates)
            {
                dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
            }
            return dates;
        }
        public List<string> GetSymbols(string market, int securityType)
        {
            List<string> symbols = new List<string>();
            List<GMSDK.Instrument> gskInsts = _md.GetInstruments(market, securityType, 0);
            foreach (GMSDK.Instrument gskInst in gskInsts)
            {
                symbols.Add(gskInst.symbol);
            }
            return symbols;
        }
        public List<string> GetActiveSymbols(string market, int securityType)
        {
            List<string> symbols = new List<string>();
            List<GMSDK.Instrument> gskInsts = _md.GetInstruments(market, securityType, 1);
            foreach (GMSDK.Instrument gskInst in gskInsts)
            {
                symbols.Add(gskInst.symbol);
            }
            return symbols;
        }
        /// <summary>
        /// 返回date指定日期的当天股本
        /// </summary>
        /// <param name="insts"></param>
        /// <param name="date">可以是日期格式，不必带上时间</param>
        /// <returns></returns>
        private int batchRequestNum = 10;
        public Dictionary<string, GMSDK.ShareIndex> GetShareIndexs(List<Instrument> insts, string date)
        {
            Dictionary<string, GMSDK.ShareIndex> shareIndexDict = new Dictionary<string, GMSDK.ShareIndex>();
            int i = 0, j = 0;
            string symbolList = "";
            foreach (Instrument inst in insts)
            {
                i++;
                j++;
                symbolList += inst.Symbol + ",";
                if (i >= this.batchRequestNum || j >= insts.Count)
                {
                    List<GMSDK.ShareIndex> shareIndexs = _md.GetShareIndex(symbolList, date, date);
                    foreach (GMSDK.ShareIndex shareIndex in shareIndexs)
                    {
                        shareIndexDict.Add(shareIndex.symbol, shareIndex);
                    }
                    i = 0;
                    symbolList = "";
                }
            }
            return shareIndexDict;
        }
        public Dictionary<string, GMSDK.MarketIndex> GetMarketIndexs(List<Instrument> insts, string date)
        {
            Dictionary<string, GMSDK.MarketIndex> marketIndexDict = new Dictionary<string, GMSDK.MarketIndex>();
            int i = 0, j = 0;
            string symbolList = "";
            foreach (Instrument inst in insts)
            {
                i++;
                j++;
                symbolList += inst.Symbol + ",";
                if (i >= this.batchRequestNum || j >= insts.Count)
                {
                    List<GMSDK.MarketIndex> marketIndexs = _md.GetMarketIndex(symbolList, date, date);
                    foreach (GMSDK.MarketIndex marketIndex in marketIndexs)
                    {
                        marketIndexDict.Add(marketIndex.symbol, marketIndex);
                    }
                    i = 0;
                    symbolList = "";
                }
            }
            return marketIndexDict;
        }
        public Dictionary<string, GMSDK.FinancialIndex> GetFinancialIndexs(List<Instrument> insts, string date)
        {
            Dictionary<string, GMSDK.FinancialIndex> financialIndexDict = new Dictionary<string, GMSDK.FinancialIndex>();
            int i = 0, j = 0;
            string symbolList = "";
            foreach (Instrument inst in insts)
            {
                i++;
                j++;
                symbolList += inst.Symbol + ",";
                if (i >= this.batchRequestNum || j >= insts.Count)
                {
                    List<GMSDK.FinancialIndex> financialIndexs = _md.GetFinancialIndex(symbolList, date, date);
                    foreach (GMSDK.FinancialIndex financialIndex in financialIndexs)
                    {
                        financialIndexDict.Add(financialIndex.symbol, financialIndex);
                    }
                    i = 0;
                    symbolList = "";
                }
            }
            return financialIndexDict;
        }
        #endregion
    }
}
