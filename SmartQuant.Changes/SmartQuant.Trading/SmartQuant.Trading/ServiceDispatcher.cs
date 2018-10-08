using SmartQuant.Services;
using System;
using System.Collections.Specialized;

namespace SmartQuant.Trading
{
	internal class ServiceDispatcher
	{
		private MetaStrategyBase metaStrategyBase;

		private ListDictionary services;

		public ServiceDispatcher(MetaStrategyBase metaStrategyBase)
		{
			this.metaStrategyBase = metaStrategyBase;
			this.services = new ListDictionary();
		}

		public void Init()
		{
			this.services.Clear();
		}

		public void Add(IService service)
		{
			if (!this.services.Contains(service))
			{
				this.services.Add(service, null);
			}
		}

		public void Online()
		{
			ServiceManager.ServiceStatusChanged += new ServiceEventHandler(this.ServiceManager_ServiceStatusChanged);
			ServiceManager.ServiceError += new ServiceErrorEventHandler(this.ServiceManager_ServiceError);
			foreach (IService service in this.services.Keys)
			{
				if (service.Status == ServiceStatus.Stopped)
				{
					service.Start();
				}
			}
		}

		public void Offline()
		{
			ServiceManager.ServiceStatusChanged -= new ServiceEventHandler(this.ServiceManager_ServiceStatusChanged);
			ServiceManager.ServiceError -= new ServiceErrorEventHandler(this.ServiceManager_ServiceError);
		}

		private void ServiceManager_ServiceStatusChanged(object sender, ServiceEventArgs args)
		{
			if (this.services.Contains(args.Service))
			{
				this.metaStrategyBase.SetServiceStatusChanged(args);
			}
		}

		private void ServiceManager_ServiceError(object sender, ServiceErrorEventArgs args)
		{
			if (this.services.Contains(args.Error.Service))
			{
				this.metaStrategyBase.SetServiceError(args);
			}
		}
	}
}
