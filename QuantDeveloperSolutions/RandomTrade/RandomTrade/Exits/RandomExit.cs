using System;
using System.Collections.Generic;
using SmartQuant;
//using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.Instruments;
//using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class RandomExit:RandomBehavior{
	
	public RandomExit(Instrument instrument,Strategy strategy):base(instrument,strategy){}
	
	public override void OnInit()
	{
		//读取日线，计算持仓周期
		this.readLastNDailys(this.instrument,this.dailyPeriod-1);
		if (HasPosition){
			BarSeries dailySeries=this.DailyBar;
			int n=dailySeries.Count;
			if (n>0){
				int i;
				for(i=n-1;i>=0;i--){
					if (dailySeries[i].DateTime<=Position.EntryDate.Date) break;
				}
				if (dailySeries[i].DateTime<Position.EntryDate.Date) i++;
				if (i<0) i=0;
				this.holdingPeriod=n-i+1;
			}
		}
		Console.WriteLine("投资组合中的证券 {0} 的持仓周期是 {1} ",this.instrument.Symbol,this.holdingPeriod);
	}

	protected override double GetPositionSize(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:
			case SignalSide.SellShort:
				break;
			case SignalSide.Sell:
				if (HasPosition)
					if (Position.Side == PositionSide.Long)
						return Position.Qty;
				break;		
			case SignalSide.BuyCover:
				if (HasPosition)
					if (Position.Side == PositionSide.Short)
						return Position.Qty;
				break;
		}		
		return 0;
	}
	
	public override void Deal(){
		base.Deal();
		string text="假设出场";
		Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
	}
	protected DateTime exitTime=Clock.Now.Date.Add(new TimeSpan(14,55,0));
	protected bool ordered=false;
	public override void OnNewTrade(Trade trade){
		base.OnNewTrade(trade);
		if (this.holdingPeriod>=this.holdingPeriodLimit&&
			trade.DateTime>=this.exitTime&&(HasPosition)&&(!this.ordered)){
			//超出持仓时间限制出场						
			this.Sell("限时出场");
			string text="限时出场";
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
			this.ordered=true;
		}
	}
}