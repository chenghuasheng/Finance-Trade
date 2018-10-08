using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class StockDivident
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string div_date;

        public double cash_div;

        public double share_div_ratio;

        public double share_trans_ratio;

        public double allotment_ratio;

        public double allotment_price;

        public StockDivident()
        {
        }
    }
}