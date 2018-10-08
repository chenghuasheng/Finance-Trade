using System;
using System.IO;
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

public class StartUp
{
	private static GMSDK.MdApi _md=GMSDK.MdApi.Instance;
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		string username="tianzhong_live@126.com";
		string password="Chs771005";
		int ret = _md.Init(username,password,GMSDK.MDMode.MD_MODE_NULL,"","","");
		if (ret != 0)
		{
			string msg = _md.StrError(ret);
			Console.WriteLine(msg);
			return;
		}
		string dataPath="e:/QDData";
		DateTime beginDate=new System.DateTime(2017,4,10);
		DateTime endDate=new System.DateTime(2017,4,17);
		string beginDateString=beginDate.ToString("yyyy-MM-dd");
		string endDateString=endDate.ToString("yyyy-MM-dd");
		Dictionary<string,Dictionary<string,double>> shareDict=new Dictionary<string,Dictionary<string,double>>();//存放股本
		List<AnalysisItem> analysisList=new List<AnalysisItem>();
		foreach(Instrument inst in InstrumentManager.Instruments){
			if (inst.SecurityDesc.IndexOf("B")>=0) continue;//除去B股
			FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
			if (dailySeries==null||dailySeries.Count<=0) continue;
			int i;
			DateTime firstDate=beginDate;
			do {
				i=dailySeries.IndexOf(firstDate);
				firstDate=firstDate.AddDays(1);
			}while(i<0&&firstDate<endDate);
			if (i<0) continue;
			i=i-5>0?i-5:0;
			firstDate=dailySeries[i].DateTime;
			ISeriesObject[] dailys=dailySeries.GetArray(firstDate,endDate);
			
			for(int k=0;k<dailys.Length;k++){
				GMDaily gmDaily=(GMDaily)dailys[k];
				if (gmDaily.DateTime>=beginDate&&gmDaily.DateTime<endDate){
					if (gmDaily.Close/gmDaily.LastClose>1.095) {//今日涨停
						AnalysisItem ai=new AnalysisItem();
						ai.Date=gmDaily.DateTime;
						ai.Symbol=inst.Symbol;
						Dictionary<string,double> sd=null;
						if (!shareDict.TryGetValue(inst.Symbol,out sd)){
							sd=new Dictionary<string,double>();
							List<GMSDK.ShareIndex> shareIndexList=_md.GetShareIndex(inst.Symbol,beginDateString,endDateString);
							foreach(GMSDK.ShareIndex si in shareIndexList){
								sd.Add(si.pub_date,si.flow_a_share);
							}
							shareDict.Add(inst.Symbol,sd);
						}
						sd.TryGetValue(ai.Date.ToString("yyyy-MM-dd"),out ai.FlowAShare);
						ai.LastClose=gmDaily.LastClose;
						ai.Open=gmDaily.Open;
						ai.High=gmDaily.High;
						ai.Low=gmDaily.Low;
						ai.Close=gmDaily.Close;
						
						if (ai.FlowAShare>0) ai.TurnoverRate=gmDaily.Volume/ai.FlowAShare*100;
						int m=k-1;
						double sumVol=0.0;
						while(m>=0&&m>=k-5){
							sumVol+=((Daily)dailys[m]).Volume;
							m--;
						}
						double last5AvgVol=sumVol/(k-m-1);
						ai.VolDivLast5AvgVol=gmDaily.Volume/last5AvgVol;
						string path=dataPath+"/"+gmDaily.DateTime.Year.ToString()+"/"+gmDaily.DateTime.Month.ToString();
						if (!Directory.Exists(path)){
							Console.WriteLine("Trade数据目录不存在！");
							return;
						}
						DataFile file=null;
						bool flag=false;
						try {
							file=DataFile.Open(path);
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
							GMQuote gmLastQuote=(GMQuote)quotes[quotes.Length-1];//最后一笔报价数据
							ai.BidSizeDivVol=(double)gmLastQuote.BidSize/gmDaily.Volume;
						}catch(Exception ex){
							Console.WriteLine(ex.Message);
						}finally{
							file.Close();
						}
						if (k>0) {
							GMDaily lastGMDaily=(GMDaily)dailys[k-1];
							if (lastGMDaily.Close/lastGMDaily.LastClose>1.098) ai.LastUpLimited=true;
						}
						if (k<dailys.Length-1){
							GMDaily nextGMDaily=(GMDaily)dailys[k+1];
							ai.NextOpen=nextGMDaily.Open;
							ai.NextHigh=nextGMDaily.High;
							ai.NextLow=nextGMDaily.Low;
							ai.NextClose=nextGMDaily.Close;
						}
						if (flag) analysisList.Add(ai);
					}
				}
			}
		}
		Console.WriteLine(analysisList.Count);
		analysisList.Sort(delegate(AnalysisItem ai1,AnalysisItem ai2){
				return ai1.Date.CompareTo(ai2.Date);
			});
		foreach(AnalysisItem ai in analysisList)
		{
			Console.WriteLine(@"Date:{0},Symbol:{1},昨收价:{2},昨日是否涨停:{3},开盘价:{4},
			最高价:{5},最低价:{6},收盘价:{7},涨停时间:{8},是否开板:{9},涨停时长:{10},
			成交量与昨日5日均量比:{11},收盘买一量与成交量比:{12},
				明日开盘价:{13},明日最高价:{14},明日最低价:{15},明日收盘价:{16},流通A股:{17},换手率:{18}",
				ai.Date,ai.Symbol,ai.LastClose,ai.LastUpLimited,ai.Open,ai.High,ai.Low,ai.Close,
				ai.WhenUpLimit,ai.UpLimitBreaked,ai.HowLongUpLimit,ai.VolDivLast5AvgVol,ai.BidSizeDivVol,
				ai.NextOpen,ai.NextHigh,ai.NextLow,ai.NextClose,ai.FlowAShare,ai.TurnoverRate);
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
			eleArray1[15]=new BsonElement("nextopen",ai.NextOpen);
			eleArray1[16]=new BsonElement("nexthigh",ai.NextHigh);
			eleArray1[17]=new BsonElement("nextlow",ai.NextLow);
			eleArray1[18]=new BsonElement("nextclose",ai.NextClose);
			BsonDocument insert=new BsonDocument(eleArray1);
			collection.Insert(insert);
		}
	}
}