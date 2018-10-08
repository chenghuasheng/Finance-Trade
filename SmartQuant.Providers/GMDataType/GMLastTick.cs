using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    [Serializable]
    public class GMLastTick : IDataObject, ISeriesObject, ICloneable
    {
        private const byte VERSION = 2;
        protected DateTime datetime;
        public DateTime DateTime
        {
            get { return this.datetime; }
            set { this.datetime = value; }
        }
        protected byte providerId;
        public byte ProviderId
        {
            get { return this.providerId; }
            set { this.providerId = value; }
        }

        protected float price;
        protected int size;
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

        [PriceView]
        public float Price
        {
            get { return price; }
            set { price = value; }
        }
        [View]
        public int Size
        {
            get { return size; }
            set { size = value; }
        }
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
        protected float ask;
        protected int askSize;
        protected float bid;
        protected int bidSize;
        //五档买价,第一档在基类中
        private float bid1;
        private float bid2;
        private float bid3;
        private float bid4;

        //五档买量,第一档在基类中
        private int bid1Size;
        private int bid2Size;
        private int bid3Size;
        private int bid4Size;

        //五档卖价,第一档在基类中
        private float ask1;
        private float ask2;
        private float ask3;
        private float ask4;

        //五档卖量,第一档在基类中
        private int ask1Size;
        private int ask2Size;
        private int ask3Size;
        private int ask4Size;
        [PriceView]
        public float Ask
        {
            get { return ask; }
            set { ask = value; }
        }
        [View]
        public int AskSize
        {
            get { return askSize; }
            set { askSize = value; }
        }
        [PriceView]
        public float Bid
        {
            get { return bid; }
            set { bid = value; }
        }
        [View]
        public int BidSize
        {
            get { return bidSize; }
            set { bidSize = value; }
        }
        [View]
        public float Bid1
        {
            get { return bid1; }
            set { bid1 = value; }
        }
        [View]
        public float Bid2
        {
            get { return bid2; }
            set { bid2 = value; }
        }
        [View]
        public float Bid3
        {
            get { return bid3; }
            set { bid3 = value; }
        }
        [View]
        public float Bid4
        {
            get { return bid4; }
            set { bid4 = value; }
        }


        [View]
        public int Bid1Size
        {
            get { return bid1Size; }
            set { bid1Size = value; }
        }
        [View]
        public int Bid2Size
        {
            get { return bid2Size; }
            set { bid2Size = value; }
        }
        [View]
        public int Bid3Size
        {
            get { return bid3Size; }
            set { bid3Size = value; }
        }
        [View]
        public int Bid4Size
        {
            get { return bid4Size; }
            set { bid4Size = value; }
        }

        [View]
        public float Ask1
        {
            get { return ask1; }
            set { ask1 = value; }
        }
        [View]
        public float Ask2
        {
            get { return ask2; }
            set { ask2 = value; }
        }
        [View]
        public float Ask3
        {
            get { return ask3; }
            set { ask3 = value; }
        }
        [View]
        public float Ask4
        {
            get { return ask4; }
            set { ask4 = value; }
        }

        [View]
        public int Ask1Size
        {
            get { return ask1Size; }
            set { ask1Size = value; }
        }
        [View]
        public int Ask2Size
        {
            get { return ask2Size; }
            set { ask2Size = value; }
        }
        [View]
        public int Ask3Size
        {
            get { return ask3Size; }
            set { ask3Size = value; }
        }
        [View]
        public int Ask4Size
        {
            get { return ask4Size; }
            set { ask4Size = value; }
        }
        public GMLastTick()
        {
        }
        public GMLastTick(DateTime datetime, float price, int size, char buyOrSell,
            double amount = 0, double totalSize = 0, double totalAmount = 0, float high = 0, float low = 0, float open = 0, float lastClose = 0, float upperLimit = 0,
            float lowerLimit = 0, float bid =0, int bidSize=0, float ask =0, int askSize=0, float bid1 = 0, float bid2 = 0, float bid3 = 0, float bid4 = 0,
            int bid1Size = 0, int bid2Size = 0, int bid3Size = 0, int bid4Size = 0, float ask1 = 0, float ask2 = 0,
            float ask3 = 0, float ask4 = 0, int ask1Size = 0, int ask2Size = 0, int ask3Size = 0, int ask4Size = 0)
        {
            this.datetime = datetime;
            this.providerId = 0;
            this.price = price;
            this.size = size;
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
            this.bid = bid;
            this.bidSize = bidSize;
            this.ask = ask;
            this.askSize = askSize;
            this.bid1 = bid1;
            this.bid2 = bid2;
            this.bid3 = bid3;
            this.bid4 = bid4;
            this.bid1Size = bid1Size;
            this.bid2Size = bid2Size;
            this.bid3Size = bid3Size;
            this.bid4Size = bid4Size;
            this.ask1 = ask1;
            this.ask2 = ask2;
            this.ask3 = ask3;
            this.ask4 = ask4;
            this.ask1Size = ask1Size;
            this.ask2Size = ask2Size;
            this.ask3Size = ask3Size;
            this.ask4Size = ask4Size;
        }
        public GMLastTick(GMLastTick gmLastTick)
        {
            this.datetime = gmLastTick.datetime;
            this.providerId = gmLastTick.providerId;
            this.price = gmLastTick.price;
            this.size = gmLastTick.size;
            this.buyOrSell = gmLastTick.buyOrSell;
            this.amount = gmLastTick.amount;
            this.totalAmount = gmLastTick.totalAmount;
            this.totalSize = gmLastTick.totalSize;
            this.high = gmLastTick.high;
            this.low = gmLastTick.low;
            this.open = gmLastTick.open;
            this.lastClose = gmLastTick.lastClose;
            this.upperLimit = gmLastTick.upperLimit;
            this.lowerLimit = gmLastTick.lowerLimit;
            this.bid = gmLastTick.bid;
            this.bidSize = gmLastTick.bidSize;
            this.ask = gmLastTick.ask;
            this.askSize = gmLastTick.askSize;
            this.bid1 = gmLastTick.bid1;
            this.bid2 = gmLastTick.bid2;
            this.bid3 = gmLastTick.bid3;
            this.bid4 = gmLastTick.bid4;
            this.bid1Size = gmLastTick.bid1Size;
            this.bid2Size = gmLastTick.bid2Size;
            this.bid3Size = gmLastTick.bid3Size;
            this.bid4Size = gmLastTick.bid4Size;
            this.ask1 = gmLastTick.ask1;
            this.ask2 = gmLastTick.ask2;
            this.ask3 = gmLastTick.ask3;
            this.ask4 = gmLastTick.ask4;
            this.ask1Size = gmLastTick.ask1Size;
            this.ask2Size = gmLastTick.ask2Size;
            this.ask3Size = gmLastTick.ask3Size;
            this.ask4Size = gmLastTick.ask4Size;
        }
        public object Clone()
        {
            return new GMLastTick(this);
        }

        public ISeriesObject NewInstance()
        {
            return new GMLastTick();
        }

        public void ReadFrom(BinaryReader reader)
        {
            byte b = reader.ReadByte();
            this.datetime = new DateTime(reader.ReadInt64());
            this.providerId = reader.ReadByte();
            price = reader.ReadSingle();
            size = reader.ReadInt32();
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
            bid = reader.ReadSingle();
            bidSize = reader.ReadInt32();
            ask = reader.ReadSingle();
            askSize = reader.ReadInt32();
            bid1 = reader.ReadSingle();
            bid2 = reader.ReadSingle();
            bid3 = reader.ReadSingle();
            bid4 = reader.ReadSingle();
            bid1Size = reader.ReadInt32();
            bid2Size = reader.ReadInt32();
            bid3Size = reader.ReadInt32();
            bid4Size = reader.ReadInt32();
            ask1 = reader.ReadSingle();
            ask2 = reader.ReadSingle();
            ask3 = reader.ReadSingle();
            ask4 = reader.ReadSingle();
            ask1Size = reader.ReadInt32();
            ask2Size = reader.ReadInt32();
            ask3Size = reader.ReadInt32();
            ask4Size = reader.ReadInt32();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(VERSION);
            writer.Write(this.datetime.Ticks);
            writer.Write(this.providerId);
            writer.Write(price);
            writer.Write(size);
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
            writer.Write(bid);
            writer.Write(bidSize);
            writer.Write(ask);
            writer.Write(askSize);
            writer.Write(bid1);
            writer.Write(bid2);
            writer.Write(bid3);
            writer.Write(bid4);
            writer.Write(bid1Size);
            writer.Write(bid2Size);
            writer.Write(bid3Size);
            writer.Write(bid4Size);
            writer.Write(ask1);
            writer.Write(ask2);
            writer.Write(ask3);
            writer.Write(ask4);
            writer.Write(ask1Size);
            writer.Write(ask2Size);
            writer.Write(ask3Size);
            writer.Write(ask4Size);
        }
    }
}
