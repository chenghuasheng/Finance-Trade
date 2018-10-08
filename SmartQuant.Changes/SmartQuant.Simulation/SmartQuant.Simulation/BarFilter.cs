using SmartQuant.Data;
using SmartQuant.Simulation.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
namespace SmartQuant.Simulation
{
	[TypeConverter(typeof(BarFilterTypeConverter))]
	public class BarFilter
	{
		private bool enabled;
		private List<BarFilterItem> items;
		[DefaultValue(false)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}
		[Editor(typeof(BarFilterItemListEditor), typeof(UITypeEditor))]
		public List<BarFilterItem> Items
		{
			get
			{
				return this.items;
			}
		}
		public BarFilter()
		{
			this.items = new List<BarFilterItem>();
			this.enabled = false;
		}
		public bool Contains(BarType barType, long barSize)
		{
			if (!this.enabled)
			{
				return true;
			}
			foreach (BarFilterItem current in this.items)
			{
				if (current.BarType == barType && current.BarSize == barSize)
				{
					return current.Enabled;
				}
			}
			return false;
		}
		public void Reset()
		{
			this.enabled = false;
			this.items.Clear();
		}
		public override string ToString()
		{
			List<string> list = new List<string>();
			list.Add(this.enabled.ToString());
			foreach (BarFilterItem current in this.items)
			{
				list.Add(string.Format("{0},{1},{2}", current.Enabled, current.BarType, current.BarSize));
			}
			return string.Join("-", list.ToArray());
		}
		internal void FromString(string value)
		{
			try
			{
				this.Reset();
				string[] array = value.Split(new char[]
				{
					'-'
				});
				this.enabled = bool.Parse(array[0]);
				for (int i = 1; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					BarFilterItem item = new BarFilterItem((BarType)Enum.Parse(typeof(BarType), array2[1]), long.Parse(array2[2]), bool.Parse(array2[0]));
					this.items.Add(item);
				}
			}
			catch (Exception ex)
			{
				if (Trace.IsLevelEnabled(TraceLevel.Error))
				{
					Trace.WriteLine(ex.ToString());
				}
				this.Reset();
			}
		}
	}
}
