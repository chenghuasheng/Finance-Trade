using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Providers;
using SmartQuant.Simulation;
using HuaQuant.Data.GM;
public class SimulationPlateMonitor:PlateMonitor{
	private SimulationDataProvider sdProvider=null;
	public SimulationPlateMonitor(IProvider provider,string eastMoneyPath,PlateMonitorForm monitorForm)
		:base(eastMoneyPath,monitorForm){
		if (provider is SimulationDataProvider){
			this.sdProvider=(SimulationDataProvider)provider;
		}
	}
	public SimulationPlateMonitor(IProvider provider,string eastMoneyPath)
		:this(provider,eastMoneyPath,null){}
	protected Dictionary<string,Trade> GetMarketOnTime(DateTime time){
		Dictionary<string,Trade> tradeDict=new Dictionary<string,Trade>();
		if (this.sdProvider!=null) {
			tradeDict=this.sdProvider.GetLastTrades(this.activeSymbols.ToArray());
		}
		return tradeDict;
	}	
	
	public void Update(DateTime time){
		this.UpdatePlateStatistic(this.GetMarketOnTime(time));
		Console.WriteLine("结束时间点<{0}>的一次更新.",time);
	}
	public Daily[] GetDailyLine(string symbol, int n, DateTime lastDate){
		if (this.sdProvider!=null){
			List<Daily> gmDailys=this.sdProvider.GetLastNDaily(symbol,n,lastDate);
			Daily[] dailys=gmDailys.ToArray();
			Util.AdjustDailys(dailys);
			return dailys;
		}else return new Daily[0];
	}
	public Bar[] GetMinLine(string symbol,DateTime beginTime, DateTime endTime){
		if (this.sdProvider!=null){
			List<Bar> gmMinBars=this.sdProvider.GetBars(symbol,60,beginTime,endTime);
			return gmMinBars.ToArray();
		}else return new Bar[0];
	}
	
}