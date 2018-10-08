using System;
using System.Collections.Generic;
using System.IO;

using SmartQuant.Data;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using SmartQuant.Series;
using SmartQuant.File;
using SmartQuant.File.Indexing;
using HuaQuant.Data.GM;
namespace Strategy.Assistants
{
    public class CheckTicksJob : Job
    {
        protected string dataPath = "e:/QDData";
        public string DataPath
        {
            get { return this.dataPath; }
            set { this.dataPath = value; }
        }
        protected DateTime curDate;
        public CheckTicksJob(string name, DateTime date, Job[] needJobs) : base(name, needJobs)
        {
            this.curDate = date;
        }
        public CheckTicksJob(string name, DateTime date) : base(name)
        {
            this.curDate = date;
        }
        protected override bool doJob()
        {
            Console.WriteLine("正在检查Tick数据的完整性...");
            try
            {
                GMRealTimeProvider provider = (GMRealTimeProvider)ProviderManager.MarketDataProviders["GMRealTimeProvider"];
                provider.Connect(10000);
                if (!provider.IsConnected) return false;
                bool overwrite = false;
                string path = this.dataPath + "/" + this.curDate.Year.ToString() + "/" + this.curDate.Month.ToString();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                DateTime beginTime = this.curDate.Add(new TimeSpan(9, 0, 0));
                DateTime endTime = this.curDate.Add(new TimeSpan(24, 0, 0));
                string beginString = beginTime.ToString("yyyy-MM-dd HH:mm:ss");
                string endString = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine("时间段:" + beginString + "----" + endString);

                DataFile file = DataFile.Open(path);
                try
                {
                    //获取股票代码列表
                    foreach (Instrument inst in InstrumentManager.Instruments)
                    {
                        //获取当天的日线数据
                        Daily dailyBar = null;
                        DailySeries dailySeries = inst.GetDailySeries(this.curDate, this.curDate);
                        if (dailySeries.Count > 0)
                        {
                            dailyBar = dailySeries[0];
                        }
                        else continue;

                        string name = inst.Symbol + ".Trade";
                        if (!file.Series.Contains(name))
                        {
                            file.Series.Add(name);
                        }
                        FileSeries series = file.Series[name];
                        ISeriesObject[] has = series.GetArray(beginTime, endTime);
                        bool needRebuild = false;
                        //检查是否丢失
                        if (has.Length <= 0)
                        {
                            Console.WriteLine("证券:{0}，在日期:{1}时丢失Tick数据", inst.Symbol, this.curDate);
                            needRebuild = true;
                        }
                        else
                        {
                            //检查是否不完整
                            GMTrade lastTrade = (GMTrade)has[has.Length - 1];
                            if (lastTrade.TotalSize < dailyBar.Volume)
                            {
                                Console.WriteLine("证券:{0}，在日期:{1}时Tick数据不全", inst.Symbol, this.curDate);
                                needRebuild = true;
                            }
                        }
                        if (needRebuild)
                        {
                            List<GMSDK.Tick> gskTicksCache = provider.MdApi.GetTicks(inst.Symbol, beginString, endString);
                            Console.WriteLine(inst.Symbol + "有" + gskTicksCache.Count.ToString() + "笔数据。");
                            if (gskTicksCache.Count > 0)
                            {
                                //添加trades
                                Console.WriteLine("存储Trade数据...");
                                List<ISeriesObject> trades = GSKToGM.ConvertTrades(gskTicksCache);
                                string name1 = inst.Symbol + ".Trade";
                                if (!file.Series.Contains(name1))
                                {
                                    file.Series.Add(name1);
                                }
                                FileSeries series1 = file.Series[name1];
                                ISeriesObject[] hasTrades = series1.GetArray(beginTime, endTime);
                                if (overwrite || hasTrades.Length != trades.Count)
                                {
                                    foreach (ISeriesObject aTrade in hasTrades) series1.Remove(aTrade.DateTime);
                                    foreach (ISeriesObject trade in trades)
                                    {
                                        series1.Add(trade);
                                    }
                                }
                                series1.Reindex(Indexer.Daily);
                                //添加quotes
                                if (inst.SecurityType == "IDX") continue;//指数没有报价数据
                                Console.WriteLine("存储Quote数据...");
                                List<ISeriesObject> quotes = GSKToGM.ConvertQuotes(gskTicksCache);
                                string name2 = inst.Symbol + ".Quote";
                                if (!file.Series.Contains(name2))
                                {
                                    file.Series.Add(name2);
                                }
                                FileSeries series2 = file.Series[name2];
                                ISeriesObject[] hasQuotes = series2.GetArray(beginTime, endTime);
                                if (overwrite || hasQuotes.Length != quotes.Count)
                                {
                                    foreach (ISeriesObject aQuote in hasQuotes) series2.Remove(aQuote.DateTime);
                                    foreach (ISeriesObject quote in quotes)
                                    {
                                        series2.Add(quote);
                                    }
                                }
                                series2.Reindex(Indexer.Daily);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
