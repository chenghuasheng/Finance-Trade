using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    /// 市场指标
    [StructLayout(LayoutKind.Sequential)]
    public class MarketIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;   //股票代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string pub_date; //公告日期  
        public double pe_ratio;            //市盈率
        public double pb_ratio;            //市净率
        public double ps_ratio;            //市销率
        public double market_value;        //市值
        public double market_value_flow;   //流通市值

    }
}

