using System;
public class BeginPlateMonitorJob:Job{
	private RealPlateMonitor plateMonitor;
	public BeginPlateMonitorJob(string name,RealPlateMonitor plateMonitor,Job[] needJobs):base(name,needJobs){
		this.plateMonitor=plateMonitor;
	}
	public BeginPlateMonitorJob(string name,RealPlateMonitor plateMonitor):this(name,plateMonitor,null){
	}
	protected override bool doJob(){
		this.plateMonitor.Begin();
		return true;
	}
}