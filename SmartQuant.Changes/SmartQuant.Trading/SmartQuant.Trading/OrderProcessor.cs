using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Providers;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading
{
	internal class OrderProcessor
	{
		private MetaStrategyBase metaStrategyBase;

		private List<IExecutionProvider> providers;

		public OrderProcessor(MetaStrategyBase metaStrategyBase)
		{
			this.metaStrategyBase = metaStrategyBase;
			this.providers = new List<IExecutionProvider>();
		}

		public void Init()
		{
			this.providers.Clear();
		}

		public void Add(IExecutionProvider provider)
		{
			if (!this.providers.Contains(provider))
			{
				this.providers.Add(provider);
			}
		}

		public void Online()
		{
			OrderManager.NewOrder += new OrderEventHandler(this.OrderManager_NewOrder);
			OrderManager.ExecutionReport += new ExecutionReportEventHandler(this.OrderManager_ExecutionReport);
			OrderManager.OrderStatusChanged += new OrderEventHandler(this.OrderManager_OrderStatusChanged);
			OrderManager.OrderDone += new OrderEventHandler(this.OrderManager_OrderDone);
		}

		public void Offline()
		{
			OrderManager.NewOrder -= new OrderEventHandler(this.OrderManager_NewOrder);
			OrderManager.ExecutionReport -= new ExecutionReportEventHandler(this.OrderManager_ExecutionReport);
			OrderManager.OrderStatusChanged -= new OrderEventHandler(this.OrderManager_OrderStatusChanged);
			OrderManager.OrderDone -= new OrderEventHandler(this.OrderManager_OrderDone);
		}

		private void OrderManager_NewOrder(OrderEventArgs args)
		{
			this.metaStrategyBase.SetNewOrder(args);
		}

		private void OrderManager_ExecutionReport(object sender, ExecutionReportEventArgs args)
		{
			this.metaStrategyBase.SetExecutionReport(args);
		}

		private void OrderManager_OrderStatusChanged(OrderEventArgs args)
		{
			this.metaStrategyBase.SetOrderStatusChanged(args);
		}

		private void OrderManager_OrderDone(OrderEventArgs args)
		{
			this.metaStrategyBase.SetOrderDone(args);
		}
	}
}
