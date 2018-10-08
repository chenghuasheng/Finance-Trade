using SmartQuant.Execution;
using SmartQuant.FIX;
using SmartQuant.Instruments;
using System;
using System.Collections.Generic;

namespace SmartQuant.Trading
{
	[StrategyComponent("{E70A6417-E7FA-4ec1-BC16-B03DE53C6E85}", ComponentType.ATSCrossComponent, Name = "Default_ATSCrossComponent", Description = "")]
	public class ATSCrossComponent : ATSStrategyMultiComponent
	{
		public const string GUID = "{E70A6417-E7FA-4ec1-BC16-B03DE53C6E85}";

		private Dictionary<SingleOrder, List<ExecutionReport>> clientOrders;

		public ATSCrossComponent()
		{
			this.clientOrders = new Dictionary<SingleOrder, List<ExecutionReport>>();
		}

		public virtual void OnStopStatusChanged(ATSStop stop)
		{
		}

		public virtual void OnStopCanceled(ATSStop stop)
		{
		}

		public virtual void OnStopExecuted(ATSStop stop)
		{
		}

		public sealed override void OnNewOrder(SingleOrder order)
		{
		}

		public ATSStop SetStop(Position position, double level, StopType type, StopMode mode)
		{
			ATSStop aTSStop = new ATSStop(position, level, type, mode);
			base.Strategy.AddStop(aTSStop);
			return aTSStop;
		}

		public ATSStop SetStop(Position position, DateTime dateTime)
		{
			ATSStop aTSStop = new ATSStop(position, dateTime);
			base.Strategy.AddStop(aTSStop);
			return aTSStop;
		}

		public MarketOrder MarketOrder(Instrument instrument, Side side, double qty)
		{
			MarketOrder marketOrder = new MarketOrder(instrument, side, qty);
			base.Strategy.RegisterOrder(marketOrder);
			return marketOrder;
		}

		public LimitOrder LimitOrder(Instrument instrument, Side side, double qty, double price)
		{
			LimitOrder limitOrder = new LimitOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(limitOrder);
			return limitOrder;
		}

		public StopOrder StopOrder(Instrument instrument, Side side, double qty, double price)
		{
			StopOrder stopOrder = new StopOrder(instrument, side, qty, price);
			base.Strategy.RegisterOrder(stopOrder);
			return stopOrder;
		}

		public StopLimitOrder StopLimitOrder(Instrument instrument, Side side, double qty, double limitPrice, double stopPrice)
		{
			StopLimitOrder stopLimitOrder = new StopLimitOrder(instrument, side, qty, limitPrice, stopPrice);
			base.Strategy.RegisterOrder(stopLimitOrder);
			return stopLimitOrder;
		}

		public TrailingStopOrder TrailingStopOrder(Instrument instrument, Side side, double qty, double delta)
		{
			TrailingStopOrder trailingStopOrder = new TrailingStopOrder(instrument, side, qty, delta);
			base.Strategy.RegisterOrder(trailingStopOrder);
			return trailingStopOrder;
		}

		internal void SetClientOrder(SingleOrder order)
		{
			this.clientOrders.Add(order, new List<ExecutionReport>());
			this.OnClientOrder(order);
		}

		public virtual void OnClientOrder(SingleOrder order)
		{
		}

		public void AcceptClientOrder(SingleOrder order)
		{
			ExecutionReport executionReport = this.CreateExecutionReport(order);
			executionReport.AvgPx = 0.0;
			executionReport.CumQty = 0.0;
			executionReport.LeavesQty = order.OrderQty;
			executionReport.ExecType = ExecType.New;
			executionReport.OrdStatus = OrdStatus.New;
			base.Strategy.SendExecutionReport(executionReport);
		}

		public void CancelClientOrder(SingleOrder order)
		{
			ExecutionReport executionReport = this.CreateExecutionReport(order);
			executionReport.OrigClOrdID = order.ClOrdID;
			executionReport.AvgPx = order.AvgPx;
			executionReport.CumQty = order.CumQty;
			executionReport.LeavesQty = order.LeavesQty;
			executionReport.ExecType = ExecType.Cancelled;
			executionReport.OrdStatus = OrdStatus.Cancelled;
			base.Strategy.SendExecutionReport(executionReport);
		}

		public void RejectClientOrder(SingleOrder order, string text)
		{
			ExecutionReport executionReport = this.CreateExecutionReport(order);
			executionReport.AvgPx = 0.0;
			executionReport.CumQty = 0.0;
			executionReport.LeavesQty = order.OrderQty;
			executionReport.Text = text;
			executionReport.ExecType = ExecType.Rejected;
			executionReport.OrdStatus = OrdStatus.Rejected;
			base.Strategy.SendExecutionReport(executionReport);
		}

		public void RejectClientOrder(SingleOrder order)
		{
			this.RejectClientOrder(order, string.Empty);
		}

		public void FillClientOrder(SingleOrder order, double price)
		{
			ExecutionReport executionReport = this.CreateExecutionReport(order);
			executionReport.LastPx = price;
			executionReport.LastQty = order.OrderQty;
			executionReport.AvgPx = price;
			executionReport.CumQty = order.OrderQty;
			executionReport.LeavesQty = 0.0;
			executionReport.ExecType = ExecType.Fill;
			executionReport.OrdStatus = OrdStatus.Filled;
			base.Strategy.SendExecutionReport(executionReport);
		}

		public void PartialFillClientOrder(SingleOrder order, double price, double qty)
		{
			List<ExecutionReport> list = this.clientOrders[order];
			ExecutionReport executionReport = list[list.Count - 1];
			ExecutionReport executionReport2 = this.CreateExecutionReport(order);
			executionReport2.LastPx = price;
			executionReport2.LastQty = qty;
			executionReport2.AvgPx = (executionReport.AvgPx * executionReport.CumQty + price * qty) / (executionReport.CumQty + qty);
			executionReport2.CumQty = executionReport.CumQty + qty;
			executionReport2.LeavesQty = executionReport.LeavesQty - qty;
			if (executionReport2.LeavesQty == 0.0)
			{
				executionReport2.ExecType = ExecType.Fill;
				executionReport2.OrdStatus = OrdStatus.Filled;
			}
			else
			{
				executionReport2.ExecType = ExecType.PartialFill;
				executionReport2.OrdStatus = OrdStatus.PartiallyFilled;
			}
			base.Strategy.SendExecutionReport(executionReport2);
		}

		private ExecutionReport CreateExecutionReport(SingleOrder order)
		{
			ExecutionReport executionReport = new ExecutionReport();
			executionReport.TransactTime = Clock.Now;
			executionReport.ExecID = DateTime.Now.ToString();
			executionReport.ClOrdID = order.ClOrdID;
			executionReport.OrderID = order.OrderID;
			executionReport.ExecTransType = ExecTransType.New;
			executionReport.Symbol = order.Symbol;
			executionReport.SecurityType = order.SecurityType;
			executionReport.SecurityExchange = order.SecurityExchange;
			executionReport.Currency = order.Currency;
			executionReport.TimeInForce = order.TimeInForce;
			executionReport.Side = order.Side;
			executionReport.OrdType = order.OrdType;
			executionReport.Price = order.Price;
			executionReport.StopPx = order.StopPx;
			executionReport.OrderQty = order.OrderQty;
			this.clientOrders[order].Add(executionReport);
			return executionReport;
		}
	}
}
