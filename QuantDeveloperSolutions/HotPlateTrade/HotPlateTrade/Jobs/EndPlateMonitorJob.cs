using System;
public class EndPlateMonitorJob:Job{
	private RealPlateMonitor plateMonitor;
	public EndPlateMonitorJob(string name,RealPlateMonitor plateMonitor,Job[] needJobs):base(name,needJobs){
		this.plateMonitor=plateMonitor;
	}
	public EndPlateMonitorJob(string name,RealPlateMonitor plateMonitor):this(name,plateMonitor,null){
	}
	protected override bool doJob(){
		this.plateMonitor.End();
		return true;
	}
}