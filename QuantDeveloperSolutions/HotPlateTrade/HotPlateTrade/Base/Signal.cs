using SmartQuant.Charting;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Simulation;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;

public class Signal : IDrawable
{
	private Strategy fStrategy;
	private DateTime fDateTime;
	private SignalType fType;
	private SignalSide fSide;
	private double fPrice;
	private double fStopPrice;
	private double fLimitPrice;
	private double fQty;
	private TimeInForce fTimeInForce;
	private bool fStrategyFill;
	private double fStrategyPrice;
	private Instrument fInstrument;
	private string fText;
	private SignalStatus fStatus;
	private Color buyColor = Color.Blue;
	private Color buyCoverColor = Color.SkyBlue;
	private Color sellColor = Color.Pink;
	private Color sellShortColor = Color.Red;
	private bool toolTipEnabled = true;
	private string toolTipFormat = "{0} {1} {2} {3} {4} {5}";
	internal FillOnBarMode FillOnBarMode;
	internal bool ForceFillOnBarMode;
	internal bool ForceMarketOrder;
	public Strategy Strategy
	{
		get
		{
			return this.fStrategy;
		}
		set
		{
			this.fStrategy = value;
		}
	}
	public DateTime DateTime
	{
		get
		{
			return this.fDateTime;
		}
	}
	public SignalType Type
	{
		get
		{
			return this.fType;
		}
		set
		{
			this.fType = value;
		}
	}
	public SignalSide Side
	{
		get
		{
			return this.fSide;
		}
		set
		{
			this.fSide = value;
		}
	}
	public double Price
	{
		get
		{
			return this.fPrice;
		}
		set
		{
			this.fPrice = value;
		}
	}
	public double StopPrice
	{
		get
		{
			return this.fStopPrice;
		}
		set
		{
			this.fStopPrice = value;
		}
	}
	public double LimitPrice
	{
		get
		{
			return this.fLimitPrice;
		}
		set
		{
			this.fLimitPrice = value;
		}
	}
	public double Qty
	{
		get
		{
			return this.fQty;
		}
		set
		{
			this.fQty = value;
		}
	}
	public TimeInForce TimeInForce
	{
		get
		{
			return this.fTimeInForce;
		}
		set
		{
			this.fTimeInForce = value;
		}
	}
	public double StrategyPrice
	{
		get
		{
			return this.fStrategyPrice;
		}
		set
		{
			this.fStrategyPrice = value;
		}
	}
	public bool StrategyFill
	{
		get
		{
			return this.fStrategyFill;
		}
		set
		{
			this.fStrategyFill = value;
		}
	}
	public Instrument Instrument
	{
		get
		{
			return this.fInstrument;
		}
		set
		{
			this.fInstrument = value;
		}
	}
	public string Text
	{
		get
		{
			return this.fText;
		}
		set
		{
			this.fText = value;
		}
	}
	public SignalStatus Status
	{
		get
		{
			return this.fStatus;
		}
		set
		{
			this.fStatus = value;
		}
	}
	[Category("Drawing Style")]
	public Color BuyColor
	{
		get
		{
			return this.buyColor;
		}
		set
		{
			this.buyColor = value;
		}
	}
	[Category("Drawing Style")]
	public Color BuyCoverColor
	{
		get
		{
			return this.buyCoverColor;
		}
		set
		{
			this.buyCoverColor = value;
		}
	}
	[Category("Drawing Style")]
	public Color SellColor
	{
		get
		{
			return this.sellColor;
		}
		set
		{
			this.sellColor = value;
		}
	}
	[Category("Drawing Style")]
	public Color SellShortColor
	{
		get
		{
			return this.sellShortColor;
		}
		set
		{
			this.sellShortColor = value;
		}
	}
	[Category("ToolTip"), Description("Enable or disable tooltip appearance for this marker.")]
	public bool ToolTipEnabled
	{
		get
		{
			return this.toolTipEnabled;
		}
		set
		{
			this.toolTipEnabled = value;
		}
	}
	[Category("ToolTip"), Description("Tooltip format string. {1} - X coordinate, {2} - Y coordinte.")]
	public string ToolTipFormat
	{
		get
		{
			return this.toolTipFormat;
		}
		set
		{
			this.toolTipFormat = value;
		}
	}
	public Signal(DateTime datetime, SignalType type, SignalSide side, double qty, double strategyPrice, Instrument instrument, string text)
	{
		this.fDateTime = datetime;
		this.fType = type;
		this.fSide = side;
		this.fQty = qty;
		this.fStrategyPrice = strategyPrice;
		this.fInstrument = instrument;
		this.fPrice = this.fInstrument.Price();
		this.fTimeInForce = TimeInForce.GTC;
		this.fText = text;
		this.fStrategy = null;
		this.fStopPrice = 0.0;
		this.fLimitPrice = 0.0;
		this.fStatus = SignalStatus.New;
	}
	public Signal(DateTime datetime, SignalType type, SignalSide side, Instrument instrument, string text) : this(datetime, type, side, 0.0, 0.0, instrument, text)
	{
	}
	public void Paint(Pad Pad, double MinX, double MaxX, double MinY, double MaxY)
	{
		int num = Pad.ClientX((double)this.DateTime.Ticks);
		int num2 = Pad.ClientY(this.fPrice);
		string.Concat(new string[]
			{
			"Stop at ",
			this.fPrice.ToString("F4"),
			" (",
			this.Status.ToString(),
			")"
			});
		new Font("Arial", 8f);
		Color color = this.buyColor;
		switch (this.Side)
		{
			case SignalSide.Buy:
				color = this.buyColor;
				break;
			case SignalSide.BuyCover:
				color = this.buyCoverColor;
				break;
			case SignalSide.Sell:
				color = this.sellColor;
				break;
			case SignalSide.SellShort:
				color = this.sellShortColor;
				break;
		}
		Pen pen = new Pen(color, 2f);
		int num3 = 8;
		double num4 = (double)Pad.ClientX(MinX);
		double num5 = (double)Pad.ClientX(MaxX);
		if ((double)(num - num3 / 2) <= num5 && (double)(num + num3 / 2) >= num4)
		{
			Pad.Graphics.DrawEllipse(pen, num - num3 / 2, num2 - num3 / 2, num3, num3);
		}
	}
	public TDistance Distance(double X, double Y)
	{
		TDistance tDistance = new TDistance();
		tDistance.X = (double)this.DateTime.Ticks;
		tDistance.Y = this.fPrice;
		tDistance.dX = Math.Abs(X - tDistance.X);
		tDistance.dY = Math.Abs(Y - tDistance.Y);
		StringBuilder stringBuilder = new StringBuilder();
		if (this.DateTime.Second != 0 || this.DateTime.Minute != 0 || this.DateTime.Hour != 0)
		{
			stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
				"Signal: ",
				this.Side.ToString(),
				this.Instrument.Symbol,
				this.Price,
				this.DateTime,
				this.Status.ToString()
				});
		}
		else
		{
			stringBuilder.AppendFormat(this.toolTipFormat, new object[]
				{
				"Signal: ",
				this.Side.ToString(),
				this.Instrument.Symbol,
				this.Price,
				this.DateTime.ToShortDateString(),
				this.Status.ToString()
				});
		}
		tDistance.ToolTipText = stringBuilder.ToString();
		return tDistance;
	}
	public void Draw()
	{
		Chart.Pad.Add(this);
	}
}
