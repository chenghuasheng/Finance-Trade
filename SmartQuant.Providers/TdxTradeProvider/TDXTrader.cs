using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace HuaQuant.Data.TDX
{
    public class TDXTrader
    {
        private int clientID=-1;
        private string ip;
        private short port;
        private string version;
        private short departmentID;
        private string accountID;
        private string fundID;
        private string password;

        private static TDXTrader instance = null;

        
        private TDXTrader()
        {
            TDXWrapper.OpenTdx();
        }
        public static TDXTrader Instance
        {
            get
            {
                if (TDXTrader.instance == null)
                {
                    TDXTrader.instance = new TDXTrader();
                }
                return TDXTrader.instance;
            }
        }
        public void Close()
        {
            TDXWrapper.CloseTdx();
        }
        public ReportArgs Logon(string ip, short port, string version, short departmentID, string accountID, string fundID, string password)
        {
            this.ip = ip;
            this.port = port;
            this.version = version;
            this.departmentID = departmentID;
            this.accountID = accountID;
            this.fundID = fundID;
            this.password = password;
            StringBuilder errInfo = new StringBuilder(256);
            this.clientID=TDXWrapper.Logon(this.ip, this.port, this.version, this.departmentID, this.accountID, this.fundID, this.password, string.Empty, errInfo);
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID==-1)
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }else
            {
                reportArgs.Succeeded = true;
            }
            return reportArgs;
        }
        public ReportArgs ReLogon()
        {
            StringBuilder errInfo = new StringBuilder(256);
            this.clientID = TDXWrapper.Logon(this.ip, this.port, this.version, this.departmentID, this.accountID, this.fundID, this.password, string.Empty, errInfo);
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID == -1)
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            else
            {
                reportArgs.Succeeded = true;
            }
            return reportArgs;
        }
        public void Logoff()
        {
            if (this.clientID != -1)
            {
                TDXWrapper.Logoff(this.clientID);
                this.clientID = -1;
            }
        }
        public ReportArgs SendOrder(OrderSide orderSide, OrderType orderType, string securityID, float price, long quantity)
        {
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID != -1)
            { 
                //股东代码似乎可以为空
                string shareholderCode = "";
                StringBuilder errInfo = new StringBuilder(256);
                StringBuilder result = new StringBuilder(1024 * 1024);
                TDXWrapper.SendOrder(this.clientID, (int)orderSide,(int)orderType,shareholderCode, securityID, price, (int)quantity, result, errInfo);
                
                if (errInfo.ToString() != string.Empty)//下单失败
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                }
                else//下单成功
                {
                    List<string[]> data = this.pickUp(result);
                    reportArgs.Succeeded = true;
                    if (data.Count > 1)
                    {
                        reportArgs.Result = data[1][0];//返回订单编号
                    }
                }
            }else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = "交易账号未登录.";
            }
            return reportArgs;
        }
        public ReportArgs CancelOrder(string orderID)
        {
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID != -1)
            {
                //交易所编号，似乎可以为空
                string exchangeID = "";
                StringBuilder errInfo = new StringBuilder(256);
                StringBuilder result = new StringBuilder(1024 * 1024);
                TDXWrapper.CancelOrder(this.clientID, exchangeID, orderID, result, errInfo);
                if (errInfo.ToString() != string.Empty)//撤单失败
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                }
                else//下单成功
                {
                    List<string[]> data = this.pickUp(result);
                    reportArgs.Succeeded = true;
                    if (data.Count > 1)
                    {
                        reportArgs.Result = data[1][0];
                    }
                }
            }
            else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = "交易账号未登录.";
            }
            return reportArgs;
        }
        public ReportArgs QueryFund()
        {
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID != -1)
            {
                StringBuilder errInfo = new StringBuilder(256);
                StringBuilder result = new StringBuilder(1024 * 1024);
                TDXWrapper.QueryData(this.clientID, 0, result, errInfo);
                if (errInfo.ToString() != string.Empty)//查询失败
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                }
                else//查询成功
                {
                    List<string[]> data = this.pickUp(result);
                    reportArgs.Succeeded = true;
                    if (data.Count > 1)
                    {
                        FundRecord fundRecord = new FundRecord();
                        fundRecord.Balance = double.Parse(data[1][1]);
                        fundRecord.Available = double.Parse(data[1][2]);
                        fundRecord.Frozen = double.Parse(data[1][3]);
                        fundRecord.Desirable = double.Parse(data[1][4]);
                        fundRecord.TotalAsserts = double.Parse(data[1][5]);
                        fundRecord.MarketValue = double.Parse(data[1][6]);
                        reportArgs.Result = fundRecord;
                    }
                }
            }else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = "交易账号未登录.";
            }
            return reportArgs;
        }
        public ReportArgs QueryPositions()
        {
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID != -1)
            {
                StringBuilder errInfo = new StringBuilder(256);
                StringBuilder result = new StringBuilder(1024 * 1024);
                TDXWrapper.QueryData(this.clientID, 1, result, errInfo);
                if (errInfo.ToString() != string.Empty)//查询失败
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                }
                else//查询成功
                {
                    List<string[]> data = this.pickUp(result);
                    reportArgs.Succeeded = true;
                    List<PositionRecord> positions = new List<PositionRecord>();
                    for(int i = 1; i < data.Count; i++)
                    {
                        PositionRecord positionRecord = new PositionRecord();
                        positionRecord.SecurityID = data[i][0];
                        positionRecord.Quantity = double.Parse(data[i][2]);
                        positionRecord.Available = double.Parse(data[i][3]);
                        positionRecord.CostPrice = double.Parse(data[i][4]);
                        switch (int.Parse(data[i][11]))
                        {
                            case 1:
                                positionRecord.SecurityExchange = "SHSE";
                                break;
                            case 0:
                                positionRecord.SecurityExchange = "SZSE";
                                break;
                        }
                        positions.Add(positionRecord);
                    }
                    reportArgs.Result = positions;
                }
            }else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = "交易账号未登录.";
            }
            return reportArgs;
        }
        public ReportArgs QueryOrders()
        {
            ReportArgs reportArgs = new ReportArgs();
            if (this.clientID != -1)
            {
                StringBuilder errInfo = new StringBuilder(256);
                StringBuilder result = new StringBuilder(1024 * 1024);
                TDXWrapper.QueryData(this.clientID, 2, result, errInfo);
                if (errInfo.ToString() != string.Empty)//查询失败
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                }
                else//查询成功
                {
                    List<string[]> data = this.pickUp(result);
                    reportArgs.Succeeded = true;
                    List<OrderRecord> resultList = new List<OrderRecord>();
                    string dateString = DateTime.Today.ToString("yyyy-MM-dd");
                    for(int i = 1; i < data.Count; i++)
                    {
                        OrderRecord orderRecord = new OrderRecord();
                        orderRecord.TransactTime = DateTime.Parse(dateString + " " + data[i][0]);
                        orderRecord.SecurityID = data[i][1];
                        orderRecord.OrderSide = (OrderSide)int.Parse(data[i][3]);
                        switch (data[i][6])
                        {
                            case "待报":
                                orderRecord.OrderStatus = OrderStatus.PendingNew;
                                break;
                            case "已报":
                                orderRecord.OrderStatus = OrderStatus.New;
                                break;
                            case "部分成交":
                                orderRecord.OrderStatus = OrderStatus.PartiallyFilled;
                                break;
                            case "已成":
                                orderRecord.OrderStatus = OrderStatus.Filled;
                                break;
                            case "已报待撤":
                                orderRecord.OrderStatus = OrderStatus.PendingCancel;
                                break;
                            case "已撤":
                                orderRecord.OrderStatus = OrderStatus.Cancelled;
                                break;
                            default:
                                orderRecord.OrderStatus = OrderStatus.Rejected;
                                break;
                        }
                        orderRecord.OrderPrice = double.Parse(data[i][7]);
                        orderRecord.OrderQty = double.Parse(data[i][8]);
                        orderRecord.InnerOrderID = data[i][9];
                        orderRecord.AvgPx = double.Parse(data[i][10]);
                        orderRecord.CumQty = double.Parse(data[i][11]);
                        orderRecord.OrderType = (OrderType)int.Parse(data[i][12]);
                        resultList.Add(orderRecord);
                    }
                    reportArgs.Result = resultList;
                }
            }
            else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = "交易账号未登录.";
            }
            return reportArgs;
        }
        private List<string[]> pickUp(StringBuilder result)
        {
            List<string[]> records = new List<string[]>();
            string text = result.ToString();
            string[] lines = text.Split('\n');
            foreach(string aline in lines)
            {
                string[] fields = aline.Split(new char[] { ' ', '\t' });
                records.Add(fields);
            }
            return records;
        }    
    }
}
