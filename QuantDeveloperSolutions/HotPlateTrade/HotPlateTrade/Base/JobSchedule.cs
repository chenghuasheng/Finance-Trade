using System;
using SmartQuant;
using System.Threading;

public class JobSchedule{
	private Thread jobThread=null;
	private JobQueue jobs=null;
	private ReminderEventHandler jobEventHandler=null;
	private DateTime startTime=new DateTime(1970,1,1);
	private int maxTimes=10;
	private TimeSpan interval=new TimeSpan(0,30,0);
	private AutoResetEvent autoEvent = null;
	private bool closed=false;
	public JobSchedule(JobQueue jobs,DateTime startTime,int maxTimes,TimeSpan interval,AutoResetEvent autoEvent){
		this.jobs=jobs;
		this.startTime=startTime;
		this.maxTimes=maxTimes;
		this.interval=interval;
		this.autoEvent=autoEvent;
		this.jobEventHandler=new ReminderEventHandler(this.doJobs);
		if (this.startTime>Clock.Now) Clock.AddReminder(this.jobEventHandler,startTime,null);
		else {
			Console.WriteLine("作业开始时间必须大于当前时间！");
			if (this.autoEvent!=null) this.autoEvent.Set();//在不能开启作业的情况下，释放一次信号量
		}
	}
	public JobSchedule(JobQueue jobs,DateTime startTime,int maxTimes,TimeSpan interval):this(jobs,startTime,maxTimes,interval,null){}
	public JobSchedule(Job job,DateTime startTime,int maxTimes,TimeSpan interval,AutoResetEvent autoEvent):this(new JobQueue(),startTime,maxTimes,interval,autoEvent){
		this.jobs.Add(job);
	}
	public JobSchedule(Job job,DateTime startTime,int maxTimes,TimeSpan interval):this(job,startTime,maxTimes,interval,null){}
	private void doJobs(ReminderEventArgs args){
		if (this.jobs!=null){
			this.jobThread=new Thread(new ThreadStart(this.jobs.Run));
			this.jobThread.Start();
			this.jobThread.Join();
			if (this.jobThread.ThreadState==ThreadState.Aborted||this.closed){
				if (this.autoEvent!=null) this.autoEvent.Set();
				Console.WriteLine("作业被中断执行。");
				return;
			}
			//如果没有完成且小于偿试的次数，N分钟后继续启动
			if((!this.jobs.Finished)&&(this.jobs.Times<maxTimes)){
				Clock.AddReminder(this.jobEventHandler,Clock.Now.Add(interval),null);
			}else {
				if (this.autoEvent!=null) this.autoEvent.Set();
				if (this.jobs.Finished) Console.WriteLine("所有作业执行完毕");
				else if (this.jobs.Times>=maxTimes) Console.WriteLine("偿试多次后仍然无法完成。");
			}	
		}
	}
	public void CloseJobs(){
		if ((this.jobThread!=null)&&
		(this.jobThread.ThreadState!=ThreadState.Aborted)&&(this.jobThread.ThreadState!=ThreadState.Stopped)){
			this.jobThread.Abort();
			/*--new--*/
			this.jobThread.Join();
			/*--new--*/
		}
		if (this.jobEventHandler!=null){
			Clock.RemoveReminder(this.jobEventHandler);
		}
		this.jobs.Clear();
		this.closed=true;
	}
}