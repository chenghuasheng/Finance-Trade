using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using HuaQuant.Data.GM;

public class StartUp
{
	private static GMSDK.MdApi _md=GMSDK.MdApi.Instance;
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		DateTime beginDate=new DateTime(2017,3,21);
		DateTime endDate=new DateTime(2017,3,22);
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance_new");
		
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
		BsonArray typesLimit=new BsonArray(3);
		typesLimit.Add(1);
		typesLimit.Add(2);
		typesLimit.Add(3);
		while(curDate<endDate){
			
			Console.WriteLine("日期："+curDate.ToString("yyyy-MM-dd"));			
			if ((tradeDates.Count<=0)||(!tradeDates.Contains(curDate))){
				Console.WriteLine("今天不是交易日");
				curDate=curDate.AddDays(1);
				continue;
			}
			string monthString=curDate.ToString("yyyy-MM");
			BsonElement[] eleArray = new BsonElement[3];
			BsonDocument temp=new BsonDocument(new BsonElement[] {new BsonElement("$lt",curDate.AddDays(1).ToString("yyyy-MM-dd")),
				new BsonElement("$gte",curDate.ToString("yyyy-MM-dd")) });	
			eleArray[0]=new BsonElement("date",temp);
			temp=new BsonDocument(new BsonElement("$gte",3));
			eleArray[1]=new BsonElement("count",temp);
			temp=new BsonDocument(new BsonElement("$in",typesLimit));
			eleArray[2]=new BsonElement("type",temp);
			Console.WriteLine("条件1：{0}，条件2：{1}，条件3：{2}",eleArray[0],eleArray[1],eleArray[2]);
			QueryDocument query = new QueryDocument(eleArray);
		
			List<BsonDocument> listBD=new List<BsonDocument>();	
			MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("PlateUpLimitStatis");
			foreach (BsonDocument document in collection.Find(query)){
				listBD.Add(document);
			}
			
			Dictionary<string,int> plateUpLimitStatis=new Dictionary<string,int>();
			foreach(BsonDocument document in listBD){
				BsonValue bsonValue=null;
				if (!document.TryGetValue("plate",out bsonValue)){					
					continue;					
				}
				if (bsonValue==null) continue;
				string name=(string)bsonValue;
				if (!plateUpLimitStatis.ContainsKey(name)) plateUpLimitStatis.Add(name,(int)document["count"]);
			}
			List<string> leaderPlates=new List<string>();
			if (plateUpLimitStatis.Count<=0) {
				Console.WriteLine("今日无明显热点");		
			}else {
				plateUpLimitStatis=SortDictionary_Desc(plateUpLimitStatis);
				int k=0;
				int lastCount=0;
				foreach(KeyValuePair<string,int> kvp in plateUpLimitStatis){
					k++;
					if ((k>2)&&(kvp.Value<lastCount)) break;
					Console.WriteLine("plate:{0},count:{1}",kvp.Key,kvp.Value);
					leaderPlates.Add(kvp.Key);
					lastCount=kvp.Value;
				}
			}
			//统计写入数据库
			MongoCollection<BsonDocument> collection1 = database.GetCollection<BsonDocument>("LeaderPlates");
			BsonElement[] eleArray1 = new BsonElement[1];
			eleArray1[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
			QueryDocument query1 = new QueryDocument(eleArray1);
			collection1.Remove(query1);	
			
			BsonElement[] eleArray2 = new BsonElement[3];
			eleArray2[0]= new BsonElement("date", curDate.ToString("yyyy-MM-dd"));
			eleArray2[1] = new BsonElement("count", leaderPlates.Count);
			BsonArray bsonArray=new BsonArray(leaderPlates.Count);
			foreach(string plate in leaderPlates){	
				bsonArray.Add(plate);
			}
			eleArray2[2] = new BsonElement("plates", bsonArray);
			BsonDocument insert=new BsonDocument(eleArray2);
			collection1.Insert(insert);
			curDate=curDate.AddDays(1);
		}
		
	}
	protected static Dictionary<string, int> SortDictionary_Desc(Dictionary<string, int> dic)
	{
		List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>(dic);
		myList.Sort(delegate(KeyValuePair<string, int> s1, KeyValuePair<string, int> s2)
			{
				return s2.Value.CompareTo(s1.Value);
			});
		dic.Clear();
		foreach (KeyValuePair<string, int> pair in myList)
		{
			dic.Add(pair.Key, pair.Value);
		}
		return dic;
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