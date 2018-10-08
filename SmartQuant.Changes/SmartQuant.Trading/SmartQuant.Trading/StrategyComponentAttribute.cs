using System;

namespace SmartQuant.Trading
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class StrategyComponentAttribute : Attribute
	{
		private Guid guid;

		private ComponentType type;

		private string name;

		private string description;

		public Guid GUID
		{
			get
			{
				return this.guid;
			}
		}

		public ComponentType Type
		{
			get
			{
				return this.type;
			}
		}

		public string Name
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

		public string Description
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

		public StrategyComponentAttribute(string guid, ComponentType type)
		{
			this.guid = new Guid(guid);
			this.type = type;
			this.name = null;
			this.description = null;
		}
	}
}
