using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
public class TradeManager {
	private TradeRecord[,,,] tradeRecords=new TradeRecord[5,4,5,4];	
	public TradeManager(){
		for(int a=0;a<5;a++)
			for(int b=0;b<4;b++)
				for (int c=0;c<5;c++)
					for (int d=0;d<4;d++){	
						this.tradeRecords[a,b,c,d]=new TradeRecord();
					}
	}
	public void ReadFromDB(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("TradeRecords");
		QueryDocument query = new QueryDocument();
		foreach (BsonDocument document in collection.Find(query)){
			int a=document["DailyLineShape"].ToInt32();
			int b=document["DailyLineSpeed"].ToInt32();
			int c=document["MinLineShape"].ToInt32();
			int d=document["MinLineSpeed"].ToInt32();
			TradeRecord currentRecord=this.tradeRecords[a,b,c,d];
			BsonArray Symbols=(BsonArray)document["Symbols"];
			foreach(BsonValue symbol in Symbols) {
				currentRecord.Symbols.Add(symbol.ToString());
			}
			currentRecord.TradeTimes=document["TradeTimes"].ToInt32();
			currentRecord.Reward=(float)document["Reward"].ToDouble();
		}
	}
	public void SaveToDB(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("TradeRecords");
		QueryDocument query = new QueryDocument();
		collection.Remove(query);
		for(int a=0;a<5;a++)
			for(int b=0;b<4;b++)
				for (int c=0;c<5;c++)
					for (int d=0;d<4;d++){
						TradeRecord currentRecord=this.tradeRecords[a,b,c,d];
						BsonElement[] eleArray = new BsonElement[7];
						eleArray[0]=new BsonElement("DailyLineShape",a);
						eleArray[1]=new BsonElement("DailyLineSpeed",b);
						eleArray[2]=new BsonElement("MinLineShape",c);
						eleArray[3]=new BsonElement("MinLineSpeed",d);
						BsonArray bsonArray=new BsonArray(currentRecord.Symbols.Count);
						foreach(string symbol in currentRecord.Symbols){
							bsonArray.Add(symbol);
						}
						eleArray[4]=new BsonElement("Symbols", bsonArray);
						eleArray[5]=new BsonElement("TradeTimes",currentRecord.TradeTimes);
						eleArray[6]=new BsonElement("Reward",currentRecord.Reward);
						BsonDocument insert=new BsonDocument(eleArray);
						collection.Insert(insert);
					}
	}
	
	public bool RecordAEntryTrade(RecognitionState dailyLineState,RecognitionState minLineState,string symbol){
		int a=(int)dailyLineState.Shape;
		int b=(int)dailyLineState.Speed;
		int c=(int)minLineState.Shape;
		int d=(int)minLineState.Speed;
		TradeRecord currentRecord=this.tradeRecords[a,b,c,d];
		if (currentRecord.Symbols.Contains(symbol)) return false;
		currentRecord.Symbols.Add(symbol);
		currentRecord.TradeTimes++;
		return true;
	}
	public bool RecordAExitTrade(string symbol,float reward){
		foreach(TradeRecord currentRecord in this.tradeRecords){
			if (currentRecord.Symbols.Contains(symbol)) {
				currentRecord.Symbols.Remove(symbol);
				currentRecord.Reward+=reward;
				return true;
			}
		}
		return false;
	}
}

public class TradeRecord {
	public List<string> Symbols=new List<string>();
	public int TradeTimes=0;
	public float Reward=0.0F;
}