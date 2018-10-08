using System;
using System.Collections.Generic;
public class StatisItem {
	private Dictionary<string,DateTime> securities= new Dictionary<string,DateTime>();
	public int Count{
		get { return this.securities.Count;}
	}
	public Dictionary<string,DateTime> Securities{
		get {return this.securities;}
	}
	
	public StatisItem(){
		this.securities=new Dictionary<string,DateTime>();
	}
	public void AddSymbol(string symbol,DateTime datetime){
		if (!this.securities.ContainsKey(symbol)) {
			this.securities.Add(symbol,datetime);
		}
	}
	public void RemoveSymbol(string symbol){
		if (this.securities.ContainsKey(symbol)){
			this.securities.Remove(symbol);
		}
	}
}