using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.Indicators;
using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;

public class UpLimitStockMarketor:Marketor{
	public UpLimitStockMarketor(Strategy strategy):base(strategy){
	}
		
	public override void OnInit(){
		DateTime curDate=Clock.Now.Date;
		//查找最N天内的涨停股
		int N=1;
		DateTime beginDate=curDate.AddDays(-1*N);
		Instrument indexInst=InstrumentManager.Instruments["SHSE.000001"];
		if (indexInst!=null){
			ISeriesObject[] indexDailys=Util.GetNDailiesBeforeDate(indexInst,curDate,N);
			if (indexDailys.Length>0){
				beginDate=indexDailys[0].DateTime;
			}
		}else {
			Console.WriteLine("请补充指数SHSE.000001的定义及日线");
			return;
		}
		DateTime endDate=curDate;
		
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance");
		//查询近N天来的涨停股票
		BsonElement[] eleArray = new BsonElement[3];
		BsonDocument temp=new BsonDocument(new BsonElement[] {new BsonElement("$lt",endDate.ToString("yyyy-MM-dd")),
			new BsonElement("$gte",beginDate.ToString("yyyy-MM-dd")) });	
		eleArray[0]=new BsonElement("date",temp);
		eleArray[1]=new BsonElement("bidsizedivvol",new BsonDocument("$lte",0.021));
		eleArray[2]=new BsonElement("flowashare",new BsonDocument(new BsonElement("$lte",10000000000)));
		Console.WriteLine("条件：{0},条件：{1},条件：{2}",eleArray[0],eleArray[1],eleArray[2]);
		QueryDocument query = new QueryDocument(eleArray);
		//添加要交易的证券
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("UpLimitAnalysis");
		foreach (BsonDocument document in collection.Find(query)){
			string symbol=document["symbol"].ToString();
			double volDivLast5AvgVol=document["voldivlast5avgvol"].ToDouble();
			double turnoverRate=document["turnoverrate"].ToDouble();
			string dateString=document["date"].ToString();
			DateTime date=DateTime.Parse(dateString);
			if (!(turnoverRate>=38||volDivLast5AvgVol>=8)) continue;//turnoverRate和volDivLast5AvgVol过滤
			Instrument inst=InstrumentManager.Instruments[symbol];
			if ((inst!=null)&&!HasPosition(inst)) {	
				ISeriesObject[] dailyBars=Util.GetNDailiesBeforeDate(inst,curDate,120);
				Util.AdjustDailys(dailyBars);//向前复权		
				this.AddInstrument(inst);
				foreach(Daily dBar in dailyBars) {
					this.Bars[inst].Add(dBar);
				}
			}		
		}
	}	
}