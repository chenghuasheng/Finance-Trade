using System;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	public class ComponentBase : IComponentBase
	{
		protected string name;

		protected string description;

		[Category("Description"), Description("Component name"), ReadOnly(true)]
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[Category("Description"), Description("Component description"), ReadOnly(true)]
		public virtual string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public virtual void Init()
		{
		}

		public virtual void Connect()
		{
		}

		public virtual void Disconnect()
		{
		}

		public virtual void OnStrategyStop()
		{
		}
	}
}
