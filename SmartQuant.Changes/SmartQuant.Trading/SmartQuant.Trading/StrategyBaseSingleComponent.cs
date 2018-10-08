using SmartQuant.Charting;
using SmartQuant.Instruments;
using SmartQuant.Series;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace SmartQuant.Trading
{
	public abstract class StrategyBaseSingleComponent : SingleInstrumentComponent, IStrategyComponent
	{
		private StrategyBase strategyBase;

		[Browsable(false)]
		public StrategyBase StrategyBase
		{
			get
			{
				return this.strategyBase;
			}
			internal set
			{
				if (this.strategyBase != null)
				{
					this.Disconnect();
				}
				this.strategyBase = value;
				if (this.strategyBase != null)
				{
					this.Connect();
				}
			}
		}

		[Browsable(false)]
		public Portfolio Portfolio
		{
			get
			{
				return this.strategyBase.Portfolio;
			}
		}

		[Browsable(false)]
		public BarSeriesList Bars
		{
			get
			{
				return this.strategyBase.Bars;
			}
		}

		[Browsable(false)]
		public BarSeries Bar
		{
			get
			{
				return this.strategyBase.Bars[this.instrument];
			}
		}

		[Browsable(false)]
		public Position Position
		{
			get
			{
				return this.strategyBase.Portfolio.Positions[this.instrument];
			}
		}

		[Browsable(false)]
		public bool HasPosition
		{
			get
			{
				return this.Position != null;
			}
		}

		[Browsable(false)]
		public NamedOrderTable Orders
		{
			get
			{
				return this.strategyBase.Orders[this.instrument];
			}
		}

		[Browsable(false)]
		public Hashtable Global
		{
			get
			{
				return this.strategyBase.Global;
			}
		}

		public StrategyBaseSingleComponent()
		{
		}

		public void Draw(IDrawable primitive, int padNumber)
		{
			this.strategyBase.MetaStrategyBase.DrawPrimitive(this.instrument, primitive, padNumber);
		}

		public void Draw(IDrawable primitive, int padNumber, Color color)
		{
			if (primitive is TimeSeries)
			{
				(primitive as TimeSeries).Color = color;
			}
			this.Draw(primitive, padNumber);
		}
	}
}
