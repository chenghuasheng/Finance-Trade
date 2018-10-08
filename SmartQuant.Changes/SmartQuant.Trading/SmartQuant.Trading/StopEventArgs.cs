using System;

namespace SmartQuant.Trading
{
	public class StopEventArgs : EventArgs
	{
		private IStop stop;

		public IStop Stop
		{
			get
			{
				return this.stop;
			}
		}

		public StopEventArgs(IStop stop)
		{
			this.stop = stop;
		}
	}
}
