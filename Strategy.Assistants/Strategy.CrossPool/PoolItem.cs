using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.CrossPool
{
    public class PoolItem
    {
        private DateTime enterTime;
        public DateTime EnterTime
        {
            get { return this.enterTime; }
        }
        private float enterPrice;
        public float EnterPrice
        {
            get { return this.enterPrice; }
        }
        public PoolItem(DateTime enterTime, float enterPrice)
        {
            this.enterTime = enterTime;
            this.enterPrice = enterPrice;
        }
    }
}
