using SmartQuant.Services;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SmartQuant.Trading.Design
{
	internal class ServiceTypeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IService service = value as IService;
			if (service != null)
			{
				return service.Name;
			}
			return "(none)";
		}
	}
}
