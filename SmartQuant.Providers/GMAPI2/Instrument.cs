using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    /// 交易代码信息
    [StructLayout(LayoutKind.Sequential)]
    public class Instrument
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;//交易代码
        public int sec_type;//代码品种,股票，基金，期货等
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sec_name;//代码名称       
        public double multiplier;            //合约乘数
        public double margin_ratio;          //保证金比率
        public double price_tick;            //价格最小变动单位
        public double upper_limit;           //当天涨停板
        public double lower_limit;           //当天跌停板
        public int is_active;                //当天是否交易
    }
}

