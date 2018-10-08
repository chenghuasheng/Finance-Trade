using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartQuant;
using SmartQuant.Instruments;

namespace Strategy.CrossPool
{
    class TransferCondition
    {
        private int conditionType = 1;//1为条件类型，2为排序类型
        public int ConditionType
        {
            get { return this.conditionType; }
            set { this.conditionType = value; }
        }
        private Pool inPool = null;
        public Pool InPool
        {
            get { return this.inPool; }
            set { this.inPool = value; }
        }
        private Pool outPool = null;
        public Pool OutPool
        {
            get { return this.outPool; }
            set { this.outPool = value; }
        }
        private int rankLimit = 10;//排序后取多少名次
        public int RankLimit
        {
            get { return this.rankLimit; }
            set { this.rankLimit = value; }
        }
        private bool sortByAsc = true;//升序或降序
        public bool SortByAsc
        {
            get { return this.sortByAsc; }
            set { this.sortByAsc = true; }
        }
        private bool deleteFromInPool = false;//是否从入池中删除
        public bool DeleteFromInPool
        {
            get { return this.deleteFromInPool; }
            set { this.deleteFromInPool = value; }
        }
        private bool clearOutPoolFrist = false;//是否首先清除出池
        public bool ClearOutPoolFrist
        {
            get { return this.clearOutPoolFrist; }
            set { this.clearOutPoolFrist = true; }
        }
        private DateTime beginTime = Clock.Now;//默认立即开始
        public DateTime BeginTime
        {
            get { return this.beginTime; }
            set { this.beginTime = value; }
        }
        private bool justOnce = false;//只执行一次
        public bool JustOnce
        {
            get { return this.justOnce; }
            set { this.justOnce = value; }
        }
        private TimeSpan keepTime = new TimeSpan(1, 0, 0, 0);//默认持续一天
        public TimeSpan KeepTime
        {
            get { return this.keepTime; }
            set { this.keepTime = value; }   
        }
        private TimeSpan interval = new TimeSpan(0, 1, 0);//默认间隔1分钟
        public TimeSpan Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }
        
        public TransferCondition()
        {
        }
        public virtual float ConditionCode(KeyValuePair<Instrument, PoolItem> item) {
            return 0;
        }

        public void Run()
        {
            if (this.beginTime < Clock.Now) this.beginTime = Clock.Now;
            Clock.AddReminder(this.runOnce, this.beginTime, null);
        }
        protected virtual void runOnce(ReminderEventArgs args)
        {
            if (this.inPool != null && this.outPool != null)
            {
                if (this.ClearOutPoolFrist) this.outPool.Clear();
                if (this.inPool.Count > 0)
                {
                    Dictionary<Instrument, float> sortDict = new Dictionary<Instrument, float>();

                    foreach (KeyValuePair<Instrument, PoolItem> item in this.inPool.GetItems())
                    {
                        if (this.outPool.ContainsInstrument(item.Key)) continue;//输出池中已有
                        float ret = this.ConditionCode(item);
                        if (this.conditionType == 1)//条件模式
                        {
                            if (ret > 0)
                            {
                                this.outPool.AddInstrument(item.Key);
                                if (this.DeleteFromInPool) this.inPool.RemoveInstrument(item.Key);
                            }
                        }
                        else if (this.conditionType == 2)//排序模式
                        {
                            sortDict.Add(item.Key, ret);
                        }
                    }
                    if (this.conditionType == 2)//排序模式
                    {
                        List<Instrument> sortList;
                        if (this.sortByAsc)
                        {
                            sortList = (sortDict.OrderBy(o => o.Value)).Select(o => o.Key).ToList();
                        }
                        else
                        {
                            sortList = (sortDict.OrderByDescending(o => o.Value)).Select(o => o.Key).ToList();
                        }

                        for (int i = 0; i < this.RankLimit && i < sortList.Count; i++)
                        {
                            Instrument item = sortList[i];
                            this.outPool.AddInstrument(item);
                            if (this.DeleteFromInPool) this.inPool.RemoveInstrument(item);
                        }
                    }
                }
                if (!this.justOnce)
                {
                    DateTime nextTime = Clock.Now.Add(this.interval);
                    if (nextTime <= this.beginTime.Add(this.keepTime))
                    {
                        Clock.AddReminder(this.runOnce, nextTime, null);
                    }
                }
            }
        }
    }
}
