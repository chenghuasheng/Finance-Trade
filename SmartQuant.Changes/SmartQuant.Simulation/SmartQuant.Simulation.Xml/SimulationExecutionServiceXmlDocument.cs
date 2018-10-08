using SmartQuant.Xml;
using System;
namespace SmartQuant.Simulation.Xml
{
	internal class SimulationExecutionServiceXmlDocument : XmlDocumentBase
	{
		public OrderEntryListXmlNode Entries
		{
			get
			{
				return base.GetChildNode<OrderEntryListXmlNode>();
			}
		}
		public SimulationExecutionServiceXmlDocument() : base("configuration")
		{
		}
	}
}
