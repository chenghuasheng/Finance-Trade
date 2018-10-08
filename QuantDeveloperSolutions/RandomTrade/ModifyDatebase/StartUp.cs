using System;
using MongoDB.Bson;
using MongoDB.Driver;

public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("RandomTradeRecords");
		foreach(BsonDocument aRecord in collection.FindAll()){
		int traded=0;
		int oriInside=0;
		if (aRecord["HoldingPeriod"]==1){
			oriInside=0;
		}else if (aRecord["HoldingPeriod"]>1){
			oriInside=1;
		}
		traded=Math.Abs((int)aRecord["Inside"]-oriInside);
		QueryDocument query = new QueryDocument(new BsonElement("_id",aRecord["_id"]));
		UpdateDocument update=new UpdateDocument();
		QueryDocument query2=new QueryDocument();
		query2.Add(new BsonElement("OriInside",oriInside));
		query2.Add(new BsonElement("Traded",traded));
		update.Add(new BsonElement("$set",query2));
		collection.Update(query, update);
	}
		}
}