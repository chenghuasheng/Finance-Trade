using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
namespace SmartQuant.Simulation.Design
{
	internal class SimulatorTypeEditor : UITypeEditor
	{
		private IWindowsFormsEditorService editorService;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				this.editorService = (provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService);
				if (this.editorService != null)
				{
					Simulator simulator = value as Simulator;
					SimulatorEditorForm dialog = new SimulatorEditorForm(simulator);
					this.editorService.ShowDialog(dialog);
				}
				return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
