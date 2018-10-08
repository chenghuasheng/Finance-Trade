using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    /// <summary>
    /// 周期数据类，定义各种Bar
    /// </summary>
    [Serializable]
    public class GMBar:SmartQuant.Data.Bar
    {
        private float lastClose;
        private double amount;
        private float adjFactor;
        private int flag;

        [PriceView]
        public float LastClose
        {
            get { return lastClose; }
            set { lastClose = value; }
        }
        [View]
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        
        public float AdjFactor
        {
            get { return adjFactor; }
            set { adjFactor = value; }
        }
        
        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        public GMBar()
        {
        }
        public GMBar(GMBar bar):base(bar)
        {
            this.lastClose = bar.lastClose;
            this.amount = bar.amount;
            this.adjFactor = bar.adjFactor;
            this.flag = bar.flag;
        }
        public GMBar(DateTime datetime, double open, double high, double low, double close, long volume, long size,
            float lastClose = 0, double amount = 0, float adjFactor = 0, int flag = 0):base(datetime,open,high,low,close,volume,size)
        {
            this.lastClose = lastClose;
            this.amount = amount;
            this.adjFactor = adjFactor;
            this.flag = flag;
        }
        public GMBar(BarType barType, long size, DateTime beginTime, DateTime endTime, double open, double high, double low, double close, long volume, long openInt=0, float lastClose = 0, double amount = 0, float adjFactor = 0, int flag = 0)
            : base(barType, size, beginTime, endTime, open, high, low, close, volume, openInt)
        {
            this.lastClose = lastClose;
            this.amount = amount;
            this.adjFactor = adjFactor;
            this.flag = flag;
        }

        public override object Clone()
        {
            return new GMBar(this);
        }
        public override ISeriesObject NewInstance()
        {
            return new GMBar();
        }
        public override void ReadFrom(BinaryReader reader)
        {
            base.ReadFrom(reader);
            this.lastClose = reader.ReadSingle();
            this.amount = reader.ReadDouble();
            this.adjFactor = reader.ReadSingle();
            this.flag = reader.ReadInt32();
        }
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.lastClose);
            writer.Write(this.amount);
            writer.Write(this.adjFactor);
            writer.Write(this.flag);
        }

    }
}
