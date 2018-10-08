using System;
using System.Collections;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Instruments;


public class Marketor{
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
		
	public TradeArrayList Trades
	{
		get { return this.strategy.Trades; }
	}
	
	public QuoteArrayList Quotes
	{
		get { return this.strategy.Quotes; }
	}
	
	public Portfolio Portfolio
	{
		get
		{
			return this.strategy.Portfolio;
		}
	}
	
	public Hashtable Global
	{
		get
		{
			return this.strategy.Global;
		}
	}
	public bool MarketOpen{
		get {
			return this.strategy.MarketOpen;
		}
		set {
			this.strategy.MarketOpen=value;
		}
	}
	public Marketor(Strategy strategy){
		this.strategy=strategy;
	}
	public virtual void OnInit(){
	}
	public void AddInstrument(Instrument instrument){
		this.strategy.AddInstrument(instrument);
	}	
	public bool HasPosition(Instrument instrument){
		return (this.Portfolio.Positions[instrument]!=null);
	}
}