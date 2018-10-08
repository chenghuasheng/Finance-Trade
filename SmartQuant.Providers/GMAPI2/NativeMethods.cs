using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    internal class NativeMethods
    {
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gm_version();
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gm_strerror(int errorno);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_login([MarshalAs(UnmanagedType.LPStr)] [In] string username, [MarshalAs(UnmanagedType.LPStr)] [In] string password, [MarshalAs(UnmanagedType.LPStr)] [In] string addr);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_run();
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_poll();
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_logout();
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_init_ex(int mode, [MarshalAs(UnmanagedType.LPStr)] [In] string subscribe_symbol_list, [MarshalAs(UnmanagedType.LPStr)] [In] string start_time, [MarshalAs(UnmanagedType.LPStr)] [In] string end_time, [MarshalAs(UnmanagedType.LPStr)] [In] string rest_addr, [MarshalAs(UnmanagedType.LPStr)] [In] string live_addr);
        [DllImport("libgm.dll")]
        public static extern int gm_md_reconnect();
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_subscribe([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_unsubscribe([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_md_set_login_callback(MDLoginCallback cb);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_md_set_event_callback(MDEventCallback cb);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_md_set_tick_callback(MDTickCallback cb);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_md_set_bar_callback(MDBarCallback cb);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gm_md_set_error_callback(MDErrorCallback cb);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_ticks([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr tick, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_ticks([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, ref IntPtr tick, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_ticks_by_time([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int n, [MarshalAs(UnmanagedType.LPStr)] [In] string end_time, ref IntPtr tick, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_bars([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int bar_type, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr bar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_bars([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, int bar_type, ref IntPtr bar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_bars_by_time([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int bar_type, int n, [MarshalAs(UnmanagedType.LPStr)] [In] string end_time, ref IntPtr bar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_dailybars([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr dbar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_dailybars([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, ref IntPtr dbar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_dailybars_by_time([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int n, [MarshalAs(UnmanagedType.LPStr)] [In] string end_time, ref IntPtr dbar, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_calendar([MarshalAs(UnmanagedType.LPStr)] [In] string exchange, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr res, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        
        public static extern int gm_md_get_instruments([MarshalAs(UnmanagedType.LPStr)] [In] string exchange, int sec_type, int is_active, ref IntPtr instrument, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_instruments_by_name([MarshalAs(UnmanagedType.LPStr)] [In] string name, ref IntPtr instrument, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_constituents([MarshalAs(UnmanagedType.LPStr)] [In] string index_symbol, ref IntPtr constituent, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_financial_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr financial_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_financial_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, ref IntPtr financial_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_financial_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int n, ref IntPtr financial_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_share_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr share_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_share_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, ref IntPtr share_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_share_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int n, ref IntPtr share_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_market_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, [MarshalAs(UnmanagedType.LPStr)] [In] string t_begin, [MarshalAs(UnmanagedType.LPStr)] [In] string t_end, ref IntPtr market_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_market_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol_list, ref IntPtr market_index, ref int count);
        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gm_md_get_last_n_market_index([MarshalAs(UnmanagedType.LPStr)] [In] string symbol, int n, ref IntPtr market_index, ref int count);
    }
}
