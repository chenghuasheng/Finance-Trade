using System;
using System.Collections;
namespace SmartQuant.Simulation.Design
{
	internal class OrderEntryListComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			OrderEntryViewItem orderEntryViewItem = (OrderEntryViewItem)x;
			OrderEntryViewItem orderEntryViewItem2 = (OrderEntryViewItem)y;
			return DateTime.Compare(orderEntryViewItem.Entry.DateTime, orderEntryViewItem2.Entry.DateTime);
		}
	}
}
