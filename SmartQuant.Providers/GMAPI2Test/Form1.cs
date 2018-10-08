using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMSDK;

namespace GMAPI2Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                this.label1.Text = "login error.";
                return;
            }
            List<DailyBar> dailys = md.GetDailyBars("SZSE.300044", "2018-01-01", "2018-08-06");
            //List<DailyBar> dailys = md.GetLastNDailyBars("SZSE.300044", 10, "2016-03-28");
            this.adjust(dailys);
            ShowData(dailys.OfType<object>().ToList());
            this.label1.Text = dailys.Count.ToString();
        }
        private void adjust(List<DailyBar> dailys)
        {
            int n = dailys.Count;
            for (int i = n - 2; i >= 0; i--)
            {
                if (dailys[i].adj_factor != dailys[i + 1].adj_factor)
                {//向进复权
                    dailys[i].close = dailys[i].close * dailys[i].adj_factor / dailys[i + 1].adj_factor;
                    dailys[i].volume = dailys[i].volume * dailys[i + 1].adj_factor / dailys[i].adj_factor;
                    dailys[i].adj_factor = dailys[i + 1].adj_factor;

                }
            }
        }
        private void ShowData(List<object> data)
        {
            if (data.Count > 0)
            {
                Type tmpClass = data[0].GetType();
                int i = 0;
                object[] cols;
                int mode;
                if (tmpClass.GetProperties().Count() > 0)
                {
                    cols = tmpClass.GetProperties();
                    mode = 0;
                }
                else {
                    cols = tmpClass.GetFields();
                    mode = 1;
                }


                foreach (var p in cols)
                {
                    DataGridViewTextBoxColumn dgc = new DataGridViewTextBoxColumn();
                    dgc.Name = (mode == 0) ? ((System.Reflection.PropertyInfo)p).Name : ((System.Reflection.FieldInfo)p).Name;
                    this.dataGridView1.Columns.Add(dgc);
                    i++;
                }

                //Debug.WriteLine(tmpClass.GetProperties().Count().ToString());
                foreach (object aa in data)
                {
                    object[] values = new object[i];
                    int j = 0;
                    foreach (var p in cols)
                    {
                        values[j] = (mode == 0) ? ((System.Reflection.PropertyInfo)p).GetValue(aa, null).ToString() : ((System.Reflection.FieldInfo)p).GetValue(aa).ToString();
                        j++;
                    }
                    this.dataGridView1.Rows.Add(values);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            //List<Instrument> insts = md.GetInstruments("SHSE", 1, 1);
            //insts.AddRange(md.GetInstruments("SZSE", 1, 1));
            List<Tick> tickLists = new List<Tick>();
            //foreach (Instrument inst in insts)
            //{
            //    List<Tick> tickList = md.GetLastNTicks(inst.symbol, 1, "2016-03-26 15:01:00");
            //    tickLists.AddRange(tickList);
            //}
            List<Tick> tickList = md.GetLastNTicks("SHSE.600830", 1, "2018-08-13 15:00:00");
            //List<Tick> tickList = md.GetTicks("SHSE.600830", "2018-08-13 00:00:00", "2018-08-13 15:05:00");
            //List<Tick> tickList = md.GetTicks("SZSE.300334", "2017-05-12 00:00:00", "2017-05-12 15:00:00");
            tickLists.AddRange(tickList);
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (Tick tick in tickLists)
            {
                tick.strtime = startTimeUTC.AddSeconds(tick.utc_time).ToString();
            }
            ShowData(tickLists.OfType<object>().ToList());
            this.label1.Text = tickLists.Count.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<Bar> bars = md.GetBars("SHSE.600004", 60, "2017-03-09 00:00:00", "2017-03-10 15:00:00");
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (Bar bar in bars)
            {
                bar.strtime = startTimeUTC.AddSeconds(bar.utc_time).ToString();
            }
            ShowData(bars.OfType<object>().ToList());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<Instrument> insts = md.GetInstruments("SHSE", 1, 0);
            ShowData(insts.OfType<object>().ToList());
            this.label1.Text = insts.Count.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<FinancialIndex> finance = md.GetLastNFinancialIndex("SHSE.600006", 3);
            ShowData(finance.OfType<object>().ToList());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<Instrument> insts = md.GetInstruments("SHSE", 1, 1);
            insts.AddRange(md.GetInstruments("SZSE", 1, 1));
            List<ShareIndex> shareindexList = new List<ShareIndex>();
            
            DateTime jt = new DateTime(2016, 3, 28);

            //int i = 0, j = 0;
            //string symbolList = "";
            //foreach (Instrument inst in insts)
            //{
            //    i++;
            //    j++;
            //    symbolList += inst.symbol + ",";
            //    if (i >= 10 || j >= insts.Count)
            //    {
            //         List <GMSDK.ShareIndex> shareIndexs = md.GetShareIndex(symbolList, jt.ToString("yyyy-MM-dd"), jt.ToString("yyyy-MM-dd"));
            //        foreach (GMSDK.ShareIndex shareIndex in shareIndexs)
            //        {
            //            shareindexList.Add(shareIndex);
            //        }
            //        i = 0;
            //        symbolList = "";
            //    }
            //}
            shareindexList = md.GetShareIndex("SZSE.000001", "2017-04-17", "2017-04-17");
            ShowData(shareindexList.OfType<object>().ToList());
            this.label1.Text = shareindexList.Count.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<MarketIndex> marketindex = md.GetLastNMarketIndex("SHSE.600004",3);
            ShowData(marketindex.OfType<object>().ToList());
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<TradeDate> trade_dates = md.GetCalendar("SHSE", "2016-01-01", "2016-03-29");
            DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (TradeDate date in trade_dates)
            {
                date.strtime = startTimeUTC.AddSeconds(date.utc_time).ToString();
            }
            ShowData(trade_dates.OfType<object>().ToList());
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            MdApi md = MdApi.Instance;
            int ret = md.Init("tianzhong_live@126.com", "Chs771005", MDMode.MD_MODE_NULL);
            if (ret != 0)
            {
                //登录失败
                return;
            }
            List<StockAdjustFactor> stockAdjs = md.GetStockAdj("SZSE.300044", "2018-01-01", "2018-08-06");
            ShowData(stockAdjs.OfType<object>().ToList());

        }
    }
}
