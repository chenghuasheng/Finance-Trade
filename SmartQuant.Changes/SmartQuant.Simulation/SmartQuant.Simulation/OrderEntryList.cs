using SmartQuant.Simulation.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace SmartQuant.Simulation
{
	[Editor(typeof(OrderEntryListEditor), typeof(UITypeEditor))]
	public class OrderEntryList : ICollection, IEnumerable
	{
		private SortedList<DateTime, List<OrderEntry>> entries;
		private List<OrderEntry> list;
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}
		public object SyncRoot
		{
			get
			{
				return null;
			}
		}
		public OrderEntry[]this[DateTime datetime]
		{
			get
			{
				List<OrderEntry> list;
				if (this.entries.TryGetValue(datetime, out list))
				{
					return list.ToArray();
				}
				return new OrderEntry[0];
			}
		}
		internal OrderEntryList()
		{
			this.entries = new SortedList<DateTime, List<OrderEntry>>();
			this.list = new List<OrderEntry>();
		}
		public void CopyTo(Array array, int index)
		{
			this.list.ToArray().CopyTo(array, index);
		}
		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}
		public void Clear()
		{
			this.entries.Clear();
			this.SynchronizeList();
		}
		public OrderEntry[] GetByIndex(int index)
		{
			return this[this.entries.Keys[0]];
		}
		public void Add(OrderEntry entry)
		{
			List<OrderEntry> list;
			if (!this.entries.TryGetValue(entry.DateTime, out list))
			{
				list = new List<OrderEntry>();
				this.entries.Add(entry.DateTime, list);
			}
			list.Add(entry);
			this.SynchronizeList();
		}
		private void SynchronizeList()
		{
			this.list.Clear();
			foreach (List<OrderEntry> current in this.entries.Values)
			{
				foreach (OrderEntry current2 in current)
				{
					this.list.Add(current2);
				}
			}
		}
	}
}
