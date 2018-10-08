using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;
using MongoDB.Bson;

public static class BarFeeder{
	private static GMRealTimeProvider provider=null;
	private static DateTime dealTime;
	private static DateTime dealDate;
	private static string dealDateString;
	private static string dealTimeString;
	
	private static Dictionary<string,List<Daily>> dailyDict=new Dictionary<string,List<Daily>>();
	private static Dictionary<string,List<Bar>> bar5Dict=new Dictionary<string,List<Bar>>();
	public static void Init(GMRealTimeProvider provider,DateTime dealTime){
		BarFeeder.provider=provider;
		BarFeeder.dealTime=dealTime;
		dealDate=dealTime.Date;
		dealDateString=Utils.FormatDate(dealDate);
		dealTimeString=Utils.FormatTime(dealTime);
		
	}
	public static void ClearBars(){
		dailyDict.Clear();
		bar5Dict.Clear();
	}
	public static List<Daily> GetDailys(string symbol,int n){
		return GetDailys(symbol,n,false,null);
	}
	public static List<Daily> GetDailys(string symbol,int n,bool needRevise,Trade lastTrade){
		List<Daily> dailys=null;
		if (!dailyDict.TryGetValue(symbol,out dailys)){
			dailys=GetLastNDailys(symbol,n);
			if (needRevise&&dailys.Count>=2){
				ReviseDailys(dailys,symbol,lastTrade);
			}
			dailyDict.Add(symbol,dailys);
		}
		return dailys;
	}
	public static List<Bar> GetBar5s(string symbol,int n){
		List<Bar> bars=null;
		if (!bar5Dict.TryGetValue(symbol,out bars)){
			bars=provider.GetLastNBars(symbol,300,n,dealTimeString);
			bar5Dict.Add(symbol,bars);
		}
		return bars;
	}
	//如果本地有日线，则先读取本地数据
	public static List<Daily> GetLastNDailys(string symbol,int n){
		List<Daily> dailys;
		Instrument inst=InstrumentManager.Instruments[symbol];
		if (inst!=null){
			dailys=BarUtils.GetLastNDailys(inst,dealDate,n);
		}else{
			dailys=provider.GetLastNDailys(symbol,n,dealDateString);
		}
		return dailys;
	}
	private static void ReviseDailys(List<Daily> dailys,string symbol,Trade lastTrade){
		GMTrade gmTrade=(GMTrade)lastTrade;
		//去掉当天日线后
		int m=dailys.Count;
		if (dailys[m-1].DateTime==dealDate){
			BarUtils.CoverDailyFormTrade(dailys[m-1],gmTrade);
		}else {
			dailys.RemoveAt(0);
			GMDaily newDaily=new GMDaily(dailys[m-2] as GMDaily);
			BarUtils.CoverDailyFormTrade(newDaily,gmTrade);
			dailys.Add(newDaily);
		}
		//向前复权
		BarUtils.AdjustDailys(dailys);
	}
}