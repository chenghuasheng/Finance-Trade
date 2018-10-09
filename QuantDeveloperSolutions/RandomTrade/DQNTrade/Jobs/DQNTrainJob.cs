using System;
using System.Collections.Generic;

public class DQNTrainJob:Job{
	private DateTime endDate;
	private DateTime beginDate;
	public DQNTrainJob(string name,DateTime jobDate,List<DateTime> tradeDates,Job[] needJobs):base(name,needJobs){
		this.endDate=jobDate;
		//确定训练起止时间	
		this.beginDate=jobDate.AddDays(1-Const.DQNTrainDays);
		int i=tradeDates.IndexOf(jobDate);
		if (i>=0){
			int j=i-Const.DQNTrainDays+1;
			j=j<0?0:j;
			this.beginDate=this.beginDate>tradeDates[j]?tradeDates[j]:this.beginDate;
		}
	}
	public DQNTrainJob(string name,DateTime jobDate,List<DateTime> tradeDates):this(name,jobDate,tradeDates,null){}
	protected override bool doJob(){
		string beginDateString=Utils.FormatDate(this.beginDate);
		string endDateString=Utils.FormatDate(this.endDate);
		string args=string.Format("{0} {1} -n 4 -i True",beginDateString,endDateString);
		Console.WriteLine("开始训练：{0}",args);
		CMDAgent.RunPythonScript(@"E:\pyfiles\trade_dqn_v2.0\trade_learning.py",args);
		return true;
	}
}