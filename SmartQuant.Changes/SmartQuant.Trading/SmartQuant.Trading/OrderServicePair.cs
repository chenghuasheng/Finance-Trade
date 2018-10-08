using SmartQuant.FIX;
using SmartQuant.Services;
using System;

namespace SmartQuant.Trading
{
	internal class OrderServicePair
	{
		private FIXNewOrderSingle order;

		private IExecutionService service;

		public FIXNewOrderSingle Order
		{
			get
			{
				return this.order;
			}
		}

		public IExecutionService Service
		{
			get
			{
				return this.service;
			}
		}

		public OrderServicePair(FIXNewOrderSingle order, IExecutionService service)
		{
			this.order = order;
			this.service = service;
		}
	}
}
