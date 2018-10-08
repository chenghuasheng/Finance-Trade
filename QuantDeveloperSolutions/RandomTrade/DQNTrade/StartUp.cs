using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using SmartQuant.Providers;
using HuaQuant.Data.GM;
public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		bool liveMode=true;
		DQNTradeStrategy myStrategy=new DQNTradeStrategy();
		if (liveMode){
			GMRealTimeProvider provider=(GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];		
			myStrategy.StrategyMode=StrategyMode.Live;
			myStrategy.MarketDataProvider=provider;
			myStrategy.ExecutionProvider=ProviderManager.ExecutionSimulator;
			myStrategy.ResetPortfolio=false;
			myStrategy.SaveOrders=false;//是否保存委托订单
			myStrategy.Portfolio.Persistent=true;//是否保存投资组合
		}else {
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
		MessageBox.Show("Press OK to stop ATS", "SmartQuant Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);
		myStrategy.Stop();
		aThread.Abort();
		aThread.Join();
	}
}