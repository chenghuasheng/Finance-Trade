using System;
using System.Runtime.InteropServices;

namespace GMSDK
{
    internal class NativeMethods
    {
        public NativeMethods()
        {
        }

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_get_timeout_val();

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_login([In] string username, [In] string password, [In] string addr);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_logout();

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_bars([In] string symbol, int bar_type, [In] string t_begin, [In] string t_end, ref IntPtr bar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_calendar([In] string exchange, [In] string t_begin, [In] string t_end, ref IntPtr res, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_constituents([In] string index_symbol, ref IntPtr constituent, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_dailybars([In] string symbol, [In] string t_begin, [In] string t_end, ref IntPtr dbar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_divident([In] string syboml, [In] string t_begin, [In] string t_end, ref IntPtr res, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_financial_index([In] string symbol, [In] string t_begin, [In] string t_end, ref IntPtr financial_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_instruments([In] string exchange, int sec_type, int is_active, ref IntPtr instrument, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_instruments_by_name([In] string name, ref IntPtr instrument, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_bars([In] string symbol_list, int bar_type, ref IntPtr bar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_dailybars([In] string symbol_list, ref IntPtr dbar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_financial_index([In] string symbol_list, ref IntPtr financial_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_market_index([In] string symbol_list, ref IntPtr market_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_bars_by_time([In] string symbol, int bar_type, int n, [In] string end_time, ref IntPtr bar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_dailybars_by_time([In] string symbol, int n, [In] string end_time, ref IntPtr dbar, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_financial_index([In] string symbol, int n, ref IntPtr financial_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_market_index([In] string symbol, int n, ref IntPtr market_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_share_index([In] string symbol, int n, ref IntPtr share_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_n_ticks_by_time([In] string symbol, int n, [In] string end_time, ref IntPtr tick, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_share_index([In] string symbol_list, ref IntPtr share_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_last_ticks([In] string symbol_list, ref IntPtr tick, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_market_index([In] string symbol, [In] string t_begin, [In] string t_end, ref IntPtr market_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_share_index([In] string symbol, [In] string t_begin, [In] string t_end, ref IntPtr share_index, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_stock_adj([In] string syboml, [In] string t_begin, [In] string t_end, ref IntPtr res, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_ticks([In] string symbol, [In] string t_begin, [In] string t_end, ref IntPtr tick, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_get_virtual_contract([In] string syboml, [In] string t_begin, [In] string t_end, ref IntPtr res, ref int count);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_init_ex(int mode, [In] string subscribe_symbol_list, [In] string start_time, [In] string end_time, [In] string rest_addr, [In] string live_addr);

        [DllImport("libgm.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_reconnect();

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_resubscribe([In] string symbol_list);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_md_set_bar_callback(MDBarCallback cb);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_md_set_error_callback(MDErrorCallback cb);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_md_set_event_callback(MDEventCallback cb);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_md_set_login_callback(MDLoginCallback cb);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_md_set_tick_callback(MDTickCallback cb);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_subscribe([In] string symbol_list);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_md_unsubscribe([In] string symbol_list);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int gm_run();

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_set_language([In] string language);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern void gm_set_timeout_val(int sec);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern IntPtr gm_strerror(int errorno);

        [DllImport("libgm.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.None, ExactSpelling = false)]
        public static extern IntPtr gm_version();

     
    }
}