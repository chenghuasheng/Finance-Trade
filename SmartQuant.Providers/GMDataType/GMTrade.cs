using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    /// <summary>
    /// 分笔交易数据类
    /// </summary>
    [Serializable]
    public class GMTrade:SmartQuant.Data.Trade
    {
        private char buyOrSell;//买卖标志
        private float high;//最高价
        private float low;//最低价
        private float open;//开盘价
        private float lastClose;//昨收价

        private float upperLimit;//涨停价
        private float lowerLimit;//跌停价

        private double totalSize;//总量
        private double totalAmount;//总额
        private double amount;//此笔成交额 ,另外此笔成交价(price),此笔成交量(size)在父类中已经定义

        [View]
        public char BuyOrSell
        {
            get { return buyOrSell; }
            set { buyOrSell = value; }
        }

        [View]
        public float High
        {
            get { return high; }
            set { high = value; }
        }
        [View]
        public float Low
        {
            get { return low; }
            set { low = value; }
        }
        [View]
        public float Open
        {
            get { return open; }
            set { open = value; }
        }
        [View]
        public float LastClose
        {
            get { return lastClose; }
            set { lastClose = value; }
        }
        [View]
        public float UpperLimit
        {
            get { return upperLimit; }
            set { upperLimit = value; }
        }
        [View]
        public float LowerLimit
        {
            get { return lowerLimit; }
            set { lowerLimit = value; }
        }
        [View]
        public double TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }
        [View]
        public double TotalAmount
        {
            get { return totalAmount; }
            set { totalAmount = value; }
        }
        [View]
        public double Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public GMTrade()
        {
        }
        public GMTrade(DateTime datetime, double price, int size, char buyOrSell,
            double amount = 0, double totalSize = 0, double totalAmount = 0, float high = 0, float low = 0, float open = 0, float lastClose = 0, float upperLimit = 0,
            float lowerLimit = 0):base(datetime,price,size)
        {

            this.buyOrSell = buyOrSell;
            this.amount = amount;
            this.totalAmount = totalAmount;
            this.totalSize = totalSize;
            this.high = high;
            this.low = low;
            this.open = open;
            this.lastClose = lastClose;
            this.upperLimit = upperLimit;
            this.lowerLimit = lowerLimit;
        }
        public GMTrade(GMTrade trade):base(trade)
        {
            this.buyOrSell = trade.buyOrSell;
            this.amount = trade.amount;
            this.totalAmount = trade.totalAmount;
            this.totalSize = trade.totalSize;
            this.high = trade.high;
            this.low = trade.low;
            this.open = trade.open;
            this.lastClose = trade.lastClose;
            this.upperLimit = trade.upperLimit;
            this.lowerLimit = trade.lowerLimit;
        }
        public override object Clone()
        {
            return new GMTrade(this);
        }
        public override ISeriesObject NewInstance()
        {
            return new GMTrade();
        }
        public override void ReadFrom(BinaryReader reader)
        {
            base.ReadFrom(reader);
            buyOrSell = reader.ReadChar();
            amount = reader.ReadDouble();
            totalAmount = reader.ReadDouble();
            totalSize = reader.ReadDouble();
            high = reader.ReadSingle();
            low = reader.ReadSingle();
            open = reader.ReadSingle();
            lastClose = reader.ReadSingle();
            upperLimit = reader.ReadSingle();
            lowerLimit = reader.ReadSingle();
        }
       
        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
            writer.Write(buyOrSell);
            writer.Write(amount);
            writer.Write(totalAmount);
            writer.Write(totalSize);
            writer.Write(high);
            writer.Write(low);
            writer.Write(open);
            writer.Write(lastClose);
            writer.Write(upperLimit);
            writer.Write(lowerLimit);
        }
    }
}
