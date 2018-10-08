using SmartQuant.Data;
using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.FIXData;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Trading.Design;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace SmartQuant.Trading
{
	public class Strategy : StrategyBase
	{
		private CrossEntry crossEntry;

		private CrossExit crossExit;

		private CrossEntry runtimeCrossEntry;

		private CrossExit runtimeCrossExit;

		private Entry entry;

		private Exit exit;

		private MoneyManager moneyManager;

		private RiskManager riskManager;

		private ExposureManager exposureManager;

		private Dictionary<Instrument, Entry> entries;

		private Dictionary<Instrument, Exit> exits;

		private Dictionary<Instrument, MoneyManager> moneyManagers;

		private Dictionary<Instrument, RiskManager> riskManagers;

		[Browsable(false)]
		public MetaStrategy MetaStrategy
		{
			get
			{
				return base.MetaStrategyBase as MetaStrategy;
			}
		}

		[Category("Components"), Editor(typeof(CrossEntryTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public CrossEntry CrossEntry
		{
			get
			{
				if (this.metaStrategyBase == null || !this.metaStrategyBase.IsRunning)
				{
					return this.crossEntry;
				}
				return this.runtimeCrossEntry;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.crossEntry != null)
				{
					this.crossEntry.Disconnect();
					this.crossEntry.StrategyBase = null;
				}
				this.crossEntry = value;
				if (this.crossEntry != null)
				{
					this.crossEntry.StrategyBase = this;
					this.crossEntry.Connect();
				}
				base.EmitComponentChanged(ComponentType.CrossEntry);
			}
		}

		[Category("Components"), Editor(typeof(CrossExitTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public CrossExit CrossExit
		{
			get
			{
				if (this.metaStrategyBase == null || !this.metaStrategyBase.IsRunning)
				{
					return this.crossExit;
				}
				return this.runtimeCrossExit;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.crossExit != null)
				{
					this.crossExit.Disconnect();
					this.crossExit.StrategyBase = null;
				}
				this.crossExit = value;
				if (this.crossExit != null)
				{
					this.crossExit.StrategyBase = this;
					this.crossExit.Connect();
				}
				base.EmitComponentChanged(ComponentType.CrossExit);
			}
		}

		[Category("Components"), Editor(typeof(EntryTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public Entry Entry
		{
			get
			{
				return this.entry;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.entry != null)
				{
					this.entry.Disconnect();
					this.entry.StrategyBase = null;
				}
				this.entry = value;
				if (this.entry != null)
				{
					this.entry.StrategyBase = this;
					this.entry.Connect();
				}
				base.EmitComponentChanged(ComponentType.Entry);
			}
		}

		[Category("Components"), Editor(typeof(ExitTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public Exit Exit
		{
			get
			{
				return this.exit;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.exit != null)
				{
					this.exit.Disconnect();
					this.exit.StrategyBase = null;
				}
				this.exit = value;
				if (this.exit != null)
				{
					this.exit.StrategyBase = this;
					this.exit.Connect();
				}
				base.EmitComponentChanged(ComponentType.Exit);
			}
		}

		[Category("Components"), Editor(typeof(MoneyManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public MoneyManager MoneyManager
		{
			get
			{
				return this.moneyManager;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.moneyManager != null)
				{
					this.moneyManager.Disconnect();
					this.moneyManager.StrategyBase = null;
				}
				this.moneyManager = value;
				if (this.moneyManager != null)
				{
					this.moneyManager.StrategyBase = this;
					this.moneyManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.MoneyManager);
			}
		}

		[Category("Components"), Editor(typeof(RiskManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public RiskManager RiskManager
		{
			get
			{
				return this.riskManager;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.riskManager != null)
				{
					this.riskManager.Disconnect();
					this.riskManager.StrategyBase = null;
				}
				this.riskManager = value;
				if (this.riskManager != null)
				{
					this.riskManager.StrategyBase = this;
					this.riskManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.RiskManager);
			}
		}

		[Category("Components"), Editor(typeof(ExposureManagerTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ExposureManager ExposureManager
		{
			get
			{
				return this.exposureManager;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.exposureManager != null)
				{
					this.exposureManager.Disconnect();
					this.exposureManager.StrategyBase = null;
				}
				this.exposureManager = value;
				if (this.exposureManager != null)
				{
					this.exposureManager.StrategyBase = this;
					this.exposureManager.Connect();
				}
				base.EmitComponentChanged(ComponentType.ExposureManager);
			}
		}

		internal Dictionary<Instrument, Entry> Entries
		{
			get
			{
				return this.entries;
			}
		}

		internal Dictionary<Instrument, Exit> Exits
		{
			get
			{
				return this.exits;
			}
		}

		internal Dictionary<Instrument, MoneyManager> MoneyManagers
		{
			get
			{
				return this.moneyManagers;
			}
		}

		internal Dictionary<Instrument, RiskManager> RiskManagers
		{
			get
			{
				return this.riskManagers;
			}
		}

		public Strategy(string name, string description) : base(name, description)
		{
			this.CrossExit = (StrategyComponentManager.GetComponent("{D779BA8E-C0CA-44cf-8745-99105365882F}", this) as CrossExit);
			this.CrossEntry = (StrategyComponentManager.GetComponent("{664274F3-FDE1-46da-A84F-556E4A0EB170}", this) as CrossEntry);
			this.Entry = (StrategyComponentManager.GetComponent("{94FAFF9D-5281-4c67-A599-B893F1F58B38}", this) as Entry);
			this.Exit = (StrategyComponentManager.GetComponent("{6FEE0044-0FD2-418d-94E6-400834BEE5D3}", this) as Exit);
			this.MoneyManager = (StrategyComponentManager.GetComponent("{9637DF40-0F84-46e3-AC54-0EC2D2CE2699}", this) as MoneyManager);
			this.RiskManager = (StrategyComponentManager.GetComponent("{BE0176A8-3BBD-407c-814A-D5A3E3437899}", this) as RiskManager);
			this.ExposureManager = (StrategyComponentManager.GetComponent("{0449D7E3-2016-47f6-9B80-C787B3E0F18F}", this) as ExposureManager);
			this.entries = new Dictionary<Instrument, Entry>();
			this.exits = new Dictionary<Instrument, Exit>();
			this.moneyManagers = new Dictionary<Instrument, MoneyManager>();
			this.riskManagers = new Dictionary<Instrument, RiskManager>();
			this.componentTypeList.Add(ComponentType.Entry);
			this.componentTypeList.Add(ComponentType.Exit);
			this.componentTypeList.Add(ComponentType.CrossEntry);
			this.componentTypeList.Add(ComponentType.CrossExit);
			this.componentTypeList.Add(ComponentType.RiskManager);
			this.componentTypeList.Add(ComponentType.MoneyManager);
			this.componentTypeList.Add(ComponentType.ExposureManager);
		}

		public Strategy(string name) : this(name, "")
		{
		}
        /*------------------自已添加----------------------------*/
        protected override void OnInit()
        {
            this.runtimeCrossEntry = (Activator.CreateInstance(this.crossEntry.GetType()) as CrossEntry);
            this.runtimeCrossExit = (Activator.CreateInstance(this.crossExit.GetType()) as CrossExit);
            this.runtimeCrossEntry.StrategyBase = this;
            this.runtimeCrossExit.StrategyBase = this;
            base.SetProxyProperties(this.runtimeCrossEntry, this.crossEntry);
            base.SetProxyProperties(this.runtimeCrossExit, this.crossExit);
        }
        protected override void OnCrossBehaviorInit()
        {           
            this.runtimeCrossEntry.Init();
            this.runtimeCrossExit.Init();
            this.exposureManager.Init();
        }
        protected override void OnBehaviorInit()
        {
            this.entries.Clear();
            this.exits.Clear();
            this.moneyManagers.Clear();
            this.riskManagers.Clear();
            //把持仓中的证券添加到市场中来
            foreach (Position position in this.portfolio.Positions)
            {
                Instrument instrument = position.Instrument;
                this.marketManager.AddInstrument(instrument);
            }
            foreach (Instrument instrument in this.marketManager.Instruments)
            {
                this.activeInstruments.Add(instrument);
                Entry entry = Activator.CreateInstance(this.entry.GetType()) as Entry;
                Exit exit = Activator.CreateInstance(this.exit.GetType()) as Exit;
                MoneyManager moneyManager = Activator.CreateInstance(this.moneyManager.GetType()) as MoneyManager;
                RiskManager riskManager = Activator.CreateInstance(this.riskManager.GetType()) as RiskManager;
                entry.StrategyBase = this;
                exit.StrategyBase = this;
                moneyManager.StrategyBase = this;
                riskManager.StrategyBase = this;
                entry.Instrument = instrument;
                exit.Instrument = instrument;
                moneyManager.Instrument = instrument;
                riskManager.Instrument = instrument;
                base.SetProxyProperties(entry, this.entry);
                base.SetProxyProperties(exit, this.exit);
                base.SetProxyProperties(moneyManager, this.moneyManager);
                base.SetProxyProperties(riskManager, this.riskManager);
                entry.Init();
                exit.Init();
                moneyManager.Init();
                riskManager.Init();
                this.entries.Add(instrument, entry);
                this.exits.Add(instrument, exit);
                this.moneyManagers.Add(instrument, moneyManager);
                this.riskManagers.Add(instrument, riskManager);
            } 
        }
        /*------------------------------------------------------*/
        protected override void OnStrategyStop()
		{
			this.runtimeCrossEntry.OnStrategyStop();
			this.runtimeCrossExit.OnStrategyStop();
			foreach (Entry current in this.entries.Values)
			{
				current.OnStrategyStop();
			}
			foreach (Exit current2 in this.exits.Values)
			{
				current2.OnStrategyStop();
			}
			foreach (MoneyManager current3 in this.moneyManagers.Values)
			{
				current3.OnStrategyStop();
			}
			foreach (RiskManager current4 in this.riskManagers.Values)
			{
				current4.OnStrategyStop();
			}
			this.exposureManager.OnStrategyStop();
		}

		internal SingleOrder EmitSignal(Signal signal)
		{
			signal.Strategy = this;
			MoneyManager moneyManager = this.moneyManagers[signal.Instrument];
			double positionSize = moneyManager.GetPositionSize(signal);
			if (positionSize > 0.0)
			{
				signal.Qty = positionSize;
				RiskManager riskManager = this.riskManagers[signal.Instrument];
				if (!riskManager.Validate(signal))
				{
					signal.Status = SignalStatus.Rejected;
					signal.Rejecter = ComponentType.RiskManager;
				}
				if (!this.exposureManager.Validate(signal))
				{
					signal.Status = SignalStatus.Rejected;
					signal.Rejecter = ComponentType.ExposureManager;
				}
			}
			else
			{
				signal.Status = SignalStatus.Rejected;
				signal.Rejecter = ComponentType.MoneyManager;
			}
			return this.MetaStrategy.EmitSignal(signal);
		}

		protected override void OnNewTrade(Instrument instrument, Trade trade)
		{
			foreach (Stop stop in new ArrayList(this.activeStops[instrument]))
			{
				if (stop.Connected)
				{
					stop.OnNewTrade(trade);
				}
			}
			this.runtimeCrossExit.OnTrade(instrument, trade);
			this.runtimeCrossEntry.OnTrade(instrument, trade);
			this.marketManager.OnTrade(instrument, trade);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnTrade(trade);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnTrade(trade);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnTrade(trade);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnTrade(trade);
            /*---------------------------------------*/
            //this.exits[instrument].OnTrade(trade);
            //this.entries[instrument].OnTrade(trade);
            //this.moneyManagers[instrument].OnTrade(trade);
            //this.riskManagers[instrument].OnTrade(trade);
        }

		protected override void OnNewQuote(Instrument instrument, Quote quote)
		{
			foreach (Stop stop in new ArrayList(this.activeStops[instrument]))
			{
				if (stop.Connected)
				{
					stop.OnNewQuote(quote);
				}
			}
			this.runtimeCrossExit.OnQuote(instrument, quote);
			this.runtimeCrossEntry.OnQuote(instrument, quote);
			this.marketManager.OnQuote(instrument, quote);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnQuote(quote);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnQuote(quote);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnQuote(quote);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnQuote(quote);
            /*---------------------------------------*/
            /*this.exits[instrument].OnQuote(quote);
			this.entries[instrument].OnQuote(quote);
			this.moneyManagers[instrument].OnQuote(quote);
			this.riskManagers[instrument].OnQuote(quote);*/
        }

		protected override void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
		{
			this.runtimeCrossExit.OnMarketDepth(instrument, marketDepth);
			this.runtimeCrossEntry.OnMarketDepth(instrument, marketDepth);
			this.marketManager.OnMarketDepth(instrument, marketDepth);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnMarketDepth(marketDepth);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnMarketDepth(marketDepth);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnMarketDepth(marketDepth);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnMarketDepth(marketDepth);
            /*---------------------------------------*/
            /*this.exits[instrument].OnMarketDepth(marketDepth);
			this.entries[instrument].OnMarketDepth(marketDepth);
			this.moneyManagers[instrument].OnMarketDepth(marketDepth);
			this.riskManagers[instrument].OnMarketDepth(marketDepth);*/
        }

		protected override void OnNewFundamental(Instrument instrument, Fundamental fundamental)
		{
			this.runtimeCrossExit.OnFundamental(instrument, fundamental);
			this.runtimeCrossEntry.OnFundamental(instrument, fundamental);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnFundamental(fundamental);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnFundamental(fundamental);
            /*---------------------------------------*/
            /*this.exits[instrument].OnFundamental(fundamental);
			this.entries[instrument].OnFundamental(fundamental);*/
        }

		protected override void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
		{
			this.runtimeCrossExit.OnCorporateAction(instrument, corporateAction);
			this.runtimeCrossEntry.OnCorporateAction(instrument, corporateAction);
			this.marketManager.OnCorporateAction(instrument, corporateAction);
            this.exposureManager.OnCorporateAction(instrument, corporateAction);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnCorporateAction(corporateAction);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnCorporateAction(corporateAction);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnCorporateAction(corporateAction);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnCorporateAction(corporateAction);
            /*---------------------------------------*/
            /*this.exits[instrument].OnCorporateAction(corporateAction);
			this.entries[instrument].OnCorporateAction(corporateAction);
			this.moneyManagers[instrument].OnCorporateAction(corporateAction);
			this.riskManagers[instrument].OnCorporateAction(corporateAction);*/
        }

		protected override void OnNewBarOpen(Instrument instrument, Bar bar)
		{
			foreach (Stop stop in new ArrayList(this.activeStops[instrument]))
			{
				if (stop.Connected)
				{
					stop.OnNewBarOpen(bar);
				}
			}
			this.runtimeCrossExit.OnBarOpen(instrument, bar);
			this.runtimeCrossEntry.OnBarOpen(instrument, bar);
			this.marketManager.OnBarOpen(instrument, bar);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnBarOpen(bar);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnBarOpen(bar);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnBarOpen(bar);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnBarOpen(bar);
            /*---------------------------------------*/
            /*this.exits[instrument].OnBarOpen(bar);
			this.entries[instrument].OnBarOpen(bar);
			this.moneyManagers[instrument].OnBarOpen(bar);
			this.riskManagers[instrument].OnBarOpen(bar);*/
        }

		protected override void OnNewBar(Instrument instrument, Bar bar)
		{
			foreach (Stop stop in new ArrayList(this.activeStops[instrument]))
			{
				if (stop.Connected)
				{
					stop.OnNewBar(bar);
				}
			}
			this.runtimeCrossExit.OnBar(instrument, bar);
			this.runtimeCrossEntry.OnBar(instrument, bar);
			this.marketManager.OnBar(instrument, bar);
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnBar(bar);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnBar(bar);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnBar(bar);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnBar(bar);
            /*---------------------------------------*/
            /*this.exits[instrument].OnBar(bar);
			this.entries[instrument].OnBar(bar);
			this.moneyManagers[instrument].OnBar(bar);
			this.riskManagers[instrument].OnBar(bar);*/
        }

		protected override void OnNewBarSlice(long barSize)
		{
			this.runtimeCrossExit.OnBarSlice(barSize);
			this.runtimeCrossEntry.OnBarSlice(barSize);
			this.marketManager.OnBarSlice(barSize);
		}

		protected override void OnProviderConnected(IProvider provider)
		{
			this.runtimeCrossExit.OnProviderConnected(provider);
			this.runtimeCrossEntry.OnProviderConnected(provider);
			this.marketManager.OnProviderConnected(provider);
		}

		protected override void OnProviderDisconnected(IProvider provider)
		{
			this.runtimeCrossExit.OnProviderDisconnected(provider);
			this.runtimeCrossEntry.OnProviderDisconnected(provider);
			this.marketManager.OnProviderDisconnected(provider);
		}

		protected override void OnProviderError(IProvider provider, int id, int code, string message)
		{
			this.runtimeCrossExit.OnProviderError(provider, id, code, message);
			this.runtimeCrossEntry.OnProviderError(provider, id, code, message);
			this.marketManager.OnProviderError(provider, id, code, message);
		}

		protected override void OnPositionOpened(Position position)
		{
			this.runtimeCrossExit.OnPositionOpened(position);
			this.runtimeCrossEntry.OnPositionOpened(position);
			this.exposureManager.OnPositionOpened(position);
			Instrument instrument = position.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnPositionOpened();
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnPositionOpened();
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnPositionOpened();
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnPositionOpened();
            /*---------------------------------------*/
            /*this.exits[instrument].OnPositionOpened();
			this.entries[instrument].OnPositionOpened();
			this.moneyManagers[instrument].OnPositionOpened();
			this.riskManagers[instrument].OnPositionOpened();*/
        }

		protected override void OnPositionChanged(Position position)
		{
			this.runtimeCrossExit.OnPositionChanged(position);
			this.runtimeCrossEntry.OnPositionChanged(position);
			this.exposureManager.OnPositionChanged(position);
			Instrument instrument = position.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnPositionChanged();
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnPositionChanged();
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnPositionChanged();
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnPositionChanged();
            /*---------------------------------------*/
            /*this.exits[instrument].OnPositionChanged();
			this.entries[instrument].OnPositionChanged();
			this.moneyManagers[instrument].OnPositionChanged();
			this.riskManagers[instrument].OnPositionChanged();*/
        }

		protected override void OnPositionClosed(Position position)
		{
			foreach (Stop stop in new ArrayList(this.activeStops[position.Instrument]))
			{
				if ((stop.Type == StopType.Time && stop.Status == StopStatus.Active) || stop.Connected)
				{
					stop.OnPositionClosed(position);
				}
			}
			this.runtimeCrossExit.OnPositionClosed(position);
			this.runtimeCrossEntry.OnPositionClosed(position);
			this.exposureManager.OnPositionClosed(position);
			Instrument instrument = position.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnPositionClosed();
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnPositionClosed();
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnPositionClosed();
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnPositionClosed();
            /*---------------------------------------*/
            /*this.exits[instrument].OnPositionClosed();
			this.entries[instrument].OnPositionClosed();
			this.moneyManagers[instrument].OnPositionClosed();
			this.riskManagers[instrument].OnPositionClosed();*/
        }

		protected override void OnPortfolioValueChanged(Position position)
		{
			this.MetaStrategy.MetaRiskManager.OnStrategyPortfolioValueChanged(this);
			if (!this.isActive)
			{
				return;
			}
			this.runtimeCrossExit.OnPortfolioValueChanged(position);
			this.runtimeCrossEntry.OnPortfolioValueChanged(position);
			this.exposureManager.OnPortfolioValueChanged(position);
			Instrument instrument = position.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnPositionValueChanged();
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnPositionValueChanged();
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnPositionValueChanged();
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnPositionValueChanged();
            /*---------------------------------------*/
            /*this.exits[instrument].OnPositionValueChanged();
			this.entries[instrument].OnPositionValueChanged();
			this.moneyManagers[instrument].OnPositionValueChanged();
			this.riskManagers[instrument].OnPositionValueChanged();*/
        }

		public override void ClosePosition(Instrument instrument, double price, ComponentType component, string text)
		{
			Position position = this.portfolio.Positions[instrument];
			if (position != null)
			{
				switch (position.Side)
				{
				case PositionSide.Long:
					this.EmitSignal(new Signal(Clock.Now, component, SignalType.Market, SignalSide.Sell, position.Qty, price, instrument, text));
					return;
				case PositionSide.Short:
					this.EmitSignal(new Signal(Clock.Now, component, SignalType.Market, SignalSide.BuyCover, position.Qty, price, instrument, text));
					break;
				default:
					return;
				}
			}
		}

		protected override void OnNewOrder(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnNewOrder(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnNewOrder(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnNewOrder(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnNewOrder(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnNewOrder(order);
			this.runtimeCrossExit.OnNewOrder(order);
			this.entries[instrument].OnNewOrder(order);
			this.exits[instrument].OnNewOrder(order);*/
		}

		protected override void OnExecutionReport(SingleOrder order, ExecutionReport report)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnExecutionReport(order, report);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnExecutionReport(order, report);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnExecutionReport(order, report);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnExecutionReport(order, report);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnExecutionReport(order, report);
			this.runtimeCrossExit.OnExecutionReport(order, report);
			this.entries[instrument].OnExecutionReport(order, report);
			this.exits[instrument].OnExecutionReport(order, report);*/
        }

		protected override void OnOrderPartiallyFilled(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderPartiallyFilled(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderPartiallyFilled(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderPartiallyFilled(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderPartiallyFilled(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderPartiallyFilled(order);
			this.runtimeCrossExit.OnOrderPartiallyFilled(order);
			this.entries[instrument].OnOrderPartiallyFilled(order);
			this.exits[instrument].OnOrderPartiallyFilled(order);*/
        }

		protected override void OnOrderStatusChanged(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderStatusChanged(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderStatusChanged(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderStatusChanged(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderStatusChanged(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderStatusChanged(order);
			this.runtimeCrossExit.OnOrderStatusChanged(order);
			this.entries[instrument].OnOrderStatusChanged(order);
			this.exits[instrument].OnOrderStatusChanged(order);*/
        }

		protected override void OnOrderFilled(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderFilled(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderFilled(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderFilled(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderFilled(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderFilled(order);
			this.runtimeCrossExit.OnOrderFilled(order);
			this.entries[instrument].OnOrderFilled(order);
			this.exits[instrument].OnOrderFilled(order);*/
        }

		protected override void OnOrderCancelled(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderCancelled(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderCancelled(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderCancelled(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderCancelled(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderCancelled(order);
			this.runtimeCrossExit.OnOrderCancelled(order);
			this.entries[instrument].OnOrderCancelled(order);
			this.exits[instrument].OnOrderCancelled(order);*/
        }

		protected override void OnOrderRejected(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderRejected(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderRejected(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderRejected(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderRejected(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderRejected(order);
			this.runtimeCrossExit.OnOrderRejected(order);
			this.entries[instrument].OnOrderRejected(order);
			this.exits[instrument].OnOrderRejected(order);*/
        }

		protected override void OnOrderDone(SingleOrder order)
		{
			Instrument instrument = order.Instrument;
            /*---------------------------------------*/
            Exit exit = this.exits[instrument];
            if (exit != null) exit.OnOrderDone(order);
            Entry entry = this.entries[instrument];
            if (entry != null) entry.OnOrderDone(order);
            MoneyManager moneyManager = this.moneyManagers[instrument];
            if (moneyManager != null) moneyManager.OnOrderDone(order);
            RiskManager riskManager = this.riskManagers[instrument];
            if (riskManager != null) riskManager.OnOrderDone(order);
            /*---------------------------------------*/
            /*this.runtimeCrossEntry.OnOrderDone(order);
			this.runtimeCrossExit.OnOrderDone(order);
			this.entries[instrument].OnOrderDone(order);
			this.exits[instrument].OnOrderDone(order);*/
        }

		public override IComponentBase GetComponent(ComponentType type)
		{
			if (type <= ComponentType.RiskManager)
			{
				if (type <= ComponentType.ExposureManager)
				{
					switch (type)
					{
					case ComponentType.Entry:
						return this.Entry;
					case (ComponentType)3:
						break;
					case ComponentType.Exit:
						return this.Exit;
					default:
						if (type == ComponentType.ExposureManager)
						{
							return this.ExposureManager;
						}
						break;
					}
				}
				else
				{
					if (type == ComponentType.MoneyManager)
					{
						return this.MoneyManager;
					}
					if (type == ComponentType.RiskManager)
					{
						return this.RiskManager;
					}
				}
			}
			else if (type <= ComponentType.ReportManager)
			{
				if (type == ComponentType.MarketManager)
				{
					return base.MarketManager;
				}
				if (type == ComponentType.ReportManager)
				{
					return base.ReportManager;
				}
			}
			else
			{
				if (type == ComponentType.CrossEntry)
				{
					return this.CrossEntry;
				}
				if (type == ComponentType.CrossExit)
				{
					return this.CrossExit;
				}
			}
			throw new InvalidOperationException("Invalid strategy1 component type");
		}

		public override void SetComponent(ComponentType type, IComponentBase component)
		{
			if (type <= ComponentType.RiskManager)
			{
				if (type <= ComponentType.ExposureManager)
				{
					switch (type)
					{
					case ComponentType.Entry:
						this.Entry = (component as Entry);
						return;
					case (ComponentType)3:
						break;
					case ComponentType.Exit:
						this.Exit = (component as Exit);
						return;
					default:
						if (type == ComponentType.ExposureManager)
						{
							this.ExposureManager = (component as ExposureManager);
							return;
						}
						break;
					}
				}
				else
				{
					if (type == ComponentType.MoneyManager)
					{
						this.MoneyManager = (component as MoneyManager);
						return;
					}
					if (type == ComponentType.RiskManager)
					{
						this.RiskManager = (component as RiskManager);
						return;
					}
				}
			}
			else if (type <= ComponentType.ReportManager)
			{
				if (type == ComponentType.MarketManager)
				{
					base.MarketManager = (component as MarketManager);
					return;
				}
				if (type == ComponentType.ReportManager)
				{
					base.ReportManager = (component as ReportManager);
					return;
				}
			}
			else
			{
				if (type == ComponentType.CrossEntry)
				{
					this.CrossEntry = (component as CrossEntry);
					return;
				}
				if (type == ComponentType.CrossExit)
				{
					this.CrossExit = (component as CrossExit);
					return;
				}
			}
			throw new InvalidOperationException("Invalid strategy1 component type");
		}
	}
}
