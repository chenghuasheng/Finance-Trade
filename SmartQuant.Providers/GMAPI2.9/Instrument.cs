using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class Instrument
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;

        public int sec_type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string sec_name;

        public double multiplier;

        public double margin_ratio;

        public double price_tick;

        public double upper_limit;

        public double lower_limit;

        public int is_active;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string update_time;

        public Instrument()
        {
        }
    }
}