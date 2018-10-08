using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    /// <summary>
    /// 自定义数据类
    /// </summary>
    [Serializable]
    public class GMData:IDataObject, ISeriesObject, ICloneable
    {
        private const byte VERSION = 2;
        protected DateTime datetime;
        public DateTime DateTime
        {
	         get { return this.datetime;}
	         set { this.datetime=value;}
        }
        protected byte providerId;
        public byte ProviderId
        {
	         get { return this.providerId; }
	         set { this.providerId=value;}
        }
        
        protected float dataValue=0;
        [View]
        public float DataValue{
            get {return this.dataValue;}
            set {this.dataValue=value;}
        }

        public GMData()
        {
        }
        public GMData(DateTime datetime, float dataValue)
        {
            this.datetime = datetime;
            this.dataValue = dataValue;
        }
        public GMData(GMData data)
        {
            this.datetime = data.datetime;
            this.dataValue = data.dataValue;
        }
        public object Clone()
        {
            return new GMData(this);
        }
        public ISeriesObject NewInstance()
        {
            return new GMData();
        }

        public void ReadFrom(System.IO.BinaryReader reader)
        {
            byte b = reader.ReadByte();
            this.datetime = new DateTime(reader.ReadInt64());
            this.dataValue = reader.ReadSingle();
            this.providerId = reader.ReadByte();
        }

        public void WriteTo(System.IO.BinaryWriter writer)
        {
            writer.Write(VERSION);
            writer.Write(this.datetime.Ticks);
            writer.Write(this.dataValue);
            writer.Write(this.providerId);
        }   
    }
}
