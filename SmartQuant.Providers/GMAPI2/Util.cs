using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace GMSDK
{
    internal class Util
    {
        public static List<T> convert<T>(IntPtr obj, int count)
        {
            List<T> list = new List<T>();
            
            for (int i = 0; i < count; i++)
            {
                list.Add((T)((object)Marshal.PtrToStructure(obj, typeof(T))));
                obj= (IntPtr)(obj.ToInt64()+Marshal.SizeOf(typeof(T)));//此处把指针转换为64位整数,以适应64位系统
            }
            return list;
        }
        public static T convert<T>(IntPtr obj)
        {
            if (obj != IntPtr.Zero)
            {
                return (T)((object)Marshal.PtrToStructure(obj, typeof(T)));
            }
            return default(T);
        }
    }
}
