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
	protected DateTime curDate;
	public SaveDailysJob(string name,DateTime date,Job[] needJobs):base(name,needJobs){
		this.curDate=date;
	}
	public SaveDailysJob(string name,DateTime date):base(name){
		this.curDate=date;
	}
	protected override bool doJob(){
		Console.WriteLine("正在下载Daily数据...");
		/*Console.WriteLine("测试作业.........");
		return true;*/
		try {
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			provider.Connect(10000);
			if (provider.IsConnected){
				string endString=this.curDate.ToString("yyyy-MM-dd");
				foreach(Instrument inst in InstrumentManager.Instruments){
					string symbol=inst.Symbol;
					List<GMSDK.DailyBar> gmsdkDailys=provider.MdApi.GetDailyBars(symbol,endString,endString);
					List<ISeriesObject> gmDailys=GSKToGM.ConvertDailys(gmsdkDailys);
					Console.WriteLine("证券:{0}有{1}笔新日线.",symbol,gmDailys.Count);
					if (gmDailys.Count>0) inst.Add((Daily)(gmDailys[0]));
				}
				return true;
			}
		}catch(Exception ex){
			Console.WriteLine(ex.Message);
		}
		return false;
	}
}