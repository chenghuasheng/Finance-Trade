using System;
public class Factor:Result{
	
	private double minVal;
	public double MinVal{
		get { return this.minVal;}
		set { this.minVal=value;}
	}
	private double maxVal;
	public double MaxVal{
		get { return this.maxVal;}
		set { this.maxVal=value;}
	}
	private double step;
	public double Step{
		get { return this.step;}
		set { this.step=value;}
	}

	public Factor():base(){
		this.minVal=1.0;
		this.maxVal=100.0;
		this.step=1.0;
	}
}
