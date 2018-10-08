using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
public class TradeManager {		
	private MongoCollection<BsonDocument> _collection;
	public TradeManager(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		this._collection = database.GetCollection<BsonDocument>("TradeRecords");
	}
	public void RecordAEntryTrade(DateTime date,RecognitionState dailyLineState,RecognitionState minLineState,string symbol){
		string dateString=date.ToString("yyyy-MM-dd");
		int dailyShape=(int)dailyLineState.Shape;
		int dailySlope=(int)dailyLineState.Slope;
		int dailySpeed=(int)dailyLineState.Speed;
		
		int minShape=(int)minLineState.Shape;
		int minSlope=(int)minLineState.Slope;
		int minSpeed=(int)minLineState.Speed;
		
		QueryDocument query = new QueryDocument();
		query.Add(new  BsonElement("Date",dateString));
		query.Add(new  BsonElement("Symbol",symbol));
		this._collection.Remove(query);
		
		BsonElement[] eleArray = new BsonElement[9];
		eleArray[0]=new BsonElement("Date",dateString);
		eleArray[1]=new BsonElement("DailyShape",dailyShape);
		eleArray[2]=new BsonElement("DailySlope",dailySlope);
		eleArray[3]=new BsonElement("DailySpeed",dailySpeed);
		eleArray[4]=new BsonElement("MinShape",minShape);
		eleArray[5]=new BsonElement("MinSlope",minSlope);
		eleArray[6]=new BsonElement("MinSpeed",minSpeed);
		eleArray[7]=new BsonElement("Symbol",symbol);
		eleArray[8]=new BsonElement("Reward",0.0);
		BsonDocument insert=new BsonDocument(eleArray);
		this._collection.Insert(insert);
	}
	public void RecordAExitTrade(DateTime date,string symbol,float reward){
		string dateString=date.ToString("yyyy-MM-dd");
		QueryDocument query = new QueryDocument();
		query.Add(new  BsonElement("Date",dateString));
		query.Add(new  BsonElement("Symbol",symbol));
		foreach(BsonDocument result in this._collection.Find(query)){			
			UpdateDocument update=new UpdateDocument(result.ToDictionary());
			update["Reward"]=reward;
			this._collection.Update(query,update);
			break;
		}
	}
}
