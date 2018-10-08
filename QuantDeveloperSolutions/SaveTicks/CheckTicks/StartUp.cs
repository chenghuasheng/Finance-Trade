using System;
using System.Collections.Generic;
using System.IO;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.File.Indexing;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;
using GMSDK;
public class StartUp
{
	private static GMSDK.MdApi _md=GMSDK.MdApi.Instance;
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		string dataPath="e:/QDData";
		bool overwrite=false;
		DateTime beginDate=new System.DateTime(2017,4,5);
		DateTime endDate=new System.DateTime(2017,4,7);
		
		string username="tianzhong_live@126.com";
		string password="Chs771005";
		
		int ret = _md.Init(username,password,MDMode.MD_MODE_NULL,"","","");
		if (ret != 0)
		{
			string msg = _md.StrError(ret);
			Console.WriteLine(msg);
			return;
		}
		DateTime curDate=beginDate;
		//获取交易日历，以确定是否开市日
		string dateString1="";
		if (endDate.AddMonths(-1)>beginDate) dateString1=beginDate.ToString("yyyy-MM-dd");
		else dateString1=endDate.AddMonths(-1).ToString("yyyy-MM-dd");
		string dateString2=endDate.AddDays(1).ToString("yyyy-MM-dd");
		List<DateTime> tradeDates=GetTradeDates("SHSE",dateString1,dateString2);
		
		while(curDate<endDate){
			
			Console.WriteLine("日期："+curDate.ToString("yyyy-MM-dd"));			
			if ((tradeDates.Count<=0)||(!tradeDates.Contains(curDate))){
				Console.WriteLine("今天不是交易日");
				curDate=curDate.AddDays(1);
				continue;
			}
			string path=dataPath+"/"+curDate.Year.ToString()+"/"+curDate.Month.ToString();
			if (!Directory.Exists(path)){
				Directory.CreateDirectory(path);
			}
		
			DateTime beginTime=curDate.Add(new TimeSpan(9,0,0)); 
			DateTime endTime=curDate.Add(new TimeSpan(24,0,0));
			string beginString=beginTime.ToString("yyyy-MM-dd HH:mm:ss");
			string endString=endTime.ToString("yyyy-MM-dd HH:mm:ss");
			Console.WriteLine("时间段:"+beginString+"----"+endString);
			try {
				DataFile file=DataFile.Open(path);
				Random ra=new Random();
				try {
					//获取股票代码列表
					List<string> symbols=new List<string>();
					symbols=GetSymbols("SHSE",1);
					symbols.AddRange(GetSymbols("SZSE",1));
					foreach(string symbol in symbols){
						//Console.WriteLine("正在检查证券"+symbol+"的Tick数据...");
						string name=symbol+".Trade";
						if (!file.Series.Contains(name)) {
							file.Series.Add(name);
						}
						FileSeries series = file.Series[name];
						ISeriesObject[] has=series.GetArray(beginTime,endTime);
						SmartQuant.Instruments.Instrument inst=InstrumentManager.Instruments[symbol];
						//获取当天的日线数据
						Daily dailyBar=null;
						if (inst==null){
							List<GMSDK.DailyBar> gskDailyBars=_md.GetDailyBars(symbol,curDate.ToString("yyyy-MM-dd"),
								curDate.ToString("yyyy-MM-dd"));	 
							if (gskDailyBars.Count>0) {
								List<ISeriesObject> dailys=GSKToGM.ConvertDailys(gskDailyBars);
								dailyBar=(Daily)dailys[0];
							}
						}else {
							DailySeries dailySeries=inst.GetDailySeries(curDate,curDate);
							if (dailySeries.Count>0){
								dailyBar=dailySeries[0];
							}
						}
						
						bool needRebuild=false;
						//检查是否丢失
						if (has.Length<=0) {
							if (dailyBar!=null) {
								Console.WriteLine("证券:{0}，在日期:{1}时丢失Tick数据",symbol,curDate);
								needRebuild=true;
							}
						}else {
							//检查是否有重复,一般情形下不会重复，所以这里注释掉
							/*int r1=ra.Next(0,has.Length-1);
							int r2=ra.Next(0,has.Length-1);
							
							ISeriesObject randTrade1=has[r1];
							ISeriesObject randTrade2=has[r2];
								
							ISeriesObject[] repeat1=series.GetArray(randTrade1.DateTime,randTrade1.DateTime);
							ISeriesObject[] repeat2=series.GetArray(randTrade2.DateTime,randTrade2.DateTime);
							if (repeat1.Length>1&&repeat2.Length>1) {
								Console.WriteLine("r1={0},r2={1}",r1,r2);
								Console.WriteLine("证券:{0}，在日期:{1}时有重复的Tick数据",symbol,curDate);
								needRebuild=true;
							}*/
							//检查是否不完整
							GMTrade lastTrade=(GMTrade)has[has.Length-1];	
							if ((dailyBar!=null)&&(lastTrade.TotalSize<dailyBar.Volume)){
								Console.WriteLine("证券:{0}，在日期:{1}时Tick数据不全",symbol,curDate);
								needRebuild=true;
							}
						}
						
						if (needRebuild){
						
							List<GMSDK.Tick> gskTicksCache = _md.GetTicks(symbol, beginString, endString);
							Console.WriteLine(symbol+"有"+gskTicksCache.Count.ToString()+"笔数据。");
							if (gskTicksCache.Count>0){
								//添加trades
								Console.WriteLine("存储Trade数据...");
								List<ISeriesObject> trades=GSKToGM.ConvertTrades(gskTicksCache);
								string name1=symbol+".Trade";
								if (!file.Series.Contains(name1)) {
									file.Series.Add(name1);
								}
								FileSeries series1 = file.Series[name1];
								ISeriesObject[] hasTrades=series1.GetArray(beginTime,endTime);
								if (overwrite||hasTrades.Length!=trades.Count) {
									foreach(ISeriesObject aTrade in hasTrades) series1.Remove(aTrade.DateTime);
									foreach (ISeriesObject trade in trades){
										series1.Add(trade);
									}
								}			
								series1.Reindex(Indexer.Daily);
								//添加quotes
								Console.WriteLine("存储Quote数据...");
								List<ISeriesObject> quotes=GSKToGM.ConvertQuotes(gskTicksCache);
								string name2=symbol+".Quote";
								if (!file.Series.Contains(name2)) {
									file.Series.Add(name2);
								}
								FileSeries series2 = file.Series[name2];
								ISeriesObject[] hasQuotes=series2.GetArray(beginTime,endTime);
								if (overwrite||hasQuotes.Length!=quotes.Count) {
									foreach(ISeriesObject aQuote in hasQuotes) series2.Remove(aQuote.DateTime);
									foreach (ISeriesObject quote in quotes){
										series2.Add(quote);
									}
								}							
								series2.Reindex(Indexer.Daily);
							}
						}
					}	
				}catch (Exception ex){
					throw ex;
				}
				finally {
					file.Close();	
				}
			}catch (Exception ex){
				Console.WriteLine(ex.Message);
			}
			
			curDate=curDate.AddDays(1);
		}
		
	}
	
	public static List<DateTime> GetTradeDates(string market,string beginDate,string endDate)
	{
		List<GMSDK.TradeDate> tradeDates = _md.GetCalendar(market, beginDate, endDate);
		List<DateTime> dates = new List<DateTime>();
		DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		foreach (GMSDK.TradeDate tradeDate in tradeDates)
		{
			dates.Add(startTimeUTC.AddSeconds(tradeDate.utc_time));
		}
		return dates;
	}
	
	public  static List<string> GetSymbols(string market, int securityType)
	{
		List<string> symbols = new List<string>();
		List<GMSDK.Instrument> gskInsts = _md.GetInstruments(market, securityType,0);
		foreach (GMSDK.Instrument gskInst in gskInsts)
		{
			symbols.Add(gskInst.symbol);
		}
		return symbols;
	}
}