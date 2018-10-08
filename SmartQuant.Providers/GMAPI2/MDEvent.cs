using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class MDEvent
    {
        public double utc_time;
        public int event_type;
    }
}

