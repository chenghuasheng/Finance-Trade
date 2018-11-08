using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Simulation.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

using SmartQuant.File;
namespace SmartQuant.Simulation
{
    public class SimulationDataProvider : IMarketDataProvider, IProvider
    {
        private const string PROVIDER_NAME = "Simulator(market data)";
        private const string PROVIDER_TITLE = "Simulation data provider";
        private const string PROVIDER_URL = "www.smartquant.com";
        private const byte PROVIDER_ID = 1;
        private const string CATEGORY_INFO = "Information";
        private const string CATEGORY_STATUS = "Status";
        private Simulator simulator;
        private bool isConnected;
        private ProviderStatus status;
        private IBarFactory factory;
        private Hashtable seriesTable;
        private Dictionary<long, int> slices;
        public event MarketDataRequestRejectEventHandler MarketDataRequestReject;
        public event MarketDataEventHandler NewMarketData;
        public event BarEventHandler NewBar;
        public event BarEventHandler NewBarOpen;
        public event BarSliceEventHandler NewBarSlice;
        public event QuoteEventHandler NewQuote;
        public event TradeEventHandler NewTrade;
        public event MarketDepthEventHandler NewMarketDepth;
        public event FundamentalEventHandler NewFundamental;
        public event CorporateActionEventHandler NewCorporateAction;
        public event MarketDataSnapshotEventHandler MarketDataSnapshot;
        public event BarEventHandler NewMarketBar;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        public event EventHandler StatusChanged;
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
                this.factory = value;
                if (this.factory != null)
                {
                    this.factory.NewBar += new BarEventHandler(this.OnNewBar);
                    this.factory.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
                    this.factory.NewBarSlice += new BarSliceEventHandler(this.OnNewBarSlice);
                }
            }
        }
        [Category("Information")]
        public string Name
        {
            get
            {
                return "Simulator(market data)";
            }
        }
        [Category("Information")]
        public string Title
        {
            get
            {
                return "Simulation data provider";
            }
        }
        [Category("Information")]
        public byte Id
        {
            get
            {
                return 1;
            }
        }
        [Category("Information")]
        public string URL
        {
            get
            {
                return "www.smartquant.com";
            }
        }
        [Category("Status")]
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }
        [Category("Status")]
        public ProviderStatus Status
        {
            get
            {
                return this.status;
            }
        }
        [Editor(typeof(SimulatorTypeEditor), typeof(UITypeEditor))]
        public Simulator Simulator
        {
            get
            {
                return this.simulator;
            }
        }
        //---------------------------------
        private bool monthPartitionStore = false; //数据是否按月分区存放
        [Category("Data"), Description("数据是否按月分区存放"), DefaultValue(false)]
        public bool MonthPartitionStore
        {
            get
            {
                return this.monthPartitionStore;
            }
            set
            {
                this.monthPartitionStore = value;
            }
        }
        private string partitionStoreDirectory = "c:/"; //按月分区存放的根目录
        [Category("Data"), Description("按月分区存放的根目录"), DefaultValue(@"C:\")]
        public string PartitionStoreDirectory
        {
            get
            {
                return this.partitionStoreDirectory;
            }
            set
            {
                this.partitionStoreDirectory = value.Trim();
            }
        }
        private string seriesNamesToPartitionStore = "";//按月分区存储的数据序列名称
        [Category("Data"), Description("按月分区存储的数据序列名称"), DefaultValue(@"Trade,Quote")]
        public string SeriesNamesToPartitionStore
        {
            get { return this.seriesNamesToPartitionStore; }
            set { this.seriesNamesToPartitionStore = value.Trim(); }
        }

        private string lastPartition = "";
        private DataFile lastFile = null;

        //以上为自己添加
        public SimulationDataProvider() : this("Simulator(market data)", 100)
        {
        }
        public SimulationDataProvider(string name, int id)
        {
            this.simulator = new Simulator();
            this.simulator.Error += new ExceptionEventHandler(this.OnError);
            this.simulator.NewObject += new SeriesObjectEventHandler(this.OnNewObject);
            this.simulator.LeaveInterval += new IntervalEventHandler(this.OnLeaveInterval);
            this.isConnected = false;
            this.status = ProviderStatus.Unknown;
            this.BarFactory = new BarFactory(false);
            this.seriesTable = new Hashtable();
            this.slices = new Dictionary<long, int>();
            ProviderManager.Add(this);
            ProviderManager.MarketDataSimulator = this;
        }
        public void SendMarketDataRequest(FIXMarketDataRequest request)
        {
            bool subscribe = request.SubscriptionRequestType == '1';
            for (int i = 0; i < request.NoRelatedSym; i++)
            {
                Instrument instrument = InstrumentManager.Instruments[request.GetRelatedSymGroup(i).Symbol];
                string stringValue = request.GetRelatedSymGroup(i).GetStringValue(10001);
                this.AddSimulatedSeries(instrument, stringValue, subscribe);
            }
        }
        public void Connect()
        {
            Monitor.Enter(this);
            try
            {
                if (!this.isConnected)
                {
                    this.ChangeStatus(ProviderStatus.Connecting);
                    this.isConnected = true;
                    this.ChangeStatus(ProviderStatus.Connected);
                    this.EmitConnectedEvent();
                    this.ChangeStatus(ProviderStatus.LoggingIn);
                    this.ChangeStatus(ProviderStatus.LoggedIn);
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
        public void Connect(int timeout)
        {
            this.Connect();
            ProviderManager.WaitConnected(this, timeout);
        }
        public void Disconnect()
        {
            Monitor.Enter(this);
            try
            {
                if (this.isConnected)
                {
                    this.simulator.Stop();
                    this.isConnected = false;
                    this.ChangeStatus(ProviderStatus.Disconnected);
                    this.EmitDisconnectedEvent();
                    if (this.lastFile != null)//此几句为自己添加，关闭打开的数据文件
                    {
                        this.lastFile.Close();
                        this.lastFile = null;
                        this.lastPartition = "";
                    }
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
        public void Shutdown()
        {
            this.Disconnect();
        }
        public override string ToString()
        {
            return this.Name;
        }
        private void EmitConnectedEvent()
        {
            if (this.Connected != null)
            {
                this.Connected(this, EventArgs.Empty);
            }
        }
        private void EmitDisconnectedEvent()
        {
            if (this.Disconnected != null)
            {
                this.Disconnected(this, EventArgs.Empty);
            }
        }
        private void ChangeStatus(ProviderStatus status)
        {
            this.status = status;
            this.EmitStatusChangedEvent();
        }
        private void EmitStatusChangedEvent()
        {
            if (this.StatusChanged != null)
            {
                this.StatusChanged(this, EventArgs.Empty);
            }
        }
        private void OnError(ExceptionEventArgs args)
        {
            if (this.Error != null)
            {
                this.Error(new ProviderErrorEventArgs(new ProviderError(Clock.Now, this, -1, -1, args.Exception.ToString())));
            }
        }
        private void OnNewObject(SeriesObjectEventArgs args)
        {
            Instrument instrument = this.seriesTable[args.Series] as Instrument;
            if (instrument == null)
            {
                instrument = InstrumentManager.Instruments[args.Series.Name.Substring(0, args.Series.Name.IndexOf('.'))];
            }
            IDataObject @object = args.Object;
            if (@object is Bar)
            {
                Bar bar = @object as Bar;
                if (bar.IsComplete)
                {
                    this.EmitNewBarEvent(instrument, bar);
                    if (bar.BarType == BarType.Time)
                    {
                        Dictionary<long, int> dictionary;
                        long size;
                        (dictionary = this.slices)[size = bar.Size] = dictionary[size] - 1;
                        if (this.slices[bar.Size] == 0)
                        {
                            this.EmitNewBarSlice(bar.Size);
                            return;
                        }
                    }
                }
                else
                {
                    this.EmitNewBarOpenEvent(instrument, bar);
                    if (bar.BarType == BarType.Time)
                    {
                        if (!this.slices.ContainsKey(bar.Size))
                        {
                            this.slices[bar.Size] = 0;
                        }
                        Dictionary<long, int> dictionary2;
                        long size2;
                        (dictionary2 = this.slices)[size2 = bar.Size] = dictionary2[size2] + 1;
                    }
                }
                return;
            }
            if (@object is Trade)
            {
                this.EmitNewTradeEvent(instrument, @object as Trade);
                return;
            }
            if (@object is Quote)
            {
                this.EmitNewQuoteEvent(instrument, @object as Quote);
                return;
            }
            if (@object is MarketDepth)
            {
                this.EmitNewMarketDepth(instrument, @object as MarketDepth);
                return;
            }
            if (@object is Fundamental)
            {
                this.EmitNewFundamental(instrument, @object as Fundamental);
                return;
            }
            if (@object is CorporateAction)
            {
                this.EmitNewCorporateAction(instrument, @object as CorporateAction);
            }
        }
        private void OnLeaveInterval(IntervalEventArgs args)
        {
            this.slices.Clear();
        }
        private void EmitNewBarSlice(long barSize)
        {
            if (this.NewBarSlice != null)
            {
                this.NewBarSlice(this, new BarSliceEventArgs(barSize, this));
            }
        }
        private void EmitNewBarEvent(IFIXInstrument instrument, Bar bar)
        {
            if (this.NewBar != null)
            {
                this.NewBar(this, new BarEventArgs(bar, instrument, this));
            }
        }
        private void EmitNewBarOpenEvent(IFIXInstrument instrument, Bar bar)
        {
            if (this.NewBarOpen != null)
            {
                this.NewBarOpen(this, new BarEventArgs(bar, instrument, this));
            }
        }
        private void EmitNewTradeEvent(IFIXInstrument instrument, Trade trade)
        {
            if (this.NewTrade != null)
            {
                this.NewTrade(this, new TradeEventArgs(trade, instrument, this));
            }
            if (this.factory != null)
            {
                this.factory.OnNewTrade(instrument, trade);
            }
        }
        private void EmitNewQuoteEvent(IFIXInstrument instrument, Quote quote)
        {
            if (this.NewQuote != null)
            {
                this.NewQuote(this, new QuoteEventArgs(quote, instrument, this));
            }
            if (this.factory != null)
            {
                this.factory.OnNewQuote(instrument, quote);
            }
        }
        private void EmitNewMarketDepth(IFIXInstrument instrument, MarketDepth marketDepth)
        {
            if (this.NewMarketDepth != null)
            {
                this.NewMarketDepth(this, new MarketDepthEventArgs(marketDepth, instrument, this));
            }
        }
        private void EmitNewFundamental(IFIXInstrument instrument, Fundamental fundamental)
        {
            if (this.NewFundamental != null)
            {
                this.NewFundamental(this, new FundamentalEventArgs(fundamental, instrument, this));
            }
        }
        private void EmitNewCorporateAction(IFIXInstrument instrument, CorporateAction corporateAction)
        {
            if (this.NewCorporateAction != null)
            {
                this.NewCorporateAction(this, new CorporateActionEventArgs(corporateAction, instrument, this));
            }
        }
        private void AddSimulatedSeries(Instrument instrument, string seriesSuffix, bool subscribe)
        {
            //-------------------------------下面一段是为了读出存储在按月分区目录中的数据序列
            DataSeriesList dataSeries = null;
            if (this.seriesNamesToPartitionStore.Contains(seriesSuffix))
            {
                DateTime curDate = Clock.Now.Date;
                this.ChangeStoreFileForTime(curDate);
                if (this.lastFile != null)
                {
                    dataSeries = new DataSeriesList();
                    foreach (FileSeries series in this.lastFile.Series)
                    {
                        if (!series.Name.StartsWith(string.Concat(instrument.Symbol, '.')))
                        {
                            continue;
                        }
                        dataSeries.Add(series);
                    }
                }
            }
            if (dataSeries == null || dataSeries.Count <= 0)
            {
                dataSeries = instrument.GetDataSeries();
            }
            ///以上为自己添加
            ///下面的为原代码，第一句已在上面使用，第二句貌似是多余的
            //dataSeries = instrument.GetDataSeries();
            //new Regex(seriesSuffix);
            foreach (IDataSeries dataSeries2 in dataSeries)
            {
                if (dataSeries2.Name.Substring(instrument.Symbol.Length + 1) == seriesSuffix)
                {
                    if (subscribe)
                    {
                        if (!this.simulator.InputSeries.Contains(dataSeries2))
                        {
                            this.simulator.InputSeries.Add(dataSeries2);
                            if (!this.seriesTable.Contains(dataSeries2))
                            {
                                this.seriesTable.Add(dataSeries2, instrument);
                            }
                        }
                    }
                    else
                    {
                        this.simulator.InputSeries.Remove(dataSeries2);
                        this.seriesTable.Remove(dataSeries2);
                    }
                }
            }
        }
        private void OnNewBar(object sender, BarEventArgs args)
        {
            if (this.NewBar != null)
            {
                this.NewBar(this, new BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarOpen(object sender, BarEventArgs args)
        {
            if (this.NewBarOpen != null)
            {
                this.NewBarOpen(this, new BarEventArgs(args.Bar, args.Instrument, this));
            }
        }
        private void OnNewBarSlice(object sender, BarSliceEventArgs args)
        {
            if (this.NewBarSlice != null)
            {
                this.NewBarSlice(this, new BarSliceEventArgs(args.BarSize, this));
            }
        }
        protected void ChangeStoreFileForTime(DateTime lastTime)
        {
            string curPartition = lastTime.Year.ToString() + "/" + lastTime.Month.ToString();
            if (this.lastFile == null || curPartition != this.lastPartition)
            {
                string location = this.partitionStoreDirectory + "/" + curPartition;
                try
                {
                    if (this.lastFile != null) this.lastFile.Close();
                    DataFile newFile = DataFile.Open(location);
                    this.lastFile = newFile;
                    this.lastPartition = curPartition;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        //以下为新增
        #region Custom Public Method
        public List<string> GetSymbols(string market, int securityType = 1)
        {
            List<string> symbols = new List<string>();
           string sType=SecurityType.Defaulted;
            switch (securityType)
            {
                case 1:
                    sType = SecurityType.CommonStock;
                    break;
                case 2:
                    sType = SecurityType.MutualFund;
                    break;
                case 3:
                    sType = SecurityType.Index;
                    break;
                case 4:
                    sType = SecurityType.Future;
                    break;
            }
            foreach (Instrument inst in InstrumentManager.Instruments)
            {
                if ((market == "" || inst.SecurityExchange == market) && inst.SecurityType == sType)
                {
                    symbols.Add(inst.Symbol);
                }
            }
            return symbols;
        }
        //当前时间的下一笔交易，交易时间必须是当前时间之后
        public Trade GetNextTrade(string symbol,DateTime lastTime)
        {
            Trade trade = null;
            if (this.seriesNamesToPartitionStore.Contains("Trade"))
            {
                this.ChangeStoreFileForTime(lastTime);
                if (this.lastFile != null)
                {
                    string seriesName = symbol + ".Trade";
                    if (this.lastFile.Series.Contains(seriesName))
                    {
                        FileSeries series = this.lastFile.Series[seriesName];
                        int index = series.IndexOf(lastTime, SearchOption.Next);
                        if (index >= 0)
                        {
                            ISeriesObject seriesObj = series[index];
                            if (seriesObj.DateTime == lastTime)
                            {
                                seriesObj = series[index + 1];
                            }
                            trade = (Trade)seriesObj;
                        }
                    }
                }
            }
            else
            {
                Instrument inst = InstrumentManager.Instruments[symbol];
                if (inst != null)
                {
                    FileSeries series = (FileSeries)inst.GetDataSeries("Trade");
                    if (series != null)
                    {
                        int index = series.IndexOf(lastTime, SearchOption.Next);
                        if (index >= 0)
                        {
                            ISeriesObject seriesObj = series[index];
                            if (seriesObj.DateTime == lastTime)
                            {
                                seriesObj = series[index + 1];
                            }
                            trade = (Trade)seriesObj;
                        }
                    }
                }
            }
            return trade;
        }
        //当前时间的最后一笔交易，交易的时间可以是当前时间
        public Trade GetLastTrade(string symbol, DateTime lastTime)
        {
            Trade trade = null;
            if (this.seriesNamesToPartitionStore.Contains("Trade"))
            {
                this.ChangeStoreFileForTime(lastTime);
                if (this.lastFile != null)
                {
                    string seriesName = symbol + ".Trade";
                    if (this.lastFile.Series.Contains(seriesName))
                    {
                        FileSeries series = this.lastFile.Series[seriesName];
                        ISeriesObject seriesObj = series[lastTime, SearchOption.Prev];
                        trade=(Trade)seriesObj;
                    }
                }
            }else
            {
                Instrument inst = InstrumentManager.Instruments[symbol];
                if (inst != null)
                {
                    FileSeries series = (FileSeries)inst.GetDataSeries("Trade");
                    if (series != null)
                    {
                        ISeriesObject seriesObj = series[lastTime, SearchOption.Prev];
                        trade = (Trade)seriesObj;
                    }
                }
            }
            return trade;
        }
        //获取多个证券的最后一笔交易
        public Dictionary<string, Trade> GetLastTrades(string[] symbols,DateTime lastTime)
        {
            Dictionary<string, Trade> tradeDict = new Dictionary<string, Trade>();
            
            if (this.seriesNamesToPartitionStore.Contains("Trade"))
            {
                this.ChangeStoreFileForTime(lastTime);         
                if (this.lastFile != null)
                {
                    foreach (string symbol in symbols)
                    {
                        string seriesName = symbol + ".Trade";
                        if (this.lastFile.Series.Contains(seriesName))
                        {
                            FileSeries series = this.lastFile.Series[seriesName];
                            ISeriesObject trade = series[lastTime, SearchOption.Prev];
                            tradeDict.Add(symbol, (Trade)trade);
                        }                  
                    }
                }
            }
            else
            {
                foreach (string symbol in symbols)
                {
                    Instrument inst = InstrumentManager.Instruments[symbol];
                    if (inst != null)
                    {
                        FileSeries series = (FileSeries)inst.GetDataSeries("Trade");
                        if (series != null)
                        {
                            ISeriesObject trade = series[lastTime, SearchOption.Prev];
                            tradeDict.Add(symbol, (Trade)trade);
                        }
                    }
                }
            }
            return tradeDict;
        }
        public List<Trade> GetTrades(string symbol,DateTime beginTime,DateTime endTime)
        {
            List<Trade> trades = new List<Trade>();
            if (this.seriesNamesToPartitionStore.Contains("Trade"))
            {
                this.ChangeStoreFileForTime(beginTime);
                if (this.lastFile != null)
                {
                    string seriesName = symbol + ".Trade";
                    if (this.lastFile.Series.Contains(seriesName))
                    {
                        FileSeries series = this.lastFile.Series[seriesName];
                        ISeriesObject[] seriesObjs=series.GetArray(beginTime, endTime);
                        foreach (ISeriesObject seriesObj in seriesObjs) trades.Add((Trade)seriesObj);
                    }
                }
            }
            else
            {
                Instrument inst = InstrumentManager.Instruments[symbol];
                if (inst != null)
                {
                    FileSeries series = (FileSeries)inst.GetDataSeries("Trade");
                    if (series != null)
                    {
                        ISeriesObject[] seriesObjs = series.GetArray(beginTime, endTime);
                        foreach (ISeriesObject seriesObj in seriesObjs) trades.Add((Trade)seriesObj);
                    }
                }
            }
            return trades;
        }
        public List<Daily> GetLastNDailys(string symbol, int n, DateTime lastDate)
        {
            Instrument inst = InstrumentManager.Instruments[symbol];
            List<Daily> dailys = new List<Daily>();
            if (inst != null)
            {
                FileSeries series = (FileSeries)inst.GetDataSeries("Daily");
                if (series != null)
                {
                    int last = series.IndexOf(lastDate, SearchOption.Prev);
                    if (last >= 0)
                    {
                        int first = last - n + 1;
                        first = first < 0 ? 0 : first;
                        ISeriesObject[] seriesObjects = series.GetArray(first, last);
                        foreach (ISeriesObject seriesObject in seriesObjects)
                        {
                            dailys.Add((Daily)seriesObject);
                        }
                    }                   
                }   
            }
            return dailys;
        }
        public List<Bar> GetLastNBars(string symbol, int barSize, int n, DateTime lastTime)
        {
            Instrument inst = InstrumentManager.Instruments[symbol];
            List<Bar> bars = new List<Bar>();
            if (inst != null)
            {
                string seriesName = "Bar.Time." + barSize.ToString();
                FileSeries series = (FileSeries)inst.GetDataSeries(seriesName);
                if (series != null)
                {
                    int last = series.IndexOf(lastTime, SearchOption.Prev);
                    if (last >= 0)
                    {
                        int first = last - n + 1;
                        first = first < 0 ? 0 : first;
                        ISeriesObject[] seriesObjects = series.GetArray(first, last);
                        foreach (ISeriesObject seriesObject in seriesObjects)
                        {
                            bars.Add((Bar)seriesObject);
                        }
                    }
                }
            }
            return bars;
        }

        public void FlushAllSeries()
        {
            if (this.lastFile != null)
            {
                foreach(FileSeries series in this.lastFile.Series)
                {
                    series.Flush();
                }
            }
            foreach(Instrument inst in InstrumentManager.Instruments)
            {
                DataSeriesList seriesList = inst.GetDataSeries();
                foreach(IDataSeries series in seriesList)
                {
                    series.Flush();
                }
            }
        }
        #endregion
    }
}
