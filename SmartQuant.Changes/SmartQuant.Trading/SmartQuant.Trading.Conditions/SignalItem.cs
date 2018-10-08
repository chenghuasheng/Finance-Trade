using System;

namespace SmartQuant.Trading.Conditions
{
	public class SignalItem : RuleItem
	{
		private SignalItemType signalType;

		private IStrategyComponent component;

		public SignalItemType SignalType
		{
			get
			{
				return this.signalType;
			}
			set
			{
				this.signalType = value;
			}
		}

		public override string Name
		{
			get
			{
				return this.signalType.ToString();
			}
		}

		public override string CodeName
		{
			get
			{
				return "rule." + this.signalType;
			}
		}

		public SignalItem(SignalItemType signalType, IStrategyComponent component)
		{
			this.signalType = signalType;
			this.component = component;
		}

		public SignalItem(SignalItemType signalType)
		{
			this.signalType = signalType;
		}

		public override void Execute()
		{
			if (this.component is Entry)
			{
				Entry entry = this.component as Entry;
				switch (this.signalType)
				{
				case SignalItemType.Long:
					entry.LongEntry();
					break;
				case SignalItemType.Short:
					entry.ShortEntry();
					break;
				}
			}
			if (this.component is Exit)
			{
				Exit exit = this.component as Exit;
				switch (this.signalType)
				{
				case SignalItemType.Long:
					exit.LongExit();
					return;
				case SignalItemType.Short:
					exit.ShortExit();
					break;
				default:
					return;
				}
			}
		}
	}
}
