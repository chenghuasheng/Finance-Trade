using SmartQuant.Data;
using System;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class DataSeriesViewItem : ListViewItem
	{
		private IDataSeries series;
		internal IDataSeries Series
		{
			get
			{
				return this.series;
			}
		}
		internal DataSeriesViewItem(IDataSeries series) : base(series.Name, 0)
		{
			this.series = series;
		}
	}
}
