using System;
namespace SmartQuant.Simulation
{
	public class IntervalEventArgs : EventArgs
	{
		private Interval interval;
		public Interval Interval
		{
			get
			{
				return this.interval;
			}
		}
		public IntervalEventArgs(Interval interval)
		{
			this.interval = interval;
		}
	}
}
