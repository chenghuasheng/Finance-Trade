using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace SmartQuant.Trading.Design
{
	internal class RequestListEditor : CollectionEditor
	{
		public RequestListEditor() : base(typeof(RequestList))
		{
		}

		protected override object CreateInstance(Type itemType)
		{
			if (itemType != typeof(string))
			{
				throw new ArgumentException("Cannot create an instance of the type - " + itemType.ToString());
			}
			string result = null;
			NewRequestForm newRequestForm = new NewRequestForm();
			if (newRequestForm.ShowDialog() == DialogResult.OK)
			{
				result = newRequestForm.Request;
			}
			newRequestForm.Dispose();
			return result;
		}

		protected override IList GetObjectsFromInstance(object instance)
		{
			if (instance == null)
			{
				return null;
			}
			return base.GetObjectsFromInstance(instance);
		}
	}
}
