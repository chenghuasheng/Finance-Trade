using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.CrossPool
{
    public class PoolHistoryItem
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
        private DateTime leaveTime;
        public DateTime LeaveTime
        {
            get { return this.leaveTime; }
        }
        private float leavePrice;
        public float LeavePrice
        {
            get { return this.leavePrice; }
        }

        public PoolHistoryItem(DateTime enterTime, float enterPrice, DateTime leaveTime, float leavePrice)
        {
            this.enterTime = enterTime;
            this.enterPrice = enterPrice;
            this.leaveTime = leaveTime;
            this.leavePrice = leavePrice;
        }
    }
}
