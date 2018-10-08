using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class Bar
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string exchange;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sec_id;

        public int bar_type;

        public double utc_time;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;

        public double utc_endtime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strendtime;

        public float open;

        public float close;

        public float high;

        public float low;

        public double volume;

        public double amount;

        public float pre_close;

        public long position;

        public float adj_factor;

        public int flag;

        public Bar()
        {
        }
    }
}