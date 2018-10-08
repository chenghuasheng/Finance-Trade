using System;
using System.Collections;

namespace SmartQuant.Trading.Conditions
{
	public class EntryRule
	{
		private Entry entry;

		private ArrayList list;

		private SignalItem longSignal;

		private SignalItem shortSignal;

		public SignalItem LongEntry
		{
			get
			{
				return this.longSignal;
			}
		}

		public SignalItem ShortEntry
		{
			get
			{
				return this.shortSignal;
			}
		}

		public ArrayList ItemList
		{
			get
			{
				return this.list;
			}
		}

		public EntryRule(Entry entry)
		{
			this.entry = entry;
			this.Init();
		}

		private void Init()
		{
			this.list = new ArrayList();
			this.longSignal = new SignalItem(SignalItemType.Long, this.entry);
			this.shortSignal = new SignalItem(SignalItemType.Short, this.entry);
		}

		public void Add(RuleItem ruleItem)
		{
			this.list.Add(ruleItem);
		}

		public void Execute()
		{
			foreach (RuleItem ruleItem in this.list)
			{
				ruleItem.Execute();
			}
		}
	}
}
