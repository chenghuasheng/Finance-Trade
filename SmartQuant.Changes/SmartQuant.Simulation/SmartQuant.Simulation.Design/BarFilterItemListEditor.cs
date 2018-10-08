using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	public class BarFilterItemListEditor : CollectionEditor
	{
		private CollectionEditor.CollectionForm form;
        public static event EventHandler ListChanged;
		public BarFilterItemListEditor() : base(typeof(List<BarFilterItem>))
		{
		}
		protected override CollectionEditor.CollectionForm CreateCollectionForm()
		{
			this.form = base.CreateCollectionForm();
			this.form.FormClosed += new FormClosedEventHandler(this.form_FormClosed);
			return this.form;
		}
		private void form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.form.DialogResult == DialogResult.OK && BarFilterItemListEditor.ListChanged != null)
			{
				BarFilterItemListEditor.ListChanged(this, EventArgs.Empty);
			}
		}
	}
}
