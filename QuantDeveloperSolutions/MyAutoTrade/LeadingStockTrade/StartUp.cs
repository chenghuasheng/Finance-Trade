using System;
using System.Threading;
using System.Windows.Forms;
using SmartQuant.Providers;

public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		LeadingStrategy myStrategy=new LeadingStrategy();		
		//myStrategy.StrategyMode=StrategyMode.Live;
		if (myStrategy.StrategyMode==StrategyMode.Live) {
			myStrategy.MarketDataProvider=ProviderManager.MarketDataProviders["GMRealTimeProvider"];
			myStrategy.ExecutionProvider=ProviderManager.ExecutionSimulator;
			myStrategy.ResetPortfolio=false;
			//myStrategy.SaveOrders=true;//是否保存委托订单
			myStrategy.Portfolio.Persistent=true;//是否保存投资组合
		}
		myStrategy.SimulationCash=100000;
		//myStrategy.ResetPortfolio=false;
		//myStrategy.CheckBuyPower=true;
		myStrategy.SimulationEntryDate=new DateTime(2017,1,1);
		myStrategy.SimulationExitDate=new DateTime(2017,12,31);
		Thread aThread=new Thread(new ThreadStart(myStrategy.Start));
		if (aThread.ThreadState==ThreadState.Unstarted){
			aThread.Start();
		}
		MessageBox.Show("Press OK to stop ATS", "SmartQuant Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);
		myStrategy.Stop();
		aThread.Abort();
		while((aThread.ThreadState!=ThreadState.Aborted)&&(aThread.ThreadState!=ThreadState.Stopped)){
			Thread.Sleep(1);
		}
		Console.WriteLine("Stopped!");
	}
}