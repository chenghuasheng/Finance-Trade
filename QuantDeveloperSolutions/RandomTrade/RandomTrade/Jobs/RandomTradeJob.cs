using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;

public class RandomTradeJob:Job{
	private Strategy strategy;
	private int indexDailyPeriod=20;
	private int indexMin5Period=10;
	private GMRealTimeProvider provider=null;
	public RandomTradeJob(string name,Strategy strategy,Job[] needJobs):base(name,needJobs){
		this.strategy=strategy;
		this.provider=(GMRealTimeProvider)this.strategy.MarketDataProvider;
	}
	public RandomTradeJob(string name,Strategy strategy):this(name,strategy,null){}
	
	protected override bool doJob(){
		bool ret=false;
		//已有证券假设出场
		foreach(Behavior behavior in this.strategy.Behaviors){
			if (behavior is RandomExit){	
				((RandomExit)behavior).Deal();
			}
		}
		//对所有证券获取最新价格，按涨幅排序
		List<string> activeSymbols=new List<String>();
		activeSymbols.AddRange(this.provider.GetSymbols("SHSE",1,1));
		activeSymbols.AddRange(this.provider.GetSymbols("SZSE",1,1));
		Dictionary<String,Trade> lastTrades=this.provider.GetLastTrades(activeSymbols,true);
		
		List<Stock> stocks=new List<Stock>();
		foreach(KeyValuePair<String,Trade> kvp in lastTrades){
			GMTrade gmTrade=(GMTrade)kvp.Value;
			Stock stock=new Stock();
			stock.Symbol=kvp.Key;
			stock.Price=gmTrade.Price;
			stock.IncPercent=gmTrade.Price/gmTrade.LastClose-1;
			stock.UpLimited=(gmTrade.Price==gmTrade.UpperLimit)?true:false;
			stocks.Add(stock);
		}
		stocks.Sort(delegate(Stock s1,Stock s2){
				return s2.IncPercent.CompareTo(s1.IncPercent);
			});
		
		Random r=new Random((int)DateTime.Now.Ticks);
		int k=0;
		foreach(Stock stock in stocks){
			if (k>=100) break;
			if (stock.UpLimited) continue;
			else {
				int a=r.Next(100);
				if (a>=50) {
					Instrument inst=InstrumentManager.Instruments[stock.Symbol];
					if (inst!=null&&this.strategy.Portfolio.Positions[inst]==null) {
						RandomEntry entry=new RandomEntry(inst,this.strategy);
						this.strategy.AddBehavior(inst,entry);
						entry.Deal();
						k++;
					}
				}
			}
		}
		ret=true;
		return ret;
	}
}