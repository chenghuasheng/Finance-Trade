using SmartQuant.FIX;
using SmartQuant.Instruments;
using System;
namespace SmartQuant.Simulation
{
	public class OrderEntry
	{
		private bool enabled;
		private DateTime datetime;
		private Instrument instrument;
		private Side side;
		private OrdType ordType;
		private double price;
		private double stopPx;
		private double orderQty;
		private string text;
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}
		public DateTime DateTime
		{
			get
			{
				return this.datetime;
			}
			set
			{
				this.datetime = value;
			}
		}
		public Instrument Instrument
		{
			get
			{
				return this.instrument;
			}
			set
			{
				this.instrument = value;
			}
		}
		public Side Side
		{
			get
			{
				return this.side;
			}
			set
			{
				this.side = value;
			}
		}
		public OrdType OrdType
		{
			get
			{
				return this.ordType;
			}
			set
			{
				this.ordType = value;
			}
		}
		public double Price
		{
			get
			{
				return this.price;
			}
			set
			{
				this.price = value;
			}
		}
		public double StopPx
		{
			get
			{
				return this.stopPx;
			}
			set
			{
				this.stopPx = value;
			}
		}
		public double OrderQty
		{
			get
			{
				return this.orderQty;
			}
			set
			{
				this.orderQty = value;
			}
		}
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}
	}
}
