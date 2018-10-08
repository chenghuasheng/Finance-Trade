using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    [StructLayout(LayoutKind.Sequential)]
    public class Tick
    {
        /// 交易所
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string exchange;
        /// 合约ID
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sec_id;
        ///  UTC时间戳
        public double utc_time;
        ///  交易时间
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string strtime;
        /// 最新价
        public float last_price;
        /// 开盘价
        public float open;
        /// 最高价
        public float high;
        /// 最低价
        public float low;
        /// 1-5档买价
        public float bid_p1;
        public float bid_p2;
        public float bid_p3;
        public float bid_p4;
        public float bid_p5;
        /// 1-5档买量
        public int bid_v1;
        public int bid_v2;
        public int bid_v3;
        public int bid_v4;
        public int bid_v5;
        /// 1-5档卖价
        public float ask_p1;
        public float ask_p2;
        public float ask_p3;
        public float ask_p4;
        public float ask_p5;
        /// 1-5档卖量
        public int ask_v1;
        public int ask_v2;
        public int ask_v3;
        public int ask_v4;
        public int ask_v5;
        ///  成交总量/最新成交量,累计值
        public double cum_volume;
        ///  成交总金额/最新成交额,累计值
        public double cum_amount;
        ///  合约持仓量(期),累计值
        public System.Int64 cum_position;
        ///  瞬时成交额
        public double last_amount;
        ///  瞬时成交量
        public int last_volume;
        /// 涨停价
        public float upper_limit;
        /// 跌停价
        public float lower_limit;
        /// 今日结算价
        public float settle_price;
        /// (保留)交易类型,对应多开,多平等类型
        public int trade_type;
        //  昨收价
        public float pre_close;
    }
}