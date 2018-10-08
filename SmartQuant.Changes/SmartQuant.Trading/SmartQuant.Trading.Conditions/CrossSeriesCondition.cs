using SmartQuant.Series;
using System;

namespace SmartQuant.Trading.Conditions
{
	public class CrossSeriesCondition : Condition
	{
		private DoubleSeries series1;

		private DoubleSeries series2;

		private RuleItemList above;

		private RuleItemList below;

		private RuleItemList none;

		public RuleItemList Above
		{
			get
			{
				return this.above;
			}
			set
			{
				this.above = value;
			}
		}

		public RuleItemList Below
		{
			get
			{
				return this.below;
			}
			set
			{
				this.below = value;
			}
		}

		public RuleItemList None
		{
			get
			{
				return this.none;
			}
			set
			{
				this.none = value;
			}
		}

		public DoubleSeries Series1
		{
			get
			{
				return this.series1;
			}
			set
			{
				this.series1 = value;
			}
		}

		public DoubleSeries Series2
		{
			get
			{
				return this.series2;
			}
			set
			{
				this.series2 = value;
			}
		}

		public override string Name
		{
			get
			{
				if (this.series1 != null && this.series2 != null)
				{
					return string.Concat(new string[]
					{
						"CrossSeriesCondition(",
						this.series1.Name,
						", ",
						this.series2.Name,
						")"
					});
				}
				return "CrossSeriesCondition(...)";
			}
		}

		public override string CodeName
		{
			get
			{
				return this.series1.Name.Replace(" ", "") + "Crosses" + this.series2.Name.Replace(" ", "");
			}
		}

		public override string GetInitCode(string name)
		{
			return string.Concat(new string[]
			{
				"CrossSeriesCondition ",
				name,
				" = new CrossSeriesCondition(",
				this.series1.Name.Replace(" ", ""),
				", ",
				this.series2.Name.Replace(" ", ""),
				");"
			});
		}

		public CrossSeriesCondition(DoubleSeries series1, DoubleSeries series2)
		{
			this.series1 = series1;
			this.series2 = series2;
			this.Init();
		}

		public CrossSeriesCondition()
		{
			this.Init();
		}

		private void Init()
		{
			this.above = new RuleItemList();
			this.below = new RuleItemList();
			this.none = new RuleItemList();
			this.childs.Add("Above", this.above);
			this.childs.Add("Below", this.below);
			this.childs.Add("None", this.none);
		}

		public override void Execute()
		{
			if (this.series1.Count == 0 || this.series2.Count == 0)
			{
				this.none.Execute();
				return;
			}
			switch (this.series1.Crosses(this.series2, this.series1.LastDateTime))
			{
			case ECross.Above:
				this.above.Execute();
				return;
			case ECross.Below:
				this.below.Execute();
				return;
			case ECross.None:
				this.none.Execute();
				return;
			default:
				return;
			}
		}
	}
}
