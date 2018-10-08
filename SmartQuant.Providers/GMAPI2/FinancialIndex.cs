using System;
using System.Runtime.InteropServices;
namespace GMSDK
{
    /// 财务指标
    [StructLayout(LayoutKind.Sequential)]
    public class FinancialIndex
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string symbol;                        //股票代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string pub_date;                      //公告日期              
        public double eps;                           //每股收益    
        public double bvps;                          //每股净资产  
        public double cfps;                          //每股现金流  
        public double afps;                          //每股公积金  
        public double total_asset;                   //总资产      
        public double current_asset;                 //流动资产    
        public double fixed_asset;                   //固定资产    
        public double liability;                     //负债合计    
        public double current_liability;             //流动负债    
        public double longterm_liability;            //长期负债    
        public double equity;                        //所有者权益  
        public double income;                        //主营业务收入
        public double operating_profit;              //主营业务利润
        public double net_profit;                    //净利润    

    }
}
