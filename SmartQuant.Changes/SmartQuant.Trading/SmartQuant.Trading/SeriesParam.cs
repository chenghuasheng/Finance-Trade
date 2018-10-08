using SmartQuant.Instruments;
using SmartQuant.Series;
using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class SeriesParam
	{
		private Hashtable fParam = new Hashtable();

		public TimeSeries this[Instrument instrument]
		{
			get
			{
				return (TimeSeries)this.fParam[instrument];
			}
			set
			{
				this.fParam[instrument] = value;
			}
		}
	}
}
