using System;

namespace SmartQuant.Trading
{
	public class TriggerEventArgs : EventArgs
	{
		private Trigger trigger;

		public Trigger Trigger
		{
			get
			{
				return this.trigger;
			}
		}

		public TriggerEventArgs(Trigger trigger)
		{
			this.trigger = trigger;
		}
	}
}
