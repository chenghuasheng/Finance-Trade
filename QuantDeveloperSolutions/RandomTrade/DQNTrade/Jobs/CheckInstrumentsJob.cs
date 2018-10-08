using System;
using System.Collections.Generic;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using HuaQuant.Data.GM;
public class CheckInstrumentsJob:Job{
	public CheckInstrumentsJob(string name,Job[] needJobs):base(name,needJobs){}
	public CheckInstrumentsJob(string name):base(name){}
	protected override bool doJob(){
		Console.WriteLine("正在检查证券定义...");
		/*Console.WriteLine("测试作业.........");
		return true;*/
		try {
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			provider.Connect(10000);
			if (provider.IsConnected){
				List<GMSDK.Instrument> gskInsts=provider.MdApi.GetInstruments("SHSE",1,0);
				gskInsts.AddRange(provider.MdApi.GetInstruments("SZSE",1,0));
				foreach(GMSDK.Instrument gskInst in gskInsts){
					if(InstrumentManager.Instruments[gskInst.symbol] == null) 
					{
						Instrument newInst = new Instrument(gskInst.symbol, "CS"); 
						string[] ss = gskInst.symbol.Split('.');
						newInst.SecurityExchange = ss[0];//市场
						newInst.SecurityID = ss[1];//代码
						newInst.SecurityIDSource = "8";
						newInst.Save();
						Console.WriteLine("新证券:{0}已添加.",newInst.Symbol);
					}  
				}
				return true;
			}
		}catch(Exception ex){
			Console.WriteLine(ex.Message);		
		}
		return false;
	}
}