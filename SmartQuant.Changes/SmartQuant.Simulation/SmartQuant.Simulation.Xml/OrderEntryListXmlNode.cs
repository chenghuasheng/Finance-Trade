using SmartQuant.Xml;
using System;
namespace SmartQuant.Simulation.Xml
{
	internal class OrderEntryListXmlNode : ListXmlNode<OrderEntryXmlNode>
	{
		public override string NodeName
		{
			get
			{
				return "entries";
			}
		}
		public OrderEntryXmlNode Add(OrderEntry entry)
		{
			OrderEntryXmlNode orderEntryXmlNode = base.AppendChildNode();
			orderEntryXmlNode.Enabled = entry.Enabled;
			orderEntryXmlNode.DateTime.Value = entry.DateTime;
			orderEntryXmlNode.Symbol.Value = ((entry.Instrument == null) ? "" : entry.Instrument.Symbol);
			orderEntryXmlNode.Side.Value = entry.Side;
			orderEntryXmlNode.OrdType.Value = entry.OrdType;
			orderEntryXmlNode.Price.Value = entry.Price;
			orderEntryXmlNode.StopPx.Value = entry.StopPx;
			orderEntryXmlNode.OrderQty.Value = entry.OrderQty;
			orderEntryXmlNode.Text.Value = entry.Text;
			return orderEntryXmlNode;
		}
	}
}
