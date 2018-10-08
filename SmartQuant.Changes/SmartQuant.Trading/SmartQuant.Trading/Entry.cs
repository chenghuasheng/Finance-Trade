using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Simulation;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{94FAFF9D-5281-4c67-A599-B893F1F58B38}", ComponentType.Entry, Name = "Default_Entry", Description = "")]
	public class Entry : StrategySingleComponent
	{
		public const string GUID = "{94FAFF9D-5281-4c67-A599-B893F1F58B38}";

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
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.SellShort, instrument, text);
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument)
		{
			return this.ShortEntry(instrument, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder LongEntry(string text)
		{
			return this.LongEntry(this.instrument, text);
		}

		public virtual SingleOrder LongEntry()
		{
			return this.LongEntry(this.instrument);
		}

		public virtual SingleOrder ShortEntry(string text)
		{
			return this.ShortEntry(this.instrument, text);
		}

		public virtual SingleOrder ShortEntry()
		{
			return this.ShortEntry(this.instrument);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, double price)
		{
			return this.ShortEntry(instrument, price, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder LongEntry(double price, string text)
		{
			return this.LongEntry(this.instrument, price, text);
		}

		public virtual SingleOrder LongEntry(double price)
		{
			return this.LongEntry(this.instrument, price);
		}

		public virtual SingleOrder ShortEntry(double price, string text)
		{
			return this.ShortEntry(this.instrument, price, text);
		}

		public virtual SingleOrder ShortEntry(double price)
		{
			return this.ShortEntry(this.instrument, price);
		}

		public virtual SingleOrder LongEntry(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortEntry(Instrument instrument, FillOnBarMode mode)
		{
			return this.ShortEntry(instrument, mode, "ShortEntry " + base.Strategy.Name);
		}

		public virtual SingleOrder LongEntry(FillOnBarMode mode, string text)
		{
			return this.LongEntry(this.instrument, mode, text);
		}

		public virtual SingleOrder LongEntry(FillOnBarMode mode)
		{
			return this.LongEntry(this.instrument, mode);
		}

		public virtual SingleOrder ShortEntry(FillOnBarMode mode, string text)
		{
			return this.ShortEntry(this.instrument, mode, text);
		}

		public virtual SingleOrder ShortEntry(FillOnBarMode mode)
		{
			return this.ShortEntry(this.instrument, mode);
		}

		public virtual SingleOrder Buy(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text));
		}

		public virtual SingleOrder Buy(Instrument instrument)
		{
			return this.Buy(instrument, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(string text)
		{
			return this.Buy(this.instrument, text);
		}

		public virtual SingleOrder Buy()
		{
			return this.Buy(this.instrument);
		}

		public virtual SingleOrder Buy(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Buy(Instrument instrument, double price)
		{
			return this.Buy(instrument, price, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(double price, string text)
		{
			return this.Buy(this.instrument, price, text);
		}

		public virtual SingleOrder Buy(double price)
		{
			return this.Buy(this.instrument, price);
		}

		public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode)
		{
			return this.Buy(instrument, mode, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(string text, FillOnBarMode mode)
		{
			return this.Buy(this.instrument, mode, text);
		}

		public virtual SingleOrder Buy(FillOnBarMode mode)
		{
			return this.Buy(this.instrument, mode);
		}

		public virtual SingleOrder BuyMarket(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceMarketOrder = true;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyMarket(Instrument instrument)
		{
			return this.Buy(instrument, "BuyMarket " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyMarket(string text)
		{
			return this.Buy(this.instrument, text);
		}

		public virtual SingleOrder BuyMarket()
		{
			return this.Buy(this.instrument);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Limit, SignalSide.Buy, instrument, text);
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

		public virtual SingleOrder BuyLimit(double price, TimeInForce timeInForce, string text)
		{
			return this.BuyLimit(this.instrument, price, timeInForce, text);
		}

		public virtual SingleOrder BuyLimit(double price, string text)
		{
			return this.BuyLimit(this.instrument, price, text);
		}

		public virtual SingleOrder BuyLimit(double price, TimeInForce timeInForce)
		{
			return this.BuyLimit(this.instrument, price, timeInForce);
		}

		public virtual SingleOrder BuyLimit(double price)
		{
			return this.BuyLimit(this.instrument, price);
		}

		public virtual SingleOrder BuyStop(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Stop, SignalSide.Buy, instrument, text);
			signal.StopPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyStop(Instrument instrument, double price)
		{
			return this.BuyStop(instrument, price, "BuyStop " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyStop(double price, string text)
		{
			return this.BuyStop(this.instrument, price, text);
		}

		public virtual SingleOrder BuyStop(double price)
		{
			return this.BuyStop(this.instrument, price);
		}

		public virtual SingleOrder BuyStopLimit(Instrument instrument, double stopPrice, double limitPrice, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.StopLimit, SignalSide.Buy, instrument, text);
			signal.StopPrice = stopPrice;
			signal.LimitPrice = limitPrice;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyStopLimit(Instrument instrument, double stopPrice, double limitPrice)
		{
			return this.BuyStopLimit(instrument, stopPrice, limitPrice, "BuyStopLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyStopLimit(double stopPrice, double limitPrice, string text)
		{
			return this.BuyStopLimit(this.instrument, stopPrice, limitPrice, text);
		}

		public virtual SingleOrder BuyStopLimit(double stopPrice, double limitPrice)
		{
			return this.BuyStopLimit(this.instrument, stopPrice, limitPrice);
		}

		public virtual SingleOrder BuyTrailingStop(double delta, string text)
		{
			if (!base.Strategy.IsInstrumentActive(this.instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.TrailingStop, SignalSide.Buy, this.instrument, text);
			signal.StopPrice = delta;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder BuyTrailingStop(double delta)
		{
			return this.BuyTrailingStop(delta, string.Format("BuyTrailingStop {0}", base.Strategy.Name));
		}

		public virtual SingleOrder Sell(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Sell, instrument, text));
		}

		public virtual SingleOrder Sell(Instrument instrument)
		{
			return this.Sell(instrument, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder Sell(string text)
		{
			return this.Sell(this.instrument, text);
		}

		public virtual SingleOrder Sell()
		{
			return this.Sell(this.instrument);
		}

		public virtual SingleOrder Sell(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Sell(Instrument instrument, double price)
		{
			return this.Sell(instrument, price, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder Sell(string text, double price)
		{
			return this.Sell(this.instrument, price, text);
		}

		public virtual SingleOrder Sell(double price)
		{
			return this.Sell(this.instrument, price);
		}

		public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode)
		{
			return this.Sell(instrument, mode, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder Sell(string text, FillOnBarMode mode)
		{
			return this.Sell(this.instrument, mode, text);
		}

		public virtual SingleOrder Sell(FillOnBarMode mode)
		{
			return this.Sell(this.instrument, mode);
		}

		public virtual SingleOrder SellMarket(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceMarketOrder = true;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellMarket(Instrument instrument)
		{
			return this.Sell(instrument, "SellMarket " + base.Strategy.Name);
		}

		public virtual SingleOrder SellMarket(string text)
		{
			return this.Sell(this.instrument, text);
		}

		public virtual SingleOrder SellMarket()
		{
			return this.Sell(this.instrument);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price, TimeInForce timeInForce, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Limit, SignalSide.Sell, instrument, text);
			signal.LimitPrice = price;
			signal.TimeInForce = timeInForce;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price, string text)
		{
			return this.SellLimit(instrument, price, TimeInForce.GTC, text);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price, TimeInForce timeInForce)
		{
			return this.SellLimit(instrument, price, timeInForce, "SellLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price)
		{
			return this.SellLimit(instrument, price, "SellLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder SellLimit(double price, TimeInForce timeInForce, string text)
		{
			return this.SellLimit(this.instrument, price, timeInForce, text);
		}

		public virtual SingleOrder SellLimit(double price, string text)
		{
			return this.SellLimit(this.instrument, price, text);
		}

		public virtual SingleOrder SellLimit(double price, TimeInForce timeInForce)
		{
			return this.SellLimit(this.instrument, price, timeInForce);
		}

		public virtual SingleOrder SellLimit(double price)
		{
			return this.SellLimit(this.instrument, price);
		}

		public virtual SingleOrder SellStop(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Stop, SignalSide.Sell, instrument, text);
			signal.StopPrice = price;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellStop(Instrument instrument, double price)
		{
			return this.SellStop(instrument, price, "SellStop " + base.Strategy.Name);
		}

		public virtual SingleOrder SellStop(double price, string text)
		{
			return this.SellStop(this.instrument, price, text);
		}

		public virtual SingleOrder SellStop(double price)
		{
			return this.SellStop(this.instrument, price);
		}

		public virtual SingleOrder SellStopLimit(Instrument instrument, double stopPrice, double limitPrice, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.StopLimit, SignalSide.Sell, instrument, text);
			signal.StopPrice = stopPrice;
			signal.LimitPrice = limitPrice;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellStopLimit(Instrument instrument, double stopPrice, double limitPrice)
		{
			return this.SellStopLimit(instrument, stopPrice, limitPrice, "SellStopLimit " + base.Strategy.Name);
		}

		public virtual SingleOrder SellStopLimit(double stopPrice, double limitPrice, string text)
		{
			return this.SellStopLimit(this.instrument, stopPrice, limitPrice, text);
		}

		public virtual SingleOrder SellStopLimit(double stopPrice, double limitPrice)
		{
			return this.SellStopLimit(this.instrument, stopPrice, limitPrice);
		}

		public virtual SingleOrder SellTrailingStop(double delta, string text)
		{
			if (!base.Strategy.IsInstrumentActive(this.instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.TrailingStop, SignalSide.Sell, this.instrument, text);
			signal.StopPrice = delta;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellTrailingStop(double delta)
		{
			return this.SellTrailingStop(delta, string.Format("SellTrailingStop {0}", base.Strategy.Name));
		}

		public virtual SingleOrder SellShort(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.SellShort, instrument, text));
		}

		public virtual SingleOrder SellShort(Instrument instrument)
		{
			return this.SellShort(instrument, "SellShort " + base.Strategy.Name);
		}

		public virtual SingleOrder SellShort(string text)
		{
			return this.SellShort(this.instrument, text);
		}

		public virtual SingleOrder SellShort()
		{
			return this.SellShort(this.instrument);
		}

		public virtual SingleOrder SellShort(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.Entry, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellShort(Instrument instrument, FillOnBarMode mode)
		{
			return this.SellShort(instrument, mode, "SellShort " + base.Strategy.Name);
		}

		public virtual SingleOrder SellShort(string text, FillOnBarMode mode)
		{
			return this.SellShort(this.instrument, mode, text);
		}

		public virtual SingleOrder SellShort(FillOnBarMode mode)
		{
			return this.SellShort(this.instrument, mode);
		}
	}
}
