using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Providers;
using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;
public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		
		GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
		provider.Connect(10000);
		List<GMSDK.TradeDate> tradeDates = provider.MdApi.GetCalendar("SHSE", "2016-01-01", "2018-12-31");
		List<DateTime> dates = new List<DateTime>();
		DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		foreach (GMSDK.TradeDate tradeDate in tradeDates)
		{
			dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
		}
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("TradeDates");
		collection.RemoveAll();
		foreach(DateTime date in dates)
		{
			BsonDocument insert=new BsonDocument(new BsonElement("Date",date.ToString("yyyy-MM-dd")));
			collection.Insert(insert);
		}
	}
}