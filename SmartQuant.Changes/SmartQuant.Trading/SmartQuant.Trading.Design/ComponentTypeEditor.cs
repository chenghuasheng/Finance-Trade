using System;
using System.Collections.Generic;

namespace SmartQuant.Trading.Design
{
	internal class ComponentTypeEditor : ComboBoxTypeEditor
	{
		private ComponentType componentType;

		public ComponentTypeEditor(ComponentType componentType) : base(false)
		{
			this.componentType = componentType;
		}

		protected override List<KeyValuePair<string, object>> GetItems()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (ComponentRecord componentRecord in StrategyComponentManager.GetComponentList(this.componentType))
			{
				IComponentBase component = StrategyComponentManager.GetComponent(componentRecord.GUID, this.instance);
				list.Add(new KeyValuePair<string, object>(component.Name, component));
			}
			return list;
		}
	}
}
