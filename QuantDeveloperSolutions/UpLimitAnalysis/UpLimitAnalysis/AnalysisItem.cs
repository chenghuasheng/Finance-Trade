using System;
public class AnalysisItem{
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
	public double NextOpen;
	public double NextHigh;
	public double NextLow;
	public double NextClose;
}