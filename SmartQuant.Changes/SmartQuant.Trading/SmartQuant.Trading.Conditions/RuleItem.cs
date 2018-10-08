using System;

namespace SmartQuant.Trading.Conditions
{
	public abstract class RuleItem
	{
		public virtual string Name
		{
			get
			{
				return "";
			}
		}

		public abstract string CodeName
		{
			get;
		}

		public RuleItem()
		{
		}

		public abstract void Execute();
	}
}
