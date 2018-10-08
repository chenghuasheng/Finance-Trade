using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace HuaQuant.Data.Sina
{
    public class SinaQuoter
    {
        private WebClient _client;
        private string urlBase = "http://hq.sinajs.cn/";
        private static SinaQuoter instance = null;
        public static SinaQuoter Instance
        {
            get
            {
                if (SinaQuoter.instance == null)
                {
                    SinaQuoter.instance = new SinaQuoter();
                }
                return SinaQuoter.instance;
            }
        }
        private SinaQuoter()
        {
            this._client = new WebClient();
        }
        public List<GMSDK.Tick> GetQuotes(List<string> symbols)
        {
            List<GMSDK.Tick> ticks = new List<GMSDK.Tick>();
            if (symbols.Count <= 0) return ticks;
            List<string> quoteStrings = new List<string>();
           
            int k = 10;
            int m = symbols.Count / k + 1;
            for (int j = 0; j < m; j++)
            {
                int start = j * k;
                int n = symbols.Count - start;
                if (n > k) n = k;
                List <string> subSymbols = symbols.GetRange(start, n);
                string url = this.urlBase + "list=";
                for (int i = 0; i < subSymbols.Count; i++)
                {
                    string newSymbol = this.ConvertSymbol(subSymbols[i]);
                    if (i > 0) url += "," + newSymbol;
                    else url += newSymbol;
                }
                Stream str = this._client.OpenRead(url);
                StreamReader reader = new StreamReader(str);
                //Console.WriteLine(url);
                string aQuote;
                while ((aQuote = reader.ReadLine()) != null)
                {
                    quoteStrings.Add(aQuote);
                }
                str.Close();
            }
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            for (int i = 0; i < symbols.Count; i++)
            {
                string[] data = quoteStrings[i].Split(',');
                if (data.Length >= 33)
                {
                    GMSDK.Tick aTick = new GMSDK.Tick();
                    aTick.open = float.Parse(data[1]);
                    aTick.pre_close = float.Parse(data[2]);
                    aTick.last_price = float.Parse(data[3]);
                    aTick.high = float.Parse(data[4]);
                    aTick.low = float.Parse(data[5]);
                    aTick.cum_volume = double.Parse(data[8]);
                    aTick.cum_amount = double.Parse(data[9]);
                    aTick.bid_v1 = int.Parse(data[10]);
                    aTick.bid_p1 = float.Parse(data[11]);
                    aTick.bid_v2 = int.Parse(data[12]);
                    aTick.bid_p2 = float.Parse(data[13]);
                    aTick.bid_v3 = int.Parse(data[14]);
                    aTick.bid_p3 = float.Parse(data[15]);
                    aTick.bid_v4 = int.Parse(data[16]);
                    aTick.bid_p4 = float.Parse(data[17]);
                    aTick.bid_v5 = int.Parse(data[18]);
                    aTick.bid_p5 = float.Parse(data[19]);
                    aTick.ask_v1 = int.Parse(data[20]);
                    aTick.ask_p1 = float.Parse(data[21]);
                    aTick.ask_v2 = int.Parse(data[22]);
                    aTick.ask_p2 = float.Parse(data[23]);
                    aTick.ask_v3 = int.Parse(data[24]);
                    aTick.ask_p3 = float.Parse(data[25]);
                    aTick.ask_v4 = int.Parse(data[26]);
                    aTick.ask_p4 = float.Parse(data[27]);
                    aTick.ask_v5 = int.Parse(data[28]);
                    aTick.ask_p5 = float.Parse(data[29]);
                    aTick.strtime = data[30] + " " + data[31];
                    DateTime time = DateTime.Parse(aTick.strtime);
                    aTick.utc_time = (time - startTimeUTC).TotalSeconds;
                    string[] temp = symbols[i].Split('.');
                    aTick.exchange = temp[0];
                    aTick.sec_id = temp[1];
                    ticks.Add(aTick);
                }
            }
            return ticks;
        }

        private string ConvertSymbol(string symbol)
        {
            string[] temp = symbol.Split('.');
            temp[0] = temp[0].Substring(0, 2).ToLower();
            return string.Join("", temp);
        }
    }
}
