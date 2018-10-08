using SmartQuant.FIX;
using SmartQuant.Simulation.Design;
using System;
using System.ComponentModel;
namespace SmartQuant.Simulation
{
	[TypeConverter(typeof(SlippageProviderTypeConverter))]
	public interface ISlippageProvider
	{
		double GetExecutionPrice(ExecutionReport report);
	}
}
