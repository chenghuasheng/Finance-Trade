using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    public class OrderRecord
    {
        private DateTime transactTime;
        public DateTime TransactTime //委托时间
        {
            get { return this.transactTime; }
            set { this.transactTime = value; }
        }
        private string securityID;
        public string SecurityID //证券代码
        {
            get { return this.securityID; }
            set { this.securityID = value; }
        }
        private OrderSide orderSide;
        public OrderSide OrderSide //买卖方向
        {
            get { return this.orderSide; }
            set { this.orderSide = value; }
        }
        private OrderStatus orderStatus;
        public OrderStatus OrderStatus //委托状态
        {
            get { return this.orderStatus; }
            set { this.orderStatus = value; }
        }
        private double orderPrice;
        public double OrderPrice //委托价格
        {
            get { return this.orderPrice; }
            set { this.orderPrice = value; }
        }
        private double orderQty;
        public double OrderQty //委托数量
        {
            get { return this.orderQty; }
            set { this.orderQty = value; }
        }
        private string innerOrderID;
        public string InnerOrderID //内部委托编号
        {
            get { return this.innerOrderID; }
            set { this.innerOrderID = value; }
        }
        private string outerOrderID;
        public string OuterOrderID
        {
            get { return this.outerOrderID; }
            set { this.outerOrderID = value; }
        }
        private double avgPx;
        public double AvgPx //成交价格
        {
            get { return this.avgPx; }
            set { this.avgPx = value; }
        }
        private double cumQty;
        public double CumQty //成交数量
        {
            get { return this.cumQty; }
            set { this.cumQty = value; }
        }
        private OrderType orderType;
        public OrderType OrderType //委托方式
        {
            get { return this.orderType; }
            set { this.orderType = value; }
        }

        public OrderRecord() { }

    }
}
