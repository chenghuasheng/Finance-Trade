using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{

    public enum OrderType
    {
        Limit,//0上海限价委托 深圳限价委托
        BestPriceForOtherOnSZSE,//1深圳对方最优价格
        BestPriceForSelfOnSZSE,//2深圳本方最优价格
        CancelAfterInstantTransactionOnSZSE,//3深圳即时成交剩余撤销
        CancelAfterFiveQuoteTransaction,//4上海五档即成剩撤 深圳五档即成剩撤
        AllTransactionOrCancelOnSZSE,//5深圳全额成交或撤销
        ToLimitAfterFiveQuoteTransactionOnSHSE//6上海五档即成转限价
    }
    public enum OrderSide
    {
        Buy,//买
        Sell,//卖
        MarginBuy,//融资买入
        MarginSell,//融券卖出
        BuyCouponRepayCoupon,//买券还券
        SellCouponRepayment,//卖券还款
        CouponRepayCoupon//现券还券
    }
    public enum OrderStatus
    {
        PendingNew,//待报
        New,//已报
        PartiallyFilled,//部分成交
        Filled,//已成
        PendingCancel,//已报待撤
        Cancelled,//已撤
        Rejected//拒绝
    }
    public enum DataType
    {
        Fund, //资金
        Share,//股份
        Order,//委托
        Transaction,//成交
        CanCancelOrder,//可撤单
        ShareholderCode,//股东代码
        FinancingBalance,//融资余额
        ShortInterest,//融券余额
        NegotiableSecurity//可融证券
    }

    public enum HistoryDataType
    {
        Order,//委托
        Transaction,//成交
        DeliveryNote//交割单
    }
}
