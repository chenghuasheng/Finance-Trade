using System;
public enum StrategyMode
{
	Simulation,
	Live
}
public enum SignalType
{
	Market,
	Limit,
	Stop,
	StopLimit,
	TrailingStop
}
public enum SignalSide
{
	Buy,
	BuyCover,
	Sell,
	SellShort
}
public enum SignalStatus
{
	New,
	Accepted,
	Rejected
}
public delegate void SignalEventHandler(SignalEventArgs args);
public class SignalEventArgs : EventArgs
{
	private Signal signal;
	public Signal Signal
	{
		get
		{
			return this.signal;
		}
	}
	public SignalEventArgs(Signal signal)
	{
		this.signal = signal;
	}
}