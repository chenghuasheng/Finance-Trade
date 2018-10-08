using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    class OrderRecordList:List<OrderRecord>
    {
        private Dictionary<string, OrderRecord> innerDict = new Dictionary<string, OrderRecord>();
        private Dictionary<string, OrderRecord> outerDice = new Dictionary<string, OrderRecord>();
        public OrderRecordList() { }
        public void AddRecord(OrderRecord orderRocord,string innerOrderID=null, string outerOrderID=null)
        {
            this.Add(orderRocord);
            if (innerOrderID != null) this.innerDict.Add(innerOrderID, orderRocord);
            if (outerOrderID != null) this.outerDice.Add(outerOrderID, orderRocord);
        }
        public OrderRecord GetRecord(string orderID,int innerOrOuter)
        {
            OrderRecord orderRecord = null;
            switch (innerOrOuter)
            {
                case 0:
                    this.innerDict.TryGetValue(orderID, out orderRecord);
                    break;
                case 1:
                    this.outerDice.TryGetValue(orderID, out orderRecord);
                    break;
                default:
                    throw new Exception("invaild orderID type.");
            }
            return orderRecord;
        }
        public OrderRecord SearchRecord(OrderRecord orderRecord)
        {
            foreach(OrderRecord odr in this)
            {
                if (odr.SecurityID == orderRecord.SecurityID &&
                    odr.OrderQty == orderRecord.OrderQty &&
                    odr.OrderPrice == orderRecord.OrderPrice &&
                    odr.OrderSide==orderRecord.OrderSide&&
                    odr.InnerOrderID == "")
                {
                    if (orderRecord.TransactTime > odr.TransactTime)
                    {
                        odr.InnerOrderID = orderRecord.InnerOrderID;
                        this.innerDict.Add(orderRecord.InnerOrderID, odr);
                        return odr;
                    }
                }
            }
            return null;
        }
    }
}
