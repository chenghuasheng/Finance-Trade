using System;
using System.Threading;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using SmartQuant.Indicators;

public class BuyGoodestPositionJob:Job{
	private Strategy strategy;
	private SingleOrder order;
	public BuyGoodestPositionJob(string name,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.strategy=strategy;
	}
	public BuyGoodestPositionJob(string name,Strategy strategy):base(name){
		this.strategy=strategy;
	}
	protected override bool doJob(){
		if (this.order==null){
			double goodestInc=0;
			Position goodestPosition=null;
			foreach(Position position in this.strategy.Portfolio.Positions){
				double inc=position.Instrument.Price()/position.EntryPrice-1;
				if (inc>goodestInc){
					goodestPosition=position;
					goodestInc=inc;
				}
			}
			if  (goodestInc<0.1||goodestPosition==null) return false;
			this.order=this.AddPosition(goodestPosition);
			if (this.order==null) return false;
			else Console.WriteLine("证券 {0} 在 {1} 时 最佳买入",goodestPosition.Instrument.Symbol,Clock.Now);
		}
		if (this.order.OrdStatus==OrdStatus.Filled) return true;
		else return false;	
	}
	
	protected SingleOrder AddPosition(Position position){
		if (position!= null)
		{
			string text="买进最佳持仓";
			Instrument instrument=position.Instrument;
			BarSeries dailyBars=this.strategy.Bars[instrument,BarType.Time,86400];
			double lastATR=1.0;
			if(dailyBars.Count>5){
				lastATR=ATR.Value(dailyBars,dailyBars.LastIndex,5,EIndicatorStyle.MetaStock);//昨日ATR
			}
			int expectQty=(int)(this.strategy.Portfolio.GetTotalEquity() * this.strategy.PositionLevel *0.01/ lastATR);
			expectQty=expectQty/100*100;
			Signal signal;
			switch (position.Side)
			{
				case PositionSide.Long:
					signal=new Signal(Clock.Now, SignalType.Market, SignalSide.Buy, expectQty, 0, instrument, text);
					signal.TimeInForce = TimeInForce.Day;				
					return this.strategy.EmitSignal(signal);				
				case PositionSide.Short:
					signal=new Signal(Clock.Now, SignalType.Market, SignalSide.SellShort, expectQty, 0, instrument, text);
					signal.TimeInForce = TimeInForce.Day;
					return this.strategy.EmitSignal(signal);
				default:
					return null;
			}
		}else return null;
	}
}