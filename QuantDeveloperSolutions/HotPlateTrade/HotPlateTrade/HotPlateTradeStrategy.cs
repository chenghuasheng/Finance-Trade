using System;
using System.Collections.Generic;
using System.Threading;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using HuaQuant.Data.GM;


public class HotPlateTradeStrategy:Strategy{
	private PlateMonitor plateMonitor;
	private TradeManager tradeManager;
	public HotPlateTradeStrategy(string name, string description,PlateMonitor plateMonitor)
		:base(name,description){
		this.ChangeMarketOnDay=true;//每天都改变市场，即每天交易对象是改变的
		this.plateMonitor=plateMonitor;
		this.tradeManager=new TradeManager();
	}
	public HotPlateTradeStrategy(PlateMonitor plateMonitor)
		:this("HotPlateTradeStrategy","This is a strategy for trading stocks in hot plate.",plateMonitor){}
	//模拟者初始化
	protected override void SimulationInit(){
		//this.SendMarketDataRequest("Bar.Time.60");
		//this.SendMarketDataRequest("Quote");
		this.SendMarketDataRequest("Trade");
	}
	//行为初始化，包括入场和出场的设置
	protected override void BehaviorInit(){	
		if (this.marketOpen){
			/*对已进入市场选择的股票添加入场对象*/
			//foreach(Instrument instrument in this.activeInstruments){	
			//	this.AddBehavior(instrument,new UpLimitEntry(instrument,this));	
			//}
			/*对投资组合中的已有股票头寸添加出场*/
			foreach(Position pos in this.Portfolio.Positions){
				this.AddBehavior(pos.Instrument,new StockExit(pos.Instrument,this,this.tradeManager));
			}
		}
	}
	//市场初始化
	private List<DateTime> tradeDates=new List<DateTime>();
	private DateTime lastTradeDate=new DateTime(1970,1,1);
	private AutoResetEvent autoEvent = new AutoResetEvent(true); 
	private List<JobSchedule> jobSchedules=new List<JobSchedule>();
	private List<ReminderEventHandler> reminderEvents=new List<ReminderEventHandler>();
	protected override void MarketInit(){
		DateTime curDate=Clock.Now.Date;//策略当前运行时间
		Console.WriteLine("日期："+curDate.ToString("yyyy-MM-dd"));
		this.MarketOpen=true;
		if (curDate>lastTradeDate){
			//连接数据提供者
			DateTime endDate=curDate.AddYears(1);
			//下面返回的交易日历是curDate到endDate前一天为止的交易日历
			this.tradeDates=this.getTradeDates("SHSE",curDate.ToString("yyyy-MM-dd"),endDate.ToString("yyyy-MM-dd"));
			lastTradeDate=endDate.AddDays(-1);
		}
		if ((this.tradeDates.Count>0)&&(!this.tradeDates.Contains(curDate))){
			Console.WriteLine("今天不是交易日");
			this.MarketOpen=false;
			//return;
		}
		//在实盘模式下需要等待头一天的作业完成，才能市场初始化，进行今天的交易，故而这里设置等待一个信号量
		if (this.StrategyMode==StrategyMode.Live){
			this.autoEvent.WaitOne();
		}
		/*添加市场并初始化*/		
		//Marketor marketor=new UpLimitStockMarketor(this);
		//marketor.OnInit();
				
		if (this.StrategyMode==StrategyMode.Live){
			this.closeAllJobSchedules();
			//在实盘模式下，进行重连接市场数据提供者和交易提供者
			JobSchedule jbs1=new JobSchedule(this.buildReconnectJobs(),
				curDate.Add(new TimeSpan(9,0,0)),5,new TimeSpan(0,1,0));
				//Clock.Now.AddSeconds(5),5,new TimeSpan(0,1,0));
			this.jobSchedules.Add(jbs1);
			if (this.plateMonitor is RealPlateMonitor) {
				RealPlateMonitor plateMonitor=(RealPlateMonitor)this.plateMonitor;
				//添加启动板块监控作业
				DateTime beginTime=curDate.Add(new TimeSpan(9,15,0));
				DateTime tempTime=Clock.Now.AddSeconds(5);
				if (tempTime>beginTime) beginTime=tempTime;
				
				JobSchedule jbs2=new JobSchedule(new BeginPlateMonitorJob("开始板块监控",plateMonitor),
					beginTime,5,new TimeSpan(0,1,0));
				this.jobSchedules.Add(jbs2);
			
				//添加结束板块监控作业
				JobSchedule jbs3=new JobSchedule(new EndPlateMonitorJob("结束板块监控",plateMonitor),
					curDate.Add(new TimeSpan(15,10,0)),5,new TimeSpan(0,1,0));
				this.jobSchedules.Add(jbs3);
				//添加选择股票入场作业
				JobSchedule jbs4=new JobSchedule(new EntryJob("买入证券作业",plateMonitor,this,this.tradeManager),
					curDate.Add(new TimeSpan(11,0,0)),10,new TimeSpan(0,20,0));
				this.jobSchedules.Add(jbs4);
			}
			//在实盘模式下，调度每天要进行额外的作业，作业完成后发出一个信号量，通知可以进行下一天的交易
			JobSchedule jbs5=new JobSchedule(this.buildDayJobs(),curDate.Add(new TimeSpan(22,40,0))
				,5,new TimeSpan(0,20,0),this.autoEvent);
			this.jobSchedules.Add(jbs5);	
		}
	}
	
	private JobQueue buildReconnectJobs(){
		JobQueue reconnectJobs=new JobQueue();
		Job reconnectMartketJob=new ReconnectProviderJob("重连接市场数据提供者",this.MarketDataProvider);
		Job reconnectTradeJob=new ReconnectProviderJob("重连接交易提供者",this.ExecutionProvider);
		reconnectJobs.Add(reconnectMartketJob);
		reconnectJobs.Add(reconnectTradeJob);
		return reconnectJobs;
	}
	//生成每日作业队列
	private JobQueue buildDayJobs(){
		JobQueue jobs=new JobQueue();
		DateTime curDate=Clock.Now.Date;
		//DateTime curDate=new DateTime(2017,12,5);
		Job checkInstruments=new CheckInstrumentsJob("检查证券定义");		
		Job saveDailys=new SaveDailysJob("下载Daily数据",curDate,new Job[] {checkInstruments});
		jobs.Add(checkInstruments);
		jobs.Add(saveDailys);
		return jobs;
	}
	
	//清理作业
	protected override void OnStrategyStop(){
		if (this.StrategyMode==StrategyMode.Live){
			this.closeAllJobSchedules();
			this.plateMonitor.End();
		}	
	}
	private void closeAllJobSchedules(){
		foreach(JobSchedule jbs in this.jobSchedules) jbs.CloseJobs();
		this.jobSchedules.Clear();
	}
	
	protected override bool Validate(Signal signal)
	{
		if (this.plateMonitor.WeightOfAll<11) {
			Console.WriteLine("市场热度不足");
			return false;
		}else {
			return true;
		}
	}
	//获取交易日历
	protected List<DateTime> getTradeDates(string market,string beginDate,string endDate)
	{
		GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
		provider.Connect(10000);
		List<GMSDK.TradeDate> tradeDates = provider.MdApi.GetCalendar(market, beginDate, endDate);
		List<DateTime> dates = new List<DateTime>();
		DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		foreach (GMSDK.TradeDate tradeDate in tradeDates)
		{
			dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
		}
		return dates;
	}
}