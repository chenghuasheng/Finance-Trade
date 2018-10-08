using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace GMSDK
{
    public class MdApi
    {
        private static MdApi instance;

        private MDLoginCallback login_cb;

        private MDTickCallback tick_cb;

        private MDBarCallback bar_cb;

        private MDEventCallback event_cb;

        private MDErrorCallback error_cb;

        public static MdApi Instance
        {
            get
            {
                if (MdApi.instance == null)
                {
                    MdApi.instance = new MdApi();
                }
                return MdApi.instance;
            }
        }

        private MdApi()
        {
            this.login_cb = new MDLoginCallback(this.OnLogin);
            this.tick_cb = new MDTickCallback(this.OnTick);
            this.bar_cb = new MDBarCallback(this.OnBar);
            this.event_cb = new MDEventCallback(this.OnMdEvent);
            this.error_cb = new MDErrorCallback(this.OnError);
            NativeMethods.gm_md_set_login_callback(this.login_cb);
            NativeMethods.gm_md_set_tick_callback(this.tick_cb);
            NativeMethods.gm_md_set_bar_callback(this.bar_cb);
            NativeMethods.gm_md_set_event_callback(this.event_cb);
            NativeMethods.gm_md_set_error_callback(this.error_cb);
        }

        public void Close()
        {
            NativeMethods.gm_logout();
        }

        public List<Bar> GetBars(string symbol_list, int bar_type, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_bars(symbol_list, bar_type, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<Bar>(intPtr, num);
        }

        public List<TradeDate> GetCalendar(string exchange, string start_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_calendar(exchange, start_time, end_time, ref intPtr, ref num);
            return Util.convert<TradeDate>(intPtr, num);
        }

        public List<Constituent> GetConstituents(string index_symbol)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_constituents(index_symbol, ref intPtr, ref num);
            return Util.convert<Constituent>(intPtr, num);
        }

        public List<DailyBar> GetDailyBars(string symbol_list, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_dailybars(symbol_list, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<DailyBar>(intPtr, num);
        }

        public List<StockDivident> GetDivident(string symbol, string start_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_divident(symbol, start_time, end_time, ref intPtr, ref num);
            return Util.convert<StockDivident>(intPtr, num);
        }

        public List<FinancialIndex> GetFinancialIndex(string symbol_list, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_financial_index(symbol_list, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<FinancialIndex>(intPtr, num);
        }

        public List<Instrument> GetInstruments(string exchange, int sec_type, int is_active = 1)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_instruments(exchange, sec_type, is_active, ref intPtr, ref num);
            return Util.convert<Instrument>(intPtr, num);
        }

        public List<Instrument> GetInstrumentsByName(string name)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_instruments_by_name(name, ref intPtr, ref num);
            return Util.convert<Instrument>(intPtr, num);
        }

        public List<Bar> GetLastBars(string symbol_list, int bar_type)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_bars(symbol_list, bar_type, ref intPtr, ref num);
            return Util.convert<Bar>(intPtr, num);
        }

        public List<DailyBar> GetLastDailyBars(string symbol_list)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_dailybars(symbol_list, ref intPtr, ref num);
            return Util.convert<DailyBar>(intPtr, num);
        }

        public List<FinancialIndex> GetLastFinancialIndex(string symbol_list)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_financial_index(symbol_list, ref intPtr, ref num);
            return Util.convert<FinancialIndex>(intPtr, num);
        }

        public List<MarketIndex> GetLastMarketIndex(string symbol_list)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_market_index(symbol_list, ref intPtr, ref num);
            return Util.convert<MarketIndex>(intPtr, num);
        }

        public List<Bar> GetLastNBars(string symbol, int bar_type, int n = 1, string end_time = "")
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_bars_by_time(symbol, bar_type, n, end_time, ref intPtr, ref num);
            return Util.convert<Bar>(intPtr, num);
        }

        public List<DailyBar> GetLastNDailyBars(string symbol, int n = 1, string end_time = "")
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_dailybars_by_time(symbol, n, end_time, ref intPtr, ref num);
            return Util.convert<DailyBar>(intPtr, num);
        }

        public List<FinancialIndex> GetLastNFinancialIndex(string symbol, int n = 1)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_financial_index(symbol, n, ref intPtr, ref num);
            return Util.convert<FinancialIndex>(intPtr, num);
        }

        public List<MarketIndex> GetLastNMarketIndex(string symbol, int n = 1)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_market_index(symbol, n, ref intPtr, ref num);
            return Util.convert<MarketIndex>(intPtr, num);
        }

        public List<ShareIndex> GetLastNShareIndex(string symbol, int n = 1)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_share_index(symbol, n, ref intPtr, ref num);
            return Util.convert<ShareIndex>(intPtr, num);
        }

        public List<Tick> GetLastNTicks(string symbol, int n = 1, string end_time = "")
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_n_ticks_by_time(symbol, n, end_time, ref intPtr, ref num);
            return Util.convert<Tick>(intPtr, num);
        }

        public List<ShareIndex> GetLastShareIndex(string symbol_list)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_share_index(symbol_list, ref intPtr, ref num);
            return Util.convert<ShareIndex>(intPtr, num);
        }

        public List<Tick> GetLastTicks(string symbol_list)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_last_ticks(symbol_list, ref intPtr, ref num);
            return Util.convert<Tick>(intPtr, num);
        }

        public List<MarketIndex> GetMarketIndex(string symbol_list, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_market_index(symbol_list, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<MarketIndex>(intPtr, num);
        }

        public List<ShareIndex> GetShareIndex(string symbol_list, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_share_index(symbol_list, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<ShareIndex>(intPtr, num);
        }

        public List<StockAdjustFactor> GetStockAdj(string symbol, string start_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_stock_adj(symbol, start_time, end_time, ref intPtr, ref num);
            return Util.convert<StockAdjustFactor>(intPtr, num);
        }

        public List<Tick> GetTicks(string symbol_list, string begin_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_ticks(symbol_list, begin_time, end_time, ref intPtr, ref num);
            return Util.convert<Tick>(intPtr, num);
        }

        public int GetTimeoutVal()
        {
            return NativeMethods.gm_get_timeout_val();
        }

        public List<VirtualContract> GetVirtualContract(string vsymbol, string start_time, string end_time)
        {
            int num = 0;
            IntPtr intPtr = new IntPtr();
            NativeMethods.gm_md_get_virtual_contract(vsymbol, start_time, end_time, ref intPtr, ref num);
            return Util.convert<VirtualContract>(intPtr, num);
        }

        public int Init(string username, string password, MDMode mode = MDMode.MD_MODE_NULL, string subscribe_symbols = "", string start_time = "", string end_time = "")
        {
            return this.InitEx(username, password, mode, subscribe_symbols, start_time, end_time, "");
        }

        public int InitEx(string username, string password, MDMode mode = MDMode.MD_MODE_NULL, string subscribe_symbols = "", string start_time = "", string end_time = "", string gm_addr = "")
        {
            NativeMethods.gm_set_language("C#");
            int num = NativeMethods.gm_login(username, password, gm_addr);
            if (num != 0)
            {
                return num;
            }
            return NativeMethods.gm_md_init_ex((int)mode, subscribe_symbols, start_time, end_time, "", "");
        }

        private void OnBar(Bar bar)
        {
            if (this.BarEvent != null)
            {
                this.BarEvent(bar);
            }
        }

        private void OnError(int error_code, string error_msg)
        {
            if (this.ErrorEvent != null)
            {
                this.ErrorEvent(error_code, error_msg);
            }
        }

        private void OnLogin()
        {
            if (this.LoginEvent != null)
            {
                this.LoginEvent();
            }
        }

        private void OnMdEvent(MDEvent md_event)
        {
            if (this.MdEvent != null)
            {
                this.MdEvent(md_event);
            }
        }

        private void OnTick(Tick tick)
        {
            if (this.TickEvent != null)
            {
                this.TickEvent(tick);
            }
        }

        public int Reconnect()
        {
            return NativeMethods.gm_md_reconnect();
        }

        public int Resubscribe(string symbol_list)
        {
            return NativeMethods.gm_md_resubscribe(symbol_list);
        }

        public int Run()
        {
            return NativeMethods.gm_run();
        }

        public void SetTimeoutVal(int seconds)
        {
            NativeMethods.gm_set_timeout_val(seconds);
        }

        public string StrError(int errorno)
        {
            return Marshal.PtrToStringAnsi(NativeMethods.gm_strerror(errorno));
        }

        public int Subscribe(string symbol_list)
        {
            return NativeMethods.gm_md_subscribe(symbol_list);
        }

        public int Unsubscribe(string symbol_list)
        {
            return NativeMethods.gm_md_unsubscribe(symbol_list);
        }

        public string Version()
        {
            return Marshal.PtrToStringAnsi(NativeMethods.gm_version());
        }

        public event MDBarCallback BarEvent;

        public event MDErrorCallback ErrorEvent;

        public event MDLoginCallback LoginEvent;

        public event MDEventCallback MdEvent;

        public event MDTickCallback TickEvent;
    }
}