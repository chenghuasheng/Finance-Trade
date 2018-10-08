using System;
namespace SmartQuant.Simulation
{
	public class ExceptionEventArgs : EventArgs
	{
		private Exception exception;
		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}
		public ExceptionEventArgs(Exception exception)
		{
			this.exception = exception;
		}
	}
}
