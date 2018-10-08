using SmartQuant.Execution;
using SmartQuant.Instruments;
using System;
using System.Collections;

namespace SmartQuant.Trading
{
	public class OrderTable : ICollection, IEnumerable
	{
		private Hashtable orders;

		public bool IsSynchronized
		{
			get
			{
				return this.orders.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.orders.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.orders.SyncRoot;
			}
		}

		public NamedOrderTable this[Instrument instrument]
		{
			get
			{
				if (!this.orders.ContainsKey(instrument))
				{
					this.orders.Add(instrument, new NamedOrderTable());
				}
				return this.orders[instrument] as NamedOrderTable;
			}
		}

		public SingleOrder this[Instrument instrument, string name]
		{
			get
			{
				NamedOrderTable namedOrderTable = this.orders[instrument] as NamedOrderTable;
				return namedOrderTable[name];
			}
		}

		public OrderTable()
		{
			this.orders = new Hashtable();
		}

		public void CopyTo(Array array, int index)
		{
			this.orders.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			ArrayList arrayList = new ArrayList();
			foreach (DictionaryEntry dictionaryEntry in this.orders)
			{
				foreach (DictionaryEntry dictionaryEntry2 in ((IEnumerable)dictionaryEntry.Value))
				{
					arrayList.Add(dictionaryEntry2.Value);
				}
			}
			return arrayList.GetEnumerator();
		}

		public void Add(Instrument instrument, string name, SingleOrder order)
		{
			NamedOrderTable namedOrderTable;
			if (this.orders.ContainsKey(instrument))
			{
				namedOrderTable = (this.orders[instrument] as NamedOrderTable);
			}
			else
			{
				namedOrderTable = new NamedOrderTable();
				this.orders.Add(instrument, namedOrderTable);
			}
			namedOrderTable.Add(name, order);
		}

		public void Clear()
		{
			this.orders.Clear();
		}
	}
}
