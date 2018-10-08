using SmartQuant.FIX;
using System;
using System.ComponentModel;
namespace SmartQuant.Simulation
{
	public class SlippageProvider : ISlippageProvider
	{
		private double fSlippage;
		[Category("Parameter"), DefaultValue(0.0), Description("Slippage in percents, 0.01 = 1%")]
		public double Slippage
		{
			get
			{
				return this.fSlippage;
			}
			set
			{
				this.fSlippage = value;
			}
		}
		public double GetExecutionPrice(ExecutionReport report)
		{
			double num = report.AvgPx;
			if (report.OrdStatus == OrdStatus.Filled)
			{
				switch (report.Side)
				{
				case Side.Buy:
					num += num * this.fSlippage;
					break;
				case Side.Sell:
				case Side.SellShort:
					num -= num * this.fSlippage;
					break;
				}
			}
			return report.AvgPx = num;
		}
		public override string ToString()
		{
			return "Slippage Provider";
		}
	}
}
