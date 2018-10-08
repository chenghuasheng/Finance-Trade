using System;
using System.Collections.Generic;
using System.Threading;

using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using HuaQuant.Data.GM;

public class LeadingStrategy:Strategy{
	
	public LeadingStrategy(string name, string description):base(name,description){
		this.ChangeMarketOnDay=true;//每天都改变市场，即每天交易对象是改变的
	}
	public LeadingStrategy():this("LeadingStrategy","This is a strategy for trading leading stock."){}
	
	//模拟者初始化
	protected override void SimulationInit(){
		//this.SendMarketDataRequest("Bar.Time.60");
		//this.SendMarketDataRequest("Quote");
		this.SendMarketDataRequest("Trade");
	}
	//行为初始化，包括入场和出场的设置
	protected override void BehaviorInit(){	
		if (this.marketOpen){
			/*使用创业板指数进行风险控制*/
			Instrument indexInst=InstrumentManager.Instruments["SZSE.399006"];//创业板指数
			if (indexInst!=null){				
				Console.WriteLine("风控使用指数:{0}",indexInst.Symbol);
				this.AddBehavior(indexInst,new Risk(indexInst,this));
			}
			foreach(Instrument instrument in this.activeInstruments){	
				Console.WriteLine("考察证券:{0}",instrument.Symbol);
				this.AddBehavior(instrument,new UpLimitEntry(instrument,this));	
			}
			foreach(Position pos in this.Portfolio.Positions){
				this.AddBehavior(pos.Instrument,new StopExitNew(pos.Instrument,this));
			}
			//如果有需要，将指数添加进活动证券，用于订阅指数行情
			this.AddInstrument(indexInst);
		}
	}
	//市场初始化
	private List<DateTime> tradeDates=new List<DateTime>();
	private DateTime lastTradeDate=new DateTime(1970,1,1);
	private AutoResetEvent autoEvent = new AutoResetEvent(true); 
	private JobSchedule jobSchedule=null;
	protected override void MarketInit(){
		DateTime curDate=Clock.Now.Date;//策略当前运行时间
		Console.WriteLine("日期："+curDate.ToString("yyyy-MM-dd"));
		this.MarketOpen=true;
		if (curDate>lastTradeDate){
			//连接数据提供者
			DateTime endDate=curDate.AddYears(1);
			//下面返回的交易日历是curDate到endDate前一天为止的交易日历
			this.tradeDates=this.GetTradeDates("SHSE",curDate.ToString("yyyy-MM-dd"),endDate.ToString("yyyy-MM-dd"));
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
		if (this.marketors.Count<=0){
			this.AddMarketor(new UpLimitStockMarketor(this));
		}
		foreach(Marketor marketor in this.marketors){
			marketor.OnInit();
		}
		if (Portfolio.Positions.Count>1){
			//调换持仓，卖出最差的同时买入最佳的
			//new JobSchedule(this.buildChangePositionJobs(),curDate.Add(new TimeSpan(16,02,0))
			//	,5,new TimeSpan(0,0,30));
		}
		
		if (this.StrategyMode==StrategyMode.Live){
			//在实盘模式下，进行重连接市场数据提供者和交易提供者
			//new JobSchedule(this.buildReconnectJobs(),curDate.Add(new TimeSpan(9,0,0))
			//	,5,new TimeSpan(0,1,0));
			
			//在实盘模式下，调度每天要进行额外的作业，作业完成后发出一个信号量，通知可以进行下一天的交易
			if (this.jobSchedule!=null) this.jobSchedule.CloseJobs();
			this.jobSchedule=new JobSchedule(this.buildJobs(),curDate.Add(new TimeSpan(16,38,0))
				,5,new TimeSpan(0,20,0),this.autoEvent);	
		}
	}
	
	private JobQueue buildChangePositionJobs(){
		JobQueue changePositionJobs=new JobQueue();
		Job sellBadestPositionJob=new SellBadestPositionJob("卖出最差持仓",this);
		Job buyGoodestPositionJob=new BuyGoodestPositionJob("买入最佳持仓",this);
		changePositionJobs.Add(sellBadestPositionJob);
		changePositionJobs.Add(buyGoodestPositionJob);
		return changePositionJobs;
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
	private JobQueue buildJobs(){
		JobQueue jobs=new JobQueue();
		DateTime curDate=Clock.Now.Date;
		//DateTime curDate=new DateTime(2017,12,5);
		Job checkInstruments=new CheckInstrumentsJob("检查证券定义");
		Job saveTicks=new SaveTicksJob("下载Tick数据",curDate,new Job[] {checkInstruments});
		Job saveDailys=new SaveDailysJob("下载Daily数据",curDate,new Job[] {checkInstruments});
		Job checkTicks=new CheckTicksJob("检查Tick数据",curDate,new Job[]{saveTicks,saveDailys});
		Job buildUpLimits=new BuildUpLimitsJob("生成涨停数据表",curDate,new Job[]{checkTicks});
		jobs.Add(checkInstruments);
		jobs.Add(saveTicks);
		jobs.Add(saveDailys);
		jobs.Add(checkTicks);
		jobs.Add(buildUpLimits);
		return jobs;
	}
	
	//清理作业
	protected override void OnStrategyStop(){
		if (this.StrategyMode==StrategyMode.Live){
			if (this.jobSchedule!=null) this.jobSchedule.CloseJobs();	
		}
	}
	
	//全局风控
	protected override bool Validate(Signal signal)
	{
		switch (signal.Side)
		{
			case SignalSide.Buy:		
			case SignalSide.SellShort:
				if (this.CheckBuyPower) {
					if (this.Portfolio.GetPositionValue()+signal.Qty*signal.Instrument.Price()>this.Portfolio.GetTotalEquity()*this.PositionLevel)//仓位控制
						return false;
				}
				break;
			case SignalSide.Sell:		
			case SignalSide.BuyCover:
				break;	
		}
		return base.Validate(signal);
	}	
	
	protected List<DateTime> GetTradeDates(string market,string beginDate,string endDate)
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