using System;
using System.Collections;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using SmartQuant.Simulation;

public class Behavior {
	protected Instrument instrument;
	public Instrument Instrument
	{
		get
		{
			return this.instrument;
		}
		set
		{
			this.instrument = value;
		}
	}

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

	public BarSeriesList Bars
	{
		get
		{
			return this.strategy.Bars;
		}
	}
	
	public BarSeries DailyBar
	{
		get { return this.strategy.Bars[this.instrument,BarType.Time,86400]; }
	}
	
	public BarSeries Bar
	{
		get
		{
			return this.strategy.Bars[this.instrument];
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
	
	public bool HasPosition
	{
		get
		{
			return (this.Position != null);
		}
	}
	public Portfolio Portfolio
	{
		get
		{
			return this.strategy.Portfolio;
		}
	}
	public Position Position
	{
		get
		{
			return this.strategy.Portfolio.Positions[this.instrument];
		}
	}
	public Hashtable Global
	{
		get
		{
			return this.strategy.Global;
		}
	}

	public Behavior(Instrument instrument,Strategy strategy){
		this.instrument=instrument;
		this.strategy=strategy;
		this.OnInit();
	}
	public virtual void OnInit(){
	}
	public virtual void OnNewBar(Bar bar)
	{
	}
	public virtual void OnNewBarOpen(Bar bar)
	{
	}
	public virtual void OnNewCorporateAction(CorporateAction corporateAction)
	{
	}
	public virtual void OnNewFundamental(Fundamental fundamental)
	{
	}
	public virtual void OnNewMarketDepth(MarketDepth marketDepth)
	{
	}
	public virtual void OnNewQuote(Quote quote)
	{
	}
	public virtual void OnNewTrade(Trade trade)
	{
	}
	public virtual void OnNewOrder(SingleOrder order)
	{
	}
	public virtual void OnOrderCancelled(SingleOrder order)
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
	public virtual void OnOrderDone(SingleOrder order)
	{
	}
	public virtual void OnExecutionReport(SingleOrder order, ExecutionReport report)
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
	public virtual SingleOrder Buy()
	{
		return this.Buy(this.instrument);
	}
	public virtual SingleOrder Buy(Instrument instrument)
	{
		return this.Buy(instrument, "Buy " + this.Strategy.Name);
	}
	public virtual SingleOrder Buy(FillOnBarMode mode)
	{
		return this.Buy(this.instrument, mode);
	}
	public virtual SingleOrder Buy(string text, FillOnBarMode mode)
	{
		return this.Buy(this.instrument, mode, text);
	}
	public virtual SingleOrder Buy(double price)
	{
		return this.Buy(this.instrument, price);
	}
	public virtual SingleOrder Buy(string text)
	{
		return this.Buy(this.instrument, text);
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
	public virtual SingleOrder Buy(double price, string text)
	{
		return this.Buy(this.instrument, price, text);
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
	public virtual SingleOrder BuyLimit(double price)
	{
		return this.BuyLimit(this.instrument, price);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price)
	{
		return this.BuyLimit(instrument, price, "BuyLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder BuyLimit(double price, TimeInForce timeInForce)
	{
		return this.BuyLimit(this.instrument, price, timeInForce);
	}
	public virtual SingleOrder BuyLimit(double price, string text)
	{
		return this.BuyLimit(this.instrument, price, text);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price, TimeInForce timeInForce)
	{
		return this.BuyLimit(instrument, price, timeInForce, "BuyLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder BuyLimit(Instrument instrument, double price, string text)
	{
		return this.BuyLimit(instrument, price, TimeInForce.GTC, text);
	}
	public virtual SingleOrder BuyLimit(double price, TimeInForce timeInForce, string text)
	{
		return this.BuyLimit(this.instrument, price, timeInForce, text);
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
	public virtual SingleOrder Sell()
	{
		return this.Sell(this.instrument);
	}
	public virtual SingleOrder Sell(Instrument instrument)
	{
		return this.Sell(instrument, "Sell " + this.Strategy.Name);
	}
	public virtual SingleOrder Sell(FillOnBarMode mode)
	{
		return this.Sell(this.instrument, mode);
	}
	public virtual SingleOrder Sell(double price)
	{
		return this.Sell(this.instrument, price);
	}
	public virtual SingleOrder Sell(string text)
	{
		return this.Sell(this.instrument, text);
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
	public virtual SingleOrder Sell(string text, FillOnBarMode mode)
	{
		return this.Sell(this.instrument, mode, text);
	}
	public virtual SingleOrder Sell(string text, double price)
	{
		return this.Sell(this.instrument, price, text);
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
	public virtual SingleOrder SellLimit(double price)
	{
		return this.SellLimit(this.instrument, price);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price)
	{
		return this.SellLimit(instrument, price, "SellLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder SellLimit(double price, TimeInForce timeInForce)
	{
		return this.SellLimit(this.instrument, price, timeInForce);
	}
	public virtual SingleOrder SellLimit(double price, string text)
	{
		return this.SellLimit(this.instrument, price, text);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price, TimeInForce timeInForce)
	{
		return this.SellLimit(instrument, price, timeInForce, "SellLimit " + this.Strategy.Name);
	}
	public virtual SingleOrder SellLimit(Instrument instrument, double price, string text)
	{
		return this.SellLimit(instrument, price, TimeInForce.GTC, text);
	}
	public virtual SingleOrder SellLimit(double price, TimeInForce timeInForce, string text)
	{
		return this.SellLimit(this.instrument, price, timeInForce, text);
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