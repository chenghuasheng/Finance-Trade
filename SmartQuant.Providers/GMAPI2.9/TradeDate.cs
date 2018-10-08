using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class TradeDate
    {
        public double utc_time;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;

        public TradeDate()
        {
        }
    }
}