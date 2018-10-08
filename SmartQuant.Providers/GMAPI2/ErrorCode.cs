using System;
namespace GMSDK
{
    public class ErrorCode
    {
        public const int SUCCESS = 0;
        public const int ERR_CONFIG_FILE_NOT_EXIST = 1001;
        public const int ERR_CONFIG_PARSE = 1002;
        public const int ERR_AUTH_CONNECT = 1003;
        public const int ERR_AUTH_LOGIN = 1004;
        public const int ERR_REQUEST_TIMEOUT = 1005;
        public const int ERR_INVALID_PARAMETER = 1006;
        public const int ERR_STRATEGY_INIT = 1007;
        public const int ERR_INTERNAL_INIT_ERROR = 1008;
        public const int ERR_API_SERVER_CONNECT = 1009;
        /// 业务层面错误共有部分＿ 1500~1999
        public const int ERR_INVALID_SYMBOL = 1501;//"非法证券代码"
        public const int ERR_INVALID_DATE = 1502;//"非法日期格式"
        public const int ERR_INVALID_STRATEGY_ID = 1503;//"非法策略ID"
        /// 交易部分 2000 ~ 2999
        public const int ERR_TD_CONNECT = 2000;
        public const int ERR_TD_LOGIN = 2001;
        public const int ERR_TD_TIMEOUT = 2002;
        public const int ERR_TD_NO_RESULT = 2003;
        public const int ERR_TD_INVALID_SESSION = 2004;
        public const int ERR_TD_INVALID_PARAMETER = 2005;
        public const int ERR_TD_STRATEGY_LOCKED = 2006;
        public const int ERR_TD_SERVER_ERROR = 2007;
        public const int ERR_TD_CORRUPT_DATA = 2008;
        public const int ERR_TD_CONNECT_CLOSE = 2009;
        /// 数据服务部分 3000~3999
        public const int ERR_MD_CONNECT = 3000; //"数据服务连接失败"
        public const int ERR_MD_LOGIN = 3001; //"数据服务登录失败"
        public const int ERR_MD_TIMEOUT = 3002;//"数据服务请求超时"
        public const int ERR_MD_NO_RESULT = 3003;//"该条件没查到数据"
        public const int ERR_MD_BUFFER_ALLOC = 3005;//"分配缓冲区错诿"
        public const int ERR_MD_INVALID_PARAMETER = 3006;//"数据请求参数非法"
        public const int ERR_MD_SERVER_ERROR = 3007;//"数据服务内部错误"
        public const int ERR_MD_CORRUPT_DATA = 3008;//"返回数据包错诿"
        public const int ERR_MD_CONNECT_CLOSE = 3009;//"数据服务连接断开"
        /// 回测部分 4000~4999
        public const int ERR_BT_INVALID_TIMESPAN = 4000;
        public const int ERR_BT_INVALID_INITIAL_CASH = 4001;
        public const int ERR_BT_INVALID_TRANSACTION_RATIO = 4002;
        public const int ERR_BT_INVALID_COMMISSION_RATIO = 4003;
        public const int ERR_BT_INVALID_SLIPPAGE_RATIO = 4004;
        public const int ERR_BT_READ_CACHE_ERROR = 4005;
        public const int ERR_BT_WRITE_CACHE_ERROR = 4006;
        public const int ERR_BT_CONNECT = 4007;
        public const int ERR_BT_RESULT = 4008;
    }
}