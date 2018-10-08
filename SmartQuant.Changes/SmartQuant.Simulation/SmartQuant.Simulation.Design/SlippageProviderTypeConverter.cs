using System;
using System.ComponentModel;
using System.Globalization;
namespace SmartQuant.Simulation.Design
{
	internal class SlippageProviderTypeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value.ToString();
		}
	}
}
