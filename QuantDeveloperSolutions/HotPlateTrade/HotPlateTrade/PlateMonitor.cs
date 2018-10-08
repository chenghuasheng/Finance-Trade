using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SmartQuant;
using SmartQuant.Data;
using HuaQuant.Data.GM;
using MongoDB.Bson;
using MongoDB.Driver;
public abstract class PlateMonitor{
	protected string eastMoneyPath = "";
	protected PlateMonitorForm monitorForm=null;
	protected PatternRecognition patternRecognition=null;
	public PlateMonitor(string eastMoneyPath,PlateMonitorForm monitorForm){
		this.eastMoneyPath=eastMoneyPath;	
		this.monitorForm=monitorForm;
		this.patternRecognition=new PatternRecognition(null);
	}
	public PlateMonitor(string eastMoneyPath):this(eastMoneyPath,null){}
	
	protected List<Plate> plates=new List<Plate>();
	public List<Plate> Plates {
		get { return this.plates;}
	}
	protected List<Plate> areaPlates=new List<Plate>();
	public List<Plate> AreaPlates {
		get { return this.areaPlates;}
	}
	protected List<Plate> industryPlates=new List<Plate>();
	public List<Plate> IndustryPlates {
		get { return this.industryPlates;}
	}
	protected List<Plate> conceptPlates=new List<Plate>();
	public List<Plate> ConceptPlates {
		get { return this.conceptPlates;}
	}
	protected Dictionary<string,Stock> stockDict=new Dictionary<string,Stock>();

	protected List<string> activeSymbols=new List<string>();
	
	
	protected int upLimitCountOfAll = 0;
	public int UpLimitCountOfAll{
		get { return this.upLimitCountOfAll;}
	}
	protected int fivePercentCountOfAll = 0;
	public int FivePercentCountOfAll{
		get { return this.fivePercentCountOfAll;}
	}
	protected int upCountOfAll = 0;
	public int UpCountOfAll{
		get { return this.upCountOfAll;}
	}
	protected int activeCountOfAll=0;
	public int ActiveCountOfAll{
		get {return this.activeCountOfAll;}
	}
	protected float weightOfAll=0.0F;
	public float WeightOfAll{
		get { return this.weightOfAll;}
	}
	protected Dictionary<int,List<float>> plateWeightHistory=new Dictionary<int,List<float>>();//板块权重历史
	protected List<float> weightHistoryOfAll=new List<float>();
	
	//读取版块和证券信息
	protected void ReadPlatesAndStocks()
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
	protected virtual void GetActiveSymbols(){}	
	//更新板块统计
	protected void UpdatePlateStatistic(Dictionary<string,Trade> tradeDict){
		//将所有板块的统计归零
		foreach (Plate curPlate in this.plates)
		{
			curPlate.UpLimitCount = 0;
			curPlate.FivePercentCount = 0;
			curPlate.UpCount = 0;
			curPlate.ActiveCount=0;
		}
		this.upLimitCountOfAll = 0;
		this.fivePercentCountOfAll = 0;
		this.upCountOfAll= 0;
		this.activeCountOfAll=0;
		//利用最新行情更新统计
		foreach (KeyValuePair<string,Trade> kvp in tradeDict)
		{
			string symbol = kvp.Key;
			Stock curStock;
			if (this.stockDict.TryGetValue(symbol, out curStock))
			{
				GMTrade gmTrade=(GMTrade)kvp.Value;
				curStock.Price = gmTrade.Price;
				curStock.IncPercent = (gmTrade.Price / gmTrade.LastClose - 1) * 100;
				bool flag1 = curStock.UpLimited = (gmTrade.Price == gmTrade.UpperLimit);//涨停
				bool flag2 = (curStock.IncPercent >= 5);//涨幅达到百分之5
				bool flag3 = (curStock.IncPercent > 0);
				//全部统计
				if (flag1) this.upLimitCountOfAll++;
				if (flag2) this.fivePercentCountOfAll++;
				if (flag3) this.upCountOfAll++;
				this.activeCountOfAll++;
				//各版块统计
				foreach (Plate curPlate in curStock.Plates)
				{
					if (flag1) curPlate.UpLimitCount++;
					if (flag2) curPlate.FivePercentCount++;
					if (flag3) curPlate.UpCount++;
					curPlate.ActiveCount++;
				}	
			}
		}
		foreach (Plate curPlate in this.plates)
		{
			if (curPlate.ActiveCount>0){
				curPlate.Weight = (float)(curPlate.UpLimitCount * 0.5 + curPlate.FivePercentCount * 0.30 + curPlate.UpCount * 0.20) * 100 / curPlate.ActiveCount;
			}
			List<float> weightHistory;
			int key=curPlate.ID;
			if (!this.plateWeightHistory.TryGetValue(key,out weightHistory)){
				weightHistory=new List<float>();
				this.plateWeightHistory.Add(key,weightHistory);
			}
			weightHistory.Add(curPlate.Weight);
		}
		if (this.activeCountOfAll>0){
			this.weightOfAll=(float)(this.upLimitCountOfAll*0.5+this.fivePercentCountOfAll*0.30+this.upCountOfAll*0.20)*100/this.activeCountOfAll;
			this.weightHistoryOfAll.Add(this.weightOfAll);
		}
		if (this.monitorForm!=null) {
			this.monitorForm.ShowPlateStatistic(this);
		}
	}
	
	
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
	
	public virtual void Begin(){
		this.weightHistoryOfAll.Clear();
		this.plateWeightHistory.Clear();
		this.ReadPlatesAndStocks();
		this.GetActiveSymbols();
	}
	public virtual void End(){
		this.Save();
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
			if (plate.Name=="昨日触板") continue;
			//去掉"涨5个点以上"一个都没有的板块
			if (plate.FivePercentCount<=0) continue;

			if (plate.Weight<minWeight) break;
			ret.Add(plate);
			i++;
			if (i>=n) break;
		}
		return ret;
	}
	
	
	public RecognitionState GetDailyLineState(Daily[] gmDailys){
		RecognitionState ret=new RecognitionState();
		int n=gmDailys.Length;
		if (n>0){
			double[] inputs=new double[n];
			double[] outputs=new double[n];
			double[] avgPrices=new double[n];
			for(int i=0;i<n;i++){
				avgPrices[i]=((GMDaily)gmDailys[i]).Amount/gmDailys[i].Volume;
			}
			double basePrice=avgPrices[0];
			for(int i=0;i<n;i++){
				inputs[i]=i;
				outputs[i]=(avgPrices[i]/basePrice-1)*100;
			}
			ret=this.patternRecognition.Recognition(inputs,outputs);
		}
		return ret;
	}
	public RecognitionState GetMinLineState(Bar[] gmMinBars){
		RecognitionState ret=new RecognitionState();
		int n=gmMinBars.Length;
		if (n>0){
			double[] inputs=new double[n];
			double[] outputs=new double[n];
			double[] avgPrices=new double[n];
			for(int i=0;i<n;i++){
				avgPrices[i]=((GMBar)gmMinBars[i]).Amount/gmMinBars[i].Volume;
			}
			double basePrice=avgPrices[0];
			for(int i=0;i<n;i++){
				inputs[i]=i;
				outputs[i]=(avgPrices[i]/basePrice-1)*100;
			}
			ret=this.patternRecognition.Recognition(inputs,outputs);	
		}
		return ret;
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