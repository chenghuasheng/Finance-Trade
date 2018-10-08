using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Execution;

using System.ComponentModel;
namespace HuaQuant.Data.TDX
{
    public class TDXTradeProvider : IProvider, IExecutionProvider
    {
        private string ip;
        private short port;
        private string version;
        private short departmentID;
        private string accountID;
        private string fundID;
        private string password;
        private TDXTrader trader;
        private System.Timers.Timer timer=null;
        
        [Category("设置"), Description("交易服务器IP")]
        public string IP
        {
            get { return this.ip; }
            set { this.ip = value; }
        }
        [Category("设置"), Description("交易服务器Port")]
        public short Port
        {
            get { return this.port; }
            set { this.port = value; }
        }
        [Category("设置"), Description("交易客户端软件版本号")]
        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }
        
        [Category("设置"), Description("营业部编码")]
        public short DepartmentID
        {
            get { return this.departmentID; }
            set { this.departmentID = value; }
        }
        [Category("设置"), Description("客户号"), PasswordPropertyText(true)]
        public string AccountID
        {
            get { return this.accountID; }
            set { this.accountID = value; }
        }
        [Category("设置"), Description("资金账号"), PasswordPropertyText(true)]
        public string FundID
        {
            get { return this.fundID; }
            set { this.fundID = value; }
        }
        [Category("设置"), Description("密码"), PasswordPropertyText(true)]
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
        private int refreshTime = 300;
        [Category("设置"), Description("更新委托状态的间隔时间，单位秒"), DefaultValue(300)]
        public int RefreshTime
        {
            get { return this.refreshTime; }
            set { this.refreshTime = value; }
        }
        private TimeSpan beginTime = new TimeSpan(0, 0, 0);
        [Category("设置"), Description("开盘时间")]
        public TimeSpan BeginTime
        {
            get { return this.beginTime; }
            set { this.beginTime = value; }
        }
        private TimeSpan endTime = new TimeSpan(24, 0, 0);
        [Category("设置"), Description("收盘时间")]
        public TimeSpan EndTime
        {
            get { return this.endTime; }
            set { this.endTime = value; }
        }
        private double commission = 0.00025;
        [Category("设置"), Description("佣金比率")]
        public double Commission
        {
            get { return this.commission; }
            set { this.commission = value; }
        }
        private double minCommission = 5.0;
        [Category("设置"), Description("最小佣金")]
        public double MinCommission
        {
            get { return this.minCommission; }
            set { this.minCommission = value; }
        }
        private double stampDuty = 0.001;
        [Category("设置"), Description("印花税率")]
        public double StampDuty
        {
            get { return this.stampDuty; }
            set { this.stampDuty = value; }
        }

        public TDXTradeProvider()
        {
            this.trader = TDXTrader.Instance;
            ProviderManager.Add(this);
        }
        #region IProvider
        public byte Id
        {
            get
            {
                return 198;
            }
        }
        private bool isConnected = false;
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        public string Name
        {
            get
            {
                return "TDXTradeProvider";
            }
        }

        public ProviderStatus Status
        {
            get
            {
                if (!this.isConnected)
                    return ProviderStatus.Disconnected;
                else
                    return ProviderStatus.Connected;
            }
        }

        public string Title
        {
            get
            {
                return "通达信证券交易执行提供者";
            }
        }

        public string URL
        {
            get
            {
                return String.Empty;
            }
        }

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        public event EventHandler StatusChanged;

        public void OnDisconnection(object sender, EventArgs e)
        {
            this.isConnected = false;
            Console.WriteLine("已登出.");
            if (Disconnected != null)
                Disconnected(this, new EventArgs());
        }
        public void Connect()
        {
            if (!this.isConnected)
            {
                ReportArgs rargs = this.trader.Logon(this.ip, this.port, this.version, this.departmentID, this.accountID, this.fundID, this.password);
                if (rargs.Succeeded)
                {
                    this.isConnected = true;
                    if (this.timer == null)
                    {
                        this.timer = new System.Timers.Timer();
                        this.timer.Interval = this.refreshTime * 1000;
                        this.timer.Elapsed += new ElapsedEventHandler(tickTimer_Elapsed);
                        this.timer.Start();
                    }
                    Console.WriteLine("已登录.");
                    if (Connected != null)
                    {
                        Connected(this, new EventArgs());
                    }
                    if (StatusChanged != null)
                    {
                        StatusChanged(this, new EventArgs());
                    }
                }else
                {
                    this.EmitError(this.Id, -1, rargs.ErrorInfo);
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
            if (this.isConnected)
            {
                if (this.timer != null)
                {
                    this.timer.Elapsed -= tickTimer_Elapsed;
                    this.timer.Stop();
                    this.timer = null;
                }
                this.trader.Logoff();
                this.isConnected = false;

                Console.WriteLine("已登出.");
                if (Disconnected != null)
                {
                    Disconnected(this, new EventArgs());
                }
                if (StatusChanged != null)
                {
                    StatusChanged(this, new EventArgs());
                }
            }
        }

        public void Shutdown()
        {
            this.Disconnect();
            this.trader.Close();
        }
        private void EmitError(int id, int code, string message)
        {
            if (Error != null)
                Error(new ProviderErrorEventArgs(new ProviderError(DateTime.Now, this, id, code, message)));
        }
        #endregion
        #region IExcutionProvider

        private OrderRecordList orders = new OrderRecordList();
        public event ExecutionReportEventHandler ExecutionReport;
        public event OrderCancelRejectEventHandler OrderCancelReject;
        public BrokerInfo GetBrokerInfo()
        {
            BrokerInfo brokerInfo = new BrokerInfo();
            if (!this.isConnected)
            {
                this.EmitError(this.Id, -1, "The TDXTradeProvider is not connected.");
                return brokerInfo;
            }
            ReportArgs rargs = this.trader.QueryFund();
            if (rargs.Succeeded&&rargs.Result!=null)
            {
                FundRecord fundRecord = (FundRecord)rargs.Result;
                BrokerAccount brokerAccount = new BrokerAccount("通达信-" + this.fundID);
                brokerAccount.AddField("Balance", fundRecord.Balance.ToString());
                brokerAccount.AddField("Available", fundRecord.Available.ToString());
                brokerAccount.AddField("TotalAssets", fundRecord.TotalAsserts.ToString());
                brokerAccount.AddField("Desirable", fundRecord.Desirable.ToString());
                brokerAccount.AddField("MarketValue", fundRecord.MarketValue.ToString());
                brokerAccount.AddField("Frozen", fundRecord.Frozen.ToString());
                brokerAccount.BuyingPower = fundRecord.Available;
                brokerInfo.Accounts.Add(brokerAccount);
                rargs = this.trader.QueryPositions();
                if (rargs.Succeeded && rargs.Result != null)
                {
                    List<PositionRecord> positions = (List<PositionRecord>)rargs.Result;
                    foreach (PositionRecord position in positions)
                    {
                        BrokerPosition brokerPosition = new BrokerPosition();
                        brokerPosition.SecurityExchange = position.SecurityExchange;
                        brokerPosition.Symbol = position.SecurityExchange + "." + position.SecurityID;
                        brokerPosition.AddCustomField("Available", position.Available.ToString());
                        brokerPosition.Qty= position.Quantity;
                        brokerPosition.AddCustomField("CostPrice", position.CostPrice.ToString());
                        brokerPosition.LongQty = position.Quantity;
                        brokerAccount.AddPosition(brokerPosition);
                    }
                }
                else
                {
                    this.EmitError(this.Id, -1, rargs.ErrorInfo);
                }
            }
            else
            {
                this.EmitError(this.Id, -1, rargs.ErrorInfo);
            }
            

            return brokerInfo;
        }

        public void SendNewOrderSingle(NewOrderSingle order)
        {
            if (!this.isConnected)
            {
                this.EmitError(this.Id, -1, "The TDXTradeProvider is not connected.");
            }
                try
            {
                Instrument curInstrument = InstrumentManager.Instruments[order.Symbol];
                string securityID = curInstrument.SecurityID;
                OrderSide orderSide;
                switch (order.Side)
                {
                    case Side.Buy:
                        orderSide = OrderSide.Buy;
                        break;
                    case Side.Sell:
                        orderSide = OrderSide.Sell;
                        break;
                    default:
                        throw new Exception("不支持的买卖指令。");
                }
                OrderType orderType;
                switch (order.OrdType)
                {
                    case OrdType.Market:
                        orderType = OrderType.CancelAfterFiveQuoteTransaction;
                        break;
                    case OrdType.Limit:
                        orderType = OrderType.Limit;
                        break;
                    default:
                        throw new Exception("不支持的订单类型");
                }
                ReportArgs rargs=this.trader.SendOrder(orderSide, orderType, securityID, (float)order.Price, (long)order.OrderQty);
                if (rargs.Succeeded)//下单成功
                {
                    string orderID = (string)rargs.Result;//委托编号
                    OrderRecord orderRecord = new OrderRecord();
                    orderRecord.InnerOrderID = orderID;
                    orderRecord.OuterOrderID = order.ClOrdID;
                    orderRecord.SecurityID = order.SecurityID;
                    orderRecord.OrderStatus = OrderStatus.New;
                    orderRecord.OrderSide = orderSide;
                    orderRecord.OrderType = orderType;
                    orderRecord.OrderPrice = order.Price;
                    orderRecord.OrderQty = (long)order.OrderQty;
                    orderRecord.AvgPx = 0.0;
                    orderRecord.CumQty = 0;
                    orderRecord.TransactTime = Clock.Now;
                    lock (this.orders)
                    {
                        this.orders.AddRecord(orderRecord, orderID, order.ClOrdID);
                    }
                    ExecutionReport report = new ExecutionReport();
                    report.TransactTime = orderRecord.TransactTime;
                    report.ExecType = ExecType.New;
                    report.ClOrdID = order.ClOrdID;
                    report.OrdStatus = OrdStatus.New;
                    report.AvgPx = 0.0;
                    report.CumQty = 0;
                    report.OrderQty = order.OrderQty;
                    report.Price = order.Price;
                    report.LeavesQty = order.OrderQty;
                    report.Side = order.Side;
                    if (order.OrdType == OrdType.Limit)
                    {
                        report.TimeInForce = TimeInForce.Day;//强制限价单为当日有效
                    }
                    if (ExecutionReport != null)
                    {
                        ExecutionReport(this, new ExecutionReportEventArgs(report));
                    }
                    Thread.Sleep(1000);
                    this.updateOrders();
                }
                else//下单失败
                {
                    this.EmitError(this.Id, -1, rargs.ErrorInfo);
                    ExecutionReport report = new ExecutionReport();
                    report.TransactTime = DateTime.Now;
                    report.ExecType = ExecType.Rejected;
                    report.ClOrdID = order.ClOrdID;
                    report.OrdStatus = OrdStatus.Rejected;
                    report.AvgPx = 0.0;
                    report.CumQty = 0;
                    report.OrderQty = order.OrderQty;
                    report.Price = order.Price;
                    report.LeavesQty = order.OrderQty;
                    report.Side = order.Side;
                    report.Text = rargs.ErrorInfo;
                    if (ExecutionReport != null)
                    {
                        ExecutionReport(this, new ExecutionReportEventArgs(report));
                    }
                }
            }catch(Exception ex)
            {
                this.EmitError(this.Id, -1, ex.Message);
            }
        }

        public void SendOrderCancelReplaceRequest(FIXOrderCancelReplaceRequest request)
        {
            throw new NotImplementedException();
        }

        public void SendOrderCancelRequest(FIXOrderCancelRequest request)
        {
            if (!this.isConnected)
            {
                this.EmitError(this.Id, -1, "The TDXTradeProvider is not connected.");
            }
            string origOuterOrderID = request.OrigClOrdID;
            OrderRecord orderRecord = this.orders.GetRecord(origOuterOrderID, 1);
            if (orderRecord == null)
            {
                string errorInfo = "要取消的订单没有记录。";
                this.EmitError(this.Id, -1, errorInfo);
                OrderCancelReject report = new OrderCancelReject();
                report.TransactTime = DateTime.Now;
                report.OrigClOrdID = request.OrigClOrdID;
                report.ClOrdID = request.ClOrdID;
                report.CxlRejReason = CxlRejReason.UnknownOrder;
                report.CxlRejResponseTo = CxlRejResponseTo.CancelRequest;
                report.Text = errorInfo;
                report.OrdStatus = OrdStatus.Rejected;
                if (OrderCancelReject != null)
                {
                    OrderCancelReject(this, new OrderCancelRejectEventArgs(report));
                }
                return;
            }
            string origInnerOrderID = orderRecord.InnerOrderID;
            ReportArgs rargs = this.trader.CancelOrder(origInnerOrderID);
            if (rargs.Succeeded)
            {
                Thread.Sleep(1000);
                this.updateOrders();
            }else
            {
                this.EmitError(this.Id, -1, rargs.ErrorInfo);
                OrderCancelReject report = new OrderCancelReject();    
                report.TransactTime = DateTime.Now;
                report.OrigClOrdID= request.OrigClOrdID;
                report.ClOrdID = request.ClOrdID;
                report.CxlRejReason = CxlRejReason.BrokerOption;
                report.CxlRejResponseTo = CxlRejResponseTo.CancelRequest;
                report.Text = rargs.ErrorInfo;
                report.OrdStatus = OrdStatus.Rejected;
                if (OrderCancelReject!= null)
                {
                    OrderCancelReject(this,new OrderCancelRejectEventArgs(report));
                }
            }
        }
        
        public void SendOrderStatusRequest(FIXOrderStatusRequest request)
        {
            if (!this.isConnected)
            {
                this.EmitError(this.Id, -1, "The TDXTradeProvider is not connected.");
            }
            string outerOrderID = request.OrderID;
            OrderRecord orderRecord = this.orders.GetRecord(outerOrderID, 1);
            if (orderRecord == null)
            {
                orderRecord = new OrderRecord();
                SingleOrder order = (SingleOrder)OrderManager.Orders.All[request.OrderID];
                if (order != null)
                {
                    orderRecord.TransactTime = order.TransactTime;
                    orderRecord.OrderQty = (long)order.OrderQty;
                    orderRecord.OrderPrice = order.Price;
                    orderRecord.SecurityID = order.SecurityID;
                    switch (order.OrdStatus)
                    {
                        case OrdStatus.New:
                            orderRecord.OrderStatus = OrderStatus.New;
                            break;
                        case OrdStatus.PartiallyFilled:
                            orderRecord.OrderStatus = OrderStatus.PartiallyFilled;
                            break;
                        case OrdStatus.Filled:
                            orderRecord.OrderStatus = OrderStatus.Filled;
                            break;
                        case OrdStatus.PendingCancel:
                            orderRecord.OrderStatus = OrderStatus.PendingCancel;
                            break;
                        case OrdStatus.Cancelled:
                            orderRecord.OrderStatus = OrderStatus.Cancelled;
                            break;
                        case OrdStatus.Rejected:
                            orderRecord.OrderStatus = OrderStatus.Rejected;
                            break;
                        default:
                            orderRecord.OrderStatus = OrderStatus.New;
                            break;
                    }
                    orderRecord.OuterOrderID = order.ClOrdID;
                    orderRecord.AvgPx = order.AvgPx;
                    orderRecord.CumQty = (long)order.CumQty;
                    switch (order.Side)
                    {
                        case (Side.Buy):
                            orderRecord.OrderSide = OrderSide.Buy;
                            break;
                        case (Side.Sell):
                            orderRecord.OrderSide = OrderSide.Sell;
                            break;
                        default:
                            throw new Exception("不支持的买卖指令。");
                    }
                    lock (this.orders)
                    {
                        this.orders.AddRecord(orderRecord, null, outerOrderID);
                    }
                }  
            }
            this.updateOrders();
        }
        private void tickTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime today = Clock.Now.Date;
            if (e.SignalTime >= today + this.beginTime && e.SignalTime <= today + this.endTime)
            {
                this.updateOrders();
            }
        }
        private object updating=new object();
        private void updateOrders()
        {
            if (!this.isConnected)
            {
                this.EmitError(this.Id, -1, "The TDXTradeProvider is not connected.");
            }
            lock (this.updating)
            {
                ReportArgs rargs = this.trader.QueryOrders();
                if (rargs.Succeeded)
                {
                    List<OrderRecord> orderRecords = (List<OrderRecord>)rargs.Result;
                    foreach (OrderRecord orderRecord in orderRecords)
                    {
                        bool needReport = false;
                        OrderRecord prevOrderRecord = this.orders.GetRecord(orderRecord.InnerOrderID, 0);
                        if (prevOrderRecord == null)
                        {
                            prevOrderRecord = this.orders.SearchRecord(orderRecord);
                        }
                        if (prevOrderRecord != null)
                        {
                            if (prevOrderRecord.OrderStatus != orderRecord.OrderStatus)
                            {
                                needReport = true;
                            }
                            else if (orderRecord.OrderStatus == OrderStatus.PartiallyFilled
                               && prevOrderRecord.CumQty < orderRecord.CumQty)
                            {
                                needReport = true;
                            }
                        }
                        if (needReport)
                        {
                            ExecutionReport report = new ExecutionReport();
                            report.TransactTime = orderRecord.TransactTime;
                            switch (orderRecord.OrderStatus)
                            {
                                case OrderStatus.New:
                                    report.ExecType = ExecType.New;
                                    report.OrdStatus = OrdStatus.New;
                                    break;
                                case OrderStatus.PartiallyFilled:
                                    report.ExecType = ExecType.PartialFill;
                                    report.OrdStatus = OrdStatus.PartiallyFilled;
                                    break;
                                case OrderStatus.Filled:
                                    report.ExecType = ExecType.Fill;
                                    report.OrdStatus = OrdStatus.Filled;
                                    break;
                                case OrderStatus.PendingCancel:
                                    report.ExecType = ExecType.PendingCancel;
                                    report.OrdStatus = OrdStatus.PendingCancel;
                                    break;
                                case OrderStatus.Cancelled:
                                    report.ExecType = ExecType.Cancelled;
                                    report.OrdStatus = OrdStatus.Cancelled;
                                    break;
                                case OrderStatus.Rejected:
                                    report.ExecType = ExecType.Rejected;
                                    report.OrdStatus = OrdStatus.Rejected;
                                    break;
                                default:
                                    report.ExecType = ExecType.Undefined;
                                    report.OrdStatus = OrdStatus.Undefined;
                                    break;
                            }
                            report.ClOrdID = prevOrderRecord.OuterOrderID;
                            if (orderRecord.OrderStatus == OrderStatus.PendingCancel || orderRecord.OrderStatus == OrderStatus.Cancelled)
                            {
                                report.OrigClOrdID = prevOrderRecord.OuterOrderID;
                            }
                            
                            report.OrderQty = orderRecord.OrderQty;
                            report.AvgPx = orderRecord.AvgPx;
                            report.CumQty = orderRecord.CumQty;
                            report.LeavesQty = orderRecord.OrderQty - orderRecord.CumQty;
                            switch (orderRecord.OrderSide)
                            {
                                case (OrderSide.Buy):
                                    report.Side = Side.Buy;
                                    break;
                                case (OrderSide.Sell):
                                    report.Side = Side.Sell;
                                    break;
                                default:
                                    report.Side = Side.Undefined;
                                    break;
                            }
                            //计算费率
                            if (report.CumQty > 0)
                            {
                                report.Commission = this.commission * report.CumQty * report.AvgPx;
                                report.CommType = CommType.Absolute;
                                if (report.Commission < this.minCommission) report.Commission = this.minCommission;
                                if (report.Side == Side.Sell)
                                {
                                    report.Commission += this.stampDuty * report.CumQty * report.AvgPx;
                                }
                            }
                            if (ExecutionReport != null)
                            {
                                ExecutionReport(this, new ExecutionReportEventArgs(report));
                            }
                            prevOrderRecord.OrderStatus = orderRecord.OrderStatus;
                            prevOrderRecord.AvgPx = orderRecord.AvgPx;
                            prevOrderRecord.CumQty = orderRecord.CumQty;
                        }
                    }
                }
                else
                {
                    this.EmitError(this.Id, -1, rargs.ErrorInfo);
                }
            }
        }
        #endregion
    }
}
