using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    /// <summary>
    /// 报价数据类
    /// </summary>
    [Serializable]
    public class GMQuote:SmartQuant.Data.Quote
    {
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
        
        public GMQuote()
        {
        }
        public GMQuote(GMQuote quote)
            : base(quote)
        {
            this.bid1 = quote.bid1;
            this.bid2 = quote.bid2;
            this.bid3 = quote.bid3;
            this.bid4 = quote.bid4;            
            this.bid1Size = quote.bid1Size;
            this.bid2Size = quote.bid2Size;
            this.bid3Size = quote.bid3Size;
            this.bid4Size = quote.bid4Size;            
            this.ask1 = quote.ask1;
            this.ask2 = quote.ask2;
            this.ask3 = quote.ask3;
            this.ask4 = quote.ask4;            
            this.ask1Size = quote.ask1Size;
            this.ask2Size = quote.ask2Size;
            this.ask3Size = quote.ask3Size;
            this.ask4Size = quote.ask4Size;
            
        }
        public GMQuote(DateTime datetime, double bid, int bidSize, double ask, int askSize, float bid1 = 0, float bid2 = 0, float bid3 = 0, float bid4 = 0,
            int bid1Size = 0, int bid2Size = 0, int bid3Size = 0, int bid4Size = 0, float ask1 = 0, float ask2 = 0,
            float ask3 = 0, float ask4 = 0, int ask1Size = 0, int ask2Size = 0, int ask3Size = 0, int ask4Size = 0)
            : base(datetime, bid, bidSize, ask, askSize)
        {
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

        public override object Clone()
        {
            return new GMQuote(this);
        }
        public override ISeriesObject NewInstance()
        {
            return new GMQuote();
        }
        public override void ReadFrom(BinaryReader reader)
        {
            base.ReadFrom(reader);
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

        public override void WriteTo(BinaryWriter writer)
        {
            base.WriteTo(writer);
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
