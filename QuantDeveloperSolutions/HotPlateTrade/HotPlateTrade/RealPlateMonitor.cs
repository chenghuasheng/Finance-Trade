using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Providers;
using HuaQuant.Data.GM;
public class RealPlateMonitor:PlateMonitor{
	private GMRealTimeProvider gmProvider=null;
	private bool isRunning=false;
	public RealPlateMonitor(IProvider provider,string eastMoneyPath,PlateMonitorForm monitorForm)
		:base(eastMoneyPath,monitorForm){
		if (provider is GMRealTimeProvider){
			this.gmProvider=(GMRealTimeProvider)provider;
		}
	}
	public RealPlateMonitor(IProvider provider,string eastMoneyPath)
		:this(provider,eastMoneyPath,null){}
	protected override void GetActiveSymbols(){
		this.activeSymbols.Clear();
		if (this.gmProvider!=null){		
			this.activeSymbols.AddRange(this.gmProvider.GetSymbols("SHSE", 1, 1));
			this.activeSymbols.AddRange(this.gmProvider.GetSymbols("SZSE", 1, 1));
		}
	}
	
	protected Dictionary<string,Trade> GetLastMarket(){
		//获取当天活动证券的最新行情
		Dictionary<string,Trade> tradeDict=new Dictionary<string,Trade>();	
		if (this.gmProvider!=null){
			tradeDict=this.gmProvider.GetLastTrades(this.activeSymbols);
		}
		return tradeDict;
			
	}
	private DateTime beginTime=Clock.Now.Date.Add(new TimeSpan(9,15,0));
	private DateTime endTime=Clock.Now.Date.Add(new TimeSpan(15,1,0));
	private int updateInterval=50;//更新时间间隔，以秒为单位
	private int timerInterval=1000;//计时器的时间间隔，以毫秒为单位
	private Thread updateThread=null;
	private System.Timers.Timer timer=new System.Timers.Timer();
	//启动板块统计更新
	public override void Begin(){
		base.Begin();
		Console.WriteLine("开始板块监控...");
		this.isRunning=true;
		this.update();
		this.timer.Interval=this.timerInterval;
		this.timer.Elapsed+=this.timer_elapsed;
		this.timer.Start();
	}
	//停止板块统计更新
	public override void End(){
		if (this.isRunning) {
			Console.WriteLine("结束板块监控...");
			this.timer.Stop();
			this.timer.Elapsed-=this.timer_elapsed;
			this.timeSpan=0;
			this.stopUpdate();
			this.isRunning=false;
		}
		base.End();
	}
	private double timeSpan=0;
	//计时器事件
	private void timer_elapsed(object sender,ElapsedEventArgs e){
		if (Clock.Now<this.beginTime||Clock.Now>this.endTime) return;
		this.timeSpan+=this.timer.Interval;
		if (this.timeSpan>=(this.updateInterval*1000)){
			this.update();
			this.timeSpan=0;
		}
	}
	private void update(){
		if (this.updateThread==null||
			this.updateThread.ThreadState==ThreadState.Aborted||
			this.updateThread.ThreadState==ThreadState.Stopped)
		{
			this.updateThread=new Thread(new ThreadStart(this.doUpdate));
			this.updateThread.Start();
		}	
	}
	private void doUpdate(){
		this.UpdatePlateStatistic(this.GetLastMarket());
		Console.WriteLine("结束一次更新.");
	}
	private void stopUpdate(){
		if ((this.updateThread!=null)&&(this.updateThread.ThreadState!=ThreadState.Aborted)&&(
		this.updateThread.ThreadState!=ThreadState.Stopped)){
			this.updateThread.Abort();
			/*--new--*/
			this.updateThread.Join();
			/*--new--*/
			this.updateThread=null;
		}	
	}
	
	public Daily[] GetDailyLine(string symbol,int n,string lastDateString){
		if (this.gmProvider!=null) {
			List<Daily> gmDailys=this.gmProvider.GetLastNDaily(symbol,n,lastDateString);
			Daily[] dailys=gmDailys.ToArray();
			Util.AdjustDailys(dailys);
			return dailys;
		}else return new Daily[0];
	}
	public Bar[] GetMinLine(string symbol,string beginTimeString,string endTimeString){
		if (this.gmProvider!=null) {
			List<Bar> gmMinBars=this.gmProvider.GetBars(symbol,60,beginTimeString,endTimeString);
			return gmMinBars.ToArray();
		}else return new Bar[0];
	}
}