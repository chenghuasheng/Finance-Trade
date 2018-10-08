using SmartQuant.Data;
using System;
namespace SmartQuant.Simulation
{
	internal class BarObject : IDataObject
	{
		internal Bar Bar;
		private DateTime datetime;
		public byte ProviderId
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
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
		internal BarObject(Bar bar)
		{
			this.Bar = (bar.Clone() as Bar);
		}
	}
}
