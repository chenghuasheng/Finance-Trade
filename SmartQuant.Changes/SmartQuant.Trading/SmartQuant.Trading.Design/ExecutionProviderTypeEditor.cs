using SmartQuant.Providers;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading.Design
{
	public class ExecutionProviderTypeEditor : ComboBoxTypeEditor
	{
		public ExecutionProviderTypeEditor() : base(true)
		{
		}

		protected override List<KeyValuePair<string, object>> GetItems()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (IProvider provider in ProviderManager.ExecutionProviders)
			{
				list.Add(new KeyValuePair<string, object>(provider.Name, provider));
			}
			return list;
		}
	}
}
