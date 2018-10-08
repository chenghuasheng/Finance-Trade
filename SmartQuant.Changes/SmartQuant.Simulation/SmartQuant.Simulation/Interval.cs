using System;
namespace SmartQuant.Simulation
{
	public class Interval
	{
		private DateTime begin;
		private DateTime end;
		public DateTime Begin
		{
			get
			{
				return this.begin;
			}
			set
			{
				this.begin = value;
			}
		}
		public DateTime End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end = value;
			}
		}
		public Interval(DateTime begin, DateTime end)
		{
			if (end < begin)
			{
				throw new ArgumentException("The end date/time less than begin");
			}
			this.begin = begin;
			this.end = end;
		}
		public Interval() : this(new DateTime(1900, 1, 1), new DateTime(2100, 1, 1))
		{
		}
		public override string ToString()
		{
			return "Interval";
		}
	}
}
