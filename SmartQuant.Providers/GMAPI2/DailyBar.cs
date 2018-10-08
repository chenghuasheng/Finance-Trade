using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class DailyBar
    {
        /// 交易所
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string exchange;
        /// 合约ID
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sec_id;
        /// bar类型，以秒为单位，比如1分钟bar, bar_type=60
        public int bar_type;
        /// utc时间戳
        public double utc_time;
        /// 交易时间
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;
        /// 开盘价
        public float open;
        /// 收盘价
        public float close;
        /// 最高价
        public float high;
        /// 最低价
        public float low;
        /// 成交量
        public double volume;
        /// 成交额
        public double amount;
        /// 持仓量
        public long position;
        /// 今日结算价
        public float settle_price;
        /// 涨停价
        public float upper_limit;
        /// 跌停价
        public float lower_limit;
        /// 昨收
        public float pre_close;
        /// 复权因子
        public float adj_factor;
        /// 复权/停牌标记
        public int flag;
    }
}