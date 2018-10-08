using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Services;
using SmartQuant.Simulation.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
namespace SmartQuant.Simulation
{
	public class SimulationExecutionService : ServiceBase, IExecutionService, IMarketService, IService
	{
		private const string CATEGORY_INFO = "Information";
		private const string CATEGORY_STATUS = "Status";
		private const string CATEGORY_SETTINGS = "Settings";
		private const string CONFIGURATION_FILE_NAME = "simulator.execution.xml";
		private OrderEntryList entries;
		private List<OrderEntry> entryQueue;
        public event FIXNewOrderSingleEventHandler NewOrderSingle;
        public event FIXOrderCancelRequestEventHandler OrderCancelRequest;
        public event FIXOrderCancelReplaceRequestEventHandler OrderCancelReplaceRequest;
        public event FIXLogonEventHandler Logon;
        public event FIXLogoutEventHandler Logout;
		[Category("Settings")]
		public OrderEntryList Entries
		{
			get
			{
				return this.entries;
			}
		}
		[Category("Information")]
		public override byte Id
		{
			get
			{
				return 1;
			}
		}
		[Category("Information")]
		public override string Name
		{
			get
			{
				return "Simulator(execution)";
			}
		}
		[Category("Information")]
		public override string Description
		{
			get
			{
				return "Simulation Execution Service";
			}
		}
		[Category("Status")]
		public override ServiceStatus Status
		{
			get
			{
				return base.Status;
			}
		}
		public SimulationExecutionService()
		{
			this.entries = new OrderEntryList();
			this.entryQueue = new List<OrderEntry>();
			this.LoadConfiguration();
			ServiceManager.Add(this);
			ServiceManager.ExecutionSimulator = this;
		}
		public override void Start()
		{
			if (this.status == ServiceStatus.Stopped)
			{
				Simulator simulator = ((SimulationDataProvider)ProviderManager.MarketDataSimulator).Simulator;
				simulator.StateChanged += new EventHandler(this.simulator_StateChanged);
				simulator.NewObject += new SeriesObjectEventHandler(this.simulator_NewObject);
				base.SetServiceStatus(ServiceStatus.Started);
			}
		}
		public override void Stop()
		{
			if (this.status == ServiceStatus.Started)
			{
				Simulator simulator = ((SimulationDataProvider)ProviderManager.MarketDataSimulator).Simulator;
				simulator.StateChanged -= new EventHandler(this.simulator_StateChanged);
				simulator.NewObject -= new SeriesObjectEventHandler(this.simulator_NewObject);
				base.SetServiceStatus(ServiceStatus.Stopped);
			}
		}
		public void Send(FIXExecutionReport message)
		{
		}
		public void Send(FIXOrderCancelReject message)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public void Send(FIXLogon message)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public void Send(FIXLogout message)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		private void simulator_StateChanged(object sender, EventArgs e)
		{
			Simulator simulator = ((SimulationDataProvider)ProviderManager.MarketDataSimulator).Simulator;
			switch (simulator.CurrentState)
			{
			case SimulatorState.Stopped:
				this.entryQueue.Clear();
				return;
			case SimulatorState.Running:
				this.entryQueue.Clear();
				foreach (OrderEntry orderEntry in this.entries)
				{
					if (orderEntry.Enabled)
					{
						this.entryQueue.Add(orderEntry);
					}
				}
				return;
			default:
				return;
			}
		}
		private void simulator_NewObject(SeriesObjectEventArgs args)
		{
			while (this.entryQueue.Count > 0)
			{
				OrderEntry orderEntry = this.entryQueue[0];
				if (!(orderEntry.DateTime <= args.Object.DateTime))
				{
					break;
				}
				this.entryQueue.RemoveAt(0);
				this.EmitNewOrderSingle(orderEntry);
			}
		}
		private void EmitNewOrderSingle(OrderEntry entry)
		{
			SingleOrder singleOrder = new SingleOrder();
			singleOrder.TransactTime = Clock.Now;
			singleOrder.Instrument = entry.Instrument;
			singleOrder.Side = entry.Side;
			singleOrder.OrdType = entry.OrdType;
			singleOrder.Price = entry.Price;
			singleOrder.StopPx = entry.StopPx;
			singleOrder.OrderQty = entry.OrderQty;
			singleOrder.Text = entry.Text;
			if (this.NewOrderSingle != null)
			{
				this.NewOrderSingle(this, new FIXNewOrderSingleEventArgs(singleOrder));
			}
		}
		internal void SaveConfiguration()
		{
			SimulationExecutionServiceXmlDocument simulationExecutionServiceXmlDocument = new SimulationExecutionServiceXmlDocument();
			foreach (OrderEntry entry in this.entries)
			{
				simulationExecutionServiceXmlDocument.Entries.Add(entry);
			}
			simulationExecutionServiceXmlDocument.Save(this.GetConfigurationFileName());
		}
		private void LoadConfiguration()
		{
			string configurationFileName = this.GetConfigurationFileName();
			if (System.IO.File.Exists(configurationFileName))
			{
				SimulationExecutionServiceXmlDocument simulationExecutionServiceXmlDocument = new SimulationExecutionServiceXmlDocument();
				simulationExecutionServiceXmlDocument.Load(configurationFileName);
				foreach (OrderEntryXmlNode orderEntryXmlNode in simulationExecutionServiceXmlDocument.Entries)
				{
					OrderEntry orderEntry = new OrderEntry();
					orderEntry.Enabled = orderEntryXmlNode.Enabled;
					orderEntry.DateTime = orderEntryXmlNode.DateTime.Value;
					orderEntry.Instrument = InstrumentManager.Instruments[orderEntryXmlNode.Symbol.Value];
					orderEntry.Side = orderEntryXmlNode.Side.Value;
					orderEntry.OrdType = orderEntryXmlNode.OrdType.Value;
					orderEntry.Price = orderEntryXmlNode.Price.Value;
					orderEntry.StopPx = orderEntryXmlNode.StopPx.Value;
					orderEntry.OrderQty = orderEntryXmlNode.OrderQty.Value;
					orderEntry.Text = orderEntryXmlNode.Text.Value;
					this.entries.Add(orderEntry);
				}
			}
		}
		private string GetConfigurationFileName()
		{
			return string.Format("{0}\\{1}", Framework.Installation.IniDir.FullName, "simulator.execution.xml");
		}
	}
}
