using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace SmartQuant.Simulation.Design
{
	internal class OrderEntryListEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService windowsFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				if (windowsFormsEditorService != null)
				{
					OrderEntryList orderEntryList = (OrderEntryList)value;
					OrderEntryListEditorForm orderEntryListEditorForm = new OrderEntryListEditorForm();
					orderEntryListEditorForm.Entries = orderEntryList;
					if (windowsFormsEditorService.ShowDialog(orderEntryListEditorForm) == DialogResult.OK)
					{
						orderEntryList.Clear();
						foreach (OrderEntry entry in orderEntryListEditorForm.Entries)
						{
							orderEntryList.Add(entry);
						}
						SimulationExecutionService simulationExecutionService = (SimulationExecutionService)context.Instance;
						simulationExecutionService.SaveConfiguration();
					}
				}
				return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
