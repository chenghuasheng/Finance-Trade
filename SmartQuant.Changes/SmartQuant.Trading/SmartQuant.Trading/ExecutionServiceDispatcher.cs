using SmartQuant.FIX;
using SmartQuant.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SmartQuant.Trading
{
	internal class ExecutionServiceDispatcher
	{
		private MetaStrategyBase metaStrategyBase;

		private ListDictionary services;

		private Dictionary<string, OrderServicePair> orderServicePairs;

		public ExecutionServiceDispatcher(MetaStrategyBase metaStrategyBase)
		{
			this.metaStrategyBase = metaStrategyBase;
			this.services = new ListDictionary();
			this.orderServicePairs = new Dictionary<string, OrderServicePair>();
		}

		public void Init()
		{
			this.services.Clear();
			this.orderServicePairs.Clear();
		}

		public void Add(IExecutionService service)
		{
			if (!this.services.Contains(service))
			{
				this.services.Add(service, null);
			}
		}

		public void Online()
		{
			ServiceManager.NewOrderSingle += new NewOrderSingleEventHandler(this.ServiceManager_NewOrderSingle);
		}

		public void Offline()
		{
			ServiceManager.NewOrderSingle -= new NewOrderSingleEventHandler(this.ServiceManager_NewOrderSingle);
		}

		private void ServiceManager_NewOrderSingle(object sender, NewOrderSingleEventArgs args)
		{
			if (this.services.Contains(args.Service))
			{
				this.orderServicePairs.Add(args.Order.ClOrdID, new OrderServicePair(args.Order, args.Service));
				this.metaStrategyBase.SetNewClientOrder(args);
			}
		}

		public void SendExecutionReport(FIXExecutionReport report)
		{
			OrderServicePair orderServicePair = this.orderServicePairs[report.ClOrdID];
			orderServicePair.Service.Send(report);
		}
	}
}
