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
	public class ATSStrategy : StrategyBase
	{
		private ATSComponent atsComponent;

		private ATSCrossComponent atsCrossComponent;

		private ATSCrossComponent atsRuntimeCrossComponent;

		private Dictionary<Instrument, ATSComponent> atsComponents;

		[Browsable(false)]
		public ATSMetaStrategy ATSMetaStrategy
		{
			get
			{
				return base.MetaStrategyBase as ATSMetaStrategy;
			}
		}

		[Category("Components"), Editor(typeof(ATSComponentTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ATSComponent ATSComponent
		{
			get
			{
				return this.atsComponent;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.atsComponent != null)
				{
					this.atsComponent.Disconnect();
					this.atsComponent.StrategyBase = null;
				}
				this.atsComponent = value;
				if (this.atsComponent != null)
				{
					this.atsComponent.StrategyBase = this;
					this.atsComponent.Connect();
				}
				base.EmitComponentChanged(ComponentType.ATSComponent);
			}
		}

		[Category("Components"), Editor(typeof(ATSCrossComponentTypeEditor), typeof(UITypeEditor)), TypeConverter(typeof(ComponentTypeConverter))]
		public ATSCrossComponent ATSCrossComponent
		{
			get
			{
				if (this.metaStrategyBase == null || !this.metaStrategyBase.IsRunning)
				{
					return this.atsCrossComponent;
				}
				return this.atsRuntimeCrossComponent;
			}
			set
			{
				if (this.metaStrategyBase != null && (this.metaStrategyBase.IsRunning || !this.metaStrategyBase.DesignMode))
				{
					throw new InvalidOperationException("MetaStrategy is not in design mode");
				}
				if (this.atsCrossComponent != null)
				{
					this.atsCrossComponent.Disconnect();
					this.atsCrossComponent.StrategyBase = null;
				}
				this.atsCrossComponent = value;
				if (this.atsCrossComponent != null)
				{
					this.atsCrossComponent.StrategyBase = this;
					this.atsCrossComponent.Connect();
				}
				base.EmitComponentChanged(ComponentType.ATSCrossComponent);
			}
		}

		public ATSStrategy(string name, string description) : base(name, description)
		{
			this.ATSCrossComponent = (StrategyComponentManager.GetComponent("{E70A6417-E7FA-4ec1-BC16-B03DE53C6E85}", this) as ATSCrossComponent);
			this.ATSComponent = (StrategyComponentManager.GetComponent("{AC3C53E2-6C94-4718-A5D8-8A475D8B4EB7}", this) as ATSComponent);
			this.atsComponents = new Dictionary<Instrument, ATSComponent>();
			this.componentTypeList.Add(ComponentType.ATSComponent);
			this.componentTypeList.Add(ComponentType.ATSCrossComponent);
		}
        /*------------------自已添加----------------------------*/
        protected override void OnInit()
        {
            this.atsRuntimeCrossComponent = (Activator.CreateInstance(this.atsCrossComponent.GetType()) as ATSCrossComponent);
            this.atsRuntimeCrossComponent.StrategyBase = this;
            base.SetProxyProperties(this.atsRuntimeCrossComponent, this.atsCrossComponent);
        }
        protected override void OnCrossBehaviorInit()
        {
            this.atsRuntimeCrossComponent.Init();
        }
        protected override void OnBehaviorInit()
        {
            this.atsComponents.Clear();
            //把持仓中的证券添加到市场中来
            foreach (Position position in this.portfolio.Positions)
            {
                Instrument instrument = position.Instrument;
                this.marketManager.AddInstrument(instrument);
            }
            foreach (Instrument instrument in this.marketManager.Instruments)
            {
                this.activeInstruments.Add(instrument);
                ATSComponent aTSComponent = Activator.CreateInstance(this.atsComponent.GetType()) as ATSComponent;
                aTSComponent.StrategyBase = this;
                aTSComponent.Instrument = instrument;
                base.SetProxyProperties(aTSComponent, this.atsComponent);
                aTSComponent.Init();
                this.atsComponents.Add(instrument, aTSComponent);
            }
        }  
        /*------------------------------------------------------*/
		protected override void OnStrategyStop()
		{
			this.atsRuntimeCrossComponent.OnStrategyStop();
			foreach (ATSComponent current in this.atsComponents.Values)
			{
				current.OnStrategyStop();
			}
		}

		internal void AddStop(ATSStop stop)
		{
			this.stops.Add(stop);
			this.activeStops[stop.Instrument].Add(stop);
			stop.StatusChanged += new StopEventHandler(this.stop_StatusChanged);
			base.EmitStopAdded(stop);
		}

		private void stop_StatusChanged(StopEventArgs args)
		{
			ATSStop aTSStop = args.Stop as ATSStop;
			if (aTSStop.Status != StopStatus.Active)
			{
				this.activeStops[aTSStop.Instrument].Remove(aTSStop);
			}
			this.OnStopStatusChanged(aTSStop);
			if (aTSStop.Status == StopStatus.Executed)
			{
				this.OnStopExecuted(aTSStop);
			}
			if (aTSStop.Status == StopStatus.Canceled)
			{
				this.OnStopCanceled(aTSStop);
			}
			base.EmitStopStatusChanged(aTSStop);
		}

		internal void OnStopStatusChanged(ATSStop stop)
		{
			this.atsRuntimeCrossComponent.OnStopStatusChanged(stop);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[stop.Instrument];
            if (aTSComponent != null) aTSComponent.OnStopStatusChanged(stop);
            /*---------------------------------------------------*/
            //this.atsComponents[stop.Instrument].OnStopStatusChanged(stop);
        }

        internal void OnStopCanceled(ATSStop stop)
		{
			this.atsRuntimeCrossComponent.OnStopCanceled(stop);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[stop.Instrument];
            if (aTSComponent != null) aTSComponent.OnStopCanceled(stop);
            /*---------------------------------------------------*/
            //this.atsComponents[stop.Instrument].OnStopCanceled(stop);
        }

        internal void OnStopExecuted(ATSStop stop)
		{
			this.atsRuntimeCrossComponent.OnStopExecuted(stop);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[stop.Instrument];
            if (aTSComponent != null) aTSComponent.OnStopExecuted(stop);
            /*---------------------------------------------------*/
            //this.atsComponents[stop.Instrument].OnStopExecuted(stop);
        }

        protected override void OnNewTrade(Instrument instrument, Trade trade)
		{
			foreach (ATSStop aTSStop in new ArrayList(this.activeStops[instrument]))
			{
				if (aTSStop.Connected)
				{
					aTSStop.OnNewTrade(trade);
				}
			}
			this.marketManager.OnTrade(instrument, trade);
			this.atsRuntimeCrossComponent.OnTrade(instrument, trade);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnTrade(trade);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnTrade(trade);
        }

        protected override void OnNewQuote(Instrument instrument, Quote quote)
		{
			foreach (ATSStop aTSStop in new ArrayList(this.activeStops[instrument]))
			{
				if (aTSStop.Connected)
				{
					aTSStop.OnNewQuote(quote);
				}
			}
			this.marketManager.OnQuote(instrument, quote);
			this.atsRuntimeCrossComponent.OnQuote(instrument, quote);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnQuote(quote);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnQuote(quote);
        }

        protected override void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
		{
			this.marketManager.OnMarketDepth(instrument, marketDepth);
			this.atsRuntimeCrossComponent.OnMarketDepth(instrument, marketDepth);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnMarketDepth(marketDepth);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnMarketDepth(marketDepth);
		}

		protected override void OnNewFundamental(Instrument instrument, Fundamental fundamental)
		{
			this.marketManager.OnFundamental(instrument, fundamental);
			this.atsRuntimeCrossComponent.OnFundamental(instrument, fundamental);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnFundamental(fundamental);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnFundamental(fundamental);
        }

		protected override void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
		{
			this.marketManager.OnCorporateAction(instrument, corporateAction);
			this.atsRuntimeCrossComponent.OnCorporateAction(instrument, corporateAction);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnCorporateAction(corporateAction);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnCorporateAction(corporateAction);
        }

		protected override void OnNewBarOpen(Instrument instrument, Bar bar)
		{
			foreach (ATSStop aTSStop in new ArrayList(this.activeStops[instrument]))
			{
				if (aTSStop.Connected)
				{
					aTSStop.OnNewBarOpen(bar);
				}
			}
			this.marketManager.OnBarOpen(instrument, bar);
			this.atsRuntimeCrossComponent.OnBarOpen(instrument, bar);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnBarOpen(bar);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnBarOpen(bar);
        }

		protected override void OnNewBar(Instrument instrument, Bar bar)
		{
			foreach (ATSStop aTSStop in new ArrayList(this.activeStops[instrument]))
			{
				if (aTSStop.Connected)
				{
					aTSStop.OnNewBar(bar);
				}
			}
			this.marketManager.OnBar(instrument, bar);
			this.atsRuntimeCrossComponent.OnBar(instrument, bar);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[instrument];
            if (aTSComponent != null) aTSComponent.OnBar(bar);
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnBar(bar);
        }

        protected override void OnNewBarSlice(long barSize)
		{
			this.marketManager.OnBarSlice(barSize);
			this.atsRuntimeCrossComponent.OnBarSlice(barSize);
		}

		protected override void OnProviderConnected(IProvider provider)
		{
			this.marketManager.OnProviderConnected(provider);
			this.atsRuntimeCrossComponent.OnProviderConnected(provider);
		}

		protected override void OnProviderDisconnected(IProvider provider)
		{
			this.marketManager.OnProviderDisconnected(provider);
			this.atsRuntimeCrossComponent.OnProviderDisconnected(provider);
		}

		protected override void OnProviderError(IProvider provider, int id, int code, string message)
		{
			this.marketManager.OnProviderError(provider, id, code, message);
			this.atsRuntimeCrossComponent.OnProviderError(provider, id, code, message);
		}

		protected override void OnPositionOpened(Position position)
		{
			this.atsRuntimeCrossComponent.OnPositionOpened(position);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[position.Instrument];
            if (aTSComponent != null) aTSComponent.OnPositionOpened();
            /*---------------------------------------------------*/
            //this.atsComponents[position.Instrument].OnPositionOpened();
		}

		protected override void OnPositionChanged(Position position)
		{
			this.atsRuntimeCrossComponent.OnPositionChanged(position);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[position.Instrument];
            if (aTSComponent != null) aTSComponent.OnPositionChanged();
            /*---------------------------------------------------*/
            //this.atsComponents[position.Instrument].OnPositionChanged();
        }

        protected override void OnPositionClosed(Position position)
		{
			Instrument instrument = position.Instrument;
			foreach (ATSStop aTSStop in new ArrayList(this.activeStops[instrument]))
			{
				if ((aTSStop.Type == StopType.Time && aTSStop.Status == StopStatus.Active) || aTSStop.Connected)
				{
					aTSStop.OnPositionClosed(position);
				}
			}
			this.atsRuntimeCrossComponent.OnPositionClosed(position);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[position.Instrument];
            if (aTSComponent != null) aTSComponent.OnPositionClosed();
            /*---------------------------------------------------*/
            //this.atsComponents[instrument].OnPositionClosed();
        }

        protected override void OnPortfolioValueChanged(Position position)
		{
			this.atsRuntimeCrossComponent.OnPositionValueChanged(position);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[position.Instrument];
            if (aTSComponent != null) aTSComponent.OnPositionValueChanged();
            /*---------------------------------------------------*/
            //this.atsComponents[position.Instrument].OnPositionValueChanged();
        }

        internal void RegisterOrder(SingleOrder order)
		{
			order.Strategy = base.Name;
			order.StrategyComponent = ComponentType.ATSComponent.ToString();
			base.MetaStrategyBase.RegisterOrder(this, order);
		}

		protected override void OnNewOrder(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnNewOrder(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnNewOrder(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnNewOrder(order);
		}

		protected override void OnExecutionReport(SingleOrder order, ExecutionReport report)
		{
			this.atsRuntimeCrossComponent.OnExecutionReport(order, report);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnExecutionReport(order, report);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnExecutionReport(order, report);
        }

        protected override void OnOrderPartiallyFilled(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderPartiallyFilled(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderPartiallyFilled(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderPartiallyFilled(order);
        }

		protected override void OnOrderStatusChanged(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderStatusChanged(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderStatusChanged(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderStatusChanged(order);
        }

		protected override void OnOrderFilled(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderFilled(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderFilled(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderFilled(order);
		}

		protected override void OnOrderCancelled(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderCancelled(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderCancelled(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderCancelled(order);
		}

		protected override void OnOrderRejected(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderRejected(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderRejected(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderRejected(order);
		}

		protected override void OnOrderDone(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.OnOrderDone(order);
            /*---------------------------------------------------*/
            ATSComponent aTSComponent = this.atsComponents[order.Instrument];
            if (aTSComponent != null) aTSComponent.OnOrderDone(order);
            /*---------------------------------------------------*/
            //this.atsComponents[order.Instrument].OnOrderDone(order);
        }

		protected override void OnNewClientOrder(SingleOrder order)
		{
			this.atsRuntimeCrossComponent.SetClientOrder(order);
		}

		public override void ClosePosition(Instrument instrument, double price, ComponentType component, string text)
		{
            Position position = this.portfolio.Positions[instrument];
            if (position != null)
            {
                MarketOrder marketOrder;
                switch (position.Side)
                {
                    case PositionSide.Long:
                        marketOrder = new MarketOrder(instrument, Side.Sell, position.Qty, text);
                        this.RegisterOrder(marketOrder);
                        marketOrder.Send();
                        return;
                    case PositionSide.Short:
                        marketOrder = new MarketOrder(instrument, Side.Buy, position.Qty, text);
                        this.RegisterOrder(marketOrder);
                        marketOrder.Send();
                        return;
                    default:
                        return;
                }
            }
        }

		public override IComponentBase GetComponent(ComponentType type)
		{
			if (type <= ComponentType.ReportManager)
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
				if (type == ComponentType.ATSComponent)
				{
					return this.ATSComponent;
				}
				if (type == ComponentType.ATSCrossComponent)
				{
					return this.ATSCrossComponent;
				}
			}
			throw new InvalidOperationException("Invalid strategy1 component type");
		}

		public override void SetComponent(ComponentType type, IComponentBase component)
		{
			if (type <= ComponentType.ReportManager)
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
				if (type == ComponentType.ATSComponent)
				{
					this.ATSComponent = (component as ATSComponent);
					return;
				}
				if (type == ComponentType.ATSCrossComponent)
				{
					this.ATSCrossComponent = (component as ATSCrossComponent);
					return;
				}
			}
			throw new InvalidOperationException("Invalid strategy1 component type");
		}
	}
}
