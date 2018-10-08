using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.File;
using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;
public static class Utils{
	//获取某个证券某天以前的N条日线数据
	public static ISeriesObject[] GetNDailysBeforeDate(Instrument inst,DateTime date,int n){
		FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
		if (dailySeries==null||dailySeries.Count<=0) return new ISeriesObject[]{};
		DateTime lastDate=date;
		int i;
		do {
			lastDate=lastDate.AddDays(-1);
			i=dailySeries.IndexOf(lastDate);
		}while(i<0&&lastDate>dailySeries.FirstDateTime);
		if (i<0) return new ISeriesObject[]{};
		int j=i-n+1>0?i-n+1:0;
		return dailySeries.GetArray(j,i);
	}
	/*获取某个证券日线的个数*/
	public static int GetDailyCountBeforeDate(Instrument inst,DateTime date){
		FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
		if (dailySeries==null||dailySeries.Count<=0) return 0;
		DateTime lastDate=date;
		int i;
		do {
			lastDate=lastDate.AddDays(-1);
			i=dailySeries.IndexOf(lastDate);
		}while(i<0&&lastDate>dailySeries.FirstDateTime);
		if (i<0) return 0;
		else return i-0+1;
	}
	//将日线向前复权
	public static void AdjustDailys(ISeriesObject[] gmDailys){//进行向前复权
		int num=gmDailys.Length;
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
	
	/*新增*/
	public static string FormatDate(DateTime date){
		return date.ToString("yyyy-MM-dd");
	}
	public static string FormatTime(DateTime time){
		return time.ToString("yyyy-MM-dd HH:mm:ss");
	}
	
	//获取交易日历
	public static List<DateTime> GetTradeDates(string market,string beginDate,string endDate)
	{
		GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
		provider.Connect(10000);
		List<GMSDK.TradeDate> tradeDates = provider.MdApi.GetCalendar(market, beginDate, endDate);
		List<DateTime> dates = new List<DateTime>();
		DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		foreach (GMSDK.TradeDate tradeDate in tradeDates)
		{
			dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
		}
		return dates;
	}
	public static List<DateTime> GetTradeDatesLocal(string beginDate,string endDate){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("TradeDates");
		BsonElement[] eleArray=new BsonElement[2];
		eleArray[0]=new BsonElement("$gte",beginDate);
		eleArray[1]=new BsonElement("$lte",endDate);
		QueryDocument query=new QueryDocument(eleArray);
		List<DateTime> dates=new List<DateTime>();
		foreach(BsonDocument record in collection.Find(query)){	
			dates.Add(record["Date"].AsDateTime);
		}
		return dates;
	}
}