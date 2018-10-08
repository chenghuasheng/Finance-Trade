using SmartQuant.Data;
using System;
namespace SmartQuant.Simulation
{
	internal class QueueEntry : IComparable
	{
		internal IDataSeries Series;
		internal IDataObject Object;
		internal int CurrentPosition;
		internal int EndPosition;
		public virtual int CompareTo(object obj)
		{
			QueueEntry queueEntry = obj as QueueEntry;
			if (this.Object.DateTime > queueEntry.Object.DateTime)
			{
				return 1;
			}
			if (this.Object.DateTime < queueEntry.Object.DateTime)
			{
				return -1;
			}
			return 0;
		}
	}
}
