using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.File;
using HuaQuant.Data.GM;
using MongoDB.Bson; 

public static class BarUtils{
	public static void CoverDailyFormTrade(Daily daily,GMTrade gmTrade){
		daily.DateTime=gmTrade.DateTime.Date;
		daily.High=gmTrade.High;
		daily.Open=gmTrade.Open;
		daily.Low=gmTrade.Low;
		daily.Close=gmTrade.Price;
		daily.Volume=(long)gmTrade.TotalSize;
		GMDaily gmDaily=daily as GMDaily;
		if (gmDaily!=null){
			gmDaily.LastClose=gmTrade.LastClose;
			gmDaily.Amount=gmTrade.Amount;
		}	
	}
	public static List<NormalBar> NormalBars(List<Daily> dailys){
		List<Bar> bars=new List<Bar>();
		foreach(Daily daily in dailys) bars.Add(daily);
		return NormalBars(bars);
	}
	public static List<NormalBar> NormalBars(List<Bar> bars){
		List<NormalBar> ret=new List<NormalBar>();
		double basePrice=0.0;
		double baseVol=0.0;
		foreach(Bar bar in bars){
			if (bar.High>basePrice) basePrice=bar.High;
			if (bar.Volume>baseVol) baseVol=bar.Volume;
		}
		foreach(Bar bar in bars){
			NormalBar nbar=new NormalBar();
			nbar.Close=bar.Close/basePrice;
			nbar.High=bar.High/basePrice;
			nbar.Low=bar.Low/basePrice;
			nbar.Open=bar.Open/basePrice;
			nbar.Volume=bar.Volume/baseVol;
			ret.Add(nbar);
		}
		return ret;
	}
	
	public static BsonArray BarsToBsonArray(List<NormalBar> nbars ){
		BsonArray wholeArray=new BsonArray(nbars.Count);
		foreach(NormalBar nbar in nbars){
			BsonArray subArray=new BsonArray(5);
			subArray.Add(nbar.Open);
			subArray.Add(nbar.High);
			subArray.Add(nbar.Low);
			subArray.Add(nbar.Close);
			subArray.Add(nbar.Volume);
			wholeArray.Add(subArray);
		}
		return wholeArray;
	}
	public static void AdjustDailys(List<Daily> gmDailys){//进行向前复权
		int num=gmDailys.Count;
		if (num>1){
			GMDaily lastDaily=(GMDaily)gmDailys[num-1];
			for(int i=num-2;i>=0;i--){
				GMDaily curDaily=(GMDaily)gmDailys[i];
				if (curDaily.AdjFactor!=lastDaily.AdjFactor){
					curDaily.Close=curDaily.Close*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.High=curDaily.High*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Low=curDaily.Low*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Open=curDaily.Open*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.LastClose=curDaily.LastClose*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Volume=(long)(curDaily.Volume*lastDaily.AdjFactor/curDaily.AdjFactor);
					curDaily.AdjFactor=lastDaily.AdjFactor;
				}
				lastDaily=curDaily;	
			}
		}
	}
	
	public static List<Daily> GetLastNDailys(Instrument inst,DateTime lastDate,int n){
		List<Daily> ret=new List<Daily>();
		FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
		if (dailySeries==null||dailySeries.Count<=0) return ret;
		int i=dailySeries.IndexOf(lastDate,SearchOption.Prev);
		if (i<0) return ret;
		int j=i-n+1>0?i-n+1:0;
		foreach(ISeriesObject isObj in dailySeries.GetArray(j,i)){
			ret.Add((Daily)isObj);
		}
		return ret;
	}
}