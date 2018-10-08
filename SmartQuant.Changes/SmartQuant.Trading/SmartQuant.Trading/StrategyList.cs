using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class StrategyList : ICollection, IEnumerable
	{
		private SortedList list;

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public StrategyBase this[string name]
		{
			get
			{
				return this.list[name] as StrategyBase;
			}
		}

		internal StrategyList()
		{
			this.list = new SortedList();
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.Values.GetEnumerator();
		}

		internal void Add(StrategyBase strategy)
		{
			if (this.list.Contains(strategy.Name))
			{
				throw new ApplicationException("Can not add Strategy that is already present in the list : " + strategy.Name);
			}
			this.list.Add(strategy.Name, strategy);
		}

		internal void Remove(StrategyBase strategy)
		{
			this.list.Remove(strategy.Name);
		}

		public bool Contains(string name)
		{
			return this.list.ContainsKey(name);
		}
	}
}
