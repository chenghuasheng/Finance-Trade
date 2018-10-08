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
public class StopExitNew:Behavior{
	private int lengthATR=5;//ATR所采用周期
	private int positionCycle=10;//持仓周期
	private int positionCycleLimit=3;//持仓周期限制
	private double positionMaxPrice=0;//持仓后最大价格
	private double stopLossPrice=0;//绝对止损价格
	private double stopLossFactor=2;//绝对止损因子  
	private double moveStopPrice=0;//移动止盈价格
	private double moveStopFactor=1.4;//移动止盈因子 
	private double stopLossATR=1;//绝对止损ATR
	private double moveStopATR=1;//移动止盈止损ATR
	private bool orderDone=false;//已下单
	private bool isStagnant=false;
	public StopExitNew(Instrument instrument,Strategy strategy):base(instrument,strategy)
	{
	}
	public override void OnInit()
	{
		if (HasPosition){
			//60天日线
			DateTime curDate=Clock.Now.Date;
			ISeriesObject[] dailyBars=Util.GetNDailiesBeforeDate(this.instrument,curDate,60);
			Util.AdjustDailys(dailyBars);//向前复权
			foreach(Daily dBar in dailyBars) {
				this.Bar.Add(dBar);
			}
			if(this.DailyBar.Count>this.lengthATR){
				this.moveStopATR=ATR.Value(this.DailyBar,this.DailyBar.LastIndex,lengthATR,EIndicatorStyle.MetaStock);//昨日ATR
			}
			int n=this.DailyBar.Count;
			int i;
			for(i=n-1;i>=0;i--){
				if (this.DailyBar[i].DateTime<=Position.EntryDate.Date) break;
			}
			if (this.DailyBar[i].DateTime<Position.EntryDate.Date) i++;
			if (i<0) i=0;
			this.positionCycle=n-i+1;
			for(int j=i;j<n;j++){
				if (this.DailyBar[j].High>this.positionMaxPrice) this.positionMaxPrice=this.DailyBar[j].High;//入场后的最高价格，不包括入场那一天
			}			
			this.calcMoveStopPrice();
			int length=i>this.lengthATR?this.lengthATR:i-1;
			if (length>0){
				this.stopLossATR=ATR.Value(this.DailyBar,i-1,length,EIndicatorStyle.MetaStock);//入场前一天的ATR
			}
			this.calcStopLossPrice();
			//this.judgeStagnant();
			Console.WriteLine("投资组合中的证券 {0} :",Position.Instrument.Symbol);
			Console.WriteLine("持仓周期是 {0},绝对止损价格是 {1},持仓后最大价格是 {2}",this.positionCycle,this.stopLossPrice,this.positionMaxPrice);
		}
	}
	public override void OnNewTrade(Trade trade){
		
		if (HasPosition&&(!this.orderDone)&&trade.Size>0){
			GMTrade gmTrade=(GMTrade)trade;
			if (gmTrade.High>this.positionMaxPrice) {//更新历史最高值，并计算是否开始监控止盈
				this.positionMaxPrice=gmTrade.High;
				this.calcMoveStopPrice();
			}
			if (this.positionCycle>1&&gmTrade.Price>gmTrade.LowerLimit) //T+1交易并且可以卖出
			{		
				double avgPrice=gmTrade.TotalAmount/gmTrade.TotalSize;
				if (gmTrade.Price<=this.stopLossPrice)   {
					this.doSell(gmTrade,"绝对止损出场");//是亏损的
				}else if ((gmTrade.Price<=this.moveStopPrice)&&(gmTrade.Price>=Position.EntryPrice)) {
					this.doSell(gmTrade,"移动止盈出场");//是盈利的
				//}else if (gmTrade.Price<avgPrice*0.99&&gmTrade.Price<Position.EntryPrice){
				//	this.doSell(gmTrade,"破均价止损出场");
				}else if (gmTrade.DateTime.TimeOfDay>=new TimeSpan(14,55,0)){
					//超出持仓时间限制出场，在实盘时可提高资金的使用率
					/*if (this.positionCycle>=this.positionCycleLimit) 
					{							
						this.doSell(gmTrade,"限时出场");//可能是盈利的，也可能是亏损的	
					}*/
				}
			}
		}
	}
	protected void doSell(GMTrade gmTrade,string text){
		double slipPrice=gmTrade.Price*0.99;
		slipPrice=slipPrice<gmTrade.LowerLimit?gmTrade.LowerLimit:slipPrice;
		if (SellLimit(slipPrice,TimeInForce.Day,text)!=null){
			Console.WriteLine("证券 {0} 在 {1} 时 {2}",this.instrument.Symbol,Clock.Now,text);
			this.orderDone=true;
		}
	}
	//计算移动止盈价格
	protected void calcMoveStopPrice(){
		double multiple=this.positionMaxPrice/Position.EntryPrice-1;
		double factor=this.moveStopFactor-multiple*2;
		this.moveStopPrice=this.positionMaxPrice-this.moveStopATR*factor;
	}
	//计算绝对止损价格
	protected void calcStopLossPrice(){
		this.stopLossPrice=Position.EntryPrice-this.stopLossATR*this.stopLossFactor;
	}
	
	protected void judgeStagnant(){
		this.isStagnant=false;
		int end=this.DailyBar.LastIndex;
		if (end<5) return ;
		double maxPrice=double.MinValue;
		double minPrice=double.MaxValue;
		for(int i=end;i>=end-4;i--){
			if (this.DailyBar[i].High>maxPrice) maxPrice=this.DailyBar[i].High;
			if (this.DailyBar[i].Low<minPrice) minPrice=this.DailyBar[i].Low;
		}
		double inc=maxPrice-minPrice;
		if (inc<this.stopLossATR*1.2) this.isStagnant=true;
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
}