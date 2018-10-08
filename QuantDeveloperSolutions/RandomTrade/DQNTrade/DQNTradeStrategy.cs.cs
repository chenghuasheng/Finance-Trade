using System;
using System.Collections.Generic;
using System.Threading;
using SmartQuant;
using SmartQuant.Instruments;


public class DQNTradeStrategy:Strategy{
	public DQNTradeStrategy(string name, string description):base(name,description){
		this.ChangeMarketOnDay=true;//每天都改变市场，即每天交易对象是改变的	
	}
	public DQNTradeStrategy():this("DQNTradeStrategy","This is a strategy for DQN trading stocks"){}
	
	protected override void BehaviorInit(){	
		if (this.marketOpen){
			/*对已进入市场选择的股票添加入场对象*/
			//foreach(Instrument instrument in this.activeInstruments){	
			//	this.AddBehavior(instrument,new UpLimitEntry(instrument,this));	
			//}
			/*对投资组合中的已有股票头寸添加出场*/
			foreach(Position pos in this.Portfolio.Positions){
				this.AddBehavior(pos.Instrument,new DQNExit(pos.Instrument,this));
			}
		}
	}
	
	private AutoResetEvent autoEvent = new AutoResetEvent(true); 
	private List<JobSchedule> jobSchedules=new List<JobSchedule>();
	private List<ReminderEventHandler> reminderEvents=new List<ReminderEventHandler>();
	private List<DateTime> tradeDates=new List<DateTime>();
	protected override void MarketInit(){
		DateTime curDate=Clock.Now.Date;//策略当前运行时间
		DateTime jobDate=curDate;//作业内部时间,可以与当前运行时间不一致
		//DateTime jobDate=new DateTime(2018,9,18);
		Console.WriteLine("日期：{0}，当前作业日为 {1} ",Utils.FormatDate(curDate),Utils.FormatDate(jobDate));
		this.MarketOpen=true;
		
		//连接数据提供者
		DateTime beginDate=curDate.AddMonths(-2);
		DateTime endDate=curDate.AddMonths(2);
		string beginDateString=Utils.FormatDate(beginDate);
		string endDateString=Utils.FormatDate(endDate);
		//下面返回交易日历,确定是否为交易日
		
		this.tradeDates=Utils.GetTradeDates("SHSE",beginDateString,endDateString);	
		if (this.tradeDates.Count<=0) {
			this.tradeDates=Utils.GetTradeDatesLocal(beginDateString,endDateString);
		}
		int n=this.tradeDates.Count;
		int i=0;
		if (n>0) {
			while(i<n&&this.tradeDates[i]<jobDate) i++;
			if (i>=n||this.tradeDates[i]>jobDate){
				Console.WriteLine("作业日不是交易日。");
				this.MarketOpen=false;
				return;
			}
			if (i+1<n) 
				this.Global["NextTradeDate"]=Utils.FormatDate(this.tradeDates[i+1]);//下一个交易日
		}else{
			Console.WriteLine("交易日历为空。");
			return;
		}	
		//在实盘模式下需要等待头一天的作业完成，才能市场初始化，进行今天的交易，故而这里设置等待一个信号量
		if (this.StrategyMode==StrategyMode.Live){
			this.autoEvent.WaitOne();
		}
		
		if (this.StrategyMode==StrategyMode.Live){
			this.closeAllJobSchedules();
			//在实盘模式下，进行重连接市场数据提供者和交易提供者
			JobSchedule jbs1=new JobSchedule(this.buildReconnectJobs(),
				curDate.Add(new TimeSpan(9,0,0)),5,new TimeSpan(0,1,0));
			this.jobSchedules.Add(jbs1);
			
			//添加随机交易作业
			JobSchedule jbs3=new JobSchedule(new DQNTradeJob("DQN交易作业",jobDate,this),
				curDate.Add(new TimeSpan(10,20,0)),10,new TimeSpan(0,20,0));
			this.jobSchedules.Add(jbs3);
			
			//在实盘模式下，调度每天要进行额外的作业，作业完成后发出一个信号量，通知可以进行下一天的交易
			JobSchedule jbs5=new JobSchedule(this.buildDayJobs(jobDate),curDate.Add(new TimeSpan(21,30,0))
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
	private JobQueue buildDayJobs(DateTime jobDate){
		JobQueue jobs=new JobQueue();
		Job checkInstruments=new CheckInstrumentsJob("检查证券定义");		
		Job saveDailys=new SaveDailysJob("下载Daily数据",jobDate,new Job[] {checkInstruments});
		Job saveRandomTradeRecordsJob=new SaveRandomTradeRecordsJob("保存随机交易记录",jobDate,
			this,new Job[]{saveDailys});
		Job dqnTrainJob=new DQNTrainJob("神经网络训练",jobDate,this.tradeDates,new Job[]{saveRandomTradeRecordsJob});
		//Job dqnTrainJob=new DQNTrainJob("神经网络训练",jobDate,this.tradeDates);
		jobs.Add(checkInstruments);
		jobs.Add(saveDailys);
		jobs.Add(saveRandomTradeRecordsJob);
		jobs.Add(dqnTrainJob);
		return jobs;
	}
	
	//清理作业
	protected override void OnStrategyStop(){
		if (this.StrategyMode==StrategyMode.Live){
			this.closeAllJobSchedules();
		}	
	}
	private void closeAllJobSchedules(){
		foreach(JobSchedule jbs in this.jobSchedules) jbs.CloseJobs();
		this.jobSchedules.Clear();
	}
	
}