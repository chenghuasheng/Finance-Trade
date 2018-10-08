using System;
using System.Collections.Generic;
using System.IO;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.File.Indexing;
using HuaQuant.Data.GM;

using MongoDB.Bson;
using MongoDB.Driver;
public class BuildUpLimitsJob:Job{
	protected string dataPath="e:/QDData";
	protected DateTime curDate;
	public BuildUpLimitsJob(string name,DateTime date,Job[] needJobs):base(name,needJobs){
		this.curDate=date;
	}
	public BuildUpLimitsJob(string name,DateTime date):base(name){
		this.curDate=date;
	}
	protected override bool doJob(){
		Console.WriteLine("正在读取Tick数据生成涨停板数据分析表...");
		/*Console.WriteLine("测试作业.........");
		return true;*/
		try {
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			provider.Connect(10000);
			if (!provider.IsConnected) return false;
			string path=this.dataPath+"/"+this.curDate.Year.ToString()+"/"+this.curDate.Month.ToString();
			if (!Directory.Exists(path)){
				Console.WriteLine("Trade数据目录不存在！");
				return false;
			}
			string dateString=this.curDate.ToString("yyyy-MM-dd");
			List<AnalysisItem> analysisList=new List<AnalysisItem>();
			DataFile file=null;
			try {
				file=DataFile.Open(path);
				foreach(Instrument inst in InstrumentManager.Instruments){
					if (inst.SecurityDesc.IndexOf("B")>=0) continue;//除去B股
					if (inst.SecurityType=="IDX") continue;//除去指数
					FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
					if (dailySeries==null||dailySeries.Count<=0) continue;
					int i=dailySeries.IndexOf(this.curDate);
					if (i<0) continue;//没有日线，当日不开盘
					i=i-10>0?i-10:0;//往前推10个交易日
					DateTime firstDate=dailySeries[i].DateTime;
					ISeriesObject[] dailys=dailySeries.GetArray(firstDate,this.curDate);
					int k=dailys.Length-1;
					GMDaily gmDaily=(GMDaily)dailys[k];
			
					if (gmDaily.Close/gmDaily.LastClose>1.095) {//今日涨停
						AnalysisItem ai=new AnalysisItem();
						ai.Date=gmDaily.DateTime;
						ai.Symbol=inst.Symbol;
						GMSDK.ShareIndex si=null;
						//先读取当日股本，如果没有，则读取最近股本。一般情况下当日股本与最近股本相同，除非当天正好是除权日
						List<GMSDK.ShareIndex> shareIndexList=provider.MdApi.GetShareIndex(ai.Symbol,dateString,dateString);
						if (shareIndexList.Count<=0) shareIndexList=provider.MdApi.GetLastShareIndex(ai.Symbol);
						if (shareIndexList.Count>0) si=shareIndexList[0];
						if (si!=null){
							ai.FlowAShare=si.flow_a_share;
						}else {
							ai.FlowAShare=0.0;
						}
						
						ai.LastClose=gmDaily.LastClose;
						ai.Open=gmDaily.Open;
						ai.High=gmDaily.High;
						ai.Low=gmDaily.Low;
						ai.Close=gmDaily.Close;
						//换手率	
						if (ai.FlowAShare>0) {
							ai.TurnoverRate=gmDaily.Volume/ai.FlowAShare*100;
						}else {
							ai.TurnoverRate=0.0;
						}
						//当日量与前5日均量比
						int m=k-1;
						double sumVol=0.0;
						while(m>=0&&m>=k-5){
							sumVol+=((Daily)dailys[m]).Volume;
							m--;
						}
						double last5AvgVol=sumVol/(k-m-1);
						ai.VolDivLast5AvgVol=gmDaily.Volume/last5AvgVol;		
						//计算封板时间、时长等
						bool flag=false;
						string name=inst.Symbol+".Trade";
						if (!file.Series.Contains(name)) continue;
						FileSeries series = file.Series[name];
						ISeriesObject[] trades=series.GetArray(gmDaily.DateTime,gmDaily.DateTime.AddDays(1));
						if (trades.Length<=0) continue;
						GMTrade gmLastTrade=(GMTrade)trades[trades.Length-1];//最后一笔交易数据
						if (gmLastTrade.Price<gmLastTrade.UpperLimit) continue;	
						GMTrade lastGMTrade=null;
						DateTime openTimeAM=gmDaily.Date.Add(new TimeSpan(9,25,0));
						DateTime openTimePM=gmDaily.Date.Add(new TimeSpan(13,0,0));
						foreach(ISeriesObject aTrade in trades){
							GMTrade gmTrade=(GMTrade)aTrade;
							if (!flag&&gmTrade.Price==gmTrade.UpperLimit&&gmTrade.DateTime>=openTimeAM){
								ai.WhenUpLimit=gmTrade.DateTime-ai.Date;
								flag=true;
							}
							if (flag&&gmTrade.Price<gmTrade.UpperLimit){
								ai.UpLimitBreaked=true;
							}
							//将下午13：00之后的每笔时间减去90分钟，这样便于计算封涨停的时长
							if (gmTrade.DateTime>=openTimePM){
								gmTrade.DateTime=gmTrade.DateTime.AddMinutes(-90);
							}
							if (flag&&lastGMTrade!=null&&gmTrade.Price==gmTrade.UpperLimit&&lastGMTrade.Price==gmTrade.UpperLimit){	
								ai.HowLongUpLimit+=gmTrade.DateTime-lastGMTrade.DateTime;
							}
							if (flag) lastGMTrade=gmTrade;	
						}
						//封成比
						name=inst.Symbol+".Quote";
						if (!file.Series.Contains(name)) continue;
						series = file.Series[name];
						ISeriesObject[] quotes=series.GetArray(gmDaily.DateTime,gmDaily.DateTime.AddDays(1));
						if (quotes.Length<=0) continue;
						GMQuote gmLastQuote=(GMQuote)quotes[quotes.Length-1];//最后一刻报价数据
						ai.BidSizeDivVol=(double)gmLastQuote.BidSize/gmDaily.Volume;
				
						if (k>0) {
							GMDaily lastGMDaily=(GMDaily)dailys[k-1];
							if (lastGMDaily.Close/lastGMDaily.LastClose>=1.099) ai.LastUpLimited=true;
						}
						if (flag) analysisList.Add(ai);
					}		
				}
			}catch(Exception ex){
				throw ex;
			}finally{
				file.Close();
			}
			Console.WriteLine("今日共有{0}只涨停。",analysisList.Count);
			foreach(AnalysisItem ai in analysisList)
			{
				Console.WriteLine(@"Date:{0},Symbol:{1},昨收价:{2},昨日是否涨停:{3},开盘价:{4},
					最高价:{5},最低价:{6},收盘价:{7},涨停时间:{8},是否开板:{9},涨停时长:{10},
					成交量与昨日5日均量比:{11},收盘买一量与成交量比:{12},流通A股:{13},换手率:{14}",
					ai.Date,ai.Symbol,ai.LastClose,ai.LastUpLimited,ai.Open,ai.High,ai.Low,ai.Close,
					ai.WhenUpLimit,ai.UpLimitBreaked,ai.HowLongUpLimit,ai.VolDivLast5AvgVol,ai.BidSizeDivVol,
					ai.FlowAShare,ai.TurnoverRate);
			}
		
			MongoClient client = new MongoClient("mongodb://localhost:27017");
			MongoServer server = client.GetServer();
			MongoDatabase database = server.GetDatabase("finance");
		
			MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("UpLimitAnalysis");
			foreach(AnalysisItem ai in analysisList)
			{
				BsonElement[] eleArray = new BsonElement[2];
				eleArray[0]= new BsonElement("date", ai.Date.ToString("yyyy-MM-dd"));
				eleArray[1]= new BsonElement("symbol", ai.Symbol);
				QueryDocument query = new QueryDocument(eleArray);
				collection.Remove(query);
				BsonElement[] eleArray1 = new BsonElement[19];
				eleArray1[0]=new BsonElement("date",ai.Date.ToString("yyyy-MM-dd"));
				eleArray1[1]=new BsonElement("symbol",ai.Symbol);
				eleArray1[2]=new BsonElement("flowashare",ai.FlowAShare);
				eleArray1[3]=new BsonElement("lastclose",ai.LastClose);
				eleArray1[4]=new BsonElement("lastuplimited",ai.LastUpLimited);
				eleArray1[5]=new BsonElement("open",ai.Open);
				eleArray1[6]=new BsonElement("high",ai.High);
				eleArray1[7]=new BsonElement("low",ai.Low);
				eleArray1[8]=new BsonElement("close",ai.Close);
				eleArray1[9]=new BsonElement("whenuplimit",ai.WhenUpLimit.ToString());
				eleArray1[10]=new BsonElement("uplimitbreaked",ai.UpLimitBreaked);
				eleArray1[11]=new BsonElement("howlonguplimit",ai.HowLongUpLimit.ToString());
				eleArray1[12]=new BsonElement("turnoverrate",ai.TurnoverRate);
				eleArray1[13]=new BsonElement("voldivlast5avgvol",ai.VolDivLast5AvgVol);
				eleArray1[14]=new BsonElement("bidsizedivvol",ai.BidSizeDivVol);
				BsonDocument insert=new BsonDocument(eleArray1);
				collection.Insert(insert);
			}
			return true;
		}catch (Exception ex){
			Console.WriteLine(ex.Message);
		}
		return false;
	}
	protected struct AnalysisItem{
		public DateTime Date;
		public string Symbol;
		public double FlowAShare;
		public double LastClose;
		public bool LastUpLimited;
		public double Open;
		public double High;
		public double Low;
		public double Close;
		public TimeSpan WhenUpLimit;
		public bool UpLimitBreaked;
		public TimeSpan HowLongUpLimit;
		public double TurnoverRate;
		public double VolDivLast5AvgVol;
		public double BidSizeDivVol;
	}
}
