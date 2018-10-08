using SmartQuant.FIX;
using System;
namespace SmartQuant.Simulation
{
	public class CustomCommissionProvider : CommissionProvider
	{
		public CustomCommissionProvider()
		{
			this.fCommType = CommType.PerShare;
			this.fCommission = 100.0;
		}
	}
}
