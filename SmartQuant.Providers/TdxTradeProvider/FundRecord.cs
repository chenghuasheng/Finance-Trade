using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    public class FundRecord
    {
        /// <summary>
        /// totalAsserts=balance+marketValue
        /// balance=available+frozen
        /// desirable是可以取出的资金，而当天卖出持仓换得的资金是得第二天才能取出的
        /// </summary>
        private double balance;//资金余额
        private double available;//可用资金
        private double frozen;//冻结资金
        private double desirable;//可取资金
        private double marketValue;//市值
        private double totalAsserts;//总资产
        

        public double Balance
        {
            get { return balance; }
            set { this.balance = value; }
        }
        public double Available
        {
            get { return available; }
            set { this.available = value; }
        }
        public double Frozen
        {
            get { return frozen; }
            set { this.frozen = value; }
        }
        public double Desirable
        {
            get { return this.desirable; }
            set { this.desirable = value; }
        }
        public double TotalAsserts
        {
            get { return totalAsserts; }
            set { this.totalAsserts = value; }
        }
        public double MarketValue
        {
            get { return marketValue; }
            set { this.marketValue = value; }
        }
        
        public FundRecord(){ }  
    }
}
