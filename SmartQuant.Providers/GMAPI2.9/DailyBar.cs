using System;
using System.Runtime.InteropServices;

namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class DailyBar
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string exchange;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sec_id;

        public int bar_type;

        public double utc_time;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;

        public float open;

        public float close;

        public float high;

        public float low;

        public double volume;

        public double amount;

        public long position;

        public float settle_price;

        public float upper_limit;

        public float lower_limit;

        public float pre_close;

        public float adj_factor;

        public int flag;

        public DailyBar()
        {
        }
    }
}