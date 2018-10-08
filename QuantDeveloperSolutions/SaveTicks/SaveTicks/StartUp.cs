using System;
using System.Collections.Generic;
using System.IO;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.File.Indexing;
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
		DateTime beginDate=new System.DateTime(2017,1,1);
		DateTime endDate=new System.DateTime(2017,4,1);
		
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
				try {
					//获取股票代码列表
					List<string> symbols=new List<string>();
					symbols.Add("SHSE.000001");
					symbols.Add("SHSE.000002");
					symbols.Add("SZSE.399001");
					symbols.Add("SZSE.399006");
					//symbols=GetSymbols("SHSE",1);
					//symbols.AddRange(GetSymbols("SZSE",1));
					foreach(string symbol in symbols){
						Console.WriteLine("正在处理证券"+symbol+"的Tick数据...");
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
				catch (Exception ex){
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