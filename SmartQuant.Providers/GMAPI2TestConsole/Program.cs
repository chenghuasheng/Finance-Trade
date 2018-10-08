using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMSDK;
using System.Threading;

namespace GMAPI2TestConsole
{
    class Program
    {
        //private static Thread _runThread;
        //private static MdApi _md;
        //static void on_tick(Tick tick)
        //{
        //    System.Console.Write(string.Format("tick:  {0}  {1}.{2} p={3} v={4} {5}", tick.strtime, tick.exchange, tick.sec_id, tick.last_price, tick.last_volume, tick.trade_type.ToString()));
        //    Console.Write(string.Format("bp1={0},bv1={1},bp2={2},bv2={3},bp3={4},bv3={5},bp4={6},bv4={7},bp5={8},bv5={9}", tick.bid_p1,
        //        tick.bid_v1, tick.bid_p2, tick.bid_v2, tick.bid_p3, tick.bid_v3, tick.bid_p4, tick.bid_v4, tick.bid_p5, tick.bid_v5));
        //    Console.WriteLine(string.Format("ap1={0},av1={1},ap2={2},av2={3},ap3={4},av3={5},ap4={6},av4={7},ap5={8},av5={9}", tick.ask_p1,
        //        tick.ask_v1, tick.ask_p2, tick.ask_v2, tick.ask_p3, tick.ask_v3, tick.ask_p4, tick.ask_v4, tick.ask_p5, tick.ask_v5));
        //}
        //static void on_bar(Bar bar)
        //{
        //    System.Console.WriteLine(string.Format("bar: {0}  {1}.{2} {3}", bar.strtime, bar.exchange, bar.sec_id, bar.close));
        //}
        //static void on_login()
        //{
        //    System.Console.WriteLine("Logined.!");
        //}
        //static void on_error(int errorcode, string errormsg)
        //{
        //    System.Console.WriteLine(string.Format("error:code {0},{1}", errorcode, errormsg));
        //}
        //static void on_md(MDEvent e)
        //{
        //    System.Console.WriteLine(string.Format("time :{0},type:{1}", e.utc_time, e.event_type));
        //}
        //static void doRun()
        //{
        //    _md.Run();
        //}
        static void Main(string[] args)
        {
            //_md = MdApi.Instance;

            //// 实时行情
            //int ret = _md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_LIVE, "", "2017-04-05 09:30:00", "2017-04-05 09:31:00");
            //if (ret != 0)
            //{
            //    //登录失败
            //    return;
            //}
            //_md.TickEvent += Program.on_tick;
            //_md.BarEvent += Program.on_bar;
            //_md.LoginEvent += Program.on_login;
            //_md.ErrorEvent += Program.on_error;
            //_md.MdEvent += Program.on_md;
            //_md.Subscribe("SHSE.600004.bar.60");
            //_md.Subscribe("SHSE.600006.tick");
            //_runThread = new Thread(new ThreadStart(Program.doRun));
            //if (_runThread.ThreadState == ThreadState.Unstarted)
            //{
            //    _runThread.Start();
            //}

            //Console.ReadLine();
            ////_md.Unsubscribe("SHSE.600004.tick");

            //Console.ReadLine();
            //_md.Close();
            //_runThread.Abort();
            //Console.Write("here");

            HuaQuant.Data.Sina.SinaQuoter sq = HuaQuant.Data.Sina.SinaQuoter.Instance;
            List<string> symbols = new List<string>();
            symbols.Add("SHSE.600000");
            symbols.Add("SZSE.000001");
            symbols.Add("SZSE.000002");
            symbols.Add("SZSE.000003");
            symbols.Add("SZSE.000004");
            symbols.Add("SZSE.000005");
            symbols.Add("SZSE.000006");
            symbols.Add("SZSE.000007");
            symbols.Add("SZSE.000008");
            symbols.Add("SZSE.000009");
            symbols.Add("SZSE.000010");
            symbols.Add("SZSE.000011");
            List<GMSDK.Tick>  ticks=sq.GetQuotes(symbols);
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (GMSDK.Tick tick in ticks)
            {
                Console.WriteLine("symbol:{0},time:{1},price:{2}", tick.exchange+"."+tick.sec_id, startTimeUTC.AddSeconds(tick.utc_time), tick.last_price);
            }
            Console.ReadLine();

        }
    }
}
