using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class Tick
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string exchange;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sec_id;

        public double utc_time;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;

        public float last_price;

        public float open;

        public float high;

        public float low;

        public float bid_p1;

        public float bid_p2;

        public float bid_p3;

        public float bid_p4;

        public float bid_p5;

        public int bid_v1;

        public int bid_v2;

        public int bid_v3;

        public int bid_v4;

        public int bid_v5;

        public float ask_p1;

        public float ask_p2;

        public float ask_p3;

        public float ask_p4;

        public float ask_p5;

        public int ask_v1;

        public int ask_v2;

        public int ask_v3;

        public int ask_v4;

        public int ask_v5;

        public double cum_volume;

        public double cum_amount;

        public long cum_position;

        public double last_amount;

        public int last_volume;

        public float upper_limit;

        public float lower_limit;

        public float settle_price;

        public int trade_type;

        public float pre_close;

        public Tick()
        {
        }
    }
}