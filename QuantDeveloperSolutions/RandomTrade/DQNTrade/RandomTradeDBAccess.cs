using System;
using MongoDB.Bson;
using MongoDB.Driver;

public static class RandomTradeDBAccess{
	static MongoClient client = new MongoClient("mongodb://localhost:27017");
	static MongoServer server = client.GetServer();
	static MongoDatabase database = server.GetDatabase("FinanceLast");
	static MongoCollection<BsonDocument> collection =database.GetCollection<BsonDocument>("RandomTradeExperiencePool");
	
	public static MongoCursor GetRecordsByNextTradeDate(string dateString){
		QueryDocument query=new QueryDocument(new BsonElement("NextTradeDate",dateString));
		return collection.Find(query);
	}
	public static void RemoveRecordsByDate(string dateString){
		QueryDocument query = new QueryDocument(new BsonElement("Date", dateString));
		collection.Remove(query);
	}
	public static void SaveRocord(string dateString,string symbol,int holdingPeriod,BsonArray indexDailys,
		BsonArray indexMin5s,BsonArray stockDailys,BsonArray stockMin5s,BsonArray rewards,BsonArray risks,string nextTradeDateString){
		BsonElement[] eleArray = new BsonElement[10];
		eleArray[0]= new BsonElement("Date", dateString);
		eleArray[1]= new BsonElement("Symbol",symbol);
		eleArray[2]= new BsonElement("HoldingPeriod",holdingPeriod);
		eleArray[3]= new BsonElement("IndexDaily",indexDailys);
		eleArray[4]= new BsonElement("IndexMin5",indexMin5s);
		eleArray[5]= new BsonElement("StockDaily",stockDailys);
		eleArray[6]= new BsonElement("StockMin5",stockMin5s);
		eleArray[7]= new BsonElement("Reward",rewards);
		eleArray[8]= new BsonElement("Risk",risks);
		eleArray[9]= new BsonElement("NextTradeDate",nextTradeDateString);
		BsonDocument insert=new BsonDocument(eleArray);
		collection.Insert(insert);
	}
	public static void UpdateNextTradeDate(string _id,string nextTradeDateString){
		QueryDocument query = new QueryDocument(new BsonElement("_id",new BsonObjectId(_id))); 
		UpdateDocument update=new UpdateDocument();
		update.Add(new BsonElement("$set",new BsonDocument(new BsonElement("NextTradeDate",nextTradeDateString))));
		collection.Update(query, update);	
	}
}