using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Series;
using HuaQuant.Data.GM;
public class SaveDailysJob:Job{
	protected DateTime dealDate;
	protected List<string> hasDailySymbols=new List<string>();
	public SaveDailysJob(string name,DateTime jobDate,Job[] needJobs):base(name,needJobs){
		this.dealDate=jobDate;
	}
	public SaveDailysJob(string name,DateTime jobDate):this(name,jobDate,null){}
	protected override bool doJob(){
		Console.WriteLine("正在下载Daily数据...");
		bool flag=true;
		try {
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			provider.Connect(10000);
			if (provider.IsConnected){
				string endString=Utils.FormatDate(this.dealDate);
				string lastTimeString=Utils.FormatTime(this.dealDate.AddDays(1));
				foreach(Instrument inst in InstrumentManager.Instruments){
					string symbol=inst.Symbol;
					if (this.hasDailySymbols.Contains(symbol)) continue;
					List<Daily> gmDailys=new List<Daily>();
					int i=0;
					bool hasTrade=true;
					do {
						gmDailys=provider.GetDailys(symbol,endString,endString);
						i++;
						if (gmDailys.Count>0) {
							inst.Add(gmDailys[0]);
							this.hasDailySymbols.Add(symbol);
						}else {		
							List<Trade> gmTrades=null;	
							gmTrades=provider.GetLastNTrades(symbol,lastTimeString,1);
							if (gmTrades.Count<=0||gmTrades[0].DateTime<this.dealDate||((GMTrade)gmTrades[0]).Open<=0.0) {
								Console.WriteLine("证券{0}：当天没有交易",symbol);
								hasTrade=false;
							}else {
								Console.WriteLine("证券{0}：当天有交易，但没有读取到日线",symbol);
							}
						}
					}while(gmDailys.Count<=0&&i<5&&hasTrade);
					if (gmDailys.Count<=0&&hasTrade){
						Console.WriteLine("尝试多次({0})获取{1}的最新日线失败",i,symbol);
						flag=false;
					}
				}
			}else {
				Console.WriteLine("GMRealTimeProvider is not connected.");
				flag=false;
			}
		}catch(Exception ex){
			Console.WriteLine(ex.Message);
			flag=false;
		}
		return flag;
	}
}