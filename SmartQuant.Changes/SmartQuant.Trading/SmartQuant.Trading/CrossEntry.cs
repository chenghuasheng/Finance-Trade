using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Simulation;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{664274F3-FDE1-46da-A84F-556E4A0EB170}", ComponentType.CrossEntry, Name = "Default_CrossEntry", Description = "")]
	public class CrossEntry : StrategyMultiComponent
	{
		public const string GUID = "{664274F3-FDE1-46da-A84F-556E4A0EB170}";

		public virtual SingleOrder EmitSignal(Signal signal)
		{
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongEntry(Instrument instrument)
		{
			return this.LongEntry(instrument, "LongEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.SellShort, instrument, text);
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument)
		{
			return this.ShortEntry(instrument, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, double price)
		{
			return this.LongEntry(instrument, price, "LongEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, double price)
		{
			return this.ShortEntry(instrument, price, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, FillOnBarMode mode)
		{
			return this.LongEntry(instrument, mode, "LongEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, FillOnBarMode mode)
		{
			return this.ShortEntry(instrument, mode, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text));
		}

		public virtual SingleOrder Buy(Instrument instrument)
		{
			return this.Buy(instrument, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Buy(Instrument instrument, double price)
		{
			return this.Buy(instrument, price, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode)
		{
			return this.Buy(instrument, mode, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyMarket(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceMarketOrder = true;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyMarket(Instrument instrument)
		{
			return this.Buy(instrument, "BuyMarket " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Limit, SignalSide.Buy, instrument, text);
			signal.LimitPrice = price;
			signal.TimeInForce = timeInForce;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price, string text)
		{
			return this.BuyLimit(instrument, price, TimeInForce.GTC, text);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce)
		{
			return this.BuyLimit(instrument, price, timeInForce, "BuyLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price)
		{
			return this.BuyLimit(instrument, price, "BuyLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyStop(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Stop, SignalSide.Buy, instrument, text);
			signal.StopPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyStop(Instrument instrument, double price)
		{
			return this.BuyStop(instrument, price, "BuyStop " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyStopLimit(Instrument instrument, double stopPrice, double limitPrice, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.StopLimit, SignalSide.Buy, instrument, text);
			signal.StopPrice = stopPrice;
			signal.LimitPrice = limitPrice;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyStopLimit(Instrument instrument, double stopPrice, double limitPrice)
		{
			return this.BuyStopLimit(instrument, stopPrice, limitPrice, "BuyStopLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyTrailingStop(Instrument instrument, double delta, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.TrailingStop, SignalSide.Buy, instrument, text);
			signal.StopPrice = delta;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyTrailingStop(Instrument instrument, double delta)
		{
			return this.BuyTrailingStop(instrument, delta, string.Format("BuyTrailingStop {0}", base.Strategy.Name));
		}

		public virtual SingleOrder Sell(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Sell, instrument, text));
		}

		public virtual SingleOrder Sell(Instrument instrument)
		{
			return this.Sell(instrument, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder Sell(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Sell(Instrument instrument, double price)
		{
			return this.Sell(instrument, price, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode)
		{
			return this.Sell(instrument, mode, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder SellMarket(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceMarketOrder = true;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellMarket(Instrument instrument)
		{
			return this.Sell(instrument, "SellMarket " + base.Strategy.Name);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Limit, SignalSide.Sell, instrument, text);
			signal.LimitPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price)
		{
			return this.SellLimit(instrument, price, "SellLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder SellStop(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Stop, SignalSide.Sell, instrument, text);
			signal.StopPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellStop(Instrument instrument, double price)
		{
			return this.SellStop(instrument, price, "SellStop " + base.Strategy.Name);
		}

		public virtual SingleOrder SellStopLimit(Instrument instrument, double stopPrice, double limitPrice, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.StopLimit, SignalSide.Sell, instrument, text);
			signal.StopPrice = stopPrice;
			signal.LimitPrice = limitPrice;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellStopLimit(Instrument instrument, double stopPrice, double limitPrice)
		{
			return this.SellStopLimit(instrument, stopPrice, limitPrice, "SellStopLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder SellTrailingStop(Instrument instrument, double delta, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.TrailingStop, SignalSide.Sell, instrument, text);
			signal.StopPrice = delta;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellTrailingStop(Instrument instrument, double delta)
		{
			return this.SellTrailingStop(instrument, delta, string.Format("SellTrailingStop {0}", base.Strategy.Name));
		}

		public virtual SingleOrder SellShort(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.SellShort, instrument, text));
		}

		public virtual SingleOrder SellShort(Instrument instrument)
		{
			return this.SellShort(instrument, "SellShort " + base.Strategy.Name);
		}

		public virtual SingleOrder SellShort(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossEntry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellShort(Instrument instrument, FillOnBarMode mode)
		{
			return this.SellShort(instrument, mode, "SellShort " + base.Strategy.Name);
		}
	}
}
