﻿using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MDErrorCallback(int error_code, [MarshalAs(UnmanagedType.LPStr)] [In] string error_msg);
}