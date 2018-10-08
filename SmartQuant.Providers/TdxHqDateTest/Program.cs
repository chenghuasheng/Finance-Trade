using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaQuant.Data.GM;

namespace TdxHqDateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TdxHqData th = TdxHqData.Instance;
            ReportArgs rags =th.Connect("121.14.110.200", 443);
            if (rags.Succeeded)
            {
                Console.WriteLine("succeeded.");
                List<string> symbols = new List<string>();
                symbols.Add("SZSE.000001");
                symbols.Add("SZSE.000070");
                symbols.Add("SHSE.603078");
                symbols.Add("SHSE.600000");
                rags = th.GetQuotes(symbols);
                if (rags.Succeeded)
                {
                    List<GMSDK.Tick> ticks = (List<GMSDK.Tick>) rags.Result;
                    DateTime startTimeUTC = DateTime.Now.Date;
                   
                    foreach (GMSDK.Tick tick in ticks)
                    {
                        string symbol = tick.exchange + '.' + tick.sec_id;
                        double t = tick.utc_time;

                        DateTime time = new DateTime(2017, 4, 28).AddSeconds(t/100);

                        Console.WriteLine("symbol:{0},price:{1},volume:{2},time:{3},tt:{4}", 
                            symbol, tick.last_price, 
                            tick.last_volume,time , t);
                    }
                }else
                {
                    Console.WriteLine("wrong:" + rags.ErrorInfo);
                }
            }else
            {
                Console.WriteLine("wrong:" + rags.ErrorInfo);
            }
            Console.ReadLine();

        }
    }
}
