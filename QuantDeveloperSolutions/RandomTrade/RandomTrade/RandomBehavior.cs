using System;
using System.Collections.Generic;
using System.Threading;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Series;
using HuaQuant.Data.GM; 
using MongoDB.Bson;
using MongoDB.Driver;
public abstract class RandomBehavior:Behavior{
	protected int holdingPeriod =0;//持仓周期
	protected int holdingPeriodLimit=5;//持仓周期限制
	protected double lastPrice=0.0;//最后价格
	
	protected int dailyPeriod=40;//日线周期
	protected int min5Period=10;//分线周期
	protected int indexDailyPeriod=20;
	protected int indexMin5Period=10;
	
	protected GMRealTimeProvider provider=null;
	protected DateTime timeWhenDeal=DateTime.Today;
	protected double lastPriceWhenDeal=0.0;
	protected MongoCollection<BsonDocument> collection=null;
	
	
	public RandomBehavior(Instrument instrument,Strategy strategy):base(instrument,strategy){
		this.provider=(GMRealTimeProvider)this.strategy.MarketDataProvider;
	}	
	public override void OnNewTrade(Trade trade){
		if (HasPosition){
			this.lastPrice=trade.Price;
		}
	}
	public virtual void Deal(){
		this.timeWhenDeal=DateTime.Now;
		this.lastPriceWhenDeal=this.lastPrice;
	}
	public virtual void Save(){
		//修正持仓周期
		if (HasPosition&&this.holdingPeriod<1) this.holdingPeriod=1;//当天入场的
		if (this.holdingPeriod<1) return;
				
		//提取日线，分线数据，还有大盘的数据
		string symbol=this.instrument.Symbol;
		Instrument indexInst=null;
		if (symbol.IndexOf("SHSE.")>=0){
			indexInst=InstrumentManager.Instruments["SHSE.000001"];
		}else if (symbol.IndexOf("SZSE.3")>=0){
			indexInst=InstrumentManager.Instruments["SZSE.399006"];
		}else {
			indexInst=InstrumentManager.Instruments["SZSE.399001"];
		}
		//指数的日线和分线
		BarSeries indexDaily=this.getDailySeries(indexInst);
		if (indexDaily.Count<=0) {
			this.readLastNDailys(indexInst,this.indexDailyPeriod-1);
			this.addTodayDaily(indexInst);
		}
		BarSeries indexMin5=this.getMin5Series(indexInst);
		if (indexMin5.Count<=0) this.readLastNMin5s(indexInst,this.indexMin5Period);
		//股票的日线和分线
		BarSeries stockDaily=this.getDailySeries(this.instrument);
		if (stockDaily.Count<=0) this.readLastNDailys(this.instrument,this.dailyPeriod-1);
		BarSeries stockMin5=this.getMin5Series(this.instrument);
		if (stockMin5.Count<=0) this.readLastNMin5s(this.instrument,this.min5Period);
		
		if (stockDaily.Count<1) return;//没有日线的不记录
		//计算当天的奖赏
		double rewardForInside=0.0;//在场内的奖赏
		double rewardForOutside=0.0;//在场外的奖赏
		
		if (this.holdingPeriod==1) {
			rewardForInside=Math.Log(this.lastPrice/Position.EntryPrice);
		}else if (this.holdingPeriod>1){
			rewardForInside=Math.Log(this.lastPrice/stockDaily.Last.Close);
			rewardForOutside=Math.Log(this.lastPriceWhenDeal/stockDaily.Last.Close);
		}
		//Console.WriteLine("symbol:{0} --- last price is {1} --- entry Price is {2},has {3} ticks",
		//	this.instrument.Symbol,this.lastPrice,Position.EntryPrice,this.Trades[this.instrument].Count);
		//当天的日线,交易决断前一刻的数据
		this.addTodayDaily(this.instrument);
		//正规化数据
		List<NormalizedBar> indexNormalizedDaily=this.NormalizeBars(indexDaily);
		List<NormalizedBar> indexNormalizedMin5=this.NormalizeBars(indexMin5);
		List<NormalizedBar> stockNormalizedDaily=this.NormalizeBars(stockDaily);
		List<NormalizedBar> stockNormalizedMin5=this.NormalizeBars(stockMin5);
		
		//写入数据库
		try{
			this.initDB();	
			string curDateString=Clock.Now.Date.ToString("yyyy-MM-dd");
			//周期大于1且当天停盘没有交易的,不插入新记录，但要修改之前记录的下一个交易日期
			TradeArray trades=this.Trades[this.instrument];
			if (this.holdingPeriod>1&&
			(trades.Count<=0||trades.LastDateTime<Clock.Now.Date)){
				Console.WriteLine("证券{0}:今日没有交易",this.instrument.Symbol);
				BsonElement[] eleArray2 = new BsonElement[3];
				eleArray2[0]= new BsonElement("NextTradeDate", curDateString);
				eleArray2[1]= new BsonElement("Symbol",this.instrument.Symbol);
				eleArray2[2]= new BsonElement("Inside",1);
				QueryDocument query2 = new QueryDocument(eleArray2);
				UpdateDocument update=new UpdateDocument();
				update.Add(new BsonElement("$set",new QueryDocument(new BsonElement("NextTradeDate",(string)this.strategy.Global["NextTradeDate"]))));
				this.collection.Update(query2, update);
				return;
			}
			//如果当天已写入记录，先删除
			BsonElement[] eleArray = new BsonElement[2];
			eleArray[0]= new BsonElement("Date", curDateString);
			eleArray[1]= new BsonElement("Symbol",this.instrument.Symbol);
			QueryDocument query = new QueryDocument(eleArray);
			this.collection.Remove(query);	
			//写入在场外的记录，如果是第一天，则是假设没有买入，如果是第二天及以后，则是假设卖出
			//此情况下，下一交易日为空
			BsonElement[] eleArray1 = new BsonElement[12];
			eleArray1[0]= new BsonElement("Date", curDateString);
			eleArray1[1]= new BsonElement("Symbol",this.instrument.Symbol);
			eleArray1[2]= new BsonElement("HoldingPeriod",this.holdingPeriod);
			eleArray1[3]= new BsonElement("Inside",0);//当前是否在场内
			eleArray1[4]= new BsonElement("IndexDaily",this.GetBsonArrayFromBars(indexNormalizedDaily));
			eleArray1[5]= new BsonElement("IndexMin5",this.GetBsonArrayFromBars(indexNormalizedMin5));
			eleArray1[6]= new BsonElement("StockDaily",this.GetBsonArrayFromBars(stockNormalizedDaily));
			eleArray1[7]= new BsonElement("StockMin5",this.GetBsonArrayFromBars(stockNormalizedMin5));
			eleArray1[8]= new BsonElement("Reward",rewardForOutside);
			eleArray1[9]= new BsonElement("NextTradeDate","");
			BsonDocument insert=new BsonDocument(eleArray1);
			this.collection.Insert(insert);
			//写入在场内的记录，如果是第一天，则是买入，如果是第二天及以后，则是继续持有
			eleArray1 = new BsonElement[10];
			eleArray1[0]= new BsonElement("Date", curDateString);
			eleArray1[1]= new BsonElement("Symbol",this.instrument.Symbol);
			eleArray1[2]= new BsonElement("HoldingPeriod",this.holdingPeriod);
			eleArray1[3]= new BsonElement("Inside",1);//当前是否在场内
			eleArray1[4]= new BsonElement("IndexDaily",this.GetBsonArrayFromBars(indexNormalizedDaily));
			eleArray1[5]= new BsonElement("IndexMin5",this.GetBsonArrayFromBars(indexNormalizedMin5));
			eleArray1[6]= new BsonElement("StockDaily",this.GetBsonArrayFromBars(stockNormalizedDaily));
			eleArray1[7]= new BsonElement("StockMin5",this.GetBsonArrayFromBars(stockNormalizedMin5));
			eleArray1[8]= new BsonElement("Reward",rewardForInside);
			//已到了持仓周期限制，并且确实已卖出的，下一交易日为空
			if (this.holdingPeriod>=this.holdingPeriodLimit&&(!HasPosition)){	
				eleArray1[9]= new BsonElement("NextTradeDate","");
			}else {
				eleArray1[9]= new BsonElement("NextTradeDate",(string)this.strategy.Global["NextTradeDate"]);
			}
			insert=new BsonDocument(eleArray1);
			this.collection.Insert(insert);
		}catch(Exception ex){
			Console.WriteLine(ex.Message);
		}
	}
	protected BarSeries getDailySeries(Instrument inst){	
		return this.Bars[inst,BarType.Time,86400];
	}
	protected BarSeries getMin5Series(Instrument inst){	
		return this.Bars[inst,BarType.Time,300];
	}
	protected List<NormalizedBar> NormalizeBars(BarSeries bars){
		List<NormalizedBar> ret=new List<NormalizedBar>();
		double basePrice=0.0;
		double baseVol=0.0;
		foreach(Bar bar in bars){
			if (bar.High>basePrice) basePrice=bar.High;
			if (bar.Volume>baseVol) baseVol=bar.Volume;
		}
		
		foreach(Bar bar in bars){
			NormalizedBar nbar=new NormalizedBar();
			nbar.Close=bar.Close/basePrice;
			nbar.High=bar.High/basePrice;
			nbar.Low=bar.Low/basePrice;
			nbar.Open=bar.Open/basePrice;
			nbar.Volume=bar.Volume/baseVol;
			ret.Add(nbar);
		}
		return ret;
	}
	
	protected BsonArray GetBsonArrayFromBars(List<NormalizedBar> nbars ){
		BsonArray bsonArray=new BsonArray(nbars.Count);
		foreach(NormalizedBar nbar in nbars){
			BsonArray subBsonArray=new BsonArray(5);
			subBsonArray.Add(nbar.Open);
			subBsonArray.Add(nbar.High);
			subBsonArray.Add(nbar.Low);
			subBsonArray.Add(nbar.Close);
			subBsonArray.Add(nbar.Volume);
			bsonArray.Add(subBsonArray);
		}
		return bsonArray;
	}
	

	protected void readLastNDailys(Instrument inst,int n){
		//读取日线
		DateTime lastDate=this.timeWhenDeal.Date;
		ISeriesObject[] dailyBars=Util.GetNDailysBeforeDate(inst,lastDate,n);
		Util.AdjustDailys(dailyBars);//向前复权
		BarSeries barSeries=this.getDailySeries(inst);
		foreach(Daily dBar in dailyBars) {
			barSeries.Add(dBar);
		}
	}
	protected void readLastNMin5s(Instrument inst,int n){
		//获取分线
		List<Bar> min5Bars=new List<Bar>();
		int i=0;
		string lastTimeString=this.timeWhenDeal.ToString("yyyy-MM-dd HH:mm:ss");
		do {
			min5Bars=this.provider.GetLastNBars(inst.Symbol,300,n,lastTimeString);
			i++;
			Thread.Sleep(50);
		}while(min5Bars.Count==0&&i<=5);
		
		if (min5Bars.Count>0){
			BarSeries barSeries=this.getMin5Series(inst);
			foreach(Bar bar in min5Bars){
				barSeries.Add(bar);
			}
		}else {		
			Console.WriteLine("尝试多次({0})获取{1}的最新5分线失败",i,inst.Symbol);
		}
	}
	protected Trade getLastTrade(Instrument inst){
		//获取某时刻前最后一个Trade
		List<Trade> trades=null;
		int i=0;
		string lastTimeString=this.timeWhenDeal.ToString("yyyy-MM-dd HH:mm:ss");
		do {
			trades=this.provider.GetLastNTrades(inst.Symbol,lastTimeString,1);
			i++;
			Thread.Sleep(50);
		}while(trades.Count<=0&&i<=5);
		
		if (trades.Count<=0){	
			Console.WriteLine("尝试多次({0})获取{1}的最新价格失败",i,inst.Symbol);
			return null;
		}else {
			return trades[0];
		}
	}
	protected void addTodayDaily(Instrument inst){
		Trade lastTrade=this.getLastTrade(inst);
		if (lastTrade!=null){
			GMTrade gmTrade=(GMTrade)lastTrade;
			GMDaily daily=new GMDaily();
			daily.Date=gmTrade.DateTime.Date;
			daily.High=gmTrade.High;
			daily.Open=gmTrade.Open;
			daily.Low=gmTrade.Low;
			daily.Close=gmTrade.Price;
			daily.Volume=(long)gmTrade.TotalSize;
			BarSeries barSeries=this.getDailySeries(inst);
			barSeries.Add(daily);
		}
	}
	protected void initDB(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		this.collection = database.GetCollection<BsonDocument>("RandomTradeRecords");
	}
}