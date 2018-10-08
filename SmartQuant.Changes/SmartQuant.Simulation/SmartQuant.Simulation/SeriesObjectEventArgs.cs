using SmartQuant.Data;
using System;
namespace SmartQuant.Simulation
{
	public class SeriesObjectEventArgs : EventArgs
	{
		private IDataSeries series;
		private IDataObject obj;
		public IDataObject Object
		{
			get
			{
				return this.obj;
			}
		}
		public IDataSeries Series
		{
			get
			{
				return this.series;
			}
		}
		public SeriesObjectEventArgs(IDataSeries series, IDataObject obj)
		{
			this.series = series;
			this.obj = obj;
		}
	}
}
