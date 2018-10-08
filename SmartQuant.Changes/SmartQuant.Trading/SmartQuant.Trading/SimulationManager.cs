using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Simulation;
using SmartQuant.Trading.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace SmartQuant.Trading
{
	[StrategyComponent("{872476E5-3774-4687-828F-34978288A6E0}", ComponentType.SimulationManager, Name = "Default_SimulationManager", Description = "")]
	public class SimulationManager : ComponentBase
	{
		public const string GUID = "{872476E5-3774-4687-828F-34978288A6E0}";

		public const string CATEGORY_SIMULATOR = "Feed Simulator";

		public const string CATEGORY_ACCOUNT = "Account";

		private const string CATEGORY_FILL_DATA = "Execution - Fill Data";

		private const string CATEGORY_FILL_MODE = "Execution - Fill Mode";

		private const SimulationMode DEFAULT_SIMULATION_MODE = SimulationMode.MaxSpeed;

		private const double DEFAULT_SPEED_MULTIPLIER = 1.0;

		private const int DEFAULT_STEP_SIZE = 86400;

		private const double DEFAULT_CASH = 10000.0;

		private const bool DEFAULT_FILL_ON_BAR = true;

		private const bool DEFAULT_FILL_ON_QUOTE = true;

		private const bool DEFAULT_FILL_ON_TRADE = true;

		private const FillOnBarMode DEFAULT_FILL_ON_BAR_MODE = FillOnBarMode.LastBarClose;

		private const FillOnTradeMode DEFAULT_FILL_ON_TRADE_MODE = FillOnTradeMode.LastTrade;

		private const FillOnQuoteMode DEFAULT_FILL_ON_QUOTE_MODE = FillOnQuoteMode.LastQuote;

		private SimulationMode fMode = SimulationMode.MaxSpeed;

		private double fSpeedMultiplier = 1.0;

		private int fStep = 86400;

		private DateTime fEntryDate = new DateTime(1970, 1, 1);

		private DateTime fExitDate = DateTime.Today;

		private double fCash = 10000.0;

		private Currency fCurrency = CurrencyManager.DefaultCurrency;

		private RequestList requests;

		private RequestList staticRequests;

		[Category("Execution - Commission & Slippage"), Description("Commission")]
		public ICommissionProvider CommissionProvider
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).CommissionProvider;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).CommissionProvider = value;
			}
		}

		[Category("Execution - Commission & Slippage"), Description("Slippage")]
		public ISlippageProvider SlippageProvider
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).SlippageProvider;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).SlippageProvider = value;
			}
		}

		[Category("Execution - Fill Data"), DefaultValue(true)]
		public bool FillOnQuote
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuote;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuote = value;
			}
		}

		[Category("Execution - Fill Data"), DefaultValue(true)]
		public bool FillOnTrade
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTrade;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTrade = value;
			}
		}

		[Category("Execution - Fill Data"), DefaultValue(true)]
		public bool FillOnBar
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBar;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBar = value;
			}
		}

		[Category("Execution - Fill Mode"), DefaultValue(FillOnTradeMode.LastTrade)]
		public FillOnTradeMode FillOnTradeMode
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTradeMode;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnTradeMode = value;
			}
		}

		[Category("Execution - Fill Mode"), DefaultValue(FillOnQuoteMode.LastQuote)]
		public FillOnQuoteMode FillOnQuoteMode
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuoteMode;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnQuoteMode = value;
			}
		}

		[Category("Execution - Fill Mode"), DefaultValue(FillOnBarMode.LastBarClose)]
		public FillOnBarMode FillOnBarMode
		{
			get
			{
				return (ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBarMode;
			}
			set
			{
				(ProviderManager.ExecutionSimulator as SimulationExecutionProvider).FillOnBarMode = value;
			}
		}

		[Browsable(false)]
		public RequestList Requests
		{
			get
			{
				return this.requests;
			}
		}

		[Category("Feed Simulator"), Editor(typeof(RequestListEditor), typeof(UITypeEditor))]
		public RequestList StaticRequests
		{
			get
			{
				return this.staticRequests;
			}
		}

		[Category("Feed Simulator"), DefaultValue(SimulationMode.MaxSpeed)]
		public SimulationMode Mode
		{
			get
			{
				return this.fMode;
			}
			set
			{
				this.fMode = value;
			}
		}

		[Category("Feed Simulator"), DefaultValue(1.0)]
		public double SpeedMultiplier
		{
			get
			{
				return this.fSpeedMultiplier;
			}
			set
			{
				this.fSpeedMultiplier = value;
			}
		}

		[Category("Feed Simulator"), DefaultValue(86400), Description("Step size in seconds")]
		public int Step
		{
			get
			{
				return this.fStep;
			}
			set
			{
				this.fStep = value;
			}
		}

		[Category("Feed Simulator"), DefaultValue(typeof(DateTime), "01/01/1970")]
		public DateTime EntryDate
		{
			get
			{
				return this.fEntryDate;
			}
			set
			{
				this.fEntryDate = value;
			}
		}

		[Category("Feed Simulator")]
		public DateTime ExitDate
		{
			get
			{
				return this.fExitDate;
			}
			set
			{
				this.fExitDate = value;
			}
		}

		[Category("Account"), DefaultValue(10000.0)]
		public double Cash
		{
			get
			{
				return this.fCash;
			}
			set
			{
				this.fCash = value;
			}
		}

		[Category("Account")]
		public Currency Currency
		{
			get
			{
				return this.fCurrency;
			}
			set
			{
				this.fCurrency = value;
			}
		}

		public SimulationManager()
		{
			this.requests = new RequestList();
			this.staticRequests = new RequestList();
		}

		public override void Init()
		{
		}

		public void SendMarketDataRequest(string request)
		{
			if (!this.requests.Contains(request))
			{
				this.requests.Add(request);
			}
		}
	}
}
