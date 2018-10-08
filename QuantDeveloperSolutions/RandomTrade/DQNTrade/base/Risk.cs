using System;
using System.Collections;
using System.Collections.Generic;
using SmartQuant;
using SmartQuant.FIX;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.Indicators;
using SmartQuant.Instruments;
using HuaQuant.Data.GM;

public class Risk:Behavior {
	public double PositionLevel{
		get {return this.strategy.PositionLevel;}
		internal set {this.strategy.PositionLevel=value;}
	}
	public Risk(Instrument instrument,Strategy strategy):base(instrument,strategy){}
	public override void OnInit(){
		DateTime curDate=Clock.Now.Date;
		ISeriesObject[] dailyBars=Utils.GetNDailysBeforeDate(this.instrument,curDate,60);
		foreach(Daily dBar in dailyBars) {
			this.Bar.Add(dBar);
		}
		bool overWrite=false;
		FileSeries series=(FileSeries)this.instrument.GetDataSeries("RiskLevel");
		if (series==null) series=(FileSeries)this.instrument.AddDataSeries("RiskLevel");
		float prevRiskLevel=1.0F;
		if (series.Count>0){
			DateTime lastDate=curDate;
			int i;
			do {
				lastDate=lastDate.AddDays(-1);
				i=series.IndexOf(lastDate);
			}while(i<0&&lastDate>series.FirstDateTime);
			if (i>=0) {
				ISeriesObject[] riskArray=series.GetArray(i,i);
				GMData gmData=(GMData)riskArray[0];
				prevRiskLevel=gmData.DataValue;
			}
		}
		GMDaily gmLastDaily=(GMDaily)this.DailyBar.Last;
		double inc=(gmLastDaily.Close/gmLastDaily.LastClose-1)*100;
		double abs=Math.Abs(inc);
		double riskLevel=prevRiskLevel+inc/abs*(int)(abs/0.5)*0.1;
		riskLevel=riskLevel<0.5?0.5:(riskLevel>1.2?1.2:riskLevel);
		GMData gmData1=new GMData(curDate,(float)riskLevel);
		if (overWrite){
			series.Remove(curDate);
			series.Add(gmData1);
		}else if (!series.Contains(curDate)) {
			series.Add(gmData1);
		}
		//this.PositionLevel=(float)riskLevel;
		Console.WriteLine("昨日涨跌幅：{0},今日风险水平为：{1}",inc,riskLevel);
	}
	public override void OnNewBar(Bar bar)
	{
	}
	public override void OnNewTrade(Trade trade)
	{
	}
}