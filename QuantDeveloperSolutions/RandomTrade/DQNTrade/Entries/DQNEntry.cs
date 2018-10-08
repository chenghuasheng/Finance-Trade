using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Series;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class DQNEntry:Behavior{
	private int minSize=100;//最小购买单位
	private double menoyLevel=0.04;
	private double qValueIn=0.0;
	private double qValueOut=0.0;
	public DQNEntry(Instrument instrument,Strategy strategy):base(instrument,strategy){}
	protected override double GetPositionSize(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:
			case SignalSide.SellShort:
				if (!HasPosition){
					//return this.minSize;
					int expectQty=(int)(Portfolio.GetTotalEquity() * menoyLevel/this.instrument.Price());
					int size=(expectQty/minSize)*minSize;
					if (size<minSize) size=minSize;
					return (double)size;	
				}
				break;
			case SignalSide.Sell:	
			case SignalSide.BuyCover:
				break;
		}		
		return 0;
	}
	public override void OnInit(){
	}
	public void Deal(double qValueIn,double qValueOut){
		if (!HasPosition){
			this.qValueIn=qValueIn;
			this.qValueOut=qValueOut;
			this.strategy.AddInstrument(this.instrument);
			this.strategy.AddMarketDataRequest(this.instrument);
			string text="预测入场";
			this.Buy(text);
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
			Console.WriteLine("预测的QValue为：inside={0},outside={1}",qValueIn,qValueOut);
		}
	}
	public override void OnOrderDone(SingleOrder order)
	{
		if (order.OrdStatus==OrdStatus.Filled||order.OrdStatus==OrdStatus.PartiallyFilled) {	
			string entryDateString=Utils.FormatDate(Position.EntryDate);
			string symbol=this.instrument.Symbol;
			DQNTradeDBAccess.SaveNewTradeRecord(entryDateString,symbol,1,this.qValueIn,this.qValueOut,0);					
		}
	}
}
