using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MDTickCallback(Tick tick);
}

