using System;
using System.IO;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;

public class StartUp
{
	private static GMSDK.MdApi _md=GMSDK.MdApi.Instance;
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		DateTime beginDate=new DateTime(2017,3,24);
		DateTime endDate=new DateTime(2017,3,25);
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance");
		
		string username="tianzhong_live@126.com";
		string password="Chs771005";
		
		int ret = _md.Init(username,password,GMSDK.MDMode.MD_MODE_NULL,"","","");
		if (ret != 0)
		{
			string msg = _md.StrError(ret);
			Console.WriteLine(msg);
			return;
		}
		
		DateTime curDate=beginDate;
		//获取交易日历，以确定是否开市日
		string dateString1="";
		if (endDate.AddMonths(-1)>beginDate) dateString1=beginDate.ToString("yyyy-MM-dd");
		else dateString1=endDate.AddMonths(-1).ToString("yyyy-MM-dd");
		string dateString2=endDate.AddDays(1).ToString("yyyy-MM-dd");
		List<DateTime> tradeDates=GetTradeDates("SHSE",dateString1,dateString2);
		
		while(curDate<endDate){
			
			Console.WriteLine("日期："+curDate.ToString("yyyy-MM-dd"));			
			if ((tradeDates.Count<=0)||(!tradeDates.Contains(curDate))){
				Console.WriteLine("今天不是交易日");
				curDate=curDate.AddDays(1);
				continue;
			}
			string monthString=curDate.ToString("yyyy-MM");
			//区域统计数据准备
			MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Areas."+monthString);
			Dictionary<string,List<string>> areaDict=new Dictionary<string,List<string>>();	
			foreach (BsonDocument document in collection.FindAll())
			{
				List<string> ls=null;
				if (!areaDict.TryGetValue((string)document["code"],out ls)){
					ls=new List<string>();
					areaDict.Add((string)document["code"],ls);
				}
				if (!(document["area"] is BsonNull)){
					ls.Add((string)document["area"]);
				}
			}
			Dictionary<string,StatisItem> areaUpLimitStatis=new Dictionary<string,StatisItem>();
			
			//行业统计数据准备
			collection = database.GetCollection<BsonDocument>("Industries."+monthString);
			Dictionary<string,List<string>> industryDict=new Dictionary<string,List<string>>();	
			foreach (BsonDocument document in collection.FindAll())
			{
				List<string> ls=null;
				if (!industryDict.TryGetValue((string)document["code"],out ls)){
					ls=new List<string>();
					industryDict.Add((string)document["code"],ls);
				}
				if (!(document["c_name"] is BsonNull)){
					ls.Add((string)document["c_name"]);
				}
			}
			Dictionary<string,StatisItem> industryUpLimitStatis=new Dictionary<string,StatisItem>();
		
			//概念统计数据准备
			collection = database.GetCollection<BsonDocument>("Concepts."+monthString);
			Dictionary<string,List<string>> conceptDict=new Dictionary<string,List<string>>();	
			foreach (BsonDocument document in collection.FindAll())
			{
				List<string> ls=null;
				if (!conceptDict.TryGetValue((string)document["code"],out ls)){
					ls=new List<string>();
					conceptDict.Add((string)document["code"],ls);
				}
				if (!(document["c_name"] is BsonNull)){
					ls.Add((string)document["c_name"]);
				}
			}
			Dictionary<string,StatisItem> conceptUpLimitStatis=new Dictionary<string,StatisItem>();
			
			//找出日线当天接近涨停股
			List<Instrument> toUpLimit=new List<Instrument>();		
			foreach(Instrument instrument in InstrumentManager.Instruments){
				if (instrument.SecurityType=="CS"&&instrument.SecurityDesc.IndexOf('B')<0) {
					DailySeries series=instrument.GetDailySeries(curDate.AddMonths(-3),curDate);
					//下面这里要求日线数大于20，是为了排除新股
					if (series.Count>20){		
						GMDaily gmDaily=(GMDaily)series[series.Count-1];//最近一天的日线
						if (gmDaily.Date!=curDate) continue;
						//下面是对于st,*st的，因为名字判断不准确，故去掉
						//if ((instrument.SecurityDesc.IndexOf("ST")>=0||instrument.SecurityDesc.IndexOf('S')>=0)
						//&&(gmDaily.Close/gmDaily.LastClose>1.044)){
						//	toUpLimit.Add(instrument);
						//}
						//下面是对于普通股
						if (gmDaily.Close/gmDaily.LastClose>1.094) {
							
							toUpLimit.Add(instrument);
						}	
					}
				}
			}
			//读出Trade数据，确定是否真的涨停及涨停时间，交统计数据
			string dataPath="e:/QDData";
			string path=dataPath+"/"+curDate.Year.ToString()+"/"+curDate.Month.ToString();
			if (!Directory.Exists(path)){
				Console.WriteLine("Trade数据目录不存在！");
				return;
			}
			try {
				DataFile file=DataFile.Open(path);
				try {
					foreach(Instrument instrument in toUpLimit){					
						string name=instrument.Symbol+".Trade";
						if (!file.Series.Contains(name)) {
							throw new Exception(instrument.Symbol+"的Trade数据不存在，请补充完整。");
						}
						FileSeries series = file.Series[name];
						ISeriesObject[] trades=series.GetArray(curDate,curDate.AddDays(1));
						if (trades.Length<=0) continue;
						GMTrade gmLastTrade=(GMTrade)trades[trades.Length-1];//最后一笔交易数据
						if (gmLastTrade.Price<gmLastTrade.UpperLimit) continue;
						foreach(ISeriesObject aTrade in trades){
							GMTrade gmTrade=(GMTrade)aTrade;
							if (gmTrade.Price==gmTrade.UpperLimit&&gmTrade.DateTime>=curDate.Add(new TimeSpan(9,25,0))) {
								Console.WriteLine(instrument.Symbol+instrument.SecurityDesc+"在"+gmTrade.DateTime+"时涨停");
								//区域统计
								if (areaDict.ContainsKey(instrument.SecurityID)) {							
									List<string> areaList=areaDict[instrument.SecurityID];
									foreach(string area in areaList){
										StatisItem si=null;
										if (!areaUpLimitStatis.TryGetValue(area,out si)) {
											si=new StatisItem();
											areaUpLimitStatis.Add(area,si);
										}
										si.AddSymbol(instrument.Symbol,gmTrade.DateTime);
									}
								}
								//行业统计
								if (industryDict.ContainsKey(instrument.SecurityID)) {	
									List<string> industryList=industryDict[instrument.SecurityID];
									foreach(string industry in industryList){
										StatisItem si=null;
										if (!industryUpLimitStatis.TryGetValue(industry,out si)) {
											si=new StatisItem();
											industryUpLimitStatis.Add(industry,si);
										}
										si.AddSymbol(instrument.Symbol,gmTrade.DateTime);
									}
								}
								//概念统计
								if (conceptDict.ContainsKey(instrument.SecurityID)) {
									List<string> conceptList=conceptDict[instrument.SecurityID];
									foreach(string concept in conceptList){
										StatisItem si=null;
										if (!conceptUpLimitStatis.TryGetValue(concept,out si)) {
											si=new StatisItem();
											conceptUpLimitStatis.Add(concept,si);
										}
										si.AddSymbol(instrument.Symbol,gmTrade.DateTime);
									}
								}
								break;
							}
						}
					}
				
				}catch(Exception ex){
					throw ex;
				}
				finally{
					file.Close();
				}
				//区域统计写入数据库
				MongoCollection<BsonDocument> collection1 = database.GetCollection<BsonDocument>("AreasUpLimitStatis");
				BsonElement[] eleArray = new BsonElement[1];
				eleArray[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
				QueryDocument query = new QueryDocument(eleArray);
				collection1.Remove(query);
				foreach(KeyValuePair<string,StatisItem> kvp in areaUpLimitStatis){	
					BsonElement[] eleArray1 = new BsonElement[4];
					eleArray1[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
					eleArray1[1] = new BsonElement("area", kvp.Key);
					eleArray1[2] = new BsonElement("count", kvp.Value.Count);
					
					BsonArray bsonArray=new BsonArray(kvp.Value.Count);
					foreach(KeyValuePair<string,DateTime> kvp1 in kvp.Value.Securities){
						bsonArray.Add(new BsonDocument(new BsonElement[2] { new BsonElement("symbol",kvp1.Key),
							new BsonElement("time",kvp1.Value.ToString("yyyy-MM-dd HH:mm:ss"))}));
					}
					eleArray1[3] = new BsonElement("securities", bsonArray);
					BsonDocument insert=new BsonDocument(eleArray1);
					collection1.Insert(insert);
				}
			
				//行业统计写入数据库
				MongoCollection<BsonDocument> collection2 = database.GetCollection<BsonDocument>("IndustriesUpLimitStatis");
				collection2.Remove(query);
				foreach(KeyValuePair<string,StatisItem> kvp in industryUpLimitStatis){	
					BsonElement[] eleArray1 = new BsonElement[4];
					eleArray1[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
					eleArray1[1] = new BsonElement("industry", kvp.Key);
					eleArray1[2] = new BsonElement("count", kvp.Value.Count);
					
					BsonArray bsonArray=new BsonArray(kvp.Value.Count);
					foreach(KeyValuePair<string,DateTime> kvp1 in kvp.Value.Securities){
						bsonArray.Add(new BsonDocument(new BsonElement[2] { new BsonElement("symbol",kvp1.Key),
							new BsonElement("time",kvp1.Value.ToString("yyyy-MM-dd HH:mm:ss"))}));
					}
					eleArray1[3] = new BsonElement("securities", bsonArray);
					BsonDocument insert=new BsonDocument(eleArray1);
					collection2.Insert(insert);
				}
				//概念统计写入数据库
				MongoCollection<BsonDocument> collection3 = database.GetCollection<BsonDocument>("ConceptsUpLimitStatis");
				collection3.Remove(query);
				foreach(KeyValuePair<string,StatisItem> kvp in conceptUpLimitStatis){	
					BsonElement[] eleArray1 = new BsonElement[4];
					eleArray1[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
					eleArray1[1] = new BsonElement("concept", kvp.Key);
					eleArray1[2] = new BsonElement("count", kvp.Value.Count);
					
					BsonArray bsonArray=new BsonArray(kvp.Value.Count);
					foreach(KeyValuePair<string,DateTime> kvp1 in kvp.Value.Securities){
						bsonArray.Add(new BsonDocument(new BsonElement[2] { new BsonElement("symbol",kvp1.Key),
							new BsonElement("time",kvp1.Value.ToString("yyyy-MM-dd HH:mm:ss"))}));
					}
					eleArray1[3] = new BsonElement("securities", bsonArray);
					BsonDocument insert=new BsonDocument(eleArray1);
					collection3.Insert(insert);
				}
			}catch(Exception ex){
				Console.WriteLine(ex.Message);
			}
			curDate=curDate.AddDays(1);
		}
	}
	
	public static List<DateTime> GetTradeDates(string market,string beginDate,string endDate)
	{
		List<GMSDK.TradeDate> tradeDates = _md.GetCalendar(market, beginDate, endDate);
		List<DateTime> dates = new List<DateTime>();
		DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		foreach (GMSDK.TradeDate tradeDate in tradeDates)
		{
			dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
		}
		return dates;
	}
}