using System;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class StockEntry:Behavior{
	private int minSize=100;//最小购买股数
	private double positionMaxPrice=0;//持仓后最大价格
	RecognitionState dailyLineState;
	RecognitionState minLineState;
	private TradeManager tradeManager;
	private bool recorded=false;
	public StockEntry(Instrument instrument,RecognitionState dailyLineState,RecognitionState minLineState,TradeManager tradeManager,Strategy strategy):base(instrument,strategy){
		this.tradeManager=tradeManager;
		this.dailyLineState=dailyLineState;
		this.minLineState=minLineState;
	}
	public StockEntry(Instrument instrument,Strategy strategy):this(instrument,null,null,null,strategy){}
	public override void OnInit(){
		if (!HasPosition){
			this.strategy.AddInstrument(this.instrument);
			this.strategy.AddMarketDataRequest(this.instrument);
			string text="多头入场";
			this.Buy(text);
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
		}
	}
	public override void OnNewTrade(Trade trade){
		if (HasPosition&&(trade.Size>0)){
			GMTrade gmTrade=(GMTrade)trade;
			if (gmTrade.High>this.positionMaxPrice) {//更新历史最高值
				this.positionMaxPrice=gmTrade.High;
			}
		}
	}
	public override void OnOrderDone(SingleOrder order)
	{
		if (order.OrdStatus==OrdStatus.Filled||order.OrdStatus==OrdStatus.PartiallyFilled) {
			if ((!this.recorded)&&(this.tradeManager!=null)
			&&(this.dailyLineState!=null)&&(this.minLineState!=null)){
				DateTime entryDate=Position.EntryDate.Date;
				string symbol=this.instrument.Symbol;
				this.tradeManager.RecordAEntryTrade(entryDate,this.dailyLineState,this.minLineState,symbol);
				this.recorded=true;
			}
		}
	}
	
	protected override double GetPositionSize(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:
			case SignalSide.SellShort:
				if (!HasPosition){
					return minSize;
				}
				break;
			case SignalSide.Sell:	
			case SignalSide.BuyCover:
				break;
		}		
		return 0;
	}	
	public override void Close(){
		this.instrument.SetDoubleValue(18888,this.positionMaxPrice);
		this.instrument.Save();
	}
}
