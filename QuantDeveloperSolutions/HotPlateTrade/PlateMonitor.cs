using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Timers;

using SmartQuant;
using GMSDK;
using MongoDB.Bson;
using MongoDB.Driver;
public class PlateMonitor{
	private MdApi _md =null;
	private string eastMoneyPath = "";
	private PlateMonitorForm monitorForm=null;
	private bool isRunning=false;
	public PlateMonitor(MdApi mdApi,string eastMoneyPath,PlateMonitorForm monitorForm){
		this._md=mdApi;
		this.eastMoneyPath=eastMoneyPath;	
		this.monitorForm=monitorForm;
	}
	public PlateMonitor(MdApi mdApi,string eastMoneyPath):this(mdApi,eastMoneyPath,null){}
	
	private List<Plate> plates=new List<Plate>();
	public List<Plate> Plates {
		get { return this.plates;}
	}
	private List<Plate> areaPlates=new List<Plate>();
	public List<Plate> AreaPlates {
		get { return this.areaPlates;}
	}
	private List<Plate> industryPlates=new List<Plate>();
	public List<Plate> IndustryPlates {
		get { return this.industryPlates;}
	}
	private List<Plate> conceptPlates=new List<Plate>();
	public List<Plate> ConceptPlates {
		get { return this.conceptPlates;}
	}
	private Dictionary<string,Stock> stockDict=new Dictionary<string,Stock>();

	private List<string> activeSymbols=new List<string>();
	private int numOfUpdateBatch=100;
	
	private int upLimitCountOfAll = 0;
	public int UpLimitCountOfAll{
		get { return this.upLimitCountOfAll;}
	}
	private int fivePercentCountOfAll = 0;
	public int FivePercentCountOfAll{
		get { return this.fivePercentCountOfAll;}
	}
	private int upCountOfAll = 0;
	public int UpCountOfAll{
		get { return this.upCountOfAll;}
	}
	public int StockCountOfAll{
		get {return this.stockDict.Count;}
	}
	private float weightOfAll=0.0F;
	public float WeightOfAll{
		get { return this.weightOfAll;}
	}
	private Dictionary<int,List<float>> plateWeightHistory=new Dictionary<int,List<float>>();//板块权重历史
	private List<float> weightHistoryOfAll=new List<float>();
	
	//读取版块和证券信息
	private void ReadPlatesAndStocks()
	{
		this.plates.Clear();
		this.industryPlates.Clear();
		this.areaPlates.Clear();
		this.conceptPlates.Clear();
		this.stockDict.Clear();
		
		FileStream fs = null;
		try
		{
			if (this.eastMoneyPath=="") return ;
			string filePath = this.eastMoneyPath + @"\bklist_new_xx.dat";
			fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			StreamReader reader = new StreamReader(fs, Encoding.Default);
			string text;
			string[] markets = new string[2] { "SZSE", "SHSE" };
			while (!reader.EndOfStream)
			{
				text = reader.ReadLine();
				int pos = text.LastIndexOf(";");
				if (pos < 0) continue;
				string symbolListString = text.Substring(pos + 1);
				if (symbolListString.Length <= 0) continue;
				string plateInfoString = text.Substring(0, pos);
				string[] plateInfoArray = plateInfoString.Split(new char[] { ';' });
				Plate curPlate = new Plate();
				curPlate.ID = int.Parse(plateInfoArray[0]);
				curPlate.Type = int.Parse(plateInfoArray[1]);
				curPlate.Name = plateInfoArray[5];
				string[] symbolArray = symbolListString.Split(new char[] { ':' });
				foreach (string symbol in symbolArray)
				{
					if (symbol.Length < 3) continue;
					int marketNo = int.Parse(symbol.Substring(0, 1));
					string newSymbol = markets[marketNo] + "." + symbol.Substring(2);
					Stock curStock;
					if (!this.stockDict.TryGetValue(newSymbol, out curStock))
					{
						curStock = new Stock();
						curStock.Symbol = newSymbol;
						this.stockDict.Add(newSymbol, curStock);
					}
					curStock.Plates.Add(curPlate);
					curPlate.Stocks.Add(curStock);
				}
				this.plates.Add(curPlate);
				switch (curPlate.Type)
				{
					case 1:
						this.areaPlates.Add(curPlate);
						break;
					case 2:
						this.industryPlates.Add(curPlate);
						break;
					case 3:
						this.conceptPlates.Add(curPlate);
						break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		finally
		{
			if (fs != null)
			{
				fs.Close();
			}
		}
	}
	//获取当天活动证券
	private void GetActiveSymbols()
	{
		this.activeSymbols.Clear();
		List<Instrument> gskInsts = this._md.GetInstruments("SHSE", 1, 1);
		gskInsts.AddRange(this._md.GetInstruments("SZSE", 1, 1));
		foreach (Instrument gskInst in gskInsts)
		{
			this.activeSymbols.Add(gskInst.symbol);
		}
	}	
	//更新板块统计
	private void UpdatePlateStatistic(){
		//获取当天活动证券的最新行情
		List<Tick> gskTicks = new List<Tick>();
		try
		{
			int i = 0;
			int j = 0;
			string symbolList = "";
			int num = this.activeSymbols.Count;
			foreach (string symbol in this.activeSymbols)
			{
				j++;
				i++;
				symbolList += symbol + ",";
				if (i >= this.numOfUpdateBatch || j >= num)
				{
					lock(this._md){
						gskTicks.AddRange(this._md.GetLastTicks(symbolList));
					}
					i = 0;
					symbolList = "";
				}
			}
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception.Message);
		}
		//将所有板块的统计归零
		foreach (Plate curPlate in this.plates)
		{
			curPlate.UpLimitCount = 0;
			curPlate.FivePercentCount = 0;
			curPlate.UpCount = 0;
		}
		this.upLimitCountOfAll = 0;
		this.fivePercentCountOfAll = 0;
		this.upCountOfAll= 0;
		//利用最新行情更新统计
		foreach (Tick gskTick in gskTicks)
		{
			string symbol = gskTick.exchange + "." + gskTick.sec_id;
			Stock curStock;
			if (this.stockDict.TryGetValue(symbol, out curStock))
			{
				curStock.Price = gskTick.last_price;
				curStock.IncPercent = (gskTick.last_price / gskTick.pre_close - 1) * 100;
				bool flag1 = curStock.UpLimited = (gskTick.last_price == gskTick.upper_limit);//涨停
				bool flag2 = (curStock.IncPercent >= 5);//涨幅达到百分之5
				bool flag3 = (curStock.IncPercent > 0);
				//全部统计
				if (flag1) this.upLimitCountOfAll++;
				if (flag2) this.fivePercentCountOfAll++;
				if (flag3) this.upCountOfAll++;
				//各版块统计
				if (flag1 || flag2 || flag3)
				{
					foreach (Plate curPlate in curStock.Plates)
					{
						if (flag1) curPlate.UpLimitCount++;
						if (flag2) curPlate.FivePercentCount++;
						if (flag3) curPlate.UpCount++;
					}
				}
			}
		}
		foreach (Plate curPlate in this.plates)
		{
			if (curPlate.Stocks.Count>0){
				curPlate.Weight = (float)(curPlate.UpLimitCount * 0.5 + curPlate.FivePercentCount * 0.30 + curPlate.UpCount * 0.20) * 100 / curPlate.Stocks.Count;
			}
			List<float> weightHistory;
			int key=curPlate.ID;
			if (!this.plateWeightHistory.TryGetValue(key,out weightHistory)){
				weightHistory=new List<float>();
				this.plateWeightHistory.Add(key,weightHistory);
			}
			weightHistory.Add(curPlate.Weight);
		}
		if (this.stockDict.Count>0){
			this.weightOfAll=(float)(this.upLimitCountOfAll*0.5+this.fivePercentCountOfAll*0.30+this.upCountOfAll*0.20)*100/this.stockDict.Count;
			this.weightHistoryOfAll.Add(this.weightOfAll);
		}
		if (this.monitorForm!=null) {
			this.monitorForm.ShowPlateStatistic(this);
		}
		Console.WriteLine("结束一次更新.");	
	}
	
	private DateTime beginTime=Clock.Now.Date.Add(new TimeSpan(9,15,0));
	private DateTime endTime=Clock.Now.Date.Add(new TimeSpan(15,1,0));
	private int updateInterval=50;//更新时间间隔，以秒为单位
	private int timerInterval=1000;//计时器的时间间隔，以毫秒为单位
	private Thread updateThread=null;
	private System.Timers.Timer timer=new System.Timers.Timer();
	//启动板块统计更新
	public void Begin(){
		Console.WriteLine("开始板块监控...");
		this.isRunning=true;
		this.weightHistoryOfAll.Clear();
		this.plateWeightHistory.Clear();
		this.ReadPlatesAndStocks();
		this.GetActiveSymbols();
		this.update();
		this.timer.Interval=this.timerInterval;
		this.timer.Elapsed+=this.timer_elapsed;
		this.timer.Start();
	}
	//停止板块统计更新
	public void End(){
		if (this.isRunning) {
			Console.WriteLine("结束板块监控...");
			this.timer.Stop();
			this.timer.Elapsed-=this.timer_elapsed;
			this.timeSpan=0;
			this.stopUpdate();
			this.Save();
			this.isRunning=false;
		}
	}
/*	public void Pause(){
		if (this.updateThread!=null&&this.updateThread.ThreadState==ThreadState.Running){
			this.updateThread.Suspend();
		}
		this.timer.Stop();
	}

	public void Resume(){
		if (this.updateThread!=null&&this.updateThread.ThreadState==ThreadState.Suspended){
			this.updateThread.Resume();
		}
		this.timer.Start();
	}
*/
	public List<float> GetPlateWeightHistory(int plateID){
		if (this.plateWeightHistory.ContainsKey(plateID)){
			return this.plateWeightHistory[plateID];
		}else {
			return new List<float>();
		}
	}
	public List<float> GetWeightHistoryOfAll(){
		return this.weightHistoryOfAll;
	}
	private double timeSpan=0;
	//计时器事件
	private void timer_elapsed(object sender,ElapsedEventArgs e){
		if (Clock.Now<this.beginTime||Clock.Now>this.endTime) return;
		this.timeSpan+=this.timer.Interval;
		if (this.timeSpan>=(this.updateInterval*1000)){
			this.update();
			this.timeSpan=0;
		}
	}
	private void update(){
		if (this.updateThread==null||
			this.updateThread.ThreadState==ThreadState.Aborted||
			this.updateThread.ThreadState==ThreadState.Stopped)
		{
			this.updateThread=new Thread(new ThreadStart(this.doUpdate));
			this.updateThread.Start();
		}
	}
	private void doUpdate(){
		this.UpdatePlateStatistic();
	}
	private void stopUpdate(){
		if ((this.updateThread!=null)&&(this.updateThread.ThreadState!=ThreadState.Aborted)&&(
		this.updateThread.ThreadState!=ThreadState.Stopped)){
			this.updateThread.Abort();
			/*--new--*/
			this.updateThread.Join();
			/*--new--*/
			this.updateThread=null;
		}
	}
	
	public List<Plate> GetTopNHotPlates(int n,float minWeight){
		List<Plate> ret=new List<Plate>();
		//倒序排序
		this.plates.Sort(delegate(Plate p1,Plate p2){
			return p2.Weight.CompareTo(p1.Weight);
		});
		//取满足条件的前N个
		int i=0;
		foreach(Plate plate in this.plates){
			//去掉两种特殊板块
			if (plate.Name=="昨日连板") continue;
			if (plate.Name=="昨日涨停") continue;
			//去掉"涨5个点以上"一个都没有的板块
			if (plate.FivePercentCount<=0) continue;
			
			if (plate.Weight<minWeight) break;
			/*List<float> weightHistory=this.GetPlateWeightHistory(plate.ID);
			int m=weightHistory.Count;
			if (m>0){
				double[] inputs=new double[m];
				double[] outputs=new double[m];
				for(int j=0;j<m;j++){
					inputs[j]=j;
					outputs[j]=weightHistory[j];
				}
				PatternRecognition patternRecognition=new PatternRecognition(null);
				RecognitionState weightLineState=patternRecognition.Recognition(inputs,outputs);
				if ((weightLineState.Shape==ShapeState.Rise)
				||(weightLineState.Shape==ShapeState.FallAfterRise&&weightLineState.Speed==SpeedState.Rapid)){
					ret.Add(plate);
					i++;
				}
			}*/
			ret.Add(plate);
			i++;
			if (i>=n) break;
		}
		return ret;
	}
	
	public List<DailyBar> GetDailyLine(string symbol,string lastDateString,int n){
		List<DailyBar> gskDailys;
		lock(this._md){
			gskDailys=this._md.GetLastNDailyBars(symbol,n,lastDateString);
		}
		//进行向前复权
		int num = gskDailys.Count;
		if (num > 1)
		{
			DailyBar lastDaily = gskDailys[0];
			for (int i = 1; i < num; i++)
			{
				DailyBar curDaily = gskDailys[i];
				if (curDaily.adj_factor != lastDaily.adj_factor)
				{
					curDaily.close = curDaily.close * curDaily.adj_factor / lastDaily.adj_factor;
					curDaily.high = curDaily.high * curDaily.adj_factor / lastDaily.adj_factor;
					curDaily.low = curDaily.low * curDaily.adj_factor / lastDaily.adj_factor;
					curDaily.open = curDaily.open * curDaily.adj_factor / lastDaily.adj_factor;
					curDaily.pre_close = curDaily.pre_close * curDaily.adj_factor / lastDaily.adj_factor;
					curDaily.volume = (long)(curDaily.volume * lastDaily.adj_factor / curDaily.adj_factor);
					curDaily.adj_factor = lastDaily.adj_factor;
				}
				lastDaily = curDaily;
			}
			gskDailys.Reverse();//倒序一下，因为从掘金得到的数据是最新的索引为0
		}
		return gskDailys;
	}
	public List<Bar> GetMinLine(string symbol,string beginTimeString,string endTimeString){
		List<Bar> gskMinBars;
		lock(this._md){
			gskMinBars=this._md.GetBars(symbol,60,beginTimeString,endTimeString);
		}
		return gskMinBars;
	}
	//移除监控窗体
	public void RemoveMonitorForm(){
		this.monitorForm=null;
	}
	public void Save(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("FinanceLast");
		
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("PlateHeats");
		string curDateString=Clock.Now.Date.ToString("yyyy-MM-dd");
		QueryDocument query = new QueryDocument(new BsonElement("Date",curDateString));
		collection.Remove(query);
		foreach(Plate curPlate in this.plates){
			BsonElement[] eleArray = new BsonElement[7];
			eleArray[0]=new BsonElement("Date",curDateString);
			eleArray[1]=new BsonElement("ID",curPlate.ID);
			eleArray[2]=new BsonElement("Name",curPlate.Name);
			eleArray[3]=new BsonElement("UpLimitCount",curPlate.UpLimitCount);
			eleArray[4]=new BsonElement("FivePercentCount",curPlate.FivePercentCount);
			eleArray[5]=new BsonElement("UpCount",curPlate.UpCount);
			eleArray[6]=new BsonElement("Weight",curPlate.Weight);
			BsonDocument insert=new BsonDocument(eleArray);
			collection.Insert(insert);
		}
		BsonElement[] eleArray1 = new BsonElement[7];
		eleArray1[0]=new BsonElement("Date",curDateString);
		eleArray1[1]=new BsonElement("ID",-1);
		eleArray1[2]=new BsonElement("Name","全部");
		eleArray1[3]=new BsonElement("UpLimitCount",this.upLimitCountOfAll);
		eleArray1[4]=new BsonElement("FivePercentCount",this.fivePercentCountOfAll);
		eleArray1[5]=new BsonElement("UpCount",this.upCountOfAll);
		eleArray1[6]=new BsonElement("Weight",this.weightOfAll);
		BsonDocument insert1=new BsonDocument(eleArray1);
		collection.Insert(insert1);
	}
	
	
	
	
	
}