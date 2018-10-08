using SmartQuant.Instruments;
using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class BoolParam
	{
		private Hashtable fParam = new Hashtable();

		public bool this[Instrument instrument]
		{
			get
			{
				object obj = this.fParam[instrument];
				return obj != null && (bool)obj;
			}
			set
			{
				this.fParam[instrument] = value;
			}
		}
	}
}
