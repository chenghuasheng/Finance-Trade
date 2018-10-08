using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{AC3C53E2-6C94-4718-A5D8-8A475D8B4EB7}", ComponentType.ATSComponent, Name = "Default_ATSComponent", Description = "")]
	public class ATSComponent : ATSStrategySingleComponent
	{
		public const string GUID = "{AC3C53E2-6C94-4718-A5D8-8A475D8B4EB7}";

		public virtual void OnStopStatusChanged(ATSStop stop)
		{
		}

		public virtual void OnStopCanceled(ATSStop stop)
		{
		}

		public virtual void OnStopExecuted(ATSStop stop)
		{
		}

		public ATSStop SetStop(Position position, double level, StopType type, StopMode mode)
		{
			ATSStop aTSStop = new ATSStop(position, level, type, mode);
			base.Strategy.AddStop(aTSStop);
			return aTSStop;
		}

		public ATSStop SetStop(double level, StopType type, StopMode mode)
		{
			return this.SetStop(base.Position, level, type, mode);
		}

		public ATSStop SetStop(Position position, DateTime dateTime)
		{
			ATSStop aTSStop = new ATSStop(position, dateTime);
			base.Strategy.AddStop(aTSStop);
			return aTSStop;
		}

		public ATSStop SetStop(DateTime dateTime)
		{
			return this.SetStop(base.Position, dateTime);
		}

		public MarketOrder MarketOrder(Instrument instrument, Side side, double qty, string text)
		{
			MarketOrder marketOrder = new MarketOrder(instrument, side, qty, text);
			base.Strategy.RegisterOrder(marketOrder);
			return marketOrder;
		}

		public MarketOrder SendMarketOrder(Instrument instrument, Side side, double qty, string text)
		{
			MarketOrder marketOrder = new MarketOrder(instrument, side, qty, text);
			base.Strategy.RegisterOrder(marketOrder);
			marketOrder.Send();
			return marketOrder;
		}

		public MarketOrder MarketOrder(Instrument instrument, Side side, double qty)
		{
			MarketOrder marketOrder = new MarketOrder(instrument, side, qty);
			base.Strategy.RegisterOrder(marketOrder);
			return marketOrder;
		}

		public MarketOrder SendMarketOrder(Instrument instrument, Side side, double qty)
		{
			MarketOrder marketOrder = new MarketOrder(instrument, side, qty);
			base.Strategy.RegisterOrder(marketOrder);
			marketOrder.Send();
			return marketOrder;
		}

		public MarketOrder MarketOrder(Side side, double qty, string text)
		{
			return this.MarketOrder(base.Instrument, side, qty, text);
		}

		public MarketOrder SendMarketOrder(Side side, double qty, string text)
		{
			return this.SendMarketOrder(base.Instrument, side, qty, text);
		}

		public MarketOrder MarketOrder(Side side, double qty)
		{
			return this.MarketOrder(base.Instrument, side, qty);
		}

		public MarketOrder SendMarketOrder(Side side, double qty)
		{
			return this.SendMarketOrder(base.Instrument, side, qty);
		}

		public LimitOrder LimitOrder(Instrument instrument, Side side, double qty, double price, string text)
		{
			LimitOrder limitOrder = new LimitOrder(instrument, side, qty, price, text);
			base.Strategy.RegisterOrder(limitOrder);
			return limitOrder;
		}

		public LimitOrder SendLimitOrder(Instrument instrument, Side side, double qty, double price, string text)
		{
			LimitOrder limitOrder = new LimitOrder(instrument, side, qty, price, text);
			base.Strategy.RegisterOrder(limitOrder);
			limitOrder.Send();
			return limitOrder;
		}

		public LimitOrder LimitOrder(Instrument instrument, Side side, double qty, double price)
		{
			LimitOrder limitOrder = new LimitOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(limitOrder);
			return limitOrder;
		}

		public LimitOrder SendLimitOrder(Instrument instrument, Side side, double qty, double price)
		{
			LimitOrder limitOrder = new LimitOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(limitOrder);
			limitOrder.Send();
			return limitOrder;
		}

		public LimitOrder LimitOrder(Side side, double qty, double price, string text)
		{
			return this.LimitOrder(base.Instrument, side, qty, price, text);
		}

		public LimitOrder SendLimitOrder(Side side, double qty, double price, string text)
		{
			return this.SendLimitOrder(base.Instrument, side, qty, price, text);
		}

		public LimitOrder LimitOrder(Side side, double qty, double price)
		{
			return this.LimitOrder(base.Instrument, side, qty, price);
		}

		public LimitOrder SendLimitOrder(Side side, double qty, double price)
		{
			return this.SendLimitOrder(base.Instrument, side, qty, price);
		}

		public StopOrder StopOrder(Instrument instrument, Side side, double qty, double price, string text)
		{
			StopOrder stopOrder = new StopOrder(instrument, side, qty, price, text);
			base.Strategy.RegisterOrder(stopOrder);
			return stopOrder;
		}

		public StopOrder SendStopOrder(Instrument instrument, Side side, double qty, double price, string text)
		{
			StopOrder stopOrder = new StopOrder(instrument, side, qty, price, text);
			base.Strategy.RegisterOrder(stopOrder);
			stopOrder.Send();
			return stopOrder;
		}

		public StopOrder StopOrder(Instrument instrument, Side side, double qty, double price)
		{
			StopOrder stopOrder = new StopOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(stopOrder);
			return stopOrder;
		}

		public StopOrder SendStopOrder(Instrument instrument, Side side, double qty, double price)
		{
			StopOrder stopOrder = new StopOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(stopOrder);
			stopOrder.Send();
			return stopOrder;
		}

		public StopOrder StopOrder(Side side, double qty, double price, string text)
		{
			return this.StopOrder(base.Instrument, side, qty, price, text);
		}

		public StopOrder SendStopOrder(Side side, double qty, double price, string text)
		{
			return this.SendStopOrder(base.Instrument, side, qty, price, text);
		}

		public StopOrder StopOrder(Side side, double qty, double price)
		{
			return this.StopOrder(base.Instrument, side, qty, price);
		}

		public StopOrder SendStopOrder(Side side, double qty, double price)
		{
			return this.SendStopOrder(base.Instrument, side, qty, price);
		}

		public StopLimitOrder StopLimitOrder(Instrument instrument, Side side, double qty, double limitPrice, double stopPrice, string text)
		{
			StopLimitOrder stopLimitOrder = new StopLimitOrder(instrument, side, qty, limitPrice, stopPrice, text);
			base.Strategy.RegisterOrder(stopLimitOrder);
			return stopLimitOrder;
		}

		public StopLimitOrder SendStopLimitOrder(Instrument instrument, Side side, double qty, double limitPrice, double stopPrice, string text)
		{
			StopLimitOrder stopLimitOrder = new StopLimitOrder(instrument, side, qty, limitPrice, stopPrice, text);
			base.Strategy.RegisterOrder(stopLimitOrder);
			stopLimitOrder.Send();
			return stopLimitOrder;
		}

		public StopLimitOrder StopLimitOrder(Instrument instrument, Side side, double qty, double limitPrice, double stopPrice)
		{
			StopLimitOrder stopLimitOrder = new StopLimitOrder(instrument, side, qty, limitPrice, stopPrice);
			base.Strategy.RegisterOrder(stopLimitOrder);
			return stopLimitOrder;
		}

		public StopLimitOrder SendStopLimitOrder(Instrument instrument, Side side, double qty, double limitPrice, double stopPrice)
		{
			StopLimitOrder stopLimitOrder = new StopLimitOrder(instrument, side, qty, limitPrice, stopPrice);
			base.Strategy.RegisterOrder(stopLimitOrder);
			stopLimitOrder.Send();
			return stopLimitOrder;
		}

		public StopLimitOrder StopLimitOrder(Side side, double qty, double limitPrice, double stopPrice, string text)
		{
			return this.StopLimitOrder(base.Instrument, side, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder SendStopLimitOrder(Side side, double qty, double limitPrice, double stopPrice, string text)
		{
			return this.SendStopLimitOrder(base.Instrument, side, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder StopLimitOrder(Side side, double qty, double limitPrice, double stopPrice)
		{
			return this.StopLimitOrder(base.Instrument, side, qty, limitPrice, stopPrice);
		}

		public StopLimitOrder SendStopLimitOrder(Side side, double qty, double limitPrice, double stopPrice)
		{
			return this.SendStopLimitOrder(base.Instrument, side, qty, limitPrice, stopPrice);
		}

		public TrailingStopOrder TrailingStopOrder(Side side, double qty, double delta)
		{
			TrailingStopOrder trailingStopOrder = new TrailingStopOrder(this.instrument, side, qty, delta);
			base.Strategy.RegisterOrder(trailingStopOrder);
			return trailingStopOrder;
		}

		public MarketOrder Buy(Instrument instrument, double Qty, string text)
		{
			return this.SendMarketOrder(instrument, Side.Buy, Qty, text);
		}

		public MarketOrder Buy(Instrument instrument, double Qty)
		{
			return this.SendMarketOrder(instrument, Side.Buy, Qty);
		}

		public MarketOrder Buy(double Qty, string text)
		{
			return this.SendMarketOrder(Side.Buy, Qty, text);
		}

		public MarketOrder Buy(double Qty)
		{
			return this.SendMarketOrder(Side.Buy, Qty);
		}

		public MarketOrder Sell(Instrument instrument, double Qty, string text)
		{
			return this.SendMarketOrder(instrument, Side.Sell, Qty, text);
		}

		public MarketOrder Sell(Instrument instrument, double Qty)
		{
			return this.SendMarketOrder(instrument, Side.Sell, Qty);
		}

		public MarketOrder Sell(double Qty, string text)
		{
			return this.SendMarketOrder(Side.Sell, Qty, text);
		}

		public MarketOrder Sell(double Qty)
		{
			return this.SendMarketOrder(Side.Sell, Qty);
		}

		public LimitOrder BuyLimit(Instrument instrument, double qty, double price, string text)
		{
			return this.SendLimitOrder(instrument, Side.Buy, qty, price, text);
		}

		public LimitOrder BuyLimit(Instrument instrument, double qty, double price)
		{
			return this.SendLimitOrder(instrument, Side.Buy, qty, price);
		}

		public LimitOrder BuyLimit(double qty, double price, string text)
		{
			return this.SendLimitOrder(Side.Buy, qty, price, text);
		}

		public LimitOrder BuyLimit(double qty, double price)
		{
			return this.SendLimitOrder(Side.Buy, qty, price);
		}

		public LimitOrder SellLimit(Instrument instrument, double qty, double price, string text)
		{
			return this.SendLimitOrder(instrument, Side.Sell, qty, price, text);
		}

		public LimitOrder SellLimit(Instrument instrument, double qty, double price)
		{
			return this.SendLimitOrder(instrument, Side.Sell, qty, price);
		}

		public LimitOrder SellLimit(double qty, double price, string text)
		{
			return this.SendLimitOrder(Side.Sell, qty, price, text);
		}

		public LimitOrder SellLimit(double qty, double price)
		{
			return this.SendLimitOrder(Side.Sell, qty, price);
		}

		public StopOrder BuyStop(Instrument instrument, double qty, double price, string text)
		{
			return this.SendStopOrder(instrument, Side.Buy, qty, price, text);
		}

		public StopOrder BuyStop(Instrument instrument, double qty, double price)
		{
			return this.SendStopOrder(instrument, Side.Buy, qty, price);
		}

		public StopOrder BuyStop(double qty, double price, string text)
		{
			return this.SendStopOrder(Side.Buy, qty, price, text);
		}

		public StopOrder BuyStop(double qty, double price)
		{
			return this.SendStopOrder(Side.Buy, qty, price);
		}

		public StopOrder SellStop(Instrument instrument, double qty, double price, string text)
		{
			return this.SendStopOrder(instrument, Side.Sell, qty, price, text);
		}

		public StopOrder SellStop(Instrument instrument, double qty, double price)
		{
			return this.SendStopOrder(instrument, Side.Sell, qty, price);
		}

		public StopOrder SellStop(double qty, double price, string text)
		{
			return this.SendStopOrder(Side.Sell, qty, price, text);
		}

		public StopOrder SellStop(double qty, double price)
		{
			return this.SendStopOrder(Side.Sell, qty, price);
		}

		public StopLimitOrder BuyStopLimit(Instrument instrument, double qty, double limitPrice, double stopPrice, string text)
		{
			return this.SendStopLimitOrder(instrument, Side.Buy, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder BuyStopLimit(Instrument instrument, double qty, double limitPrice, double stopPrice)
		{
			return this.SendStopLimitOrder(instrument, Side.Buy, qty, limitPrice, stopPrice);
		}

		public StopLimitOrder BuyStopLimit(double qty, double limitPrice, double stopPrice, string text)
		{
			return this.SendStopLimitOrder(Side.Buy, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder BuyStopLimit(double qty, double limitPrice, double stopPrice)
		{
			return this.SendStopLimitOrder(Side.Buy, qty, limitPrice, stopPrice);
		}

		public StopLimitOrder SellStopLimit(Instrument instrument, double qty, double limitPrice, double stopPrice, string text)
		{
			return this.SendStopLimitOrder(instrument, Side.Sell, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder SellStopLimit(Instrument instrument, double qty, double limitPrice, double stopPrice)
		{
			return this.SendStopLimitOrder(instrument, Side.Sell, qty, limitPrice, stopPrice);
		}

		public StopLimitOrder SellStopLimit(double qty, double limitPrice, double stopPrice, string text)
		{
			return this.SendStopLimitOrder(Side.Sell, qty, limitPrice, stopPrice, text);
		}

		public StopLimitOrder SellStopLimit(double qty, double limitPrice, double stopPrice)
		{
			return this.SendStopLimitOrder(Side.Sell, qty, limitPrice, stopPrice);
		}
	}
}
