using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;
public class DQNTradeJob:Job{
	private DateTime dealDate;
	private DateTime dealTime;
	private Strategy strategy;
	private GMRealTimeProvider provider=null;
	public DQNTradeJob(string name,DateTime jobDate,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.dealDate=jobDate;
		this.dealTime=this.dealDate.AddMinutes(Const.DealTimeMins);
		this.strategy=strategy;
		this.provider=(GMRealTimeProvider)this.strategy.MarketDataProvider;
	}
	public DQNTradeJob(string name,DateTime jobDate,Strategy strategy):this(name,jobDate,strategy,null){}
	protected override bool doJob(){
		bool ret=false;
		BarFeeder.Init(this.provider,this.dealTime);
		BarFeeder.ClearBars();
		
		string dealTimeString=Utils.FormatTime(this.dealTime);
		List<TradeStateRecord> tradeStateRecords=new List<TradeStateRecord>();
		List<string> positionSymbols=new List<string>();
		
		//处理已有的头寸
		Dictionary<string,DQNExit> exitDict=new Dictionary<string,DQNExit>(); 
		foreach(Behavior behavior in this.strategy.Behaviors){
			DQNExit exit=behavior as DQNExit;
			if (exit!=null){
				string symbol=exit.Instrument.Symbol;
				exitDict.Add(symbol,exit);
				TradeStateRecord aStateRecord=new TradeStateRecord();
				aStateRecord.Symbol=symbol;
				aStateRecord.HoldingPeriod=exit.HoldingPeriod;
				tradeStateRecords.Add(aStateRecord);
				positionSymbols.Add(symbol);
			}
		}
		//Dictionary<String,Trade> lastTradeDict=this.provider.GetLastTrades(positionSymbols,true);
		Dictionary<String,Trade> lastTradeDict=new Dictionary<String,Trade>();
		foreach(string symbol in positionSymbols){
			List<Trade> trades=this.provider.GetLastNTrades(symbol,dealTimeString,1);
			if (trades.Count>0&&trades[0].DateTime.Date==this.dealDate){
				lastTradeDict[symbol]=trades[0];
			}else {
				lastTradeDict[symbol]=null;
			}
		}
		this.GetDataAndPrediction(tradeStateRecords,lastTradeDict);
		foreach(BsonDocument record in DQNTradeDBAccess.GetExitingTempRecords()){		
			string symbol=record["Symbol"].AsString;
			exitDict[symbol].Deal(record["QValueIn"].AsDouble,record["QValueOut"].AsDouble);		
		}
		//处理当天入场
		tradeStateRecords.Clear();
		List<string> activeSymbols=new List<String>();
		activeSymbols.AddRange(this.provider.GetSymbols("SHSE",1,1));
		activeSymbols.AddRange(this.provider.GetSymbols("SZSE",1,1));
		//lastTradeDict=this.provider.GetLastTrades(activeSymbols,true);
		foreach(string symbol in activeSymbols){
			List<Trade> trades=this.provider.GetLastNTrades(symbol,dealTimeString,1);
			if (trades.Count>0&&trades[0].DateTime.Date==this.dealDate){
				lastTradeDict[symbol]=trades[0];
			}else {
				lastTradeDict[symbol]=null;
			}
		}	
		foreach(KeyValuePair<String,Trade> kvp in lastTradeDict){
			if (kvp.Value==null) continue;
			GMTrade gmTrade=(GMTrade)kvp.Value;
			if (gmTrade.Price<=0) continue;
			if (gmTrade.Price==gmTrade.UpperLimit) continue;
			if (gmTrade.Price/gmTrade.LastClose-1<=0) continue;
			if (positionSymbols.Contains(kvp.Key)) continue;
			TradeStateRecord aStateRecord=new TradeStateRecord();
			aStateRecord.Symbol=kvp.Key;
			aStateRecord.HoldingPeriod=1;
			tradeStateRecords.Add(aStateRecord);
		}
		this.GetDataAndPrediction(tradeStateRecords,lastTradeDict);
		List<string> enteringSymbols=new List<string>();
		MongoCursor enteringCursor=DQNTradeDBAccess.GetEnteringTempRecords(5);
		foreach(BsonDocument record in enteringCursor){
			string symbol=record["Symbol"].AsString;
			enteringSymbols.Add(symbol);	
		}
		Dictionary<string,Trade> newTradeDict=this.provider.GetLastTrades(enteringSymbols,false);
		foreach(BsonDocument record in enteringCursor){
			string symbol=record["Symbol"].AsString;
			double dealPrice=lastTradeDict[symbol].Price;
			double newPrice=newTradeDict[symbol].Price;
			if (Math.Abs(newPrice/dealPrice-1)<0.01){
				Instrument inst=InstrumentManager.Instruments[symbol];
				if (inst!=null){
					DQNEntry entry=new DQNEntry(inst,this.strategy);
					this.strategy.AddBehavior(inst,entry);
					entry.Deal(record["QValueIn"].AsDouble,record["QValueOut"].AsDouble);
				}
			}
		}
		ret=true;
		return ret;
	}

	private void GetDataAndPrediction(List<TradeStateRecord> tradeStateRecords,Dictionary<String,Trade> lastTradeDict){
		Console.WriteLine("共有记录{0}条",tradeStateRecords.Count);
		DQNTradeDBAccess.ClearTempRecords();
		Console.WriteLine("开始准备数据 ...");
		
		string dealDateString=Utils.FormatDate(this.dealDate);
		foreach(TradeStateRecord aStateRecord in tradeStateRecords){
			Trade lastTrade=lastTradeDict[aStateRecord.Symbol];
			if (lastTrade==null) continue;
			
			//读取应指数的日线和5分线，证券的日线和分线
			string indexSymbol="";
			if (aStateRecord.Symbol.IndexOf("SHSE.")>=0){
				indexSymbol="SHSE.000001";
			}else if (aStateRecord.Symbol.IndexOf("SZSE.3")>=0){
				indexSymbol="SZSE.399006";
			}else {
				indexSymbol="SZSE.399001";
			}
			List<Daily> indexDailys=BarFeeder.GetDailys(indexSymbol,Const.IndexDailyPeriod);
			List<Bar> indexMin5s=BarFeeder.GetBar5s(indexSymbol,Const.IndexMin5Period);										
			//***这里多加了当天日线，便于向前复权,以修正价格	
			List<Daily> stockDailys=BarFeeder.GetDailys(aStateRecord.Symbol,Const.StockDailyPeriod,true,lastTrade);
			//日线不足周期数的忽略
			if (stockDailys.Count<Const.StockDailyPeriod) continue;
			List<Bar> stockMin5s=BarFeeder.GetBar5s(aStateRecord.Symbol,Const.StockMin5Period);
			
			//正规化数据
			List<NormalBar> indexNormalDailys=BarUtils.NormalBars(indexDailys);
			List<NormalBar> indexNormalMin5s=BarUtils.NormalBars(indexMin5s);
			List<NormalBar> stockNormalDailys=BarUtils.NormalBars(stockDailys);
			List<NormalBar> stockNormalMin5s=BarUtils.NormalBars(stockMin5s);
			DQNTradeDBAccess.SaveTempRecord(dealDateString,aStateRecord.Symbol,aStateRecord.HoldingPeriod,
				BarUtils.BarsToBsonArray(indexNormalDailys),BarUtils.BarsToBsonArray(indexNormalMin5s),
				BarUtils.BarsToBsonArray(stockNormalDailys),BarUtils.BarsToBsonArray(stockNormalMin5s));
		}
		Console.WriteLine("预测中 ...");
		//调用神经网络进行盈利预测
		CMDAgent.RunPythonScript(@"E:\pyfiles\trade_dqn_v2.0\trade_prediction.py",dealDateString);	
	}
}