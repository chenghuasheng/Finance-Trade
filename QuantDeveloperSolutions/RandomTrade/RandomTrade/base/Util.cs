using System;
using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Series;
using SmartQuant.Instruments;
using SmartQuant.File;
using HuaQuant.Data.GM;
public class Util{
	//获取某个证券某天以前的N条日线数据
	public static ISeriesObject[] GetNDailysBeforeDate(Instrument inst,DateTime date,int n){
		FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
		if (dailySeries==null||dailySeries.Count<=0) return new ISeriesObject[]{};
		DateTime lastDate=date;
		int i;
		do {
			lastDate=lastDate.AddDays(-1);
			i=dailySeries.IndexOf(lastDate);
		}while(i<0&&lastDate>dailySeries.FirstDateTime);
		if (i<0) return new ISeriesObject[]{};
		int j=i-n+1>0?i-n+1:0;
		return dailySeries.GetArray(j,i);
	}
	/*获取某个证券日线的个数*/
	public static int GetDailyCountBeforeDate(Instrument inst,DateTime date){
		FileSeries dailySeries=(FileSeries)inst.GetDataSeries(DataManager.EDataSeries.Daily);
		if (dailySeries==null||dailySeries.Count<=0) return 0;
		DateTime lastDate=date;
		int i;
		do {
			lastDate=lastDate.AddDays(-1);
			i=dailySeries.IndexOf(lastDate);
		}while(i<0&&lastDate>dailySeries.FirstDateTime);
		if (i<0) return 0;
		else return i-0+1;
	}
	//将日线向前复权
	public static void AdjustDailys(ISeriesObject[] gmDailys){//进行向前复权
		int num=gmDailys.Length;
		if (num>1){
			GMDaily lastDaily=(GMDaily)gmDailys[num-1];
			for(int i=num-2;i>=0;i--){
				GMDaily curDaily=(GMDaily)gmDailys[i];
				if (curDaily.AdjFactor!=lastDaily.AdjFactor){
					curDaily.Close=curDaily.Close*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.High=curDaily.High*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Low=curDaily.Low*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Open=curDaily.Open*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.LastClose=curDaily.LastClose*curDaily.AdjFactor/lastDaily.AdjFactor;
					curDaily.Volume=(long)(curDaily.Volume*lastDaily.AdjFactor/curDaily.AdjFactor);
					curDaily.AdjFactor=lastDaily.AdjFactor;
				}
				lastDaily=curDaily;	
			}
		}
	}
}