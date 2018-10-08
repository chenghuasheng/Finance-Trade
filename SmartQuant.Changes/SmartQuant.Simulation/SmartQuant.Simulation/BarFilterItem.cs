using SmartQuant.Data;
using SmartQuant.Instruments;
using System;
using System.ComponentModel;
namespace SmartQuant.Simulation
{
	public class BarFilterItem
	{
		private const string CATEGORY_STATUS = "Status";
		private const string CATEGORY_PROPERTIES = "Properties";
		private bool enabled;
		private BarType barType;
		private long barSize;
		[Category("Status"), DefaultValue(true)]
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
		[Category("Properties")]
		public BarType BarType
		{
			get
			{
				return this.barType;
			}
			set
			{
				this.barType = value;
			}
		}
		[Category("Properties")]
		public long BarSize
		{
			get
			{
				return this.barSize;
			}
			set
			{
				this.barSize = value;
			}
		}
		public BarFilterItem(BarType barType, long barSize, bool enabled)
		{
			this.barType = barType;
			this.barSize = barSize;
			this.enabled = enabled;
		}
		public BarFilterItem() : this(DataManager.DefaultBarType, DataManager.DefaultBarSize, true)
		{
		}
		public override string ToString()
		{
			return string.Format("{0}({1}) - {2}", this.barType, this.barSize, this.enabled);
		}
	}
}
