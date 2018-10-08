using System;
using System.Collections;

namespace SmartQuant.Trading.Conditions
{
	public class RuleItemList : ICollection, IEnumerable
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

		public RuleItem this[int index]
		{
			get
			{
				return this.list[index] as RuleItem;
			}
		}

		public RuleItemList()
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

		public void Add(RuleItem item)
		{
			this.list.Add(item);
		}

		public void Remove(RuleItem item)
		{
			this.list.Remove(item);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public void Execute()
		{
			foreach (RuleItem ruleItem in this.list)
			{
				ruleItem.Execute();
			}
		}
	}
}
