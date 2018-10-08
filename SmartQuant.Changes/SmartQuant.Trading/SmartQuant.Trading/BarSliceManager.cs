using SmartQuant.Data;
using SmartQuant.Instruments;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading
{
	public class BarSliceManager
	{
		private int instrumentsCount;

		private Dictionary<long, BarSlice> table;

		public int InstrumentsCount
		{
			get
			{
				return this.instrumentsCount;
			}
			set
			{
				this.instrumentsCount = value;
			}
		}

		public BarSliceManager()
		{
			this.Init();
		}

		private void Init()
		{
			this.table = new Dictionary<long, BarSlice>();
		}

		internal void Reset()
		{
			this.table.Clear();
		}

		internal void AddBar(Instrument instrument, Bar bar)
		{
			BarSlice barSlice = null;
			if (!this.table.TryGetValue(bar.Size, out barSlice))
			{
				barSlice = new BarSlice(this.instrumentsCount);
				this.table.Add(bar.Size, barSlice);
			}
			barSlice.AddBar(instrument, bar);
		}

		internal BarSlice GetSlice(long barSize)
		{
			BarSlice result = null;
			this.table.TryGetValue(barSize, out result);
			return result;
		}
	}
}
