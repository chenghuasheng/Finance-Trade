using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class MarketIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string pub_date;

        public double pe_ratio;

        public double pb_ratio;

        public double ps_ratio;

        public double market_value;

        public double market_value_flow;

        public MarketIndex()
        {
        }
    }
}