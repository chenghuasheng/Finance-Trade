using System;
public class SaveTradeRecordJob:Job{
	private Strategy strategy;
	public SaveTradeRecordJob(string name,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.strategy=strategy;
	}
	public SaveTradeRecordJob(string name,Strategy strategy):this(name,strategy,null){}
	protected override bool doJob(){
		bool ret=false;
		foreach(Behavior behavior in this.strategy.Behaviors){
			if (behavior is RandomBehavior){
				((RandomBehavior)behavior).Save();
			}
		}
		ret=true;
		return ret;
	}
}