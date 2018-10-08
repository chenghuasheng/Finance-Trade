using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

public class StartUp
{
	private static GMSDK.MdApi _md=GMSDK.MdApi.Instance;
	static void Main(string[] args)
	{
		DateTime beginDate=new DateTime(2017,1,1);
		DateTime endDate=new DateTime(2017,3,17);
		
		string username="tianzhong_live@126.com";
		string password="Chs771005";
		
		int ret = _md.Init(username,password,GMSDK.MDMode.MD_MODE_NULL,"","","");
		if (ret != 0)
		{
			string msg = _md.StrError(ret);
			Console.WriteLine(msg);
			return;
		}
				
		//获取交易日历
		string beginDateString=beginDate.AddMonths(-1).ToString("yyyy-MM-dd");
		string endDateString=endDate.ToString("yyyy-MM-dd");
		List<DateTime> tradeDates=GetTradeDates("SHSE",beginDateString,endDateString);
		
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance_new");
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("PlateUpLimitStatis");
		BsonElement[] eleArray = new BsonElement[1];
		BsonDocument temp=new BsonDocument(new BsonElement[] {new BsonElement("$lt",endDateString),
			new BsonElement("$gte",beginDateString) });	
		eleArray[0]=new BsonElement("date",temp);
		Console.WriteLine(eleArray[0]);
		QueryDocument query = new QueryDocument(eleArray);
		Dictionary<string,Dictionary<string,StatisItem>> plateUpLimitStatis=new Dictionary<string,Dictionary<string,StatisItem>>();
		foreach (BsonDocument document in collection.Find(query)){
			string plate=document["plate"].ToString();
			string date=document["date"].ToString();
			Dictionary<string,StatisItem> dt=null;
			if (!plateUpLimitStatis.TryGetValue(date,out dt)){
				dt=new Dictionary<string,StatisItem>();
				plateUpLimitStatis.Add(date,dt);
			}
			StatisItem si=new StatisItem();
			BsonArray documentArray=document["securities"].AsBsonArray;
			foreach(BsonDocument subDocument in documentArray){
				string symbol=subDocument["symbol"].ToString();
				string time=subDocument["time"].ToString();
				si.AddSymbol(symbol,DateTime.Parse(time));
			}
			dt.Add(plate,si);
		}
		
		/*foreach(KeyValuePair<string,Dictionary<string,StatisItem>> kvp in plateUpLimitStatis){
			Console.WriteLine(kvp.Key);
			foreach(KeyValuePair<string,StatisItem> kvp2 in kvp.Value){
				Console.Write(kvp2.Key+"--");
			}
			Console.WriteLine();
		}*/
		Dictionary<string,Dictionary<string,Stack<StatisItem>>> result=new Dictionary<string,Dictionary<string,Stack<StatisItem>>>();
		
		int i=tradeDates.Count;
		do
		{
			i--;
			string dateString=tradeDates[i].ToString("yyyy-MM-dd");
			Dictionary<string,StatisItem> dt=null;
			if (!plateUpLimitStatis.TryGetValue(dateString,out dt)) continue;
			Dictionary<string,Stack<StatisItem>> dts=new Dictionary<string,Stack<StatisItem>>();
			
			foreach(KeyValuePair<string,StatisItem> kvp in dt){
				if (kvp.Value.Count<=1) continue;
				string plate=kvp.Key;
				Stack<StatisItem> sk=null;
				if (!dts.TryGetValue(plate,out sk)){
					sk=new Stack<StatisItem>();
					dts.Add(plate,sk);
				}
				sk.Push(kvp.Value);
				StatisItem lastSI=kvp.Value;
				int j=i;
				do {
					j--;
					string prevDateString=tradeDates[j].ToString("yyyy-MM-dd");
					Dictionary<string,StatisItem> prevDt=null;
					if (!plateUpLimitStatis.TryGetValue(prevDateString,out prevDt)) break;
					if (!prevDt.ContainsKey(plate)) break;
					StatisItem curSI=prevDt[plate];
					if (curSI.Count>=lastSI.Count) break;
					StatisItem interSI=lastSI.Intersection(curSI);
					if (interSI.Count>0) sk.Push(interSI);
					lastSI=interSI;
				}while(true);
				if (sk.Count>1){
					Console.WriteLine("date:{0},plate:{1},stack count:{2}",dateString,plate,sk.Count);
					foreach(KeyValuePair<string,DateTime> kvp1 in sk.Pop().Securities){
						Console.WriteLine("symbol:{0},time:{1}",kvp1.Key,kvp1.Value);
					}
				}
				
			}
			result.Add(dateString,dts);
		}while(i>0&&tradeDates[i]>beginDate);
		
		
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