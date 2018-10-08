using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.FIXData;
using SmartQuant.Series;
using SmartQuant.Indicators;
using SmartQuant.Instruments;
using SmartQuant.Execution;
using HuaQuant.Data.GM;
public class StockExit:Behavior{
	private int positionCycle=1;//持仓周期
	private int positionCycleLimit=3;//持仓周期限制
	private double positionMaxPrice=0;//持仓后最大价格
	private double stopLossPrice=0;//绝对止损价格
	private double stopLossFactor=0.08;//绝对止损因子  
	private double moveStopPrice=0;//移动止盈价格
	private double moveStopFactor=0.05;//移动止盈因子 
	private bool orderDone=false;//已下单
	private bool stopLossPriceReseted=false;
	private TradeManager tradeManager;
	private PriceBox lastNPrices;
	public StockExit(Instrument instrument,Strategy strategy,TradeManager tradeManager):base(instrument,strategy)
	{
		this.tradeManager=tradeManager;
		this.lastNPrices=new PriceBox(3);
	}
	public StockExit(Instrument instrument,Strategy strategy):this(instrument,strategy,null){}
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
			
			int n=this.DailyBar.Count;
			if (n>0){
				int i;
				for(i=n-1;i>=0;i--){
					if (this.DailyBar[i].DateTime<=Position.EntryDate.Date) break;
				}
				if (this.DailyBar[i].DateTime<Position.EntryDate.Date) i++;
				if (i<0) i=0;
				this.positionCycle=n-i+1;

				//入场后一天到今天的最大价格
				for(int j=i+1;j<n;j++){
					if (this.DailyBar[j].High>this.positionMaxPrice) this.positionMaxPrice=this.DailyBar[j].High;//入场后的最高价格，不包括入场那一天
				}
			}
			//入场后入场当天的最大价格
			double maxPrice=this.instrument.GetDoubleValue(18888);
			if (maxPrice>this.positionMaxPrice) this.positionMaxPrice=maxPrice;
			this.moveStopPrice=this.positionMaxPrice*(1-moveStopFactor);
			this.stopLossPrice=Position.EntryPrice*(1-this.stopLossFactor);
			
			Console.WriteLine("投资组合中的证券 {0} :",Position.Instrument.Symbol);
			Console.WriteLine("持仓周期是 {0},绝对止损价格是 {1},持仓后最大价格是 {2}",this.positionCycle,this.stopLossPrice,this.positionMaxPrice);	
		}
	}
	public override void OnNewTrade(Trade trade){
		
		if (HasPosition&&(!this.orderDone)&&trade.Size>0){
			GMTrade gmTrade=(GMTrade)trade;
			this.lastNPrices.Add(trade.Price);
			
			if (gmTrade.High>this.positionMaxPrice) {//更新历史最高值，并计算是否开始监控止盈
				this.positionMaxPrice=gmTrade.High;
				this.moveStopPrice=this.positionMaxPrice*(1-moveStopFactor);
			}
			if (gmTrade.Open<this.stopLossPrice){//开盘跌出止损价
				this.stopLossPrice=gmTrade.Open-0.01;
				this.stopLossPriceReseted=true;
			}
			if (this.positionCycle>1&&gmTrade.Price>gmTrade.LowerLimit) //T+1交易并且可以卖出
			{		
				double avgPrice=this.lastNPrices.AvgPrice;
				if (avgPrice<=this.stopLossPrice)   {
					this.doSell(gmTrade,"绝对止损出场");//是亏损的
				}else if ((avgPrice<=this.moveStopPrice)&&(gmTrade.Price>=Position.EntryPrice)) {
					this.doSell(gmTrade,"移动止盈出场");//盈利
				}else if (gmTrade.DateTime.TimeOfDay>=new TimeSpan(14,56,0)){
					//超出持仓时间限制出场，在实盘时可提高资金的使用率
					if (this.positionCycle>=this.positionCycleLimit) 
					{							
						this.doSell(gmTrade,"限时出场");//可能是盈利的，也可能是亏损的	
					}
					if (this.stopLossPriceReseted&&avgPrice<=Position.EntryPrice){
						this.doSell(gmTrade,"开盘跌出止损价延迟到此时出场");//亏损
					}
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
	//此处不能用OnOrderFilled事件，因为在这个事件中,Position已经销毁了
	public override void OnExecutionReport(SingleOrder order, ExecutionReport report)
	{
		if (report.OrdStatus==OrdStatus.Filled) {
			if (this.tradeManager!=null){
				DateTime entryDate=Position.EntryDate.Date;
				string symbol=this.instrument.Symbol;
				float reward=(float)(Position.GetPnLPercent());
				this.tradeManager.RecordAExitTrade(entryDate,symbol,reward);
			}
		}
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
	
	public override void Close(){
		this.instrument.SetDoubleValue(18888,this.positionMaxPrice);
		this.instrument.Save();
	}
}
public class PriceBox:Queue<double>{
	private int maxCount=0;
	public double AvgPrice{
		get {
			double s=0.0;
			int l=this.Count;
			if (l>0){			
				foreach(double price in this) s+=price;
				return s/l;
			}else {
				return double.MaxValue;
			}
		}
	}
	public PriceBox(int n):base(n){
		this.maxCount=n;
	}
	public void Add(double price){	
		while (this.Count>=this.maxCount){
			this.Dequeue();
		}
		this.Enqueue(price);
	}
}