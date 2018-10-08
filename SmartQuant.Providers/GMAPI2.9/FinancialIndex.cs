using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class FinancialIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string pub_date;

        public double eps;

        public double bvps;

        public double cfps;

        public double afps;

        public double total_asset;

        public double current_asset;

        public double fixed_asset;

        public double liability;

        public double current_liability;

        public double longterm_liability;

        public double equity;

        public double income;

        public double operating_profit;

        public double net_profit;

        public FinancialIndex()
        {
        }
    }
}