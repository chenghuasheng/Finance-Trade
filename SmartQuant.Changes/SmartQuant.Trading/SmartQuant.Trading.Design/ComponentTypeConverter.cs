using System;
using System.ComponentModel;
using System.Globalization;

namespace SmartQuant.Trading.Design
{
	public class ComponentTypeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IComponentBase componentBase = value as IComponentBase;
			if (componentBase != null)
			{
				return componentBase.Name;
			}
			return "(none)";
		}
	}
}
