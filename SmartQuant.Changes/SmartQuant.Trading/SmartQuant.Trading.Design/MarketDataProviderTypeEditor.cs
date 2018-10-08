using SmartQuant.Providers;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading.Design
{
	public class MarketDataProviderTypeEditor : ComboBoxTypeEditor
	{
		public MarketDataProviderTypeEditor() : base(true)
		{
		}

		protected override List<KeyValuePair<string, object>> GetItems()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (IProvider provider in ProviderManager.MarketDataProviders)
			{
				list.Add(new KeyValuePair<string, object>(provider.Name, provider));
			}
			return list;
		}
	}
}
