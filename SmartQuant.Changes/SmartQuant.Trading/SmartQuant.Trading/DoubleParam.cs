using SmartQuant.Instruments;
using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class DoubleParam
	{
		private Hashtable fParam = new Hashtable();

		public double this[Instrument instrument]
		{
			get
			{
				object obj = this.fParam[instrument];
				if (obj != null)
				{
					return (double)obj;
				}
				return 0.0;
			}
			set
			{
				this.fParam[instrument] = value;
			}
		}
	}
}
