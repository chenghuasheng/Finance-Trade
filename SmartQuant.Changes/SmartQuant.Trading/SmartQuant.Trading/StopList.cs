using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class StopList : ICollection, IEnumerable
	{
		private ArrayList fList;

		public bool IsSynchronized
		{
			get
			{
				return this.fList.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.fList.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.fList.SyncRoot;
			}
		}

		public IStop this[int index]
		{
			get
			{
				return this.fList[index] as IStop;
			}
		}

		public StopList()
		{
			this.fList = new ArrayList();
		}

		public void CopyTo(Array array, int index)
		{
			this.fList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.fList.GetEnumerator();
		}

		public void Add(IStop stop)
		{
			this.Add(stop, true);
		}

		public void Add(IStop stop, bool sort)
		{
			this.fList.Add(stop);
		}

		public void Remove(IStop stop)
		{
			this.fList.Remove(stop);
		}

		public void RemoveAt(int index)
		{
			this.fList.RemoveAt(index);
		}

		public void Clear()
		{
			this.fList.Clear();
		}

		public bool Contains(IStop stop)
		{
			return this.fList.Contains(stop);
		}

		public void Sort()
		{
			this.fList.Sort();
		}
	}
}
