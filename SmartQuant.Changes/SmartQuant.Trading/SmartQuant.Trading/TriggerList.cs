using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class TriggerList : ICollection, IEnumerable
	{
		private ArrayList list;

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public TriggerList()
		{
			this.list = new ArrayList();
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public void Add(Trigger trigger)
		{
			this.list.Add(trigger);
		}

		public void Clear()
		{
			this.list.Clear();
		}
	}
}
