using SmartQuant.Instruments;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading.Design
{
	internal class PortfolioTypeEditor : ComboBoxTypeEditor
	{
		public PortfolioTypeEditor() : base(true)
		{
		}

		protected override List<KeyValuePair<string, object>> GetItems()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (Portfolio portfolio in PortfolioManager.Portfolios)
			{
				list.Add(new KeyValuePair<string, object>(portfolio.Name, portfolio));
			}
			return list;
		}
	}
}
