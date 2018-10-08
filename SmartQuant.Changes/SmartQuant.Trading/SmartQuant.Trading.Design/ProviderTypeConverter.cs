using SmartQuant.Providers;
using System;
using System.ComponentModel;
using System.Globalization;

namespace SmartQuant.Trading.Design
{
	public class ProviderTypeConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IProvider provider = value as IProvider;
			if (provider != null)
			{
				return provider.Name;
			}
			return "(none)";
		}
	}
}
