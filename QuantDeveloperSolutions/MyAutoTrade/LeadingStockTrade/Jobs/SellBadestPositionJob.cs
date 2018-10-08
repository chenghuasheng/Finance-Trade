using System;
using System.Threading;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Execution;

public class SellBadestPositionJob:Job{
	private Strategy strategy;
	private SingleOrder order;
	public SellBadestPositionJob(string name,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.strategy=strategy;
	}
	public SellBadestPositionJob(string name,Strategy strategy):base(name){
		this.strategy=strategy;
	}
	protected override bool doJob(){
		if (this.order==null){
			double badestInc=1.0;
			Position badestPosition=null;
			foreach(Position position in this.strategy.Portfolio.Positions){
				if (position.EntryDate<Clock.Now.Date) {
					double inc=position.Instrument.Price()/position.EntryPrice-1;
					if (inc<badestInc){
						badestPosition=position;
						badestInc=inc;
					}
				}
			}
			if  (badestInc>0||badestPosition==null) return false;
			this.order=this.ClosePosition(badestPosition);
			if (this.order==null) return false;
			else Console.WriteLine("证券 {0} 在 {1} 时 最差卖出",badestPosition.Instrument.Symbol,Clock.Now);
		}
		if (this.order.OrdStatus==OrdStatus.Filled) return true;
		else return false;	
	}
	protected SingleOrder ClosePosition(Position position){
		if (position!= null)
		{
			string text="卖出最差持仓";
			Instrument instrument=position.Instrument;
			switch (position.Side)
			{
				case PositionSide.Long:
					return this.strategy.EmitSignal(new Signal(Clock.Now,  SignalType.Market, SignalSide.Sell, position.Qty, 0, instrument, text));				
				case PositionSide.Short:
					return this.strategy.EmitSignal(new Signal(Clock.Now, SignalType.Market, SignalSide.BuyCover, position.Qty, 0, instrument, text));
				default:
					return null;
			}
		}else return null;
	}
}