using SmartQuant.Data;
using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	public class ExitEventArgs : EventArgs
	{
		private Instrument instrument;

		private char side;

		private Bar bar;

		public Instrument Instrument
		{
			get
			{
				return this.instrument;
			}
		}

		public char Side
		{
			get
			{
				return this.side;
			}
		}

		public Bar Bar
		{
			get
			{
				return this.bar;
			}
		}

		public ExitEventArgs(Instrument instrument, char side, Bar bar)
		{
			this.instrument = instrument;
			this.side = side;
			this.bar = bar;
		}
	}
}
