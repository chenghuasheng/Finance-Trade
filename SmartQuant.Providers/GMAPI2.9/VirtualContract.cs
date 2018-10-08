using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class VirtualContract
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string vsymbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string trade_date;

        public VirtualContract()
        {
        }
    }
}