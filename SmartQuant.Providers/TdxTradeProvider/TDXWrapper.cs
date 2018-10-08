using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HuaQuant.Data.TDX
{
    /// <summary>
    /// 通达信交易接口定义
    /// </summary>
    public class TDXWrapper
    {
        /// <summary>
        /// 打开通达信实例
        /// </summary>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void OpenTdx();

        /// <summary>
        /// 关闭通达信实例
        /// </summary>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CloseTdx();

        /// <summary>
        /// 交易账户登录
        /// </summary>
        /// <param name="IP">券商交易服务器IP</param>
        /// <param name="Port">券商交易服务器端口</param>
        /// <param name="Version">设置通达信客户端的版本号:6.00或8.00</param>
        /// <param name="YybId">营业部编码：国泰君安为7</param>
        /// <param name="AccountNo">资金账号</param>                      ***经检验，这里对于广发证券是客户号
        /// <param name="TradeAccount">交易帐号与资金帐号相同</param>      *****经检验，这里对于广发证券是资金账号
        /// <param name="JyPassword">交易密码</param>
        /// <param name="TxPassword">通讯密码为空</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串</param>
        /// <returns>客户端ID，失败时返回-1。</returns>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern int Logon(string IP, short Port, string Version, short YybId, string AccountNo, string TradeAccount, string JyPassword, string TxPassword, StringBuilder ErrInfo);

        /// <summary>
        /// 交易账户注销
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void Logoff(int ClientID);

        /// <summary>
        /// 查询各种交易数据
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0资金  1股份   2当日委托  3当日成交     4可撤单   5股东代码  6融资余额   7融券余额  8可融证券</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryData(int ClientID, int Category, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 查询各种历史数据
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0历史委托  1历史成交   2交割单</param>
        /// <param name="StartDate">表示开始日期，格式为yyyyMMdd,比如2014年3月1日为  20140301</param>
        /// <param name="EndDate">表示结束日期，格式为yyyyMMdd,比如2014年3月1日为  20140301</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryHistoryData(int ClientID, int Category, string StartDate, string EndDate, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量查询各种交易数据,用数组传入每个委托的参数，数组第i个元素表示第i个查询的相应参数。
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0资金  1股份   2当日委托  3当日成交     4可撤单   5股东代码  6融资余额   7融券余额  8可融证券</param>
        /// <param name="Count"></param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryDatas(int ClientID, int[] Category, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 下委托交易证券
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示委托的种类，0买入 1卖出  2融资买入  3融券卖出   4买券还券   5卖券还款  6现券还券</param>
        /// <param name="PriceType">表示报价方式 0  上海限价委托 深圳限价委托 1深圳对方最优价格  2深圳本方最优价格  3深圳即时成交剩余撤销  4上海五档即成剩撤 深圳五档即成剩撤 5深圳全额成交或撤销 6上海五档即成转限价</param>
        /// <param name="Gddm">股东代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Price">委托价格</param>
        /// <param name="Quantity">委托数量</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrder(int ClientID, int Category, int PriceType, string Gddm, string Zqdm, float Price, int Quantity, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量下委托交易
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示委托的种类，0买入 1卖出  2融资买入  3融券卖出   4买券还券   5卖券还款  6现券还券</param>
        /// <param name="PriceType">表示报价方式 0  上海限价委托 深圳限价委托 1深圳对方最优价格  2深圳本方最优价格  3深圳即时成交剩余撤销  4上海五档即成剩撤 深圳五档即成剩撤 5深圳全额成交或撤销 6上海五档即成转限价</param>
        /// <param name="Gddm">股东代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Price">委托价格</param>
        /// <param name="Quantity">委托数量</param>
        /// <param name="Count">批量下单数量</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrders(int ClientID, int[] Category, int[] PriceType, string[] Gddm, string[] Zqdm, float[] Price, int[] Quantity, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 撤委托
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="ExchangeID">交易所类别， 上海A1，深圳A0(招商证券普通账户深圳是2)</param>
        /// <param name="hth">委托编号</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrder(int ClientID, string ExchangeID, string hth, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量撤单
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="ExchangeID">交易所类别， 上海A1，深圳A0(招商证券普通账户深圳是2)</param>
        /// <param name="hth"></param>
        /// <param name="Count"></param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrders(int ClientID, string[] ExchangeID, string[] hth, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 获取证券的实时五档行情
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuote(int ClientID, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量获取证券的实时五档行情
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Count">证券合约数量</param>
        /// <param name="Result">同</param>
        /// <param name="ErrInfo">同</param>
        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuotes(int ClientID, string[] Zqdm, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 融资融券直接还款
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Amount">还款金额</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>

        [DllImport("Trade.dll", CharSet = CharSet.Ansi)]
        public static extern void Repay(int ClientID, string Amount, StringBuilder Result, StringBuilder ErrInfo);
    }
}
