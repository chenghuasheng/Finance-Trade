using SmartQuant.FIX;
using SmartQuant.Simulation.Design;
using System;
using System.ComponentModel;
namespace SmartQuant.Simulation
{
	[TypeConverter(typeof(CommissionProviderTypeConverter))]
	public interface ICommissionProvider
	{
		CommType CommType
		{
			get;
			set;
		}
		double Commission
		{
			get;
			set;
		}
		FIXCommissionData GetCommissionData(FIXExecutionReport report);
	}
}
