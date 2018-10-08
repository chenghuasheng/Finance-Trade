using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;
using MongoDB.Bson;
public class SaveRandomTradeRecordsJob:Job{
	private DateTime dealDate;
	private Strategy strategy;
	private GMRealTimeProvider provider=null;
	public SaveRandomTradeRecordsJob(string name,DateTime jobDate,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.dealDate=jobDate;
		this.strategy=strategy;
		this.provider=(GMRealTimeProvider)this.strategy.MarketDataProvider;
	}
	public SaveRandomTradeRecordsJob(string name,DateTime jobDate,Strategy strategy):this(name,jobDate,strategy,null){}
	
	protected override bool doJob(){
		bool ret=false;
		List<TradeStateRecord> tradeStateRecords=new List<TradeStateRecord>();
		List<string> recordedSymbols=new List<string>();
		
		DateTime dealTime=this.dealDate.AddMinutes(Const.DealTimeMins);
		DateTime closeTime=this.dealDate.AddMinutes(Const.CloseTimeMins);
		string dealTimeString=Utils.FormatTime(dealTime);
		string closeTimeString=Utils.FormatTime(closeTime);
		string dealDateString=Utils.FormatDate(dealDate);
		//处理已记录的随机交易
		foreach(BsonDocument record in RandomTradeDBAccess.GetRecordsByNextTradeDate(dealDateString))
		{
			TradeStateRecord aTradeRecord=new TradeStateRecord();
			aTradeRecord._id=record["_id"].ToString();
			aTradeRecord.Symbol=record["Symbol"].ToString();
			aTradeRecord.HoldingPeriod=(int)record["HoldingPeriod"]+1;
			tradeStateRecords.Add(aTradeRecord);
			recordedSymbols.Add(aTradeRecord.Symbol);
		}
		//获取指定时间的最新价格，然后随机选择入场
		List<string> activeSymbols=new List<String>();
		activeSymbols.AddRange(this.provider.GetSymbols("SHSE",1,1));
		activeSymbols.AddRange(this.provider.GetSymbols("SZSE",1,1));
		
		Dictionary<String,Trade> lastTradeDict=new Dictionary<String,Trade>();
		foreach(string symbol in activeSymbols){
			List<Trade> trades=this.provider.GetLastNTrades(symbol,dealTimeString,1);
			if (trades.Count>0&&trades[0].DateTime.Date==this.dealDate){
				lastTradeDict[symbol]=trades[0];
			}else {
				lastTradeDict[symbol]=null;
			}
		}
		Console.WriteLine("今日共有 {0} 只活跃证券",lastTradeDict.Count);
		List<string> symbols=new List<string>();
		foreach(KeyValuePair<String,Trade> kvp in lastTradeDict){
			if (kvp.Value==null) continue;
			GMTrade gmTrade=(GMTrade)kvp.Value;
			if (gmTrade.Price<=0) continue;
			if (gmTrade.Price==gmTrade.UpperLimit) continue;
			if (gmTrade.Price/gmTrade.LastClose-1<=0) continue;
			if (recordedSymbols.Contains(kvp.Key)) continue;
			if (!symbols.Contains(kvp.Key)) symbols.Add(kvp.Key);
		}
		int total=symbols.Count;		
		Random randomer=new Random((int)DateTime.Now.Ticks);
		int[] randomNumbers=new int[100];
		for(int k=0;k<100;k++) randomNumbers[k]=randomer.Next(total);
		foreach(int r in randomNumbers){
			string symbol=symbols[r];
			TradeStateRecord aTradeRecord=new TradeStateRecord();
			aTradeRecord.Symbol=symbol;
			aTradeRecord.HoldingPeriod=1;
			tradeStateRecords.Add(aTradeRecord);	
		}
		Console.WriteLine("今日共需保存 {0} 条交易记录",tradeStateRecords.Count);
		//如果当天已写入记录，先删除
		RandomTradeDBAccess.RemoveRecordsByDate(dealDateString);
		BarFeeder.Init(this.provider,dealTime);
		//开始记录每一条交易		
		string nextTradeDateString=(string)this.strategy.Global["NextTradeDate"];
		foreach(TradeStateRecord aTradeRecord in tradeStateRecords){
			//读取当天日线		
			List<Daily> dailys=BarFeeder.GetLastNDailys(aTradeRecord.Symbol,1);
			Daily curDaily=null;
			if (dailys.Count>0){
				curDaily=dailys[0];
			}
			//没有当天日线则认为是停盘，有则确定收盘价格
			if (curDaily==null||curDaily.DateTime<this.dealDate){
				aTradeRecord.PriceWhenClose=0.0;
				Console.WriteLine("证券{0}:今日没有交易",aTradeRecord.Symbol);
				if (aTradeRecord.HoldingPeriod>1) {
					RandomTradeDBAccess.UpdateNextTradeDate(aTradeRecord._id,nextTradeDateString);
				}
				continue;	
			}else {
				aTradeRecord.PriceWhenClose=curDaily.Close;
				aTradeRecord.LowerPriceWhenClose=curDaily.Low;
			}		
			//读取决断时的价格
			Trade lastTrade=lastTradeDict[aTradeRecord.Symbol];
			if (lastTrade==null||lastTrade.Price<=0) {
				Console.WriteLine(" {0} 在决断时未能获取价格",aTradeRecord.Symbol);
				continue;
			}
			//读取交易价格,这里使用了下1分钟内的第一笔交易，如果没有则继续找下一分钟
			List<Trade> trades;
			Trade nextTrade=null;
			DateTime curTime=dealTime;
			DateTime nextTime;
			do {
				nextTime=curTime.AddMinutes(1);
				string curTimeString=Utils.FormatTime(curTime);
				string nextTimeString=Utils.FormatTime(nextTime);
				trades=this.provider.GetTrades(aTradeRecord.Symbol,curTimeString,nextTimeString);
				int i=0;
				int m=trades.Count;
				if (m>0){
					while(i<m&&(trades[i].DateTime<=lastTrade.DateTime||trades[i].Price>=curDaily.High)){
						i++;
					}
					if (i<m) nextTrade=trades[i];
				}
				curTime=nextTime;
			}while((nextTrade==null||nextTrade.Price<=0)&&curTime<closeTime);
			//如果没有可交易的价格	
			if (nextTrade==null){
				Console.WriteLine("Symbol {0} 在 {1} 时无法在今天内成交",aTradeRecord.Symbol,dealTime);
				aTradeRecord.PriceWhenDeal=0.0;
				if (aTradeRecord.HoldingPeriod>1) {
					RandomTradeDBAccess.UpdateNextTradeDate(aTradeRecord._id,nextTradeDateString);
				}
				continue;
			}else {
				aTradeRecord.PriceWhenDeal=nextTrade.Price;
				aTradeRecord.LowerPriceWhenDeal=((GMTrade)nextTrade).Low;
				//修正周期为1时，成交后经历的最低价
				if (aTradeRecord.HoldingPeriod<=1){
					trades=this.provider.GetTrades(aTradeRecord.Symbol,Utils.FormatTime(nextTrade.DateTime),Utils.FormatTime(closeTime));
					aTradeRecord.LowerPriceWhenClose=nextTrade.Price;
					foreach(Trade trade in trades){
						if (trade.Price<aTradeRecord.LowerPriceWhenClose){
							aTradeRecord.LowerPriceWhenClose=trade.Price;
						}
					}
				}
			}
			
			//读取应指数的日线和5分线，证券的日线和分线
			string indexSymbol="";
			if (aTradeRecord.Symbol.IndexOf("SHSE.")>=0){
				indexSymbol="SHSE.000001";
			}else if (aTradeRecord.Symbol.IndexOf("SZSE.3")>=0){
				indexSymbol="SZSE.399006";
			}else {
				indexSymbol="SZSE.399001";
			}
					
			List<Daily> indexDailys=BarFeeder.GetDailys(indexSymbol,Const.IndexDailyPeriod);
			List<Bar> indexMin5s=BarFeeder.GetBar5s(indexSymbol,Const.IndexMin5Period);										
			//***这里多加了当天日线，便于向前复权,以修正价格
			List<Daily> stockDailys=BarFeeder.GetDailys(aTradeRecord.Symbol,Const.StockDailyPeriod,true,lastTrade);
			//日线不足周期数的忽略
			if (stockDailys.Count<Const.StockDailyPeriod) continue;
			List<Bar> stockMin5s=BarFeeder.GetBar5s(aTradeRecord.Symbol,Const.StockMin5Period);
			//昨天的收盘价
			double lastClose=stockDailys[stockDailys.Count-2].Close;
					
			//计算当天的奖赏
			double rewardForInside=0.0;//在场内的奖赏
			double rewardForOutside=0.0;//在场外的奖赏
			double riskForInside=0.0;
			double riskForOutside=0.0;
			
			if (aTradeRecord.HoldingPeriod==1) {
				rewardForInside=Math.Log(aTradeRecord.PriceWhenClose/aTradeRecord.PriceWhenDeal);
				riskForInside=Math.Log(aTradeRecord.LowerPriceWhenClose/aTradeRecord.PriceWhenDeal);
			}else if (aTradeRecord.HoldingPeriod>1){
				rewardForInside=Math.Log(aTradeRecord.PriceWhenClose/lastClose);
				rewardForOutside=Math.Log(aTradeRecord.PriceWhenDeal/lastClose);
				riskForInside=Math.Log(aTradeRecord.LowerPriceWhenClose/lastClose);
				riskForOutside=Math.Log(aTradeRecord.LowerPriceWhenDeal/lastClose);
			}
			
			//正规化数据
			List<NormalBar> indexNormalDailys=BarUtils.NormalBars(indexDailys);
			List<NormalBar> indexNormalMin5s=BarUtils.NormalBars(indexMin5s);
			List<NormalBar> stockNormalDailys=BarUtils.NormalBars(stockDailys);
			List<NormalBar> stockNormalMin5s=BarUtils.NormalBars(stockMin5s);
			//封装reward
			BsonArray rewards=new BsonArray(2);
			rewards.Add(rewardForInside);
			rewards.Add(rewardForOutside);
			//封装risk
			BsonArray risks=new BsonArray(2);
			risks.Add(riskForInside);
			risks.Add(riskForOutside);
			//确定nextTradeDate
			string dateString=nextTradeDateString;
			if (aTradeRecord.HoldingPeriod>=Const.HoldingPeriodLimit) dateString="";
			//写入数据库	
			RandomTradeDBAccess.SaveRocord(dealDateString,aTradeRecord.Symbol,aTradeRecord.HoldingPeriod,
				BarUtils.BarsToBsonArray(indexNormalDailys),BarUtils.BarsToBsonArray(indexNormalMin5s),
				BarUtils.BarsToBsonArray(stockNormalDailys),BarUtils.BarsToBsonArray(stockNormalMin5s),
				rewards,risks,dateString);			
		}
		Console.WriteLine("统计完毕。");		
		ret=true;
		return ret;
	}	
}