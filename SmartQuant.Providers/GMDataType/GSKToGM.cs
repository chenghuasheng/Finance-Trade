using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartQuant.Data;

namespace HuaQuant.Data.GM
{
    public class GSKToGM
    {
        internal static DateTime startTimeUTC = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static List<ISeriesObject> ConvertBars(List<GMSDK.Bar> gskBars)
        {
            List<ISeriesObject> gmBars = new List<ISeriesObject>();
            foreach (GMSDK.Bar gskBar in gskBars)
            {
                gmBars.Add(new GMBar(startTimeUTC.AddSeconds(gskBar.utc_time), gskBar.open, gskBar.high, gskBar.low, gskBar.close, (long)gskBar.volume, gskBar.bar_type, gskBar.pre_close, gskBar.amount, gskBar.adj_factor, gskBar.flag));
            }
            return gmBars;
        } 
        public static List<ISeriesObject> ConvertDailys(List<GMSDK.DailyBar> gskDailys)
        {
            List<ISeriesObject> gmDailys = new List<ISeriesObject>();
            foreach(GMSDK.DailyBar gskDaily in gskDailys)
            {
                gmDailys.Add(new GMDaily(startTimeUTC.AddSeconds(gskDaily.utc_time), gskDaily.open, gskDaily.high, gskDaily.low, gskDaily.close, (long)Math.Round(gskDaily.volume / 100) * 100, gskDaily.pre_close, gskDaily.amount, gskDaily.adj_factor, gskDaily.flag, gskDaily.upper_limit, gskDaily.lower_limit));
            }
            return gmDailys;
        }
        public static List<ISeriesObject> ConvertTrades(List<GMSDK.Tick> gskTicks)
        {
            List<ISeriesObject> gmTrades = new List<ISeriesObject>();
            float lastPrice = 0;
            foreach (GMSDK.Tick gskTick in gskTicks)
            {
                if ((gskTick.last_volume > 0) || (gskTick.last_price != lastPrice))
                {
                    gmTrades.Add(ConvertTrade(gskTick));
                    lastPrice = gskTick.last_price;
                }
            }
            return gmTrades;
        }

        public static List<ISeriesObject> ConvertQuotes(List<GMSDK.Tick> gskTicks)
        {
            List<ISeriesObject> gmQuotes = new List<ISeriesObject>();
            foreach (GMSDK.Tick gskTick in gskTicks)
            {
                gmQuotes.Add(ConvertQuote(gskTick));
            }
            return gmQuotes;
        }
        public static GMTrade ConvertTrade(GMSDK.Tick gskTick)
        {
            char buyOrSell = 'N';
            switch (gskTick.trade_type)
            {
                case 0:
                    break;
                case 7:
                    buyOrSell = 'B';
                    break;
                case 8:
                    buyOrSell = 'S';
                    break;
            }
            return new GMTrade(startTimeUTC.AddSeconds(gskTick.utc_time), gskTick.last_price, gskTick.last_volume, buyOrSell,
                        gskTick.last_amount, gskTick.cum_volume, gskTick.cum_amount, gskTick.high, gskTick.low, gskTick.open, gskTick.pre_close,
                        gskTick.upper_limit, gskTick.lower_limit);
        }
        public static GMQuote ConvertQuote(GMSDK.Tick gskTick)
        {
            return new GMQuote(startTimeUTC.AddSeconds(gskTick.utc_time), gskTick.bid_p1, gskTick.bid_v1, gskTick.ask_p1, gskTick.ask_v1, gskTick.bid_p2,
                    gskTick.bid_p3, gskTick.bid_p4, gskTick.bid_p5, gskTick.bid_v2, gskTick.bid_v3, gskTick.bid_v4, gskTick.bid_v5, gskTick.ask_p2, gskTick.ask_p3,
                    gskTick.ask_p4, gskTick.ask_p5, gskTick.ask_v2, gskTick.ask_v3, gskTick.ask_v4, gskTick.ask_v5);
        }

        public static GMLastTick ConvertTick(GMSDK.Tick gskTick)
        {
            char buyOrSell = 'N';
            switch (gskTick.trade_type)
            {
                case 0:
                    break;
                case 7:
                    buyOrSell = 'B';
                    break;
                case 8:
                    buyOrSell = 'S';
                    break;
            }
            return new GMLastTick(startTimeUTC.AddSeconds(gskTick.utc_time), gskTick.last_price, gskTick.last_volume, buyOrSell,
                        gskTick.last_amount, gskTick.cum_volume, gskTick.cum_amount, gskTick.high, gskTick.low, gskTick.open, gskTick.pre_close,
                        gskTick.upper_limit, gskTick.lower_limit,gskTick.bid_p1, gskTick.bid_v1, gskTick.ask_p1, gskTick.ask_v1, gskTick.bid_p2,
                    gskTick.bid_p3, gskTick.bid_p4, gskTick.bid_p5, gskTick.bid_v2, gskTick.bid_v3, gskTick.bid_v4, gskTick.bid_v5, gskTick.ask_p2, gskTick.ask_p3,
                    gskTick.ask_p4, gskTick.ask_p5, gskTick.ask_v2, gskTick.ask_v3, gskTick.ask_v4, gskTick.ask_v5);
        }

        public static GMBar ConvertBar(GMSDK.Bar gskBar)
        {
            return new GMBar(startTimeUTC.AddSeconds(gskBar.utc_time), gskBar.open, gskBar.high, gskBar.low, gskBar.close, (long)gskBar.volume, gskBar.bar_type, gskBar.pre_close, gskBar.amount, gskBar.adj_factor, gskBar.flag);
        }
        public static GMDaily ConvertDaily(GMSDK.DailyBar gskDaily)
        {
            return new GMDaily(startTimeUTC.AddSeconds(gskDaily.utc_time), gskDaily.open, gskDaily.high, gskDaily.low, gskDaily.close, (long)Math.Round(gskDaily.volume / 100) * 100, gskDaily.pre_close, gskDaily.amount, gskDaily.adj_factor, gskDaily.flag, gskDaily.upper_limit, gskDaily.lower_limit);
        }
    }
}
