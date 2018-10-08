using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace SmartQuant.Trading.Design
{
	public abstract class ComboBoxTypeEditor : ObjectSelectorEditor
	{
		private bool allowNull;

		protected object instance;

		public override bool IsDropDownResizable
		{
			get
			{
				return true;
			}
		}

		protected ComboBoxTypeEditor(bool allowNull)
		{
			this.allowNull = allowNull;
		}

		protected abstract List<KeyValuePair<string, object>> GetItems();

		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
		{
			if (context != null && context.Instance != null)
			{
				this.instance = context.Instance;
				selector.Clear();
				if (this.allowNull)
				{
					selector.AddNode("(none)", null, null);
				}
				foreach (KeyValuePair<string, object> current in this.GetItems())
				{
					selector.AddNode(current.Key, current.Value, null);
				}
				object value = context.PropertyDescriptor.GetValue(context.Instance);
				if (value == null && this.allowNull)
				{
					selector.SelectedNode = selector.Nodes[0];
					return;
				}
				foreach (ObjectSelectorEditor.SelectorNode selectorNode in selector.Nodes)
				{
					if (selectorNode.value != null && selectorNode.value.Equals(value))
					{
						selector.SelectedNode = selectorNode;
						break;
					}
				}
			}
		}
	}
}
