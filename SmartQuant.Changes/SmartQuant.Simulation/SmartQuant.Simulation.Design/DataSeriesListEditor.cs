using SmartQuant.Data;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class DataSeriesListEditor : CollectionEditor
	{
		public DataSeriesListEditor() : base(typeof(DataSeriesList))
		{
		}
		protected override object CreateInstance(Type itemType)
		{
			if (itemType == typeof(IDataSeries))
			{
				IDataSeries[] result = null;
				DataSeriesForm dataSeriesForm = new DataSeriesForm();
				if (dataSeriesForm.ShowDialog() == DialogResult.OK)
				{
					result = dataSeriesForm.Series;
				}
				dataSeriesForm.Dispose();
				return result;
			}
			throw new ArgumentException("Cannot create an instance of the type - " + itemType.ToString());
		}
		protected override IList GetObjectsFromInstance(object instance)
		{
			if (instance == null)
			{
				return null;
			}
			return new ArrayList(instance as ICollection);
		}
	}
}
