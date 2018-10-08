using SmartQuant.Instruments;
using System;
namespace SmartQuant.Simulation.Design
{
	internal class InstrumentItem
	{
		private Instrument instrument;
		public Instrument Instrument
		{
			get
			{
				return this.instrument;
			}
		}
		public InstrumentItem(Instrument instrument)
		{
			this.instrument = instrument;
		}
		public override string ToString()
		{
			return this.instrument.Symbol;
		}
		public override int GetHashCode()
		{
			return this.instrument.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return this.instrument.Equals(obj);
		}
	}
}
