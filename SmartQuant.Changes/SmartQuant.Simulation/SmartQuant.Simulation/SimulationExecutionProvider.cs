using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Providers;
using SmartQuant.Instruments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace SmartQuant.Simulation
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SimulationExecutionProvider : IExecutionProvider, IProvider
	{
		private const string PROVIDER_NAME = "Simulator(execution)";
		private const string PROVIDER_TITLE = "Simulation Execution Provider";
		private const string PROVIDER_URL = "www.smartquant.com";
		private const byte PROVIDER_ID = 2;
		private const string CATEGORY_INFO = "Information";
		private const string CATEGORY_STATUS = "Status";
		private const string CATEGORY_FILL_DATA = "Fill Data";
		private const string CATEGORY_FILL_MODE = "Fill Mode";
		private const string CATEGORY_PROVIDERS = "Commission & Slippage";
		private bool fIsConnected;
		private ProviderStatus fStatus = ProviderStatus.Unknown;
		internal Hashtable fProcessors = new Hashtable();
		private bool fFillOnQuote = true;
		private bool fFillOnTrade = true;
		private bool fFillOnBar = true;
		private FillOnTradeMode fFillOnTradeMode;
		private FillOnQuoteMode fFillOnQuoteMode;
		private FillOnBarMode fFillOnBarMode;
		private bool partialFills = true;
		private ICommissionProvider fCommissionProvider = new CommissionProvider();
		private ISlippageProvider fSlippageProvider = new SlippageProvider();
		private BarFilter fBarFilter = new BarFilter();
             
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event ProviderErrorEventHandler Error;
        public event EventHandler StatusChanged;
        public event ExecutionReportEventHandler ExecutionReport;
        public event OrderCancelRejectEventHandler OrderCancelReject;
		[Category("Information")]
		public string Name
		{
			get
			{
				return "Simulator(execution)";
			}
		}
		[Category("Information")]
		public string Title
		{
			get
			{
				return "Simulation Execution Provider";
			}
		}
		[Category("Information")]
		public byte Id
		{
			get
			{
				return 2;
			}
		}
		[Category("Information")]
		public string URL
		{
			get
			{
				return "www.smartquant.com";
			}
		}
		[Category("Status")]
		public bool IsConnected
		{
			get
			{
				return this.fIsConnected;
			}
		}
		[Category("Status")]
		public ProviderStatus Status
		{
			get
			{
				return this.fStatus;
			}
		}
		[Category("Fill Data"), DefaultValue(true)]
		public bool PartialFills
		{
			get
			{
				return this.partialFills;
			}
			set
			{
				this.partialFills = value;
			}
		}
		[Category("Fill Data"), DefaultValue(true)]
		public bool FillOnQuote
		{
			get
			{
				return this.fFillOnQuote;
			}
			set
			{
				this.fFillOnQuote = value;
			}
		}
		[Category("Fill Data"), DefaultValue(true)]
		public bool FillOnTrade
		{
			get
			{
				return this.fFillOnTrade;
			}
			set
			{
				this.fFillOnTrade = value;
			}
		}
		[Category("Fill Data"), DefaultValue(true)]
		public bool FillOnBar
		{
			get
			{
				return this.fFillOnBar;
			}
			set
			{
				this.fFillOnBar = value;
			}
		}
		[Category("Fill Mode"), DefaultValue(FillOnTradeMode.LastTrade)]
		public FillOnTradeMode FillOnTradeMode
		{
			get
			{
				return this.fFillOnTradeMode;
			}
			set
			{
				this.fFillOnTradeMode = value;
			}
		}
		[Category("Fill Mode"), DefaultValue(FillOnQuoteMode.LastQuote)]
		public FillOnQuoteMode FillOnQuoteMode
		{
			get
			{
				return this.fFillOnQuoteMode;
			}
			set
			{
				this.fFillOnQuoteMode = value;
			}
		}
		[Category("Fill Mode"), DefaultValue(FillOnBarMode.LastBarClose)]
		public FillOnBarMode FillOnBarMode
		{
			get
			{
				return this.fFillOnBarMode;
			}
			set
			{
				this.fFillOnBarMode = value;
			}
		}
		[Category("Commission & Slippage")]
		public ICommissionProvider CommissionProvider
		{
			get
			{
				return this.fCommissionProvider;
			}
			set
			{
				this.fCommissionProvider = value;
			}
		}
		[Browsable(false)]
		public string CommissionProviderStr
		{
			get
			{
				List<string> list = new List<string>();
				if (this.fCommissionProvider != null)
				{
					CultureInfo invariantCulture = CultureInfo.InvariantCulture;
					list.Add(this.fCommissionProvider.CommType.ToString());
					list.Add(this.fCommissionProvider.Commission.ToString(invariantCulture));
					if (this.fCommissionProvider is CommissionProvider)
					{
						list.Add((this.fCommissionProvider as CommissionProvider).MinCommission.ToString(invariantCulture));
					}
				}
				return string.Join("|", list.ToArray());
			}
			set
			{
				if (this.fCommissionProvider == null || value == null)
				{
					return;
				}
				string[] array = value.Split(new char[]
				{
					'|'
				});
				if (array.Length < 2)
				{
					return;
				}
				if (Enum.IsDefined(typeof(CommType), array[0]))
				{
					this.fCommissionProvider.CommType = (CommType)Enum.Parse(typeof(CommType), array[0]);
				}
				double commission;
				if (double.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out commission))
				{
					this.fCommissionProvider.Commission = commission;
				}
				double minCommission;
				if (this.fCommissionProvider is CommissionProvider && array.Length == 3 && double.TryParse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out minCommission))
				{
					(this.fCommissionProvider as CommissionProvider).MinCommission = minCommission;
				}
			}
		}
		[Category("Commission & Slippage")]
		public ISlippageProvider SlippageProvider
		{
			get
			{
				return this.fSlippageProvider;
			}
			set
			{
				this.fSlippageProvider = value;
			}
		}
		[Category("Fill Data")]
		public BarFilter BarFilter
		{
			get
			{
				return this.fBarFilter;
			}
		}
		[Browsable(false)]
		public string BarFilterString
		{
			get
			{
				return this.fBarFilter.ToString();
			}
			set
			{
				this.fBarFilter.FromString(value);
			}
		}
		public void Connect()
		{
			if (!this.fIsConnected)
			{
				this.fIsConnected = true;
				this.fStatus = ProviderStatus.Connected;
				if (this.Connected != null)
				{
					this.Connected(this, EventArgs.Empty);
				}
				if (this.StatusChanged != null)
				{
					this.StatusChanged(this, EventArgs.Empty);
				}
			}
		}
		public void Connect(int timeout)
		{
			this.Connect();
			ProviderManager.WaitConnected(this, timeout);
		}
		public void Disconnect()
		{
			if (this.fIsConnected)
			{
				foreach (SimulationExecutionProcessor simulationExecutionProcessor in new ArrayList(this.fProcessors.Values))
				{
					simulationExecutionProcessor.Cancel();
				}
				this.fIsConnected = false;
				this.fStatus = ProviderStatus.Disconnected;
				if (this.Disconnected != null)
				{
					this.Disconnected(this, EventArgs.Empty);
				}
				if (this.StatusChanged != null)
				{
					this.StatusChanged(this, EventArgs.Empty);
				}
			}
		}
		public void Shutdown()
		{
			this.Disconnect();
		}
		public void SendNewOrderSingle(NewOrderSingle order)
		{
			SingleOrder singleOrder = order as SingleOrder;
			if (singleOrder.IsFilled || singleOrder.IsCancelled)
			{
				return;
			}
			this.EmitExecutionReport(new ExecutionReport
			{
				TransactTime = Clock.Now,
				ClOrdID = order.ClOrdID,
				ExecType = ExecType.New,
				OrdStatus = OrdStatus.New,
				Symbol = order.Symbol,
				OrdType = order.OrdType,
				Side = order.Side,
				Price = order.Price,
				StopPx = order.StopPx,
				OrderQty = order.OrderQty,
				CumQty = 0.0,
				LeavesQty = order.OrderQty,
				Currency = order.Currency,
				Text = order.Text
			});
			new SimulationExecutionProcessor(this, order);
		}
		public void SendOrderCancelRequest(FIXOrderCancelRequest request)
		{
			SimulationExecutionProcessor simulationExecutionProcessor = this.fProcessors[request.OrigClOrdID] as SimulationExecutionProcessor;
			if (simulationExecutionProcessor != null)
			{
				simulationExecutionProcessor.Cancel();
			}
		}
		public void SendOrderCancelReplaceRequest(FIXOrderCancelReplaceRequest request)
		{
			SimulationExecutionProcessor simulationExecutionProcessor = this.fProcessors[request.OrigClOrdID] as SimulationExecutionProcessor;
			if (simulationExecutionProcessor != null)
			{
				simulationExecutionProcessor.Replace(request);
			}
		}
		public void SendOrderStatusRequest(FIXOrderStatusRequest request)
		{
			throw new NotImplementedException();
		}
		public BrokerInfo GetBrokerInfo()
		{
			throw new NotImplementedException();
		}
		public SimulationExecutionProvider()
		{
			ProviderManager.Add(this);
			ProviderManager.ExecutionSimulator = this;
		}
		internal void EmitExecutionReport(ExecutionReport report)
		{
            if (this.ExecutionReport != null)
			{
				this.ExecutionReport(this, new ExecutionReportEventArgs(report));
			}
		}
		public override string ToString()
		{
			return this.Name;
		}
	}
}
