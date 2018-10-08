using System;
using System.Collections;
using System.IO;

namespace SmartQuant.Trading
{
	public class StrategyComponentList : ICollection, IEnumerable
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

		public StrategyComponentList()
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

		public ComponentRecord FindRecord(Guid guid)
		{
			foreach (ComponentRecord componentRecord in this.list)
			{
				if (componentRecord.GUID == guid)
				{
					return componentRecord;
				}
			}
			return null;
		}

		public ComponentRecord[] FindRecords(FileInfo file)
		{
			ArrayList arrayList = new ArrayList();
			foreach (ComponentRecord componentRecord in this.list)
			{
				if (!componentRecord.BuiltIn && componentRecord.File.FullName.ToLower() == file.FullName.ToLower())
				{
					arrayList.Add(componentRecord);
				}
			}
			return arrayList.ToArray(typeof(ComponentRecord)) as ComponentRecord[];
		}

		internal void Add(ComponentRecord record)
		{
			this.list.Add(record);
		}

		internal void Remove(ComponentRecord record)
		{
			this.list.Remove(record);
		}
	}
}
