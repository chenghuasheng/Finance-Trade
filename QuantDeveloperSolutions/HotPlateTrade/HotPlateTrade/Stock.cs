using System;
using System.Collections.Generic;
public class Stock
{
	public string Symbol;
	//public string Name;
	public List<Plate> Plates = new List<Plate>();
	public double Price = 0.0;
	public double IncPercent = 0.0;
	public bool UpLimited = false;
	//public int HotPlateCount = 0;
	//public List<DailyBar> DailyBars = new List<DailyBar>();
}