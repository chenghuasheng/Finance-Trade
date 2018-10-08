using SmartQuant.Instruments;
using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class IntParam
	{
		private Hashtable fParam = new Hashtable();

		public int this[Instrument instrument]
		{
			get
			{
				object obj = this.fParam[instrument];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				this.fParam[instrument] = value;
			}
		}
	}
}
