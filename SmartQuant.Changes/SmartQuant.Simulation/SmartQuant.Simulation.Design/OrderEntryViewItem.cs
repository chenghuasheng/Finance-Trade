using SmartQuant.FIX;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class OrderEntryViewItem : ListViewItem
	{
		private OrderEntry entry;
		private Color defaultColor;
		public OrderEntry Entry
		{
			get
			{
				return this.entry;
			}
		}
		public OrderEntryViewItem(OrderEntry entry) : base(new string[8])
		{
			this.entry = entry;
			this.defaultColor = base.ForeColor;
			this.UpdateValues();
		}
		public void UpdateValues()
		{
			base.SubItems[0].Text = this.entry.DateTime.ToString();
			base.SubItems[1].Text = ((this.entry.Instrument == null) ? "(none)" : this.entry.Instrument.Symbol);
			base.SubItems[2].Text = this.entry.Side.ToString();
			base.SubItems[3].Text = this.entry.OrdType.ToString();
			switch (this.entry.OrdType)
			{
			case OrdType.Market:
				base.SubItems[4].Text = "";
				base.SubItems[5].Text = "";
				break;
			case OrdType.Limit:
				base.SubItems[4].Text = this.entry.Price.ToString(this.entry.Instrument.PriceDisplay);
				base.SubItems[5].Text = "";
				break;
			case OrdType.Stop:
				base.SubItems[4].Text = "";
				base.SubItems[5].Text = this.entry.StopPx.ToString(this.entry.Instrument.PriceDisplay);
				break;
			case OrdType.StopLimit:
				base.SubItems[4].Text = this.entry.Price.ToString(this.entry.Instrument.PriceDisplay);
				base.SubItems[5].Text = this.entry.StopPx.ToString(this.entry.Instrument.PriceDisplay);
				break;
			}
			base.SubItems[6].Text = this.entry.OrderQty.ToString();
			base.SubItems[7].Text = this.entry.Text;
			if (this.entry.Enabled)
			{
				base.Checked = true;
				base.ForeColor = this.defaultColor;
				return;
			}
			base.Checked = false;
			base.ForeColor = SystemColors.GrayText;
		}
	}
}
