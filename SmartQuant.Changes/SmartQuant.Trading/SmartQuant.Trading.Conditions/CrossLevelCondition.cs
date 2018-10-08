using SmartQuant.Series;
using System;

namespace SmartQuant.Trading.Conditions
{
	public class CrossLevelCondition : Condition
	{
		private DoubleSeries series;

		private double level;

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

		public DoubleSeries Series
		{
			get
			{
				return this.series;
			}
			set
			{
				this.series = value;
			}
		}

		public double Level
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		public override string Name
		{
			get
			{
				if (this.series != null)
				{
					return string.Concat(new string[]
					{
						"CrossLevelCondition(",
						this.series.Name,
						", ",
						this.level.ToString(),
						")"
					});
				}
				return "CrossLevelCondition(...)";
			}
		}

		public override string CodeName
		{
			get
			{
				return this.series.Name.Replace(" ", "") + "Crosses" + this.level;
			}
		}

		public override string GetInitCode(string name)
		{
			return string.Concat(new object[]
			{
				"CrossLevelCondition ",
				name,
				" = new CrossLevelCondition(",
				this.series.Name.Replace(" ", ""),
				", ",
				this.level,
				");"
			});
		}

		public CrossLevelCondition(DoubleSeries series, double level)
		{
			this.series = series;
			this.level = level;
			this.Init();
		}

		public CrossLevelCondition()
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
			switch (this.series.Crosses(this.level, this.series.Count - 1))
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
