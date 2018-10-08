using SmartQuant.FIX;
using System;
using System.ComponentModel;
namespace SmartQuant.Simulation
{
	public class CommissionProvider : ICommissionProvider
	{
		protected CommType fCommType;
		protected double fCommission;
		protected double fMinCommission;
		[Category("Parameter"), DefaultValue(0.0), Description("Commission value (depends on Commission Type)")]
		public virtual double Commission
		{
			get
			{
				return this.fCommission;
			}
			set
			{
				this.fCommission = value;
			}
		}
		[Category("Parameter"), DefaultValue(CommType.PerShare), Description("Commission type (FIX definitions : 1 = per unit (implying shares, par, currency, etc) 2 = percentage 3 = absolute (total monetary amount))")]
		public virtual CommType CommType
		{
			get
			{
				return this.fCommType;
			}
			set
			{
				this.fCommType = value;
			}
		}
		[Category("Parameter"), DefaultValue(0.0), Description("Minimal commission, absolute value")]
		public virtual double MinCommission
		{
			get
			{
				return this.fMinCommission;
			}
			set
			{
				this.fMinCommission = value;
			}
		}
		public CommissionProvider()
		{
			this.fCommType = CommType.PerShare;
			this.fCommission = 0.0;
			this.fMinCommission = 0.0;
		}
		public virtual FIXCommissionData GetCommissionData(FIXExecutionReport report)
		{
			FIXCommissionData fIXCommissionData = new FIXCommissionData();
			fIXCommissionData.CommType = FIXCommType.ToFIX(this.fCommType);
			fIXCommissionData.Commission = this.fCommission;
			if (this.fMinCommission != 0.0)
			{
				double num;
				switch (this.fCommType)
				{
				case CommType.PerShare:
					num = this.fCommission * report.CumQty;
					break;
				case CommType.Percent:
					num = this.fCommission * report.CumQty * report.AvgPx;
					break;
				case CommType.Absolute:
					num = this.fCommission;
					break;
				default:
					throw new NotSupportedException("Commission type is not supported : " + this.fCommType);
				}
				if (num < this.fMinCommission)
				{
					fIXCommissionData.CommType = '3';
					fIXCommissionData.Commission = this.fMinCommission;
				}
			}
			return fIXCommissionData;
		}
		public override string ToString()
		{
			return "Commission Provider";
		}
	}
}
