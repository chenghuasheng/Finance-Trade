using System;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Indicators;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
using System.Collections.Generic;

public class UpLimitEntry:Behavior{
	int lengthATR=5;
	int lengthBreak=30;//突破周期
	private double portfolioRisk = 0.01;
	public double PortfolioRisk
	{
		get { return portfolioRisk;  }
		set { portfolioRisk = value; }
	}
	private int minSize=100;//最小购买股数	
	private double lastATR=0;//昨日ATR
	private bool canOrder=false;//可下单
	private bool orderDone=false;//已下单
	private DateTime openTime;
	public UpLimitEntry(Instrument instrument,Strategy strategy):base(instrument,strategy)
	{
	}
	
	public override void OnInit(){	
		DateTime curDate=Clock.Now.Date;
		if (!HasPosition){
			int length=this.DailyBar.Count>this.lengthATR?this.lengthATR:this.DailyBar.Count-1;
			if(length>0){
				this.lastATR=ATR.Value(this.DailyBar,this.DailyBar.LastIndex,length,EIndicatorStyle.MetaStock);
			}else {
				this.lastATR=1;
			}			
			this.canOrder=true;
		}
		this.openTime=curDate.Add(new TimeSpan(9,30,0));		
		this.portfolioRisk=this.portfolioRisk*this.strategy.PositionLevel;		
	}
	public override void OnNewBar(Bar bar)
	{
	}
	public override void OnNewTrade(Trade trade)
	{
		if ((!HasPosition)&&(!this.orderDone)&&this.canOrder){
			if (trade.Size<=0) return; 
			GMTrade gmTrade=(GMTrade)trade;	
			double avgPrice=gmTrade.TotalAmount/gmTrade.TotalSize;
			double inc=gmTrade.Open/gmTrade.LastClose-1;
			if (gmTrade.DateTime>=this.openTime
				&&gmTrade.Price<gmTrade.UpperLimit
				&&gmTrade.Price>avgPrice
				//&&inc<0.0
			&&gmTrade.Price>gmTrade.LastClose 
				//&&gmTrade.High>this.DailyBar.Last.High
			) {
				this.doBuy(gmTrade,"多头入场");
			}
		}
	}
	protected void doBuy(GMTrade gmTrade,string text){
		double slipPrice=gmTrade.Price*1.01;
		slipPrice=slipPrice>gmTrade.UpperLimit?gmTrade.UpperLimit:slipPrice;
		if (BuyLimit(slipPrice ,TimeInForce.Day ,text)!=null){
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
			this.orderDone=true;
		}
	}
	protected override double GetPositionSize(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:
			case SignalSide.SellShort:
				if (!HasPosition){
					int expectQty=(int)(Portfolio.GetTotalEquity() * portfolioRisk / GetPositionRisk());
					return Math.Round(expectQty*1.0/minSize)*minSize;
				}
				break;
			case SignalSide.Sell:	
			case SignalSide.BuyCover:
				break;
		}		
		return 0;
	}	
	public double GetPositionRisk()
	{		
		return this.lastATR;
	}
	
	
}