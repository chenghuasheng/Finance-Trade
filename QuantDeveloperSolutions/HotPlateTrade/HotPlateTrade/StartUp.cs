using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SmartQuant.Providers;
using HuaQuant.Data.GM;

using SmartQuant;
using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Simulation;
using SmartQuant.Series;
public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		bool liveMode=true;
		PlateMonitorForm monitorForm=null;
		HotPlateTradeStrategy myStrategy=null;
		if (liveMode){
			monitorForm=new PlateMonitorForm();
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			RealPlateMonitor plateMonitor=new RealPlateMonitor(provider,@"D:\eastmoney\swc8",monitorForm);
			myStrategy=new HotPlateTradeStrategy(plateMonitor);		
			myStrategy.StrategyMode=StrategyMode.Live;
			myStrategy.MarketDataProvider=provider;
			myStrategy.ExecutionProvider=ProviderManager.ExecutionSimulator;
			myStrategy.ResetPortfolio=false;
			//myStrategy.SaveOrders=true;//是否保存委托订单
			myStrategy.Portfolio.Persistent=true;//是否保存投资组合
		}else {
			myStrategy=new HotPlateTradeStrategy(null);	
			myStrategy.ResetPortfolio=false;//每次运行是否重置投资组合
			myStrategy.CheckBuyPower=true;
			myStrategy.SimulationEntryDate=new DateTime(2017,1,1);
			myStrategy.SimulationExitDate=new DateTime(2017,1,31);
		}
		myStrategy.SimulationCash=100000;
		
		Thread aThread=new Thread(new ThreadStart(myStrategy.Start));
		if (aThread.ThreadState==ThreadState.Unstarted){
			aThread.Start();
		}
		if (monitorForm!=null){
			Application.Run(monitorForm);
		}else {
			MessageBox.Show("Press OK to stop ATS", "SmartQuant Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		myStrategy.Stop();
		aThread.Abort();
		aThread.Join();
				
		/*GraphShowForm showForm=new GraphShowForm();
		DateTime curDate=Clock.Now.Date;
		string symbol="SZSE.300503";
		Instrument inst=InstrumentManager.Instruments[symbol];
		ISeriesObject[] dailyBars=Util.GetNDailiesBeforeDate(inst,curDate,60);
		if (dailyBars.Length>0){
		Util.AdjustDailys(dailyBars);//向前复权	
		double[] inputs=new double[60];
		double[] outputs=new double[60];
		double basePrice=((GMDaily)dailyBars[0]).Amount/((GMDaily)dailyBars[0]).Volume;
		for(int i=0;i<dailyBars.Length;i++){
		inputs[i]=i;
		outputs[i]=((GMDaily)dailyBars[i]).Amount/((GMDaily)dailyBars[i]).Volume;
		outputs[i]=(outputs[i]/basePrice-1)*100;
		}
		PatternRecognition pr=new PatternRecognition(showForm);
		RecognitionState ret=pr.Recognition(inputs,outputs);
		Console.WriteLine("slope={0},shape={1},speed={2}",ret.Slope,ret.Shape,ret.Speed);
		Application.Run(showForm);
		}else {
		Console.WriteLine("no data.");
		}*/
		Console.WriteLine("Strategy Stopped!");
	}
}