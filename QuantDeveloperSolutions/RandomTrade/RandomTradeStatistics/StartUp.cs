using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Simulation;
using SmartQuant.Providers;
using SmartQuant.Data;
using SmartQuant.Series;

using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;

public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		
		int dailyPeriod=40;//日线周期
		int min5Period=10;//分线周期
		int indexDailyPeriod=10;
		int indexMin5Period=10;
		int holdingPeriodLimit=5;//持仓周期限制
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		//读取交易日期
		List<DateTime> dates=new List<DateTime>();
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("TradeDates");
		foreach(BsonDocument record in collection.FindAll()){
			DateTime date=DateTime.Parse(record["Date"].ToString());
			dates.Add(date);
		}
		//
		collection = database.GetCollection<BsonDocument>("RandomTradeStatistics");
		SimulationDataProvider sdp=(SimulationDataProvider)ProviderManager.MarketDataSimulator;
		List<string> activeSymbols=new List<String>();
		activeSymbols.AddRange(sdp.GetSymbols("SHSE",1));
		activeSymbols.AddRange(sdp.GetSymbols("SZSE",1));
	
		DateTime beginDate=new DateTime(2017,12,11);
		DateTime endDate=new DateTime(2017,12,31);
		DateTime curDate=beginDate;
		Dictionary<string,RandomTradeRecord> randomTradeRecordDict=new Dictionary<string,RandomTradeRecord>();
		while(curDate<=endDate){
			//sdp.Connect();
			string curDateString=curDate.ToString("yyyy-MM-dd");
			Console.WriteLine("当前日期是{0}",curDateString);
			int i=dates.IndexOf(curDate);
			if (i<0) {
				Console.WriteLine("今天不是交易日。");
			}else {
				DateTime nextTradeDate=curDate.AddDays(1);
				if (i+1<dates.Count) nextTradeDate=dates[i+1];
				string nextTradeDateString=nextTradeDate.ToString("yyyy-MM-dd");
				
				DateTime dealTime=curDate.Add(new TimeSpan(10,20,0));
				DateTime closeTime=curDate.Add(new TimeSpan(15,3,0));
				
				//处理已有的头寸
				QueryDocument query=new QueryDocument(new BsonElement("NextTradeDate",curDateString));
				foreach(BsonDocument record in collection.Find(query))
				{
					RandomTradeRecord aTradeRecord=new RandomTradeRecord();
					aTradeRecord._id=record["_id"].ToString();
					aTradeRecord.Symbol=record["Symbol"].ToString();
					aTradeRecord.HoldingPeriod=(int)record["HoldingPeriod"]+1;
					randomTradeRecordDict.Add(aTradeRecord.Symbol,aTradeRecord);
				}
	
				//获取最新价格，排序，然后随机选择入场
				
				Dictionary<String,Trade> lastTrades=sdp.GetLastTrades(activeSymbols.ToArray(),dealTime);
				
				List<string> symbols=new List<string>();
				foreach(KeyValuePair<String,Trade> kvp in lastTrades){
					if (kvp.Value==null) continue;
					GMTrade gmTrade=(GMTrade)kvp.Value;
					if (gmTrade.Price<=0) continue;
					if (gmTrade.Price==gmTrade.UpperLimit) continue;
					if (gmTrade.Price/gmTrade.LastClose-1<0) continue;
					if (randomTradeRecordDict.ContainsKey(kvp.Key)) continue;
					symbols.Add(kvp.Key);
				}
				int total=symbols.Count;
				
				Random randomer=new Random((int)DateTime.Now.Ticks);
				int[] randomNumbers=new int[100];
				for(int k=0;k<100;k++) randomNumbers[k]=randomer.Next(total);
				foreach(int r in randomNumbers){
					string symbol=symbols[r];
					if (!randomTradeRecordDict.ContainsKey(symbol)){
						RandomTradeRecord aTradeRecord=new RandomTradeRecord();
						aTradeRecord.Symbol=symbol;
						aTradeRecord.HoldingPeriod=1;
						randomTradeRecordDict.Add(aTradeRecord.Symbol,aTradeRecord);
					}
				}
				symbols.Clear();
				//如果当天已写入记录，先删除
				query = new QueryDocument(new BsonElement("Date", curDateString));
				collection.Remove(query);
				
				foreach(RandomTradeRecord aTradeRecord in randomTradeRecordDict.Values){
					//读取交易价格和收盘价格
					Trade nextTrade=sdp.GetNextTrade(aTradeRecord.Symbol,dealTime);
					if (nextTrade==null||nextTrade.DateTime.Date>curDate){
						Console.WriteLine("Symbol {0} 在 {1} 时无法在下一笔成交",aTradeRecord.Symbol,dealTime);
						aTradeRecord.PriceWhenDeal=0.0;
					}else {
						aTradeRecord.PriceWhenDeal=nextTrade.Price;
					}
					
					Trade closeTrade=sdp.GetLastTrade(aTradeRecord.Symbol,closeTime);
					if (closeTrade==null||closeTrade.DateTime.Date<curDate){
						Console.WriteLine("Symbol {0} 今天没有收盘价",aTradeRecord.Symbol);
						aTradeRecord.PriceWhenClose=0.0;
					}else {
						aTradeRecord.PriceWhenClose=closeTrade.Price;
					}
					
					//没有交易价格和收盘价格则认为是停盘
					if (aTradeRecord.PriceWhenDeal<=0&&aTradeRecord.PriceWhenClose<=0){
						Console.WriteLine("证券{0}:今日没有交易",aTradeRecord.Symbol);
						if (aTradeRecord.HoldingPeriod>1) {
							query = new QueryDocument(new BsonElement("_id",aTradeRecord._id));
							UpdateDocument update=new UpdateDocument();
							update.Add(new BsonElement("$set",new BsonDocument(new BsonElement("NextTradeDate",nextTradeDateString))));
							collection.Update(query, update);
						}
						continue;
					}
					//读取应指数的日线和5分线，证券的日线和分线
					string indexSymbol="";
					if (aTradeRecord.Symbol.IndexOf("SHSE.")>=0){
						indexSymbol="SHSE.000001";
					}else if (aTradeRecord.Symbol.IndexOf("SZSE.3")>=0){
						indexSymbol="SZSE.399006";
					}else {
						indexSymbol="SZSE.399001";
					}
					
					List<Daily> indexDailys=sdp.GetLastNDailys(indexSymbol,indexDailyPeriod,curDate);
					List<Bar> indexMin5s=sdp.GetLastNBars(indexSymbol,300,indexMin5Period,dealTime);
					
					
					//***这里多加了当天日线，便于向前复权
					List<Daily> stockDailys=sdp.GetLastNDailys(aTradeRecord.Symbol,dailyPeriod,curDate);
					AdjustDailys(stockDailys);
					List<Bar> stockMin5s=sdp.GetLastNBars(aTradeRecord.Symbol,300,min5Period,dealTime);
					//去掉当天日线后，获取昨日收盘价
					int m=stockDailys.Count;
					if (m<dailyPeriod) continue;
					if (stockDailys[m-1].DateTime==curDate){
						stockDailys.RemoveAt(m-1);
					}else {
						stockDailys.RemoveAt(0);
					}
					double lastClose=stockDailys[stockDailys.Count-1].Close;
					//加入当天决断时刻的日线
					Trade trade=lastTrades[aTradeRecord.Symbol];
					if (trade==null) continue;
					Daily todayDaily=BuildDailyFormTrade((GMTrade)trade);
					stockDailys.Add(todayDaily);
	
					//正规化数据
					List<NormalizedBar> indexNormalizedDailys=NormalizeBars(indexDailys);
					List<NormalizedBar> indexNormalizedMin5s=NormalizeBars(indexMin5s);
					List<NormalizedBar> stockNormalizedDailys=NormalizeBars(stockDailys);
					List<NormalizedBar> stockNormalizedMin5s=NormalizeBars(stockMin5s);
					
					//计算当天的奖赏
					double rewardForInside=0.0;//在场内的奖赏
					double rewardForOutside=0.0;//在场外的奖赏
					
					if (aTradeRecord.HoldingPeriod==1) {
						rewardForInside=Math.Log(aTradeRecord.PriceWhenClose/aTradeRecord.PriceWhenDeal);
					}else if (aTradeRecord.HoldingPeriod>1){
						rewardForInside=Math.Log(aTradeRecord.PriceWhenClose/lastClose);
						rewardForOutside=Math.Log(aTradeRecord.PriceWhenDeal/lastClose);
					}
					//Console.WriteLine("{0}，{1}，{2}",aTradeRecord.PriceWhenDeal,aTradeRecord.PriceWhenClose,lastClosePrice);
					//写入在场外的记录，如果是第一天，则是假设没有买入，如果是第二天及以后，则是假设卖出
					//此情况下，下一交易日为空
					BsonElement[] eleArray = new BsonElement[9];
					eleArray[0]= new BsonElement("Date", curDateString);
					eleArray[1]= new BsonElement("Symbol",aTradeRecord.Symbol);
					eleArray[2]= new BsonElement("HoldingPeriod",aTradeRecord.HoldingPeriod);
					eleArray[3]= new BsonElement("IndexDaily",GetBsonArrayFromBars(indexNormalizedDailys));
					eleArray[4]= new BsonElement("IndexMin5",GetBsonArrayFromBars(indexNormalizedMin5s));
					eleArray[5]= new BsonElement("StockDaily",GetBsonArrayFromBars(stockNormalizedDailys));
					eleArray[6]= new BsonElement("StockMin5",GetBsonArrayFromBars(stockNormalizedMin5s));
					BsonArray bsonArray=new BsonArray(2);
					bsonArray.Add(rewardForInside);
					bsonArray.Add(rewardForOutside);
					eleArray[7]= new BsonElement("Reward",bsonArray);
					//已到了持仓周期限制，卖出，下一交易日为空
					if (aTradeRecord.HoldingPeriod>=holdingPeriodLimit){	
						eleArray[8]= new BsonElement("NextTradeDate","");
					}else {
						eleArray[8]= new BsonElement("NextTradeDate",nextTradeDateString);
					}
					BsonDocument insert=new BsonDocument(eleArray);
					collection.Insert(insert);
					indexDailys.Clear();
					indexMin5s.Clear();
					stockDailys.Clear();
					stockMin5s.Clear();
					indexNormalizedDailys.Clear();
					indexNormalizedMin5s.Clear();
					stockNormalizedDailys.Clear();
					stockNormalizedMin5s.Clear();	
				}
				lastTrades.Clear();
				randomTradeRecordDict.Clear();
				Console.WriteLine("统计完毕。");
			}		
			sdp.FlushAllSeries();
			//sdp.Disconnect();
			//为了防止内存溢出
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForFullGCApproach();
			GC.WaitForFullGCComplete();
			curDate=curDate.AddDays(1);
		}
	}
	protected static Daily BuildDailyFormTrade(GMTrade gmTrade){
		Daily daily=new Daily();
		daily.DateTime=gmTrade.DateTime.Date;
		daily.High=gmTrade.High;
		daily.Open=gmTrade.Open;
		daily.Low=gmTrade.Low;
		daily.Close=gmTrade.Price;
		daily.Volume=(long)gmTrade.TotalSize;
		return daily;
	}
	protected static List<NormalizedBar> NormalizeBars(List<Daily> dailys){
		List<Bar> bars=new List<Bar>();
		foreach(Daily daily in dailys) bars.Add(daily);
		return NormalizeBars(bars);
	}
	protected static List<NormalizedBar> NormalizeBars(List<Bar> bars){
		List<NormalizedBar> ret=new List<NormalizedBar>();
		double basePrice=0.0;
		double baseVol=0.0;
		foreach(Bar bar in bars){
			if (bar.High>basePrice) basePrice=bar.High;
			if (bar.Volume>baseVol) baseVol=bar.Volume;
		}
		
		foreach(Bar bar in bars){
			NormalizedBar nbar=new NormalizedBar();
			nbar.Close=bar.Close/basePrice;
			nbar.High=bar.High/basePrice;
			nbar.Low=bar.Low/basePrice;
			nbar.Open=bar.Open/basePrice;
			nbar.Volume=bar.Volume/baseVol;
			ret.Add(nbar);
		}
		return ret;
	}
	
	protected static BsonArray GetBsonArrayFromBars(List<NormalizedBar> nbars ){
		BsonArray bsonArray=new BsonArray(nbars.Count);
		foreach(NormalizedBar nbar in nbars){
			BsonArray subBsonArray=new BsonArray(5);
			subBsonArray.Add(nbar.Open);
			subBsonArray.Add(nbar.High);
			subBsonArray.Add(nbar.Low);
			subBsonArray.Add(nbar.Close);
			subBsonArray.Add(nbar.Volume);
			bsonArray.Add(subBsonArray);
		}
		return bsonArray;
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
}