using SmartQuant.Data;
using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Trading.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;

namespace SmartQuant.Trading
{
	public class MetaStrategy : MetaStrategyBase
	{
		private MetaExposureManager metaExposureManager;

		private MetaRiskManager metaRiskManager;

		private ExecutionManager executionManager;

		private SignalList signals;

        public event SignalEventHandler SignalAdded;

        public event EventHandler SignalListCleared;
		

		[Category("Components"), Editor(typeof(MetaExposureManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public MetaExposureManager MetaExposureManager
		{
			get
			{
				return this.metaExposureManager;
			}
			set
			{
				if (!base.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.metaExposureManager != null)
				{
					this.metaExposureManager.Disconnect();
					this.metaExposureManager.MetaStrategyBase = null;
				}
				this.metaExposureManager = value;
				if (this.metaExposureManager != null)
				{
					this.metaExposureManager.MetaStrategyBase = this;
					this.metaExposureManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.MetaExposureManager);
			}
		}

		[Category("Components"), Editor(typeof(MetaRiskManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public MetaRiskManager MetaRiskManager
		{
			get
			{
				return this.metaRiskManager;
			}
			set
			{
				if (!base.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.metaRiskManager != null)
				{
					this.metaRiskManager.Disconnect();
					this.metaRiskManager.MetaStrategyBase = null;
				}
				this.metaRiskManager = value;
				if (this.metaRiskManager != null)
				{
					this.metaRiskManager.MetaStrategyBase = this;
					this.metaRiskManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.MetaRiskManager);
			}
		}

		[Category("Components"), Editor(typeof(ExecutionManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ExecutionManager ExecutionManager
		{
			get
			{
				return this.executionManager;
			}
			set
			{
				if (!base.DesignMode)
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.executionManager != null)
				{
					this.executionManager.Disconnect();
					this.executionManager.MetaStrategyBase = null;
				}
				this.executionManager = value;
				if (this.executionManager != null)
				{
					this.executionManager.MetaStrategyBase = this;
					this.executionManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.ExecutionManager);
			}
		}

		[Browsable(false)]
		public SignalList Signals
		{
			get
			{
				return this.signals;
			}
		}

		public MetaStrategy(string name) : base(name)
		{
			this.MetaExposureManager = (StrategyComponentManager.GetComponent("{2DBD0B38-8399-4d0b-9FAA-7C29FC1462BC}", this) as MetaExposureManager);
			this.ExecutionManager = (StrategyComponentManager.GetComponent("{D106D35A-E1E4-4e86-8869-846289E98232}", this) as ExecutionManager);
			this.MetaRiskManager = (StrategyComponentManager.GetComponent("{521B9C4F-01AE-4488-B4A5-104027D06BB8}", this) as MetaRiskManager);
			this.signals = new SignalList();
			this.componentTypeList.Add(ComponentType.MetaRiskManager);
			this.componentTypeList.Add(ComponentType.MetaExposureManager);
			this.componentTypeList.Add(ComponentType.ExecutionManager);
		}

		protected override void OnInit()
		{
			this.signals.Clear();
			if (this.SignalListCleared != null)
			{
				this.SignalListCleared(this, EventArgs.Empty);
			}
			this.executionManager.Init();
			this.optimizationManager.Init();
			this.metaExposureManager.Init();
			this.metaRiskManager.Init();
		}

		protected override void OnMetaStrategyStop()
		{
			this.metaExposureManager.OnStrategyStop();
			this.metaRiskManager.OnStrategyStop();
			this.executionManager.OnStrategyStop();
		}

		internal SingleOrder EmitSignal(Signal signal)
		{
			bool flag = false;
			if (signal.Status == SignalStatus.New)
			{
				if (this.metaExposureManager.Validate(signal))
				{
					signal.Status = SignalStatus.Accepted;
					flag = true;
				}
				else
				{
					signal.Status = SignalStatus.Rejected;
					signal.Rejecter = ComponentType.MetaExposureManager;
				}
			}
			this.signals.Add(signal);
			if (this.SignalAdded != null)
			{
				this.SignalAdded(new SignalEventArgs(signal));
			}
			if (flag)
			{
				return this.executionManager.Execute(signal);
			}
			return null;
		}

		protected override void OnPositionOpened(Position position)
		{
			this.metaRiskManager.OnPositionOpened(position);
			this.metaExposureManager.OnPositionOpened(position);
		}

		protected override void OnPositionChanged(Position position)
		{
			this.metaRiskManager.OnPositionChanged(position);
			this.metaExposureManager.OnPositionChanged(position);
		}

		protected override void OnPositionClosed(Position position)
		{
			this.metaRiskManager.OnPositionClosed(position);
			this.metaExposureManager.OnPositionClosed(position);
		}

		protected override void OnPortfolioValueChanged(Position position)
		{
			this.metaRiskManager.OnPortfolioValueChanged(this.portfolio);
			this.metaExposureManager.OnPortfolioValueChanged(this.portfolio);
		}

		protected override void OnNewBarOpen(Instrument instrument, Bar bar)
		{
			this.metaRiskManager.OnBarOpen(instrument, bar);
			this.metaExposureManager.OnBarOpen(instrument, bar);
		}

		protected override void OnNewBar(Instrument instrument, Bar bar)
		{
			this.metaRiskManager.OnBar(instrument, bar);
			this.metaExposureManager.OnBar(instrument, bar);
		}

		protected override void OnNewTrade(Instrument instrument, Trade trade)
		{
			this.metaRiskManager.OnTrade(instrument, trade);
			this.metaExposureManager.OnTrade(instrument, trade);
		}

		protected override void OnNewQuote(Instrument instrument, Quote quote)
		{
			this.metaRiskManager.OnQuote(instrument, quote);
			this.metaExposureManager.OnQuote(instrument, quote);
		}

		protected override void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
		{
			this.metaRiskManager.OnMarketDepth(instrument, marketDepth);
			this.metaExposureManager.OnMarketDepth(instrument, marketDepth);
		}

		protected override void OnNewFundamental(Instrument instrument, Fundamental fundamental)
		{
			this.metaRiskManager.OnFundamental(instrument, fundamental);
			this.metaExposureManager.OnFundamental(instrument, fundamental);
		}

		protected override void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
		{
			this.metaRiskManager.OnCorporateAction(instrument, corporateAction);
			this.metaExposureManager.OnCorporateAction(instrument, corporateAction);
		}

		protected override void OnNewOrder(SingleOrder order)
		{
			this.executionManager.OnNewOrder(order);
		}

		protected override void OnExecutionReport(SingleOrder order, ExecutionReport report)
		{
			this.executionManager.OnExecutionReport(order, report);
		}

		protected override void OnOrderPartiallyFilled(SingleOrder order)
		{
			this.executionManager.OnOrderPartiallyFilled(order);
		}

		protected override void OnOrderStatusChanged(SingleOrder order)
		{
			this.executionManager.OnOrderStatusChanged(order);
		}

		protected override void OnOrderFilled(SingleOrder order)
		{
			this.executionManager.OnOrderFilled(order);
		}

		protected override void OnOrderCancelled(SingleOrder order)
		{
			this.executionManager.OnOrderCancelled(order);
		}

		protected override void OnOrderRejected(SingleOrder order)
		{
			this.executionManager.OnOrderRejected(order);
		}

		protected override void OnOrderDone(SingleOrder order)
		{
			this.executionManager.OnOrderDone(order);
		}

		protected override void OnProviderConnected(IProvider provider)
		{
			this.metaRiskManager.OnProviderConnected(provider);
			this.metaExposureManager.OnProviderConnected(provider);
			this.executionManager.OnProviderConnected(provider);
		}

		protected override void OnProviderDisconnected(IProvider provider)
		{
			this.metaRiskManager.OnProviderDisconnected(provider);
			this.metaExposureManager.OnProviderDisconnected(provider);
			this.executionManager.OnProviderDisconnected(provider);
		}

		protected override void OnProviderError(IProvider provider, int id, int code, string message)
		{
			this.metaRiskManager.OnProviderError(provider, id, code, message);
			this.metaExposureManager.OnProviderError(provider, id, code, message);
			this.executionManager.OnProviderError(provider, id, code, message);
		}

		public override IComponentBase GetComponent(ComponentType type)
		{
			if (type == ComponentType.MetaExposureManager)
			{
				return this.MetaExposureManager;
			}
			if (type == ComponentType.ExecutionManager)
			{
				return this.ExecutionManager;
			}
			if (type != ComponentType.MetaRiskManager)
			{
				return base.GetComponent(type);
			}
			return this.MetaRiskManager;
		}

		public override void SetComponent(ComponentType type, IComponentBase component)
		{
			if (type == ComponentType.MetaExposureManager)
			{
				this.MetaExposureManager = (component as MetaExposureManager);
				return;
			}
			if (type == ComponentType.ExecutionManager)
			{
				this.ExecutionManager = (component as ExecutionManager);
				return;
			}
			if (type != ComponentType.MetaRiskManager)
			{
				base.SetComponent(type, component);
				return;
			}
			this.MetaRiskManager = (component as MetaRiskManager);
		}
	}
}
