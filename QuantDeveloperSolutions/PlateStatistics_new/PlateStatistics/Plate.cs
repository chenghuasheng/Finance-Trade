using System;
public class Plate{
	private string name;
	public string Name{
		get { return this.name;}
	}
	private int type;
	public int Type {
		get { return this.type;}
	}
	
	public Plate(string name,int type){
		this.name=name;
		this.type=type;
	}
}