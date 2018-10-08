using System;
namespace GMSDK
{
    public class ErrorEventArgs : EventArgs
    {
        public int error_code;
        public string error_msg;
    }
}
