using System;
using System.Collections;

namespace SmartQuant.Trading.Conditions
{
	public class RulesParser
	{
		protected string text;

		public RulesParser(string text)
		{
			this.text = text;
		}

		public ArrayList Parse()
		{
			ArrayList arrayList = new ArrayList();
			this.Parse(this.text, 0, arrayList);
			return arrayList;
		}

		private void Parse(string text, int level, ArrayList ruleList)
		{
		}
	}
}
