using SmartQuant.Testing;
using SmartQuant.Testing.TesterItems;
using System;
using System.Collections;
using System.ComponentModel;

namespace SmartQuant.Trading
{
	[StrategyComponent("{5E7810DC-C9C1-427f-8CD9-1DFFE26E59B5}", ComponentType.ReportManager, Name = "Default_ReportManager", Description = "")]
	public class ReportManager : ComponentBase
	{
		public const string GUID = "{5E7810DC-C9C1-427f-8CD9-1DFFE26E59B5}";

		protected LiveTester tester;

		[Browsable(false)]
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

		protected void RemoveAllStatistics()
		{
			foreach (TesterItem testerItem in this.tester.Components)
			{
				if (testerItem is SeriesTesterItem)
				{
					(testerItem as SeriesTesterItem).Enabled = false;
				}
			}
		}

		protected void AddAllStatistics()
		{
			foreach (TesterItem testerItem in this.tester.Components)
			{
				if (testerItem is SeriesTesterItem)
				{
					(testerItem as SeriesTesterItem).Enabled = true;
				}
			}
		}

		protected void AddStatistics(string name)
		{
			TesterItem testerItem = this.tester.Components[name];
			if (testerItem is SeriesTesterItem)
			{
				(testerItem as SeriesTesterItem).Enabled = true;
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
				string[] array = new string[]
				{
					"month3",
					"month6",
					"month9",
					"year1",
					"year2"
				};
				string[] array2 = new string[]
				{
					"day1",
					"week1",
					"month1",
					"year1"
				};
				string[] array3 = new string[]
				{
					"Percentage of profitable #PER periods (%) (Inc = #INC)",
					"Average Return for every #PER period (%) (Inc = #INC)",
					"Median Return for every #PER period (%) (Inc = #INC)"
				};
				for (int i = 0; i < array3.Length; i++)
				{
					for (int j = 0; j < array.Length; j++)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							string name = array3[i].Replace("#PER", array[j]).Replace("#INC", array2[k]);
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
				foreach (string text in new ArrayList
				{
					"Number Of RoundTrips",
					"Number Of Winning RoundTrips",
					"Number Of Losing RoundTrips",
					"Percent Of Profitable (%)",
					"Value Open RoundTrips",
					"Total PnL Of All RoundTrips",
					"Total PnL Of Winning RoundTrips",
					"Total PnL Of Losing RoundTrips",
					"Profit Per Winning Trade",
					"Average RoundTrip",
					"Largest Winning RoundTrip",
					"Largest Losing RoundTrip",
					"Average Winning RoundTrip",
					"Average Losing RoundTrip",
					"Ratio avg. win / avg. loss",
					"Profit Factor",
					"Maximal Consecutive Winners",
					"Maximal Consecutive Losers",
					"Average Total Efficiency",
					"Average Entry Efficiency",
					"Average Exit Efficiency"
				})
				{
					this.tester.EnableComponent(text);
					this.tester.EnableComponent("(Long) " + text);
					this.tester.EnableComponent("(Short) " + text);
				}
			}
			if (setName == "RoundTrips Duration Statistics")
			{
				foreach (string text2 in new ArrayList
				{
					"Duration Of Last RoundTrip",
					"Average Duration Of RoundTrips",
					"Duration Of Last Winning RoundTrip",
					"Average Duration Of Winning RoundTrips",
					"Median Duration Of Winning RoundTrips",
					"Maximum Duration Of Winning RoundTrips",
					"Minimum Duration Of Winning RoundTrips",
					"Duration Of Last Losing RoundTrip",
					"Average Duration Of Losing RoundTrips",
					"Median Duration Of Losing RoundTrips",
					"Maximum Duration Of Losing RoundTrips",
					"Minimum Duration Of Losing RoundTrips"
				})
				{
					this.tester.EnableComponent(text2);
					this.tester.EnableComponent("(Long) " + text2);
					this.tester.EnableComponent("(Short) " + text2);
				}
			}
		}
	}
}
