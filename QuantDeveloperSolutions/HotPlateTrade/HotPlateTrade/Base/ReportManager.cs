using SmartQuant.Testing;
using SmartQuant.Testing.TesterItems;
using System;
using System.Collections;

public class ReportManager
{
	protected LiveTester tester;

	protected void AddAllStatistics()
	{
		foreach (TesterItem item in this.tester.Components)
		{
			if (item is SeriesTesterItem)
			{
				(item as SeriesTesterItem).Enabled = true;
			}
		}
	}

	protected void AddComponentSet(string setName)
	{
		if (setName == "Performance Analysis")
		{
			this.tester.EnableComponent("InitialWealth");
			this.tester.EnableComponent("FinalWealth");
			this.tester.EnableComponent("TotalDays");
			this.tester.EnableComponent("TradeDays");
			this.tester.EnableComponent("GainDays");
			this.tester.EnableComponent("LossDays");
			this.tester.EnableComponent("Average Return (%)");
			this.tester.EnableComponent("Average Gain (%)");
			this.tester.EnableComponent("Average Loss (%)");
			this.tester.EnableComponent("Drawdown Average");
			this.tester.EnableComponent("Drawdown Median");
			this.tester.EnableComponent("Average Annual Return (%)");
			this.tester.EnableComponent("Median Annual Return (%)");
			this.tester.EnableComponent("Maximum Annual Return (%)");
			this.tester.EnableComponent("Minimum Annual Return (%)");
			this.tester.EnableComponent("Average Monthly Return (%)");
			this.tester.EnableComponent("Median Monthly Return (%)");
			this.tester.EnableComponent("Maximum Monthly Return (%)");
			this.tester.EnableComponent("Minimum Monthly Return (%)");
		}
		if (setName == "Risk Analysis")
		{
			this.tester.EnableComponent("MAR Ratio");
			this.tester.EnableComponent("Modified Sharpe Ratio");
			this.tester.EnableComponent("Sotrino Ratio");
			this.tester.EnableComponent("CompoundAverageReturn");
			this.tester.EnableComponent("Minimum DrawDown");
			this.tester.EnableComponent("StandardDeviation");
			this.tester.EnableComponent("GainStandardDeviation");
			this.tester.EnableComponent("LossStandardDeviation");
			this.tester.EnableComponent("Skewness");
			this.tester.EnableComponent("Kurtosis");
			this.tester.EnableComponent("SharpeRatio");
			this.tester.EnableComponent("VaR95");
			this.tester.EnableComponent("VaR99");
		}
		if (setName == "Rolling Statistics")
		{
			string[] strArray = new string[] { "month3", "month6", "month9", "year1", "year2" };
			string[] strArray2 = new string[] { "day1", "week1", "month1", "year1" };
			string[] strArray3 = new string[] { "Percentage of profitable #PER periods (%) (Inc = #INC)", "Average Return for every #PER period (%) (Inc = #INC)", "Median Return for every #PER period (%) (Inc = #INC)" };
			for (int i = 0; i < strArray3.Length; i++)
			{
				for (int j = 0; j < strArray.Length; j++)
				{
					for (int k = 0; k < strArray2.Length; k++)
					{
						string name = strArray3[i].Replace("#PER", strArray[j]).Replace("#INC", strArray2[k]);
						this.tester.EnableComponent(name);
					}
				}
			}
		}
		if (setName == "Daily")
		{
			this.tester.EnableComponent("DailyWealthSeries");
			this.tester.EnableComponent("DailyPnLSeries");
			this.tester.EnableComponent("DailyReturnSeries");
			this.tester.EnableComponent("DailyDrawdownSeries");
		}
		if (setName == "Monthly")
		{
			this.tester.EnableComponent("MonthlyWealthSeries");
			this.tester.EnableComponent("MonthlyPnLSeries");
			this.tester.EnableComponent("MonthlyReturnSeries");
			this.tester.EnableComponent("MonthlyDrawdownSeries");
		}
		if (setName == "Annual")
		{
			this.tester.EnableComponent("AnnualWealthSeries");
			this.tester.EnableComponent("AnnualPnLSeries");
			this.tester.EnableComponent("AnnualReturnSeries");
			this.tester.EnableComponent("AnnualDrawdownSeries");
		}
		if (setName == "RoundTrips Statistics")
		{
			ArrayList list1 = new ArrayList();
			list1.Add("Number Of RoundTrips");
			list1.Add("Number Of Winning RoundTrips");
			list1.Add("Number Of Losing RoundTrips");
			list1.Add("Percent Of Profitable (%)");
			list1.Add("Value Open RoundTrips");
			list1.Add("Total PnL Of All RoundTrips");
			list1.Add("Total PnL Of Winning RoundTrips");
			list1.Add("Total PnL Of Losing RoundTrips");
			list1.Add("Profit Per Winning Trade");
			list1.Add("Average RoundTrip");
			list1.Add("Largest Winning RoundTrip");
			list1.Add("Largest Losing RoundTrip");
			list1.Add("Average Winning RoundTrip");
			list1.Add("Average Losing RoundTrip");
			list1.Add("Ratio avg. win / avg. loss");
			list1.Add("Profit Factor");
			list1.Add("Maximal Consecutive Winners");
			list1.Add("Maximal Consecutive Losers");
			list1.Add("Average Total Efficiency");
			list1.Add("Average Entry Efficiency");
			list1.Add("Average Exit Efficiency");
			foreach (string str2 in list1)
			{
				this.tester.EnableComponent(str2);
				this.tester.EnableComponent("(Long) " + str2);
				this.tester.EnableComponent("(Short) " + str2);
			}
		}
		if (setName == "RoundTrips Duration Statistics")
		{
			ArrayList list2 = new ArrayList();
			list2.Add("Duration Of Last RoundTrip");
			list2.Add("Average Duration Of RoundTrips");
			list2.Add("Duration Of Last Winning RoundTrip");
			list2.Add("Average Duration Of Winning RoundTrips");
			list2.Add("Median Duration Of Winning RoundTrips");
			list2.Add("Maximum Duration Of Winning RoundTrips");
			list2.Add("Minimum Duration Of Winning RoundTrips");
			list2.Add("Duration Of Last Losing RoundTrip");
			list2.Add("Average Duration Of Losing RoundTrips");
			list2.Add("Median Duration Of Losing RoundTrips");
			list2.Add("Maximum Duration Of Losing RoundTrips");
			list2.Add("Minimum Duration Of Losing RoundTrips");
			foreach (string str3 in list2)
			{
				this.tester.EnableComponent(str3);
				this.tester.EnableComponent("(Long) " + str3);
				this.tester.EnableComponent("(Short) " + str3);
			}
		}
	}

	protected void AddStatistics(string name)
	{
		TesterItem item = this.tester.Components[name];
		if (item is SeriesTesterItem)
		{
			(item as SeriesTesterItem).Enabled = true;
		}
	}

	protected void RemoveAllStatistics()
	{
		foreach (TesterItem item in this.tester.Components)
		{
			if (item is SeriesTesterItem)
			{
				(item as SeriesTesterItem).Enabled = false;
			}
		}
	}

	public LiveTester Tester
	{
		get
		{
			return this.tester;
		}
		set
		{
			this.tester = value;
		}
	}
	
	public void Init()
	{
		if (this.Tester!=null){
			this.Tester.AllowRoundTrips = true;
			this.Tester.FollowChanges = true;		
			RemoveAllStatistics();		
			//AddComponentSet("Risk Analysis");
			//AddComponentSet("Performance Analysis");
			AddComponentSet("RoundTrips Statistics");
		}
	}
}
