using System;
using System.Collections.Generic;
using SmartQuant;

public class Job{
	private string name;
	public string Name{
		get { return this.name;}
	}
	private bool finished=false;
	public bool Finished{
		get { return this.finished;}
	}
	private Job[] needJobs=null;
	public Job(string name,Job[] needJobs){
		this.name=name;
		this.needJobs=needJobs;
	}
	public Job(string name):this(name,null){}
	public  void Run(){
		if (this.finished) return;
		Console.WriteLine("在时间{0},开始作业<{1}>的执行...",Clock.Now,this.name);
		if (this.needJobs!=null){
			bool canDo=true;
			foreach(Job job in this.needJobs){
				if (!job.Finished){
					canDo=false;
					break;
				}
			}
			if (!canDo) {
				Console.WriteLine("本作业的先行作业没有完成，本作业无法启动。");
				return;
			}
		}
		if (this.doJob()){
			this.finished=true;
			Console.WriteLine("在时间{0},作业<{1}>顺利完成。",Clock.Now,this.name);
		}else {
			this.finished=false;
			Console.WriteLine("在时间{0},作业<{1}>发生故障，无法完成。",Clock.Now,this.name);
		}
	}
	protected virtual bool doJob(){
		return true;
	}
}