using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    public class ReportArgs
    {
        private bool succeeded = false;
        public bool Succeeded
        {
            get { return this.succeeded; }
            set { this.succeeded = value; }
        }
        private string errorInfo = "";
        public string ErrorInfo
        {
            get { return this.errorInfo; }
            set { this.errorInfo = value; }
        }
        private object result = null;
        public object Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
        public ReportArgs(){}
    }
}
