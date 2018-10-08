using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GMSDK
{
    internal class Util
    {
        public Util()
        {
        }

        public static List<T> convert<T>(IntPtr obj, int count)
        {
            List<T> ts = new List<T>();
            for (int i = 0; i < count; i++)
            {
                ts.Add((T)Marshal.PtrToStructure(obj, typeof(T)));
                obj = (IntPtr)((int)obj+ Marshal.SizeOf(typeof(T)));
            }
            return ts;
        }

        public static T convert<T>(IntPtr obj)
        {
            if (obj == IntPtr.Zero)
            {
                return default(T);
            }
            return (T)Marshal.PtrToStructure(obj, typeof(T));
        }
    }
}