using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaQuant.Data.TDX
{
    public class OrderIDMapping
    {
        Dictionary<string, string> innerToOuter;
        Dictionary<string, string> outerToInner;
        public OrderIDMapping()
        {
            this.innerToOuter = new Dictionary<string, string>();
            this.outerToInner = new Dictionary<string, string>();
        }
        public void AddMapping(string innerOrderID,string outerOrderID)
        {
            if (innerToOuter.ContainsKey(innerOrderID) || outerToInner.ContainsKey(outerOrderID)) {
                return;
            }
            this.innerToOuter.Add(innerOrderID, outerOrderID);
            this.outerToInner.Add(outerOrderID, innerOrderID);
        }
        public string GetOuterOrderID(string innerOrderID) {
            string outerOrderID = string.Empty;
            this.innerToOuter.TryGetValue(innerOrderID, out outerOrderID);
            return outerOrderID;
        }
        public string GetInnerOrderID(string outerOrderID) {
            string innerOrderID = string.Empty;
            this.outerToInner.TryGetValue(outerOrderID,out innerOrderID);
            return innerOrderID;
        }
    }
}
