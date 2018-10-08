using System;
using System.Collections;

namespace SmartQuant.Trading.Conditions
{
	public abstract class Condition : RuleItem
	{
		protected Hashtable childs;

		public Hashtable Childs
		{
			get
			{
				return this.childs;
			}
		}

		public abstract string GetInitCode(string name);

		public Condition()
		{
			this.childs = new Hashtable();
		}
	}
}
