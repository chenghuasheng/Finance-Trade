using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class ShareIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;                         //股票代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]                       
        public string pub_date;                       //公告日期
        public double total_share;                    //总股本
        public double flow_a_share;                   //流通A股
        public double nonflow_a_share;                //限售流通A股

    }
}
