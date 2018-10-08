using SmartQuant.Execution;
using SmartQuant.Instruments;
using SmartQuant.Simulation;
using System;

namespace SmartQuant.Trading
{
	[StrategyComponent("{D779BA8E-C0CA-44cf-8745-99105365882F}", ComponentType.CrossExit, Name = "Default_CrossExit", Description = "")]
	public class CrossExit : StrategyMultiComponent
	{
		public const string GUID = "{D779BA8E-C0CA-44cf-8745-99105365882F}";

		public virtual SingleOrder EmitSignal(Signal signal)
		{
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongExit(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text);
			Position position = base.Portfolio.Positions[instrument];
			if (position == null || position.Side != PositionSide.Long)
			{
				signal.Status = SignalStatus.Rejected;
				signal.Rejecter = ComponentType.CrossExit;
			}
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongExit(Instrument instrument)
		{
			return this.LongExit(instrument, "LongExit " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortExit(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.BuyCover, instrument, text);
			Position position = base.Portfolio.Positions[instrument];
			if (position == null || position.Side != PositionSide.Short)
			{
				signal.Status = SignalStatus.Rejected;
				signal.Rejecter = ComponentType.CrossExit;
			}
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortExit(Instrument instrument)
		{
			return this.ShortExit(instrument, "ShortExit " + base.Strategy.Name);
		}

		public virtual SingleOrder LongExit(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			Position position = base.Portfolio.Positions[instrument];
			if (position == null || position.Side != PositionSide.Long)
			{
				signal.Status = SignalStatus.Rejected;
				signal.Rejecter = ComponentType.CrossExit;
			}
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongExit(Instrument instrument, double price)
		{
			return this.LongExit(instrument, price, "LongExit " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortExit(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.BuyCover, instrument, text);
			signal.StrategyFill = true;
			signal.StrategyPrice = price;
			Position position = base.Portfolio.Positions[instrument];
			if (position == null || position.Side != PositionSide.Short)
			{
				signal.Status = SignalStatus.Rejected;
				signal.Rejecter = ComponentType.CrossExit;
			}
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortExit(Instrument instrument, double price)
		{
			return this.ShortExit(instrument, price, "ShortExit " + base.Strategy.Name);
		}

		public virtual SingleOrder LongExit(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder LongExit(Instrument instrument, FillOnBarMode mode)
		{
			return this.LongExit(instrument, mode, "LongExit " + base.Strategy.Name);
		}

		public virtual SingleOrder ShortExit(Instrument instrument, FillOnBarMode mode, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.BuyCover, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder ShortExit(Instrument instrument, FillOnBarMode mode)
		{
			return this.ShortExit(instrument, mode, "ShortExit " + base.Strategy.Name);
		}

		public virtual SingleOrder Buy(Instrument instrument, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Buy, instrument, text));
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Buy, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode)
		{
			return this.Buy(instrument, mode, "Buy " + base.Strategy.Name);
		}

		public virtual SingleOrder BuyLimit(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Limit, SignalSide.Buy, instrument, text);
			signal.LimitPrice = price;
			return base.Strategy.EmitSignal(signal);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Stop, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.StopLimit, SignalSide.Buy, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.TrailingStop, SignalSide.Buy, instrument, text);
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
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text));
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.Sell, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode)
		{
			return this.Sell(instrument, mode, "Sell " + base.Strategy.Name);
		}

		public virtual SingleOrder SellLimit(Instrument instrument, double price, string text)
		{
			if (!base.Strategy.IsInstrumentActive(instrument))
			{
				return null;
			}
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Limit, SignalSide.Sell, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Stop, SignalSide.Sell, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.StopLimit, SignalSide.Sell, instrument, text);
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.TrailingStop, SignalSide.Sell, instrument, text);
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
			return base.Strategy.EmitSignal(new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.SellShort, instrument, text));
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
			Signal signal = new Signal(Clock.Now, ComponentType.CrossExit, SignalType.Market, SignalSide.SellShort, instrument, text);
			signal.ForceFillOnBarMode = true;
			signal.FillOnBarMode = mode;
			return base.Strategy.EmitSignal(signal);
		}

		public virtual SingleOrder SellShort(Instrument instrument, FillOnBarMode mode)
		{
			return this.Sell(instrument, mode, "SellShort " + base.Strategy.Name);
		}
	}
}
