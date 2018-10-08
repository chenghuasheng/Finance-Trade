using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class ShareIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string pub_date;

        public double total_share;

        public double flow_a_share;

        public double nonflow_a_share;

        public ShareIndex()
        {
        }
    }
}