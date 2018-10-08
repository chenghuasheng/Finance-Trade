using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
//using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class DQNExit:Behavior{
	private int holdingPeriod=0;
	public int HoldingPeriod {
		get { return this.holdingPeriod;}
	}
	private double stopLossPrice=0;
	private double stopLossRate=0.08;
	public DQNExit(Instrument instrument,Strategy strategy):base(instrument,strategy){}
	
	public override void OnInit()
	{
		//读取日线，计算持仓周期	
		if (HasPosition){
			DateTime curDate=Clock.Now.Date;
			List<Daily> dailyBars=BarUtils.GetLastNDailys(this.instrument,curDate.AddDays(-1),40);
			BarUtils.AdjustDailys(dailyBars);//向前复权
			int n=dailyBars.Count;
			if (n>0){
				int i;
				for(i=n-1;i>=0;i--){
					if (dailyBars[i].DateTime<=Position.EntryDate.Date) break;
				}
				if (dailyBars[i].DateTime<Position.EntryDate.Date) i++;
				if (i<0) i=0;
				this.holdingPeriod=n-i+1;
			}
			this.stopLossPrice=Position.EntryPrice*(1-stopLossRate);
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
	
	public void Deal(double qValueIn,double qValueOut){	
		string text="预测出场";
		this.Sell(text);
		Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
		Console.WriteLine("预测的QValue为：inside={0},outside={1}",qValueIn,qValueOut);
	}
	protected DateTime exitTime=Clock.Now.Date.Add(new TimeSpan(14,55,0));
	protected bool ordered=false;
	public override void OnNewTrade(Trade trade){
		if (HasPosition&&(!this.ordered)&&trade.Size>0){
			if (trade.Price<=this.stopLossPrice)   {
				string text="绝对止损出场";
				this.Sell(text);
				Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
				this.ordered=true;
			}
			if (this.holdingPeriod>=Const.HoldingPeriodLimit&&trade.DateTime>=this.exitTime){
				//超出持仓时间限制出场						
				string text="限时出场";
				this.Sell(text);
				Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
				this.ordered=true;
			}
		}
	}
	
	//此处不能用OnOrderFilled事件，因为在这个事件中,Position已经销毁了
	public override void OnExecutionReport(SingleOrder order, ExecutionReport report)
	{
		if (report.OrdStatus==OrdStatus.Filled) {
			string symbol=this.instrument.Symbol;
			float profit=(float)(Position.GetPnLPercent());
			string entryDateString=Utils.FormatDate(Position.EntryDate);
			DQNTradeDBAccess.UpdateClosedRecord(entryDateString,symbol,this.holdingPeriod,profit);
		}
	}
}