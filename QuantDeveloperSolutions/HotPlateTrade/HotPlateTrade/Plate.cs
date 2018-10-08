using System;
using System.Collections.Generic;
public class Plate
{
	public int ID;
	public string Name;
	public int Type;
	public List<Stock> Stocks = new List<Stock>();
	public int UpLimitCount=0;
	public int FivePercentCount = 0;
	public int UpCount = 0;
	public int ActiveCount=0;
	public float Weight = 0;
}