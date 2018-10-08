using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HuaQuant.Data.TDX;
namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //DLL是32位的,因此必须把C#工程生成的目标平台从Any CPU改为X86,才能调用DLL;
            //必须把Trade.dll等4个DLL复制到Debug和Release工程目录下;
            //无论用什么语言编程，都必须仔细阅读VC版内的关于DLL导出函数的功能和参数含义说明，不仔细阅读完就提出问题者因时间精力所限，恕不解答。
           
            //StringBuilder ErrInfo = new StringBuilder(256);
            //StringBuilder Result = new StringBuilder(1024 * 1024);





            //OpenTdx();//打开通达信

            ////登录
            //int ClientID = Logon("210.21.122.241", 80, "8.20", 0, "210100014580", "20615111", "328077", string.Empty, ErrInfo);

            ////登录第二个帐号
            ////int ClientID2 = Logon("111.111.111.111", 7708, "4.20", 0, "5555555555", "1111", "555", string.Empty, ErrInfo);



            //if (ClientID == -1)
            //{
            //    Console.WriteLine(ErrInfo);
            //    return;
            //}

            //SendOrder(ClientID, 0, 0, "", "601558", 2.1f, 100, Result, ErrInfo);//下单
            //SendOrder(ClientID, 0, 0, "0142996817", "000987", 2.5f, 100, Result, ErrInfo);//第二个帐号,下单
            //Console.WriteLine(ErrInfo);
            //Console.WriteLine("下单结果: {0}", Result);
            //Console.ReadLine();

            //GetQuote(ClientID, "601988", Result, ErrInfo);//查询五档报价
            // if (ErrInfo.ToString() != string.Empty)
            // {
            //     Console.WriteLine(ErrInfo.ToString());
            //     return;
            // }
            // Console.WriteLine("行情结果: {0}", Result);

            //CancelOrder(ClientID, "", "7258", Result, ErrInfo);
            //Console.WriteLine(ErrInfo);
            //Console.WriteLine("撤单结果: {0}", Result);


            //QueryData(ClientID, 0, Result, ErrInfo);//查询资金
            //if (ErrInfo.ToString() != string.Empty)
            //{
            //    Console.WriteLine(ErrInfo.ToString());
            //    Console.ReadLine();
            //    return;
            //}
            //Console.WriteLine("查询资金结果: {0}", Result);
            //QueryData(ClientID, 1, Result, ErrInfo);//查询股份
            //if (ErrInfo.ToString() != string.Empty)
            //{
            //    Console.WriteLine(ErrInfo.ToString());
            //    Console.ReadLine();
            //    return;
            //}
            //Console.WriteLine("查询股份结果: {0}", Result);
            //QueryData(ClientID, 2, Result, ErrInfo);//查询当日委托
            //if (ErrInfo.ToString() != string.Empty)
            //{
            //    Console.WriteLine(ErrInfo.ToString());
            //    Console.ReadLine();
            //    return;
            //}
            //Console.WriteLine("查询委托结果: {0}", Result);
            ///*List<string[]> records = new List<string[]>();
            //string text = Result.ToString();
            //string[] lines = text.Split('\n');
            //foreach (string aline in lines)
            //{
            //    string[] fields = aline.Split(new char[] { ' ', '\t' });
            //    records.Add(fields);
            //}
            //Console.WriteLine("code={0},marketID={1}", records[1][0], records[1][2]);
            //Console.WriteLine("code={0},marketID={1}", records[2][0], records[2][2]);
            ///*
            //            //批量查询多个证券的五档报价
            //            string[] Zqdm = new string[] { "600030", "600031" };
            //            string[] Results = new string[Zqdm.Length];
            //            string[] ErrInfos = new string[Zqdm.Length];

            //            IntPtr[] ResultPtr = new IntPtr[Zqdm.Length];
            //            IntPtr[] ErrInfoPtr = new IntPtr[Zqdm.Length];

            //            for (int i = 0; i < Zqdm.Length; i++)
            //            {
            //                ResultPtr[i] = Marshal.AllocHGlobal(1024 * 1024);
            //                ErrInfoPtr[i] = Marshal.AllocHGlobal(256);
            //            }




            //            GetQuotes(ClientID, Zqdm, Zqdm.Length, ResultPtr, ErrInfoPtr);

            //            for (int i = 0; i < Zqdm.Length; i++)
            //            {
            //                Results[i] = Marshal.PtrToStringAnsi(ResultPtr[i]);
            //                ErrInfos[i] = Marshal.PtrToStringAnsi(ErrInfoPtr[i]);

            //                Marshal.FreeHGlobal(ResultPtr[i]);
            //                Marshal.FreeHGlobal(ErrInfoPtr[i]);
            //            }*/






            //Logoff(ClientID);//注销
            //CloseTdx();//关闭通达信
            TDXTrader trader = TDXTrader.Instance;
            ReportArgs ra= trader.Logon("183.62.246.154", 7708, "8.20", 0, "210100014580", "20615111", "328077");
            if (ra.Succeeded)
            {
                Console.WriteLine("登录成功");
                ra = trader.QueryFund();
                if (ra.Succeeded)
                {
                    if (ra.Result!=null)
                        Console.WriteLine(((FundRecord)(ra.Result)).TotalAsserts);
                }else
                {
                    Console.WriteLine(ra.ErrorInfo);
                }
                ra = trader.QueryPositions();
                if (ra.Succeeded)
                {
                    if (ra.Result != null)
                    {
                        List<PositionRecord> poss = (List < PositionRecord > )ra.Result;
                        foreach(PositionRecord p in poss)
                        {
                            Console.WriteLine(p.SecurityID);
                        }
                    }
                        
                }else
                {
                    Console.WriteLine(ra.ErrorInfo);
                }
                //ra = trader.SendOrder(OrderSide.Buy, OrderType.Limit, "601558", (float)2.01, 100);
                //ra = trader.CancelOrder("78");
                //if (ra.Succeeded)
                //{
                //    if (ra.Result != null)
                //    {
                //        string orderID = ra.Result.ToString();
                //        Console.WriteLine("委托编号:{0}", orderID);
                //    }

                //}
                //else
                //{
                //    Console.WriteLine(ra.ErrorInfo);
                //}
                ra = trader.QueryOrders();
                if (ra.Succeeded)
                {
                    List<OrderRecord> orders = (List<OrderRecord>)ra.Result;
                    foreach(OrderRecord order in orders)
                    {
                        Console.WriteLine("订单ID:{0},订单状态:{1}",order.InnerOrderID, order.OrderStatus);
                    }

                }
                else
                {
                    Console.WriteLine(ra.ErrorInfo);
                }
            }
            else
            {
                Console.WriteLine(ra.ErrorInfo);
            }
            Console.ReadLine();
            //trader.Logoff();
        }





        ///基本版
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void OpenTdx();
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CloseTdx();
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern int Logon(string IP, short Port, string Version, short YybID, string AccountNo, string TradeAccount, string JyPassword, string TxPassword, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void Logoff(int ClientID);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryData(int ClientID, int Category, StringBuilder Result, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrder(int ClientID, int Category, int PriceType, string Gddm, string Zqdm, float Price, int Quantity, StringBuilder Result, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrder(int ClientID, string ExchangeID, string hth, StringBuilder Result, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuote(int ClientID, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void Repay(int ClientID, string Amount, StringBuilder Result, StringBuilder ErrInfo);




        ///普通批量版
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryHistoryData(int ClientID, int Category, string StartDate, string EndDate, StringBuilder Result, StringBuilder ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryDatas(int ClientID, int[] Category, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrders(int ClientID, int[] Category, int[] PriceType, string[] Gddm, string[] Zqdm, float[] Price, int[] Quantity, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrders(int ClientID, string[] ExchangeID, string[] hth, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuotes(int ClientID, string[] Zqdm, int Count, IntPtr[] Result, IntPtr[] ErrInfo);







        ///高级批量版
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryMultiAccountsDatas(int[] ClientID, int[] Category, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void SendMultiAccountsOrders(int[] ClientID, int[] Category, int[] PriceType, string[] Gddm, string[] Zqdm, float[] Price, int[] Quantity, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelMultiAccountsOrders(int[] ClientID, string[] ExchangeID, string[] hth, int Count, IntPtr[] Result, IntPtr[] ErrInfo);
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void GetMultiAccountsQuotes(int[] ClientID, string[] Zqdm, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

    }
}
