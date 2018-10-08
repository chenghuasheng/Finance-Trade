using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    /// <summary>
    /// 日线数据类
    /// </summary>
    [Serializable]
    public class GMDaily:SmartQuant.Data.Daily
    {
        private float lastClose;
        private double amount;
        private float adjFactor;
        private int flag;
        /// 涨停价
        private float upperLimit;
        /// 跌停价
        private float lowerLimit;

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
        
        public float UpperLimit
        {
            get { return upperLimit; }
            set { upperLimit = value; }
        }
        public float LowerLimit
        {
            get { return lowerLimit; }
            set { lowerLimit = value; }
        }

        public GMDaily()
        {
        }
        public GMDaily(GMDaily daily):base(daily)
        {
            this.lastClose = daily.lastClose;
            this.amount = daily.amount;
            this.adjFactor = daily.adjFactor;
            this.flag = daily.flag;
            this.upperLimit = daily.upperLimit;
            this.lowerLimit = daily.lowerLimit;
        }
        public GMDaily(DateTime date, double open, double high, double low, double close, long volume,
            float lastClose = 0, double amount = 0, float adjFactor = 0, int flag = 0,float upperLimit=0,float lowerLimit=0)
            : base(date,open,high,low,close,volume)
        {
            this.lastClose = lastClose;
            this.amount = amount;
            this.adjFactor = adjFactor;
            this.flag = flag;
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }
        public GMDaily(DateTime date,double open, double high, double low, double close, long volume, long openInt=0, float lastClose = 0, double amount = 0, float adjFactor = 0, int flag = 0, float upperLimit = 0, float lowerLimit = 0)
            : base(date, open, high, low, close, volume, openInt)
        {
            this.lastClose = lastClose;
            this.amount = amount;
            this.adjFactor = adjFactor;
            this.flag = flag;
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }

        public override object Clone()
        {
            return new GMDaily(this);
        }
        public override ISeriesObject NewInstance()
        {
            return new GMDaily();
        }
        public override void ReadFrom(BinaryReader reader)
        {
            base.ReadFrom(reader);
            this.lastClose = reader.ReadSingle();
            this.amount = reader.ReadDouble();
            this.adjFactor = reader.ReadSingle();
            this.flag = reader.ReadInt32();
            this.upperLimit = reader.ReadSingle();
            this.lowerLimit = reader.ReadSingle();
        }
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(this.lastClose);
            writer.Write(this.amount);
            writer.Write(this.adjFactor);
            writer.Write(this.flag);
            writer.Write(this.upperLimit);
            writer.Write(this.lowerLimit);
        }
    }
}
