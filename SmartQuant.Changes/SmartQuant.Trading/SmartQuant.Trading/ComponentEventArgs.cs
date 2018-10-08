using System;

namespace SmartQuant.Trading
{
	public class ComponentEventArgs : EventArgs
	{
		private ComponentRecord record;

		public ComponentRecord Record
		{
			get
			{
				return this.record;
			}
		}

		public ComponentEventArgs(ComponentRecord record)
		{
			this.record = record;
		}
	}
}
