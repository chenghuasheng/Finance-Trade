using System;

namespace SmartQuant.Trading
{
	public class ComponentTypeEventArgs : EventArgs
	{
		private ComponentType componentType;

		public ComponentType ComponentType
		{
			get
			{
				return this.componentType;
			}
		}

		public ComponentTypeEventArgs(ComponentType componentType)
		{
			this.componentType = componentType;
		}
	}
}
