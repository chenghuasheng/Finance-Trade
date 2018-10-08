using System;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.Data;
using HuaQuant.Data.GM;
public class GMRealTimeProviderExtend:GMRealTimeProvider{
	public new byte Id
	{
		get
		{
			return 248;
		}
	}
	public new string Name
	{
		get
		{
			return "GMRealTimeProviderExtend";
		}
	}
	public List<string> GetSymbols(string market, int securityType,int active)
	{
		List<GMSDK.Instrument> gskInsts=new List<GMSDK.Instrument>();
		lock (this._md) {
			gskInsts = this._md.GetInstruments(market, securityType, active);
		}
		List<string> symbols = new List<string>();
		foreach(GMSDK.Instrument gskInst in gskInsts)
		{
			symbols.Add(gskInst.symbol);
		}
		return symbols;
	}
	public Dictionary<string,Trade> GetLastTrades(string symbolList)
	{
		List<GMSDK.Tick> gskTicks = new List<GMSDK.Tick>();
		lock (this._md)
		{
			gskTicks = this._md.GetLastTicks(symbolList);
		}
		Dictionary<string, Trade> tradeDict = new Dictionary<string, Trade>();
		foreach (GMSDK.Tick gskTick in gskTicks)
		{
			string symbol= gskTick.exchange + "." + gskTick.sec_id;
			tradeDict.Add(symbol, GSKToGM.ConvertTrade(gskTick));
		}
		return tradeDict;
	}
	public List<Daily> GetLastNDaily(string symbol, int n, string lastDateString)
	{
		List<GMSDK.DailyBar> gskDailys;
		lock (this._md)
		{
			gskDailys = this._md.GetLastNDailyBars(symbol, n, lastDateString);
		}
		gskDailys.Reverse();
		List<Daily> dailys = new List<Daily>();
		foreach (GMSDK.DailyBar gskDaily in gskDailys)
		{
			dailys.Add(GSKToGM.ConvertDaily(gskDaily));
		}
		return dailys;
	}
	public List<Bar> GetBars(string symbol, int barSize ,string beginTimeString, string endTimeString)
	{
		List<GMSDK.Bar> gskBars;
		lock (this._md)
		{
			gskBars = this._md.GetBars(symbol, barSize, beginTimeString, endTimeString);
		}
		List<Bar> bars = new List<Bar>();
		foreach(GMSDK.Bar gskBar in gskBars)
		{
			bars.Add(GSKToGM.ConvertBar(gskBar));
		}
		return bars;
	}
}