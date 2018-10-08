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
		MongoDatabase database = server.GetDatabase("finance");
		
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("UpLimitAnalysis");
		BsonElement[] eleArray = new BsonElement[2];
		eleArray[0]= new BsonElement("$where", "this.nextopen<this.nextclose");
		eleArray[1]= new BsonElement("turnoverrate", new BsonDocument(new BsonElement[] {new BsonElement("$gte",100)}));//因素和条件
		//eleArray[1]= new BsonElement("lastuplimited", true);//因素和条件
		QueryDocument query = new QueryDocument(eleArray);
		long c1=collection.Count(query);
		
		eleArray[0]= new BsonElement("$where", "this.nextopen>this.nextclose");
		query = new QueryDocument(eleArray);
		long c2=collection.Count(query);
		Console.WriteLine("c1={0},c2={1},rate={2}",c1,c2,(double)c1/(c1+c2));
	}
}