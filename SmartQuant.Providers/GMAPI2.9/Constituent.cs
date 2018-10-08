using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class Constituent
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;

        public double weight;

        public Constituent()
        {
        }
    }
}