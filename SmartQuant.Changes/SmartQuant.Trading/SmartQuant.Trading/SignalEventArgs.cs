using System;

namespace SmartQuant.Trading
{
	public class SignalEventArgs : EventArgs
	{
		private Signal signal;

		public Signal Signal
		{
			get
			{
				return this.signal;
			}
		}

		public SignalEventArgs(Signal signal)
		{
			this.signal = signal;
		}
	}
}
