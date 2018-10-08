using SmartQuant.Providers;
using System;
using System.Collections.Specialized;

namespace SmartQuant.Trading
{
	internal class ProviderDispatcher
	{
		private MetaStrategyBase metaStrategyBase;

		private ListDictionary providers;

		public ProviderDispatcher(MetaStrategyBase metaStrategyBase)
		{
			this.metaStrategyBase = metaStrategyBase;
			this.providers = new ListDictionary();
		}

		public void Init()
		{
			this.providers.Clear();
		}

		public void Add(IProvider provider)
		{
			if (!this.providers.Contains(provider))
			{
				this.providers.Add(provider, null);
			}
		}

		public void Online(int timeout)
		{
			ProviderManager.Connected += new ProviderEventHandler(this.ProviderManager_Connected);
			ProviderManager.Disconnected += new ProviderEventHandler(this.ProviderManager_Disconnected);
			ProviderManager.Error += new ProviderErrorEventHandler(this.ProviderManager_Error);
			foreach (IProvider provider in this.providers.Keys)
			{
				if (!provider.IsConnected)
				{
					provider.Connect(timeout);
				}
			}
		}

		public void Offline()
		{
			ProviderManager.Connected -= new ProviderEventHandler(this.ProviderManager_Connected);
			ProviderManager.Disconnected -= new ProviderEventHandler(this.ProviderManager_Disconnected);
			ProviderManager.Error -= new ProviderErrorEventHandler(this.ProviderManager_Error);
		}

		private void ProviderManager_Connected(ProviderEventArgs args)
		{
			if (this.providers.Contains(args.Provider))
			{
				this.metaStrategyBase.SetProviderConnected(args);
			}
		}

		private void ProviderManager_Disconnected(ProviderEventArgs args)
		{
			if (this.providers.Contains(args.Provider))
			{
				this.metaStrategyBase.SetProviderDisconnected(args);
			}
		}

		private void ProviderManager_Error(ProviderErrorEventArgs args)
		{
			if (this.providers.Contains(args.Error.Provider))
			{
				this.metaStrategyBase.SetProviderError(args);
			}
		}
	}
}
