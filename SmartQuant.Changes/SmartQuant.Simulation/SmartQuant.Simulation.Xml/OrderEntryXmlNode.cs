using SmartQuant.FIX;
using SmartQuant.Xml;
using System;
namespace SmartQuant.Simulation.Xml
{
	internal class OrderEntryXmlNode : XmlNodeBase
	{
		private const string ATTR_ENABLED = "enabled";
		private const string NODE_NAME_DATETIME = "datetime";
		private const string NODE_NAME_SYMBOL = "symbol";
		private const string NODE_NAME_SIDE = "side";
		private const string NODE_NAME_ORDTYPE = "ordtype";
		private const string NODE_NAME_PRICE = "price";
		private const string NODE_NAME_STOPPX = "stoppx";
		private const string NODE_NAME_ORDERQTY = "orderqty";
		private const string NODE_NAME_TEXT = "text";
		public override string NodeName
		{
			get
			{
				return "entry";
			}
		}
		public bool Enabled
		{
			get
			{
				return base.GetBooleanAttribute("enabled");
			}
			set
			{
				base.SetAttribute("enabled", value);
			}
		}
		public DateTimeValueXmlNode DateTime
		{
			get
			{
				return base.GetChildNode<DateTimeValueXmlNode>("datetime");
			}
		}
		public StringValueXmlNode Symbol
		{
			get
			{
				return base.GetChildNode<StringValueXmlNode>("symbol");
			}
		}
		public EnumValueXmlNode<Side> Side
		{
			get
			{
				return base.GetChildNode<EnumValueXmlNode<Side>>("side");
			}
		}
		public EnumValueXmlNode<OrdType> OrdType
		{
			get
			{
				return base.GetChildNode<EnumValueXmlNode<OrdType>>("ordtype");
			}
		}
		public DoubleValueXmlNode Price
		{
			get
			{
				return base.GetChildNode<DoubleValueXmlNode>("price");
			}
		}
		public DoubleValueXmlNode StopPx
		{
			get
			{
				return base.GetChildNode<DoubleValueXmlNode>("stoppx");
			}
		}
		public DoubleValueXmlNode OrderQty
		{
			get
			{
				return base.GetChildNode<DoubleValueXmlNode>("orderqty");
			}
		}
		public StringValueXmlNode Text
		{
			get
			{
				return base.GetChildNode<StringValueXmlNode>("text");
			}
		}
	}
}
