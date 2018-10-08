using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using SmartQuant.Simulation;
using SmartQuant.Providers;

public class CrossBehavior{
	protected Strategy strategy;
	public Strategy Strategy
	{
		get
		{
			return this.strategy;
		}
		internal set
		{
			this.strategy = value;
		}
	}
	[Browsable(false)]
	public BarSeriesList Bars
	{
		get
		{
			return this.strategy.Bars;
		}
	}
	public TradeArrayList Trades
	{
		get { return this.strategy.Trades; }
	}	
	public QuoteArrayList Quotes
	{
		get { return this.strategy.Quotes; }
	}
	[Browsable(false)]
	public Hashtable Global
	{
		get
		{
			return this.strategy.Global;
		}
	}
	[Browsable(false)]
	public Portfolio Portfolio
	{
		get
		{
			return this.strategy.Portfolio;
		}
	}
	public CrossBehavior(Strategy strategy){
		this.strategy=strategy;
		this.OnInit();
	}
	public virtual void OnInit(){
	}
	public virtual void OnNewBarSlice(long size)
	{
	}
	public virtual void OnNewBar(Instrument instrument, Bar bar)
	{
	}
	public virtual void OnNewBarOpen(Instrument instrument, Bar bar)
	{
	}
	public virtual void OnNewCorporateAction(Instrument instrument, CorporateAction corporateAction)
	{
	}	
	public virtual void OnNewFundamental(Instrument instrument, Fundamental fundamental)
	{
	}
	public virtual void OnNewMarketData(Instrument instrument, IMarketData data)
	{
	}
	public virtual void OnNewMarketDepth(Instrument instrument, MarketDepth marketDepth)
	{
	}
	public virtual void OnNewQuote(Instrument instrument, Quote quote)
	{
	}
	public virtual void OnNewTrade(Instrument instrument, Trade trade)
	{
	}
	public virtual void OnNewOrder(SingleOrder order)
	{
	}
	public virtual void OnExecutionReport(SingleOrder order, ExecutionReport report)
	{
	}
	public virtual void OnOrderCancelled(SingleOrder order)
	{
	}
	public virtual void OnOrderDone(SingleOrder order)
	{
	}
	public virtual void OnOrderFilled(SingleOrder order)
	{
	}
	public virtual void OnOrderPartiallyFilled(SingleOrder order)
	{
	}
	public virtual void OnOrderRejected(SingleOrder order)
	{
	}
	public virtual void OnOrderStatusChanged(SingleOrder order)
	{
	}
	public virtual void OnPositionChanged(Position position)
	{
	}
	public virtual void OnPositionClosed(Position position)
	{
	}
	public virtual void OnPositionOpened(Position position)
	{
	}
	public virtual void OnPositionValueChanged(Position position)
	{
	}
	public virtual void OnPortfolioValueChanged(Position position)
	{
	}
	public virtual void OnProviderConnected(IProvider provider)
	{
	}
	public virtual void OnProviderDisconnected(IProvider provider)
	{
	}
	public virtual void OnProviderError(IProvider provider, int id, int code, string message)
	{
	}
	protected SingleOrder EmitSignal(Signal signal)
	{
		double positionSize = this.GetPositionSize(signal);	
		if (positionSize >0.0){
			signal.Qty = positionSize;
		}else {
			signal.Status = SignalStatus.Rejected;
		}
		return this.strategy.EmitSignal(signal);
	}
	protected virtual double GetPositionSize(Signal signal)
	{
		int Qty=100;
		return Qty;
	}
	//市价买入
	public virtual SingleOrder Buy(Instrument instrument)
	{
		return this.Buy(instrument, "Buy " + this.Strategy.Name);
	}
	public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode)
	{
		return this.Buy(instrument, mode, "Buy " + this.Strategy.Name);
	}
	public virtual SingleOrder Buy(Instrument instrument, double price)
	{
		return this.Buy(instrument, price, "Buy " + this.Strategy.Name);
	}
	public virtual SingleOrder Buy(Instrument instrument, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		return this.EmitSignal(new Signal(Clock.Now, SignalType.Market, SignalSide.Buy, instrument, text));
	}
	public virtual SingleOrder Buy(Instrument instrument, FillOnBarMode mode, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now, SignalType.Market, SignalSide.Buy, instrument, text);
		signal.ForceFillOnBarMode = true;
		signal.FillOnBarMode = mode;
		return this.EmitSignal(signal);
	}
	public virtual SingleOrder Buy(Instrument instrument, double price, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now, SignalType.Market, SignalSide.Buy, instrument, text);
		signal.StrategyFill = true;
		signal.StrategyPrice = price;
		return this.EmitSignal(signal);
	}
	//限价买入

	public virtual SingleOrder BuyLimit(Instrument instrument, double price)
	{
		return this.BuyLimit(instrument, price, "BuyLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce)
	{
		return this.BuyLimit(instrument, price, timeInForce, "BuyLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price, string text)
	{
		return this.BuyLimit(instrument, price, TimeInForce.GTC, text);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now,SignalType.Limit, SignalSide.Buy, instrument, text);

		signal.LimitPrice = price;
		signal.TimeInForce = timeInForce;

		return this.EmitSignal(signal);
	}
	//市价卖出
	public virtual SingleOrder Sell(Instrument instrument)
	{
		return this.Sell(instrument, "Sell " + this.Strategy.Name);
	}
	public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode)
	{
		return this.Sell(instrument, mode, "Sell " + this.Strategy.Name);
	}
	public virtual SingleOrder Sell(Instrument instrument, double price)
	{
		return this.Sell(instrument, price, "Sell " + this.Strategy.Name);
	}
	public virtual SingleOrder Sell(Instrument instrument, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		return this.EmitSignal(new Signal(Clock.Now, SignalType.Market, SignalSide.Sell, instrument, text));
	}
	public virtual SingleOrder Sell(Instrument instrument, FillOnBarMode mode, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now, SignalType.Market, SignalSide.Sell, instrument, text);
		signal.ForceFillOnBarMode = true;
		signal.FillOnBarMode = mode;
		return this.EmitSignal(signal);
	}
	public virtual SingleOrder Sell(Instrument instrument, double price, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now,SignalType.Market, SignalSide.Sell, instrument, text);
		
		signal.StrategyFill = true;
		signal.StrategyPrice = price;
		return this.EmitSignal(signal);
	}
	//限价卖出
	public virtual SingleOrder SellLimit(Instrument instrument, double price)
	{
		return this.SellLimit(instrument, price, "SellLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price, TimeInForce timeInForce)
	{
		return this.SellLimit(instrument, price, timeInForce, "SellLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price, string text)
	{
		return this.SellLimit(instrument, price, TimeInForce.GTC, text);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price, TimeInForce timeInForce, string text)
	{
		if (!this.Strategy.IsInstrumentActive(instrument))
		{
			return null;
		}
		Signal signal = new Signal(Clock.Now,SignalType.Limit, SignalSide.Sell, instrument, text);
		signal.LimitPrice = price;
		signal.TimeInForce = timeInForce;
		return this.EmitSignal(signal);
	}	
}