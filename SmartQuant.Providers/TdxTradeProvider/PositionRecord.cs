using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    public class PositionRecord
    {
        private string securityExchange;
        private string securityID;
        private double quantity;
        private double available;
        private double costPrice;
        public string SecurityExchange
        {
            get { return securityExchange; }
            set { this.securityExchange = value; }
        }
        public string SecurityID
        {
            get { return securityID; }
            set { this.securityID = value; }
        }
        public double Available
        {
            get { return available; }
            set { this.available = value; }
        }
        public double  Quantity
        {
            get { return quantity; }
            set { this.quantity = value; }
        }
        public double CostPrice
        {
            get { return this.costPrice; }
            set { this.costPrice = value; }
        }
        public PositionRecord(){}
    }
}
