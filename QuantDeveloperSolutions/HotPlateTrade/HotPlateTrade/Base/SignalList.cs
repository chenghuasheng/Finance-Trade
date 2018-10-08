using System;
using System.Collections;
public class SignalList : ICollection, IEnumerable
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
	public Signal this[int index]
	{
		get
		{
			return this.list[index] as Signal;
		}
	}
	public SignalList()
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
	public void Clear()
	{
		this.list.Clear();
	}
	public void Add(Signal signal)
	{
		this.list.Add(signal);
	}
}