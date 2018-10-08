using System;

namespace SmartQuant.Trading
{
	public class MetaStrategyErrorEventArgs : EventArgs
	{
		private Exception exception;

		private bool ignore;

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public bool Ignore
		{
			get
			{
				return this.ignore;
			}
			set
			{
				this.ignore = value;
			}
		}

		public MetaStrategyErrorEventArgs(Exception exception)
		{
			this.exception = exception;
			this.ignore = false;
		}
	}
}
