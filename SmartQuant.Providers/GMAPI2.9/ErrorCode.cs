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

        public const int ERR_INVALID_SYMBOL = 1501;

        public const int ERR_INVALID_DATE = 1502;

        public const int ERR_INVALID_STRATEGY_ID = 1503;

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

        public const int ERR_MD_CONNECT = 3000;

        public const int ERR_MD_LOGIN = 3001;

        public const int ERR_MD_TIMEOUT = 3002;

        public const int ERR_MD_NO_RESULT = 3003;

        public const int ERR_MD_BUFFER_ALLOC = 3005;

        public const int ERR_MD_INVALID_PARAMETER = 3006;

        public const int ERR_MD_SERVER_ERROR = 3007;

        public const int ERR_MD_CORRUPT_DATA = 3008;

        public const int ERR_MD_CONNECT_CLOSE = 3009;

        public const int ERR_BT_INVALID_TIMESPAN = 4000;

        public const int ERR_BT_INVALID_INITIAL_CASH = 4001;

        public const int ERR_BT_INVALID_TRANSACTION_RATIO = 4002;

        public const int ERR_BT_INVALID_COMMISSION_RATIO = 4003;

        public const int ERR_BT_INVALID_SLIPPAGE_RATIO = 4004;

        public const int ERR_BT_READ_CACHE_ERROR = 4005;

        public const int ERR_BT_WRITE_CACHE_ERROR = 4006;

        public const int ERR_BT_CONNECT = 4007;

        public const int ERR_BT_RESULT = 4008;

        public ErrorCode()
        {
        }
    }
}