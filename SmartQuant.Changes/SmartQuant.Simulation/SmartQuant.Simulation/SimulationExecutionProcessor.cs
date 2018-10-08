using SmartQuant.Data;
using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using SmartQuant.Providers;
using System;
namespace SmartQuant.Simulation
{
	internal class SimulationExecutionProcessor
	{
		private SimulationExecutionProvider fProvider;
		private FIXNewOrderSingle fOrder;
		private bool fExecuted;
		private FillOnBarMode fFillOnBarMode;
		private double fTrailingStopExecPrice;
		private Quote fQuote = new Quote();
		public SimulationExecutionProcessor(SimulationExecutionProvider provider, FIXNewOrderSingle order)
		{
			this.fProvider = provider;
			this.fOrder = order;
			this.fFillOnBarMode = provider.FillOnBarMode;
			if ((this.fOrder as SingleOrder).OrdType == OrdType.TrailingStop)
			{
				switch ((this.fOrder as SingleOrder).Side)
				{
				case Side.Buy:
				case Side.BuyMinus:
					this.fTrailingStopExecPrice = 1.7976931348623157E+308;
					goto IL_AE;
				case Side.Sell:
				case Side.SellShort:
					this.fTrailingStopExecPrice = -1.7976931348623157E+308;
					goto IL_AE;
				}
				throw new NotSupportedException("Order side is not supported : " + order.Side.ToString());
			}
			IL_AE:
			this.fProvider.fProcessors.Add(this.fOrder.ClOrdID, this);
			this.Init();
		}
		private void Init()
		{
			if (this.fOrder is SingleOrder)
			{
				SingleOrder singleOrder = this.fOrder as SingleOrder;
				if (singleOrder.ContainsField(11201))
				{
					this.fFillOnBarMode = (FillOnBarMode)singleOrder.FillOnBarMode;
				}
				Instrument instrument = singleOrder.Instrument;
				if (singleOrder.OrdType == OrdType.Market)
				{
					if (this.fProvider.FillOnQuote && this.fProvider.FillOnQuoteMode == FillOnQuoteMode.LastQuote)
					{
						this.Process(instrument.Quote, null, null);
					}
					if (this.fProvider.FillOnTrade && this.fProvider.FillOnTradeMode == FillOnTradeMode.LastTrade)
					{
						this.Process(null, instrument.Trade, null);
					}
					if (this.fProvider.FillOnBar && (this.fFillOnBarMode == FillOnBarMode.LastBarClose || singleOrder.ForceMarketOrder) && this.fProvider.BarFilter.Contains(instrument.Bar.BarType, instrument.Bar.Size))
					{
						this.Process(null, null, instrument.Bar);
					}
				}
				else
				{
					if (instrument.Quote != null && this.fProvider.FillOnQuote)
					{
						this.Process(instrument.Quote, null, null);
					}
					if (instrument.Trade != null && this.fProvider.FillOnTrade)
					{
						this.Process(null, instrument.Trade, null);
					}
					if (instrument.Bar != null && this.fProvider.FillOnBar && this.fProvider.BarFilter.Contains(instrument.Bar.BarType, instrument.Bar.Size))
					{
						double close = instrument.Bar.Close;
						if (close != 0.0)
						{
							this.StopLimit(close, singleOrder.OrderQty);
						}
					}
				}
				if (!this.fExecuted)
				{
					if (this.fProvider.FillOnQuote)
					{
						instrument.NewQuote += new QuoteEventHandler(this.OnNewQuote);
					}
					if (this.fProvider.FillOnTrade)
					{
						instrument.NewTrade += new TradeEventHandler(this.OnNewTrade);
					}
					if (this.fProvider.FillOnBar)
					{
						if (singleOrder.ForceMarketOrder)
						{
							instrument.NewBar += new BarEventHandler(this.OnNewBar);
							instrument.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
						}
						else
						{
							if (singleOrder.OrdType == OrdType.Market)
							{
								switch (this.fFillOnBarMode)
								{
								case FillOnBarMode.LastBarClose:
								case FillOnBarMode.NextBarClose:
									instrument.NewBar += new BarEventHandler(this.OnNewBar);
									break;
								case FillOnBarMode.NextBarOpen:
									instrument.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
									break;
								}
							}
							else
							{
								instrument.NewBar += new BarEventHandler(this.OnNewBar);
								instrument.NewBarOpen += new BarEventHandler(this.OnNewBarOpen);
							}
						}
					}
					if (this.fOrder.TimeInForce == '6')
					{
						Clock.AddReminder(new ReminderEventHandler(this.Clock_Reminder), this.fOrder.ExpireTime, null);
					}
				}
			}
		}
		private void Disconnect()
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			Instrument instrument = singleOrder.Instrument;
			instrument.NewQuote -= new QuoteEventHandler(this.OnNewQuote);
			instrument.NewTrade -= new TradeEventHandler(this.OnNewTrade);
			instrument.NewBar -= new BarEventHandler(this.OnNewBar);
			instrument.NewBarOpen -= new BarEventHandler(this.OnNewBarOpen);
			if (this.fOrder.TimeInForce == '6')
			{
				Clock.RemoveReminder(new ReminderEventHandler(this.Clock_Reminder));
			}
		}
		private void Clock_Reminder(ReminderEventArgs args)
		{
			this.Cancel();
		}
		private double GetQty(SingleOrder order, Quote quote, Trade trade, Bar bar)
		{
			if (quote != null && this.fProvider.PartialFills)
			{
				switch (order.Side)
				{
				case Side.Buy:
					if (quote.AskSize > 0)
					{
						return (double)quote.AskSize;
					}
					goto IL_5E;
				case Side.Sell:
				case Side.SellShort:
					if (quote.BidSize > 0)
					{
						return (double)quote.BidSize;
					}
					goto IL_5E;
				}
				return order.OrderQty;
			}
			IL_5E:
			return order.OrderQty;
		}
		private double GetPrice(Quote quote, Trade trade, Bar bar)
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.ContainsField(11103) && singleOrder.StrategyFill)
			{
				return singleOrder.StrategyPrice;
			}
			if (quote != null && this.fProvider.FillOnQuote)
			{
				switch (singleOrder.Side)
				{
				case Side.Buy:
				case Side.BuyMinus:
					if (quote.Ask != 0.0)
					{
						return quote.Ask;
					}
					goto IL_B0;
				case Side.Sell:
				case Side.SellShort:
					if (quote.Bid != 0.0)
					{
						return quote.Bid;
					}
					goto IL_B0;
				}
				throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
			}
			IL_B0:
			if (trade != null && this.fProvider.FillOnTrade && trade.Price != 0.0)
			{
				return trade.Price;
			}
			if (bar != null && this.fProvider.FillOnBar)
			{
				if (singleOrder.StrategyComponent == "Stop")
				{
					return singleOrder.StrategyPrice;
				}
				if (singleOrder.ForceMarketOrder)
				{
					if (bar.Close != 0.0)
					{
						return bar.Close;
					}
					if (bar.Open != 0.0)
					{
						return bar.Open;
					}
				}
				switch (this.fFillOnBarMode)
				{
				case FillOnBarMode.LastBarClose:
				case FillOnBarMode.NextBarClose:
					return bar.Close;
				case FillOnBarMode.NextBarOpen:
					return bar.Open;
				}
			}
			return 0.0;
		}
		private void Process(Quote quote, Trade trade, Bar bar)
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.OrdStatus == OrdStatus.New || singleOrder.OrdStatus == OrdStatus.PendingNew || singleOrder.OrdStatus == OrdStatus.PartiallyFilled || singleOrder.OrdStatus == OrdStatus.Replaced)
			{
				double price = this.GetPrice(quote, trade, bar);
				double qty = this.GetQty(singleOrder, quote, trade, bar);
				if (price == 0.0)
				{
					return;
				}
				if (singleOrder.OrdType == OrdType.Market)
				{
					this.Execute(price, qty);
					return;
				}
				this.StopLimit(price, qty);
			}
		}
		private void Execute(double price, double qty)
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.IsDone)
			{
				return;
			}
			if (qty < singleOrder.LeavesQty)
			{
				this.fExecuted = false;
				ExecutionReport executionReport = new ExecutionReport();
				executionReport.TransactTime = Clock.Now;
				executionReport.ClOrdID = singleOrder.ClOrdID;
				executionReport.ExecType = ExecType.PartialFill;
				executionReport.OrdStatus = OrdStatus.PartiallyFilled;
				executionReport.Symbol = singleOrder.Symbol;
				executionReport.Side = singleOrder.Side;
				executionReport.OrdType = singleOrder.OrdType;
				executionReport.AvgPx = (singleOrder.AvgPx * singleOrder.CumQty + price * qty) / (singleOrder.CumQty + qty);
				executionReport.OrderQty = singleOrder.OrderQty;
				executionReport.LastQty = qty;
				executionReport.CumQty = singleOrder.CumQty + qty;
				executionReport.LeavesQty = singleOrder.LeavesQty - qty;
				executionReport.LastPx = price;
				executionReport.Price = singleOrder.Price;
				executionReport.StopPx = singleOrder.StopPx;
				executionReport.SecurityType = singleOrder.SecurityType;
				executionReport.SecurityExchange = singleOrder.SecurityExchange;
				executionReport.Currency = singleOrder.Currency;
				executionReport.Text = singleOrder.Text;
				FIXCommissionData commissionData = this.fProvider.CommissionProvider.GetCommissionData(executionReport);
				executionReport.CommType = FIXCommType.FromFIX(commissionData.CommType);
				executionReport.Commission = commissionData.Commission;
				executionReport.AvgPx = this.fProvider.SlippageProvider.GetExecutionPrice(executionReport);
				this.fProvider.EmitExecutionReport(executionReport);
				return;
			}
			this.Disconnect();
			this.fProvider.fProcessors.Remove(this.fOrder.ClOrdID);
			this.fExecuted = true;
			qty = singleOrder.LeavesQty;
			ExecutionReport executionReport2 = new ExecutionReport();
			executionReport2.TransactTime = Clock.Now;
			executionReport2.ClOrdID = singleOrder.ClOrdID;
			executionReport2.ExecType = ExecType.Fill;
			executionReport2.OrdStatus = OrdStatus.Filled;
			executionReport2.Symbol = singleOrder.Symbol;
			executionReport2.Side = singleOrder.Side;
			executionReport2.OrdType = singleOrder.OrdType;
			executionReport2.AvgPx = (singleOrder.AvgPx * singleOrder.CumQty + price * qty) / (singleOrder.CumQty + qty);
			executionReport2.OrderQty = singleOrder.OrderQty;
			executionReport2.LastQty = singleOrder.LeavesQty;
			executionReport2.CumQty = singleOrder.OrderQty;
			executionReport2.LeavesQty = 0.0;
			executionReport2.LastPx = price;
			executionReport2.Price = singleOrder.Price;
			executionReport2.StopPx = singleOrder.StopPx;
			executionReport2.SecurityType = singleOrder.SecurityType;
			executionReport2.SecurityExchange = singleOrder.SecurityExchange;
			executionReport2.Currency = singleOrder.Currency;
			executionReport2.Text = singleOrder.Text;
			FIXCommissionData commissionData2 = this.fProvider.CommissionProvider.GetCommissionData(executionReport2);
			executionReport2.CommType = FIXCommType.FromFIX(commissionData2.CommType);
			executionReport2.Commission = commissionData2.Commission;
			executionReport2.AvgPx = this.fProvider.SlippageProvider.GetExecutionPrice(executionReport2);
			this.fProvider.EmitExecutionReport(executionReport2);
		}
		internal void Cancel()
		{
			this.Disconnect();
			this.fProvider.fProcessors.Remove(this.fOrder.ClOrdID);
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			ExecutionReport executionReport = new ExecutionReport();
			executionReport.TransactTime = Clock.Now;
			executionReport.ClOrdID = singleOrder.ClOrdID;
			executionReport.OrigClOrdID = singleOrder.ClOrdID;
			executionReport.ExecType = ExecType.Cancelled;
			executionReport.OrdStatus = OrdStatus.Cancelled;
			executionReport.Symbol = singleOrder.Symbol;
			executionReport.Side = singleOrder.Side;
			executionReport.OrdType = singleOrder.OrdType;
			executionReport.AvgPx = singleOrder.AvgPx;
			executionReport.LeavesQty = singleOrder.LeavesQty;
			executionReport.CumQty = singleOrder.CumQty;
			executionReport.OrderQty = singleOrder.OrderQty;
			executionReport.Price = singleOrder.Price;
			executionReport.StopPx = singleOrder.StopPx;
			executionReport.Currency = singleOrder.Currency;
			executionReport.Text = singleOrder.Text;
			this.fProvider.EmitExecutionReport(executionReport);
		}
		internal void Replace(FIXOrderCancelReplaceRequest request)
		{
			this.Disconnect();
			this.fProvider.fProcessors.Remove(request.OrigClOrdID);
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			ExecutionReport executionReport = new ExecutionReport();
			executionReport.TransactTime = Clock.Now;
			executionReport.ClOrdID = request.ClOrdID;
			executionReport.OrigClOrdID = request.OrigClOrdID;
			executionReport.ExecType = ExecType.Replace;
			executionReport.OrdStatus = OrdStatus.Replaced;
			executionReport.Symbol = singleOrder.Symbol;
			executionReport.Side = singleOrder.Side;
			executionReport.OrdType = FIXOrdType.FromFIX(request.OrdType);
			executionReport.CumQty = singleOrder.CumQty;
			executionReport.OrderQty = request.OrderQty;
			executionReport.LeavesQty = request.OrderQty - singleOrder.CumQty;
			executionReport.Price = request.Price;
			executionReport.StopPx = request.StopPx;
			executionReport.Currency = singleOrder.Currency;
			executionReport.TimeInForce = FIXTimeInForce.FromFIX(request.TimeInForce);
			executionReport.Text = singleOrder.Text;
			this.fProvider.EmitExecutionReport(executionReport);
			new SimulationExecutionProcessor(this.fProvider, singleOrder);
		}
		private void OnNewQuote(object sender, QuoteEventArgs args)
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			Quote quote = args.Quote;
			switch (singleOrder.Side)
			{
			case Side.Buy:
				if (quote.Ask != this.fQuote.Ask || quote.AskSize != this.fQuote.AskSize)
				{
					this.Process(quote, null, null);
				}
				break;
			case Side.Sell:
			case Side.SellShort:
				if (quote.Bid != this.fQuote.Bid || quote.BidSize != this.fQuote.BidSize)
				{
					this.Process(quote, null, null);
				}
				break;
			}
			this.fQuote = quote;
		}
		private void OnNewTrade(object sender, TradeEventArgs args)
		{
			this.Process(null, args.Trade, null);
		}
		private void OnNewBar(object sender, BarEventArgs args)
		{
			if (!this.fProvider.BarFilter.Contains(args.Bar.BarType, args.Bar.Size))
			{
				return;
			}
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.OrdType == OrdType.Market)
			{
				this.Process(null, null, args.Bar);
				return;
			}
			if (args.Bar != null && this.fProvider.FillOnBar)
			{
				if (singleOrder.OrdType == OrdType.Limit)
				{
					switch (singleOrder.Side)
					{
					case Side.Buy:
					case Side.BuyMinus:
						if (args.Bar.Low <= singleOrder.Price)
						{
							this.Execute(singleOrder.Price, singleOrder.OrderQty);
							goto IL_107;
						}
						goto IL_107;
					case Side.Sell:
					case Side.SellShort:
						if (args.Bar.High >= singleOrder.Price)
						{
							this.Execute(singleOrder.Price, singleOrder.OrderQty);
							goto IL_107;
						}
						goto IL_107;
					}
					throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
				}
				IL_107:
				if (singleOrder.OrdType == OrdType.Stop)
				{
					switch (singleOrder.Side)
					{
					case Side.Buy:
					case Side.BuyMinus:
						if (args.Bar.High >= singleOrder.StopPx)
						{
							this.Execute(singleOrder.StopPx, singleOrder.OrderQty);
							goto IL_1A6;
						}
						goto IL_1A6;
					case Side.Sell:
					case Side.SellShort:
						if (args.Bar.Low <= singleOrder.StopPx)
						{
							this.Execute(singleOrder.StopPx, singleOrder.OrderQty);
							goto IL_1A6;
						}
						goto IL_1A6;
					}
					throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
				}
				IL_1A6:
				if (singleOrder.OrdType == OrdType.TrailingStop)
				{
					switch (singleOrder.Side)
					{
					case Side.Buy:
					case Side.BuyMinus:
						this.fTrailingStopExecPrice = Math.Min(this.fTrailingStopExecPrice, args.Bar.Low + singleOrder.StopPx);
						if (args.Bar.High >= this.fTrailingStopExecPrice)
						{
							this.Execute(this.fTrailingStopExecPrice, singleOrder.OrderQty);
							goto IL_28F;
						}
						goto IL_28F;
					case Side.Sell:
					case Side.SellShort:
						this.fTrailingStopExecPrice = Math.Max(this.fTrailingStopExecPrice, args.Bar.High - singleOrder.StopPx);
						if (args.Bar.Low <= this.fTrailingStopExecPrice)
						{
							this.Execute(this.fTrailingStopExecPrice, singleOrder.OrderQty);
							goto IL_28F;
						}
						goto IL_28F;
					}
					throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
				}
				IL_28F:
				if (singleOrder.OrdType == OrdType.StopLimit)
				{
					if (!singleOrder.IsStopLimitReady)
					{
						switch (singleOrder.Side)
						{
						case Side.Buy:
						case Side.BuyMinus:
							if (args.Bar.High >= singleOrder.StopPx)
							{
								singleOrder.IsStopLimitReady = true;
								goto IL_322;
							}
							goto IL_322;
						case Side.Sell:
						case Side.SellShort:
							if (args.Bar.Low <= singleOrder.StopPx)
							{
								singleOrder.IsStopLimitReady = true;
								goto IL_322;
							}
							goto IL_322;
						}
						throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
					}
					IL_322:
					if (singleOrder.IsStopLimitReady)
					{
						switch (singleOrder.Side)
						{
						case Side.Buy:
						case Side.BuyMinus:
							if (args.Bar.Low <= singleOrder.Price)
							{
								this.Execute(singleOrder.Price, singleOrder.OrderQty);
								return;
							}
							return;
						case Side.Sell:
						case Side.SellShort:
							if (args.Bar.High >= singleOrder.Price)
							{
								this.Execute(singleOrder.Price, singleOrder.OrderQty);
								return;
							}
							return;
						}
						throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
					}
				}
			}
		}
		private void OnNewBarOpen(object sender, BarEventArgs args)
		{
			if (!this.fProvider.BarFilter.Contains(args.Bar.BarType, args.Bar.Size))
			{
				return;
			}
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.OrdType == OrdType.Market)
			{
				this.Process(null, null, args.Bar);
				return;
			}
			if (args.Bar != null && this.fProvider.FillOnBar)
			{
				double open = args.Bar.Open;
				if (open != 0.0)
				{
					this.StopLimit(open, singleOrder.OrderQty);
				}
			}
		}
		private void StopLimit(double price, double qty)
		{
			SingleOrder singleOrder = this.fOrder as SingleOrder;
			if (singleOrder.OrdType == OrdType.Limit)
			{
				switch (singleOrder.Side)
				{
				case Side.Buy:
				case Side.BuyMinus:
					if (price <= singleOrder.Price)
					{
						this.Execute(price, qty);
						goto IL_80;
					}
					goto IL_80;
				case Side.Sell:
				case Side.SellShort:
					if (price >= singleOrder.Price)
					{
						this.Execute(price, qty);
						goto IL_80;
					}
					goto IL_80;
				}
				throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
			}
			IL_80:
			if (singleOrder.OrdType == OrdType.Stop)
			{
				switch (singleOrder.Side)
				{
				case Side.Buy:
				case Side.BuyMinus:
					if (price >= singleOrder.StopPx)
					{
						this.Execute(price, qty);
						goto IL_F4;
					}
					goto IL_F4;
				case Side.Sell:
				case Side.SellShort:
					if (price <= singleOrder.StopPx)
					{
						this.Execute(price, qty);
						goto IL_F4;
					}
					goto IL_F4;
				}
				throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
			}
			IL_F4:
			if (singleOrder.OrdType == OrdType.TrailingStop)
			{
				switch (singleOrder.Side)
				{
				case Side.Buy:
				case Side.BuyMinus:
					this.fTrailingStopExecPrice = Math.Min(this.fTrailingStopExecPrice, price + singleOrder.StopPx);
					if (price >= this.fTrailingStopExecPrice)
					{
						this.Execute(price, qty);
						goto IL_19E;
					}
					goto IL_19E;
				case Side.Sell:
				case Side.SellShort:
					this.fTrailingStopExecPrice = Math.Max(this.fTrailingStopExecPrice, price - singleOrder.StopPx);
					if (price <= this.fTrailingStopExecPrice)
					{
						this.Execute(price, qty);
						goto IL_19E;
					}
					goto IL_19E;
				}
				throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
			}
			IL_19E:
			if (singleOrder.OrdType == OrdType.StopLimit)
			{
				if (!singleOrder.IsStopLimitReady)
				{
					switch (singleOrder.Side)
					{
					case Side.Buy:
					case Side.BuyMinus:
						if (price >= singleOrder.StopPx)
						{
							singleOrder.IsStopLimitReady = true;
							goto IL_21D;
						}
						goto IL_21D;
					case Side.Sell:
					case Side.SellShort:
						if (price <= singleOrder.StopPx)
						{
							singleOrder.IsStopLimitReady = true;
							goto IL_21D;
						}
						goto IL_21D;
					}
					throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
				}
				IL_21D:
				if (singleOrder.IsStopLimitReady)
				{
					switch (singleOrder.Side)
					{
					case Side.Buy:
					case Side.BuyMinus:
						if (price <= singleOrder.Price)
						{
							this.Execute(price, qty);
							return;
						}
						return;
					case Side.Sell:
					case Side.SellShort:
						if (price >= singleOrder.Price)
						{
							this.Execute(price, qty);
							return;
						}
						return;
					}
					throw new NotSupportedException("Order side is not supported : " + singleOrder.Side.ToString());
				}
			}
		}
	}
}
