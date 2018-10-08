using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.GM
{
    public class TdxHqData
    {
        private string ip;
        private short port;
        private static TdxHqData instance = null;
        private TdxHqData() { }
        public static TdxHqData Instance
        {
            get
            {
                if (TdxHqData.instance == null)
                {
                    TdxHqData.instance = new TdxHqData();
                }
                return TdxHqData.instance;
            }
        }
        public ReportArgs Connect(string ip, short port)
        {
            this.ip = ip;
            this.port = port;
            ReportArgs reportArgs = new ReportArgs();
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            if (TdxHqWrapper.TdxHq_Connect(ip, port, result, errInfo))
            {
                reportArgs.Succeeded = true;
                reportArgs.Result = result.ToString();
            }else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            return reportArgs;
        }
        public void Disconnect()
        {
            TdxHqWrapper.TdxHq_Disconnect();
        }
        public ReportArgs GetQuotes(List<string> symbols)
        {
            List<string> securityIDs = new List<string>();
            List<byte> marketIDs = new List<byte>();
            foreach(string symbol in symbols)
            {
                string[] temp = symbol.Split('.');
                string market = temp[0];
                byte marketID = 0;
                switch (market)
                {
                    case "SHSE":
                        marketID = 1;
                        break;
                    case "SZSE":
                        marketID = 0;
                        break;
                }
                marketIDs.Add(marketID);
                string securityID = temp[1];
                securityIDs.Add(securityID);
            }
            short count = (short)securityIDs.Count;
            ReportArgs reportArgs = new ReportArgs();
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            if (TdxHqWrapper.TdxHq_GetSecurityQuotes(marketIDs.ToArray(), securityIDs.ToArray(),ref count, result, errInfo))
            {
                reportArgs.Succeeded = true;
                List<GMSDK.Tick> ticks = new List<GMSDK.Tick>();
                List<string[]> data = this.pickUp(result);
                for(int i = 1; i < data.Count; i++)
                {
                    GMSDK.Tick tick = new GMSDK.Tick();
                    int marketID = int.Parse(data[i][0]);
                    switch (marketID)
                    {
                        case 0:
                            tick.exchange = "SZSE";
                            break;
                        case 1:
                            tick.exchange = "SHSE";
                            break;
                    }
                    tick.sec_id = data[i][1];
                    tick.last_price = float.Parse(data[i][3]);
                    tick.pre_close = float.Parse(data[i][4]);
                    tick.open = float.Parse(data[i][5]);
                    tick.high = float.Parse(data[i][6]);
                    tick.low = float.Parse(data[i][7]);
                    tick.utc_time = double.Parse(data[i][8]);
                    tick.cum_volume = double.Parse(data[i][10]);
                    tick.last_volume = int.Parse(data[i][11]);
                    tick.cum_amount = double.Parse(data[i][12]);
                    tick.bid_p1= float.Parse(data[i][17]);
                    tick.ask_p1= float.Parse(data[i][18]);
                    tick.bid_v1= int.Parse(data[i][19]);
                    tick.ask_v1= int.Parse(data[i][20]);
                    tick.bid_p2 = float.Parse(data[i][21]);
                    tick.ask_p2 = float.Parse(data[i][22]);
                    tick.bid_v2 = int.Parse(data[i][23]);
                    tick.ask_v2= int.Parse(data[i][24]);
                    tick.bid_p3 = float.Parse(data[i][25]);
                    tick.ask_p3 = float.Parse(data[i][26]);
                    tick.bid_v3 = int.Parse(data[i][27]);
                    tick.ask_v3 = int.Parse(data[i][28]);
                    tick.bid_p4 = float.Parse(data[i][29]);
                    tick.ask_p4 = float.Parse(data[i][30]);
                    tick.bid_v4 = int.Parse(data[i][31]);
                    tick.ask_v4 = int.Parse(data[i][32]);
                    tick.bid_p5 = float.Parse(data[i][33]);
                    tick.ask_p5 = float.Parse(data[i][34]);
                    tick.bid_v5 = int.Parse(data[i][35]);
                    tick.ask_v5 = int.Parse(data[i][36]);
                    tick.upper_limit = (float)(tick.pre_close * 1.1);
                    tick.lower_limit = (float)(tick.pre_close * 0.9);       
                    ticks.Add(tick);
                }
                reportArgs.Result = ticks;
            }else
            {
                reportArgs.Succeeded = false;
                reportArgs.ErrorInfo = errInfo.ToString();
            }
            return reportArgs; 
        }
        public ReportArgs GetLastBars(List<string> symbols,int barSize)
        {
            byte barType = 7;//默认1分线
            int min = barSize / 60;
            switch (min)
            {
                case 1:
                    barType = 7;
                    break;
                case 5:
                    barType = 0;
                    break;
                case 15:
                    barType = 1;
                    break;
                case 30:
                    barType = 2;
                    break;
                case 60:
                    barType = 3;
                    break;
                case 1440:
                    barType = 4;
                    break;
                default:
                    barType = 7;
                    break;
            }
            ReportArgs reportArgs = new ReportArgs();
            reportArgs.Succeeded = true;
            Dictionary<string, GMSDK.Bar> barDict = new Dictionary<string, GMSDK.Bar>();
            reportArgs.Result = barDict;
            StringBuilder errInfo = new StringBuilder(256);
            StringBuilder result = new StringBuilder(1024 * 1024);
            short count = 1;
            foreach (string symbol in symbols)
            {
                string[] temp = symbol.Split('.');
                string market = temp[0];
                byte marketID = 0;
                switch (market)
                {
                    case "SHSE":
                        marketID = 1;
                        break;
                    case "SZSE":
                        marketID = 0;
                        break;
                }
                string securityID = temp[1];
                if (TdxHqWrapper.TdxHq_GetSecurityBars(barType,marketID,securityID,0,ref count, result, errInfo))
                {
                    List<string[]> data = this.pickUp(result);
                    GMSDK.Bar bar = new GMSDK.Bar();
                    bar.exchange = market;
                    bar.sec_id = securityID;
                    bar.strtime = data[1][0];
                    bar.utc_endtime = DateTime.Parse(bar.strtime).ToFileTimeUtc();
                    bar.bar_type = barSize;
                    bar.open = float.Parse(data[1][1]);
                    bar.close = float.Parse(data[1][2]);
                    bar.high= float.Parse(data[1][3]);
                    bar.low= float.Parse(data[1][4]);
                    bar.volume = double.Parse(data[1][5]);
                    bar.amount = double.Parse(data[1][6]);
                    barDict.Add(symbol, bar);
                }
                else
                {
                    reportArgs.Succeeded = false;
                    reportArgs.ErrorInfo = errInfo.ToString();
                    break;
                }
            }
            return reportArgs;

        }
        private List<string[]> pickUp(StringBuilder result)
        {
            List<string[]> records = new List<string[]>();
            string text = result.ToString();
            string[] lines = text.Split('\n');
            foreach (string aline in lines)
            {
                string[] fields = aline.Split(new char[] { ' ', '\t' });
                records.Add(fields);
            }
            return records;
        }
    }
    public class ReportArgs
    {
        private bool succeeded = false;
        public bool Succeeded
        {
            get { return this.succeeded; }
            set { this.succeeded = value; }
        }
        private string errorInfo = "";
        public string ErrorInfo
        {
            get { return this.errorInfo; }
            set { this.errorInfo = value; }
        }
        private object result = null;
        public object Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        public ReportArgs() { }
    }
}
