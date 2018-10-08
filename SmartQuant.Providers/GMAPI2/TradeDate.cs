using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class TradeDate
    {
        public double utc_time; //utc时间戳
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;  //ios格式时间字符串
    }
}
