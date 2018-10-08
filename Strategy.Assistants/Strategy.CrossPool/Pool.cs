using System;
using System.Collections.Generic;
using System.Linq;

using SmartQuant;
using SmartQuant.Instruments;

namespace Strategy.CrossPool
{
    public class Pool
    {
        private string title;
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }
        private bool autoLeave = false;//是否自动出池
        public bool AutoLeave
        {
            get { return this.autoLeave; }
            set { this.autoLeave = value; }
        }
        private TimeSpan leaveTime;//自动出池的时间
        public TimeSpan LeaveTime
        {
            get { return this.leaveTime; }
            set { this.leaveTime = value; }
        }
        private Dictionary<Instrument, PoolItem> items = new Dictionary<Instrument, PoolItem>();
        private bool saveHistory = false;
        public bool SaveHistory
        {
            get { return saveHistory; }
            set { saveHistory = value; }
        }
        private Dictionary<Instrument, PoolHistoryItem> historyItems = new Dictionary<Instrument, PoolHistoryItem>();

        public Pool()
            : this("InstPool", new TimeSpan(1, 0, 0, 0))
        {
        }
        public Pool(string title)
            : this(title, new TimeSpan(1, 0, 0, 0))
        {
        }
        public Pool(string title, TimeSpan leaveTime)
        {
            this.title = title;
            this.leaveTime = leaveTime;
        }

        public void AddInstrument(Instrument instrument)
        {
            float price = (float)instrument.Price();
            this.items.Add(instrument, new PoolItem(Clock.Now, price));
            if (this.autoLeave)
            {
                Clock.AddReminder(this.InstrumentOutPool, Clock.Now.Add(this.LeaveTime), instrument);
            }
        }
        private void InstrumentOutPool(ReminderEventArgs args)
        {
            Instrument inst = (Instrument)args.Data;
            if (this.items.ContainsKey(inst)) this.RemoveInstrument(inst);
        }
        public void RemoveInstrument(Instrument instrument)
        {
            if (this.saveHistory)
            {
                PoolItem aItem = this.items[instrument];
                float price = (float)instrument.Price();
                this.historyItems.Add(instrument, new PoolHistoryItem(aItem.EnterTime, aItem.EnterPrice, Clock.Now, price));
            }
            this.items.Remove(instrument);
        }
        public void Clear()
        {
            this.items.Clear();
        }
        public int Count
        {
            get { return this.items.Count; }
        }
        public bool ContainsInstrument(Instrument instrument)
        {
            return this.items.ContainsKey(instrument);
        }
        public List<KeyValuePair<Instrument, PoolItem>> GetItems()
        {
            return this.items.Select(o => new KeyValuePair<Instrument, PoolItem>(o.Key, o.Value)).ToList();
        }
        public void Output()
        {
            Console.WriteLine("The pool {0} has following instruments:", this.Title);
            foreach (KeyValuePair<Instrument, PoolItem> item in this.GetItems())
            {
                Console.WriteLine("Instrument:{0},EnterTime:{1},EnterPrice:{2}", item.Key, item.Value.EnterTime, item.Value.EnterPrice);
            }
            if (this.saveHistory)
            {
                Console.WriteLine("The history of this {0} is:", this.Title);
                foreach (KeyValuePair<Instrument, PoolHistoryItem> historyItem in this.historyItems)
                {
                    Console.WriteLine("Instrument:{0},EnterTime:{1},EnterPrice:{2},LeaveTime:{3},LeavePrice{4}", historyItem.Key,
                        historyItem.Value.EnterTime, historyItem.Value.EnterPrice, historyItem.Value.LeaveTime, historyItem.Value.LeavePrice);
                }
            }
        }
    }
}
