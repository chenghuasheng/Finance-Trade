using System;

namespace SmartQuant.Trading.Design
{
	internal class ComboBoxItem
	{
		private object value;

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public ComboBoxItem(object value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			if (this.value != null)
			{
				return this.value.ToString();
			}
			return "(none)";
		}
	}
}
