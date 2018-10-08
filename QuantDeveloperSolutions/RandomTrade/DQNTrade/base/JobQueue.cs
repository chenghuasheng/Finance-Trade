using System;
using System.Collections.Generic;

public class JobQueue:List<Job>{
	public bool Finished {
		get{
			bool finished=true;
			foreach(Job job in this){
				if (!job.Finished){
					finished=false;
					break;
				}
			}
			return finished;
		}
	}
	private int times=0;
	public int Times{
		get {return this.times;}
	}
	public void Run(){
		foreach(Job job in this){
			job.Run();
		}
		this.times++;
	}
}