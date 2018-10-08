using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading
{
	internal class MarketDataDispatcher
	{
		private MetaStrategyBase metaStrategyBase;

		private Dictionary<IMarketDataProvider, Dictionary<Instrument, List<string>>> requests;

		public MarketDataDispatcher(MetaStrategyBase metaStrategyBase)
		{
			this.metaStrategyBase = metaStrategyBase;
			this.requests = new Dictionary<IMarketDataProvider, Dictionary<Instrument, List<string>>>();
		}

		public void Init()
		{
			this.requests.Clear();
		}

		public void Add(IMarketDataProvider provider, Instrument instrument, string seriesSuffix)
		{
			Dictionary<Instrument, List<string>> dictionary = null;
			if (!this.requests.TryGetValue(provider, out dictionary))
			{
				dictionary = new Dictionary<Instrument, List<string>>();
				this.requests.Add(provider, dictionary);
			}
			List<string> list = null;
			if (!dictionary.TryGetValue(instrument, out list))
			{
				list = new List<string>();
				dictionary.Add(instrument, list);
			}
			if (!list.Contains(seriesSuffix))
			{
				list.Add(seriesSuffix);
			}
		}

		public void Online()
		{
			foreach (KeyValuePair<IMarketDataProvider, Dictionary<Instrument, List<string>>> current in this.requests)
			{
				IMarketDataProvider key = current.Key;
				foreach (KeyValuePair<Instrument, List<string>> current2 in current.Value)
				{
					Instrument key2 = current2.Key;
					foreach (string current3 in current2.Value)
					{
						if (current3 == null)
						{
							key2.RequestMarketData(key, (MarketDataType)3);
						}
						else
						{
							key2.RequestMarketData(key, (MarketDataType)3, current3);
						}
					}
				}
			}
			ProviderManager.NewBar += new BarEventHandler(this.OnNewBar);
			ProviderManager.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
			ProviderManager.NewTrade += new TradeEventHandler(this.OnNewTrade);
			ProviderManager.NewQuote += new QuoteEventHandler(this.OnNewQuote);
			ProviderManager.NewMarketDepth += new MarketDepthEventHandler(this.OnNewMarketDepth);
			ProviderManager.NewFundamental += new FundamentalEventHandler(this.OnNewFundamental);
			ProviderManager.NewCorporateAction += new CorporateActionEventHandler(this.OnNewCorporateAction);
			ProviderManager.NewBarSlice += new BarSliceEventHandler(this.OnNewBarSlice);
		}

		public void Offline()
		{
			ProviderManager.NewBar -= new BarEventHandler(this.OnNewBar);
			ProviderManager.NewBarOpen -= new BarEventHandler(this.OnNewBarOpen);
			ProviderManager.NewTrade -= new TradeEventHandler(this.OnNewTrade);
			ProviderManager.NewQuote -= new QuoteEventHandler(this.OnNewQuote);
			ProviderManager.NewMarketDepth -= new MarketDepthEventHandler(this.OnNewMarketDepth);
			ProviderManager.NewFundamental -= new FundamentalEventHandler(this.OnNewFundamental);
			ProviderManager.NewCorporateAction -= new CorporateActionEventHandler(this.OnNewCorporateAction);
			ProviderManager.NewBarSlice -= new BarSliceEventHandler(this.OnNewBarSlice);
			foreach (KeyValuePair<IMarketDataProvider, Dictionary<Instrument, List<string>>> current in this.requests)
			{
				IMarketDataProvider key = current.Key;
				if (key.BarFactory != null)
				{
					key.BarFactory.Reset();
				}
				foreach (KeyValuePair<Instrument, List<string>> current2 in current.Value)
				{
					Instrument key2 = current2.Key;
					foreach (string current3 in current2.Value)
					{
						if (current3 == null)
						{
							key2.CancelMarketData(key, (MarketDataType)3);
						}
						else
						{
							key2.CancelMarketData(key, (MarketDataType)3, current3);
						}
					}
				}
			}
		}

		private void OnNewBarOpen(object sender, BarEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewBarOpen(args);
			}
		}

		private void OnNewBar(object sender, BarEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewBar(args);
			}
		}

		private void OnNewBarSlice(object sender, BarSliceEventArgs args)
		{
			if (this.HandleMarketData(args.Provider))
			{
				this.metaStrategyBase.SetNewBarSlice(args);
			}
		}

		private void OnNewTrade(object sender, TradeEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewTrade(args);
			}
		}

		private void OnNewQuote(object sender, QuoteEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewQuote(args);
			}
		}

		private void OnNewMarketDepth(object sender, MarketDepthEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewMarketDepth(args);
			}
		}

		private void OnNewFundamental(object sender, FundamentalEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewFundamental(args);
			}
		}

		private void OnNewCorporateAction(object sender, CorporateActionEventArgs args)
		{
			if (this.HandleMarketData(args.Provider, args.Instrument))
			{
				this.metaStrategyBase.SetNewCorporateAction(args);
			}
		}

		private bool HandleMarketData(IMarketDataProvider provider, IFIXInstrument instrument)
		{
			Dictionary<Instrument, List<string>> dictionary = null;
			return this.requests.TryGetValue(provider, out dictionary) && dictionary.ContainsKey(instrument as Instrument);
		}

		private bool HandleMarketData(IMarketDataProvider provider)
		{
			return this.requests.ContainsKey(provider);
		}
	}
}
