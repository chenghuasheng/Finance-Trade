using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		int days=20;
		DateTime curDate=new DateTime(2017,1,30);
		DateTime beginDate=curDate.AddDays(-1*days);
		DateTime endDate=curDate;
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance");
		
		BsonElement[] eleArray = new BsonElement[1];
		BsonDocument temp=new BsonDocument(new BsonElement[] {new BsonElement("$lt",endDate.ToString("yyyy-MM-dd")),
			new BsonElement("$gte",beginDate.ToString("yyyy-MM-dd")) });	
		eleArray[0]=new BsonElement("date",temp);
		Console.WriteLine(eleArray[0]);
		QueryDocument query = new QueryDocument(eleArray);
		
		Dictionary<string,int> plateCount=new Dictionary<string,int>();
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("LeaderPlates");
		foreach (BsonDocument document in collection.Find(query)){
			BsonValue bsonValue=null;
			if (!document.TryGetValue("plates",out bsonValue)){
				continue;
			}
			if (bsonValue==null) continue;
			BsonArray plates=(BsonArray)bsonValue;
			foreach(BsonValue plate in plates) {
				string name=(string)plate;
				if (!plateCount.ContainsKey(name)){
					plateCount.Add(name,1);
				}else {
					plateCount[name]++;
				}
			}
		}
		
		plateCount=SortDictionary_Desc(plateCount);
		List<string> leaderPlates=new List<string>();
		int k=0;
		int lastCount=0;
		foreach(KeyValuePair<string,int> kvp in plateCount){
			k++;
			if ((k>3)&&(kvp.Value<lastCount)) break;
			Console.WriteLine("symbol:{0},count:{1}",kvp.Key,kvp.Value);
			leaderPlates.Add(kvp.Key);
			lastCount=kvp.Value;
		}
		BsonArray plateArray=new BsonArray(leaderPlates.Count);	
		foreach(string plate in leaderPlates) plateArray.Add(plate);
		
		BsonElement[] eleArray1 = new BsonElement[2];	
		eleArray1[0]=new BsonElement("date",temp);
		temp=new BsonDocument(new BsonElement("$in",plateArray));
		eleArray1[1]=new BsonElement("industry",temp);
		Console.WriteLine("条件1：{0}，条件2：{1}",eleArray1[0],eleArray1[1]);
		QueryDocument query1 = new QueryDocument(eleArray1);
		
		List<BsonDocument> listBD=new List<BsonDocument>();	
		MongoCollection<BsonDocument> collection1 = database.GetCollection<BsonDocument>("IndustriesUpLimitStatis");
		foreach (BsonDocument document in collection1.Find(query1)){
			listBD.Add(document);
		}
		eleArray1[1]=new BsonElement("area",temp);
		Console.WriteLine("条件1：{0}，条件2：{1}",eleArray1[0],eleArray1[1]);
		query1 = new QueryDocument(eleArray1);
		collection1 = database.GetCollection<BsonDocument>("AreasUpLimitStatis");
		foreach (BsonDocument document in collection1.Find(query1)){
			listBD.Add(document);
		}
		eleArray1[1]=new BsonElement("concept",temp);
		Console.WriteLine("条件1：{0}，条件2：{1}",eleArray1[0],eleArray1[1]);
		query1 = new QueryDocument(eleArray1);
		collection1 = database.GetCollection<BsonDocument>("ConceptsUpLimitStatis");
		foreach (BsonDocument document in collection1.Find(query1)){
			listBD.Add(document);
		}
		Console.WriteLine(listBD.Count);
		Dictionary<string,List<string>> securityUpLimitStatis=new Dictionary<string,List<string>>();
		foreach (BsonDocument document in listBD){
			BsonArray documentArray=(BsonArray)document["securities"];
			foreach(BsonDocument subDocument in documentArray){
				string symbol=(string)subDocument["symbol"];
				string time=(string)subDocument["time"];
				
				List<string> ls=null;
				if (!securityUpLimitStatis.TryGetValue(symbol,out ls)){
					ls=new List<string>();
					securityUpLimitStatis.Add(symbol,ls);
				}
				if (!ls.Contains(time)) ls.Add(time);	
			}
		}
		Dictionary<string,int> securityUpLimitCount=new Dictionary<string,int>();
		foreach(KeyValuePair<string,List<string>> kvp in securityUpLimitStatis){
			securityUpLimitCount.Add(kvp.Key,kvp.Value.Count);
		}
		securityUpLimitCount=SortDictionary_Desc(securityUpLimitCount);//降序排序
		foreach(KeyValuePair<string,int> kvp in securityUpLimitCount){
			Console.WriteLine("symbol:{0},count:{1}",kvp.Key,kvp.Value);
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
}