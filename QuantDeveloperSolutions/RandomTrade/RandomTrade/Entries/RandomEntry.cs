using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Series;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class RandomEntry:RandomBehavior{
	private int minSize=100;//最小购买股数
	public RandomEntry(Instrument instrument,Strategy strategy):base(instrument,strategy){}
	protected override double GetPositionSize(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:
			case SignalSide.SellShort:
				if (!HasPosition){
					return this.minSize;
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
	public override void Deal(){
		if (!HasPosition){
			base.Deal();	
			this.strategy.AddInstrument(this.instrument);
			this.strategy.AddMarketDataRequest(this.instrument);
			string text="随机入场";
			this.Buy(text);
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
		}
	}
	
	/*public override void OnNewTrade(Trade trade){
		base.OnNewTrade(trade);
		Console.WriteLine(trade.Price);
	}*/
}
