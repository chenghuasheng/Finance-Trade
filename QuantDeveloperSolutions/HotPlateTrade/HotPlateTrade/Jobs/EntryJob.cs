using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;
//using GMSDK;
public class EntryJob:Job{
	private RealPlateMonitor plateMonitor;
	private Strategy strategy;
	private TradeManager tradeManager;
	public EntryJob(string name,RealPlateMonitor plateMonitor,Strategy strategy,TradeManager tradeManager,Job[] needJobs):base(name,needJobs){
		this.plateMonitor=plateMonitor;
		this.strategy=strategy;
		this.tradeManager=tradeManager;
	}
	public EntryJob(string name,RealPlateMonitor plateMonitor,Strategy strategy,TradeManager tradeManager)
		:this(name,plateMonitor,strategy,tradeManager,null){}
	public EntryJob(string name,RealPlateMonitor plateMonitor,Strategy strategy)
		:this(name,plateMonitor,strategy,null,null){}
	
	protected override bool doJob(){
		bool ret=false;
		List<Plate> topHotPlates=plateMonitor.GetTopNHotPlates(2,17);
		List<Stock> stocks = new List<Stock>();
		foreach (Plate curPlate in topHotPlates)
		{
			Console.WriteLine("版块：{0}",curPlate.Name);
			foreach(Stock curStock in curPlate.Stocks){
				if (curStock.Price<=0) continue;//没有价格的除掉
				if (curStock.IncPercent<1||curStock.IncPercent>=5) continue;//去掉不涨的和已经涨得好高的
				if (!stocks.Contains(curStock)) stocks.Add(curStock);
			}
		}
		stocks.Sort(delegate(Stock s1,Stock s2){
				return s2.IncPercent.CompareTo(s1.IncPercent);
			});
		DateTime lastDate=Clock.Now.Date.AddDays(-1);
		DateTime beginTime=Clock.Now.Date.Add(new TimeSpan(9,30,0));
		DateTime endTime=Clock.Now;
		string lastDateString=lastDate.ToString("yyyy-MM-dd");
		string beginTimeString=beginTime.ToString("yyyy-MM-dd HH:mm:ss");
		string endTimeString=endTime.ToString("yyyy-MM-dd HH:mm:ss");
		
		foreach(Stock curStock in stocks){
			Daily[] gmDailys=this.plateMonitor.GetDailyLine(curStock.Symbol,20,lastDateString);
			RecognitionState dailyLineState=this.plateMonitor.GetDailyLineState(gmDailys);
			Bar[] gmMinBars=this.plateMonitor.GetMinLine(curStock.Symbol,beginTimeString,endTimeString);
			RecognitionState minLineState=this.plateMonitor.GetMinLineState(gmMinBars);
			
			if (!((dailyLineState.Shape==ShapeState.Rise)
			||(dailyLineState.Shape==ShapeState.RiseAfterFall&&dailyLineState.Slope==SlopeState.Steep))) continue;
			if (minLineState.Shape==ShapeState.Fall|| minLineState.Shape==ShapeState.RiseAfterFall) continue;
			if (minLineState.Shape==ShapeState.FallAfterRise&&minLineState.Speed==SpeedState.Rapid) continue;
			SmartQuant.Instruments.Instrument inst=InstrumentManager.Instruments[curStock.Symbol];
			
			if (inst!=null){
				this.strategy.AddBehavior(inst,new StockEntry(inst,dailyLineState,minLineState,this.tradeManager,this.strategy));
				ret=true;
			}
				
		}
		return ret;
	}
}