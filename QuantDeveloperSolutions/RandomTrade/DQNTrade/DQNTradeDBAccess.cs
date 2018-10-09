using System;
using MongoDB.Bson;
using MongoDB.Driver;

public static class DQNTradeDBAccess{
	static MongoClient client = new MongoClient("mongodb://localhost:27017");
	static MongoServer server = client.GetServer();
	static MongoDatabase database = server.GetDatabase("FinanceLast");
	static MongoCollection<BsonDocument> collection1 =database.GetCollection<BsonDocument>("DqnTradeObservationPool");
	static MongoCollection<BsonDocument> collection=database.GetCollection<BsonDocument>("DqnTradeRecords");
	
	public static void SaveNewTradeRecord(string dateString,string symbol,int holdingPeriod,
		double initialQValueIn,double initialQValueOut,float profit){
		BsonElement[] eleArray = new BsonElement[7];
		eleArray[0]= new BsonElement("Date", dateString);
		eleArray[1]= new BsonElement("Symbol",symbol);
		eleArray[2]= new BsonElement("HoldingPeriod",holdingPeriod);
		eleArray[3]= new BsonElement("InitialQValueIn",initialQValueIn);
		eleArray[4]= new BsonElement("InitialQValueOut",initialQValueOut);
		eleArray[5]= new BsonElement("Profit",profit);
		eleArray[6]= new BsonElement("Closed",false);
		BsonDocument insert=new BsonDocument(eleArray);
		collection.Insert(insert);
	}
	public static void UpdateClosedRecord(string dateString,string symbol,int holdingPeriod,float profit){
		BsonElement[] eleArray = new BsonElement[3];
		eleArray[0]=new BsonElement("Symbol",symbol);
		eleArray[1]=new BsonElement("Date",dateString);
		eleArray[2]=new BsonElement("Closed",false);
		QueryDocument query = new QueryDocument(eleArray);
		UpdateDocument update=new UpdateDocument();
		BsonElement[] eleArray1 = new BsonElement[3];
		eleArray1[0]=new BsonElement("HoldingPeriod",holdingPeriod);
		eleArray1[1]=new BsonElement("Profit",profit);
		eleArray1[2]=new BsonElement("Closed",true);
		update.Add(new BsonElement("$set",new BsonDocument(eleArray1)));
		collection.Update(query, update);
	}
	public static void SaveTempRecord(string dateString,string symbol,int holdingPeriod,BsonArray indexDailys,
		BsonArray indexMin5s,BsonArray stockDailys,BsonArray stockMin5s){
		BsonElement[] eleArray = new BsonElement[7];
		eleArray[0]= new BsonElement("Date", dateString);
		eleArray[1]= new BsonElement("Symbol",symbol);
		eleArray[2]= new BsonElement("HoldingPeriod",holdingPeriod);
		eleArray[3]= new BsonElement("IndexDaily",indexDailys);
		eleArray[4]= new BsonElement("IndexMin5",indexMin5s);
		eleArray[5]= new BsonElement("StockDaily",stockDailys);
		eleArray[6]= new BsonElement("StockMin5",stockMin5s);
		BsonDocument insert=new BsonDocument(eleArray);
		collection1.Insert(insert);
	}
	public static void ClearTempRecords(){
		collection1.RemoveAll();
	}
	public static MongoCursor GetExitingTempRecords(){
		QueryDocument query=new QueryDocument(new BsonElement("$where","this.QValueIn<=this.QValueOut"));
		return collection1.Find(query).SetFields(new string[]{"Symbol","QValueIn","QValueOut"});
	}
	public static MongoCursor GetEnteringTempRecords(int limit){
		QueryDocument query=new QueryDocument(new BsonElement("$where","this.QValueIn>this.QValueOut"));
		SortByDocument s = new SortByDocument();
		s.Add("QValueIn", -1);//-1=DESC
		return collection1.Find(query).SetSortOrder(s).SetLimit(limit).SetFields(new string[]{"Symbol","QValueIn","QValueOut"});;
	}
}