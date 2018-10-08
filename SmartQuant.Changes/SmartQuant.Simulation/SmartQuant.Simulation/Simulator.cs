using SmartQuant.Data;
using SmartQuant.Simulation.Design;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Threading;
namespace SmartQuant.Simulation
{
	public class Simulator
	{
		private const SimulationMode DEFAULT_SIMULATION_MODE = SimulationMode.MaxSpeed;
		private const double DEFAULT_SPEED_MULTIPLIER = 1.0;
		private const int DEFAULT_STEP = 0;
		private IntervalList intervals;
		private DataSeriesList inputSeries;
		private SimulationMode simulationMode;
		private double speedMultiplier;
		private Thread thread;
		private bool doWork;
		private bool doPause;
		private SimulatorState state;
		private int step;
		private bool stepEnabled;
		private int objectsInInterval;
        public event SeriesObjectEventHandler NewObject;
        public event ExceptionEventHandler Error;
        public event EventHandler EnterSimulation;
        public event EventHandler ExitSimulation;
        public event EventHandler StateChanged;
        public event IntervalEventHandler EnterInterval;
        public event IntervalEventHandler LeaveInterval;
		[Editor(typeof(DataSeriesListEditor), typeof(UITypeEditor))]
		public DataSeriesList InputSeries
		{
			get
			{
				return this.inputSeries;
			}
		}
		public IntervalList Intervals
		{
			get
			{
				return this.intervals;
			}
		}
		[DefaultValue(SimulationMode.MaxSpeed)]
		public SimulationMode SimulationMode
		{
			get
			{
				return this.simulationMode;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					if (this.state != SimulatorState.Stopped)
					{
						throw new InvalidOperationException("You cannot change the mode while simulator is not stopped");
					}
					this.simulationMode = value;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		[DefaultValue(1.0)]
		public double SpeedMultiplier
		{
			get
			{
				return this.speedMultiplier;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					if (this.state == SimulatorState.Running)
					{
						throw new InvalidOperationException("You cannot change the speed multiplier while simulator is running");
					}
					if (value <= 0.0)
					{
						throw new ArgumentException("The multiplier must be greater then zero");
					}
					this.speedMultiplier = value;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		[DefaultValue(0)]
		public int Step
		{
			get
			{
				return this.step;
			}
			set
			{
				Monitor.Enter(this);
				try
				{
					if (this.state == SimulatorState.Running)
					{
						throw new InvalidOperationException("Your cannot change step while simulator is running");
					}
					if (value < 0)
					{
						throw new ArgumentException("The step must be equal or greater then zero");
					}
					this.step = value;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}
		[Browsable(false)]
		public bool StepEnabled
		{
			get
			{
				return this.stepEnabled;
			}
		}
		[Browsable(false)]
		public int ObjectsInInterval
		{
			get
			{
				return this.objectsInInterval;
			}
		}
		[Browsable(false)]
		public SimulatorState CurrentState
		{
			get
			{
				return this.state;
			}
		}
		public Simulator()
		{
			this.intervals = new IntervalList();
			this.inputSeries = new DataSeriesList();
			this.simulationMode = SimulationMode.MaxSpeed;
			this.speedMultiplier = 1.0;
			this.doWork = false;
			this.doPause = false;
			this.step = 0;
			this.stepEnabled = false;
			this.objectsInInterval = 0;
			this.state = SimulatorState.Stopped;
		}
		public void Start(bool wait)
		{
			Monitor.Enter(this);
			try
			{
				if (this.state != SimulatorState.Stopped)
				{
					throw new InvalidOperationException("The simulator is already running");
				}
				this.stepEnabled = false;
				this.doWork = true;
				this.thread = new Thread(new ThreadStart(this.Run));
				this.thread.Name = "Simulator";
				this.thread.Start();
			}
			finally
			{
				Monitor.Exit(this);
			}
			if (wait)
			{
				this.thread.Join();
			}
		}
		public void Start()
		{
			this.Start(false);
		}
		public void Stop(bool wait)
		{
			Monitor.Enter(this);
			try
			{
				if (this.state == SimulatorState.Running || this.state == SimulatorState.Paused)
				{
					this.doWork = false;
					this.doPause = false;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
			if (wait)
			{
				while (this.state != SimulatorState.Stopped)
				{
					Thread.Sleep(1);
				}
			}
		}
		public void Stop()
		{
			this.Stop(true);
		}
		public void Pause()
		{
			Monitor.Enter(this);
			try
			{
				if (this.state != SimulatorState.Running)
				{
					throw new InvalidOperationException("Cannot pause simulator because it is not running");
				}
				this.doPause = true;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void Continue()
		{
			Monitor.Enter(this);
			try
			{
				if (this.state != SimulatorState.Paused)
				{
					throw new InvalidOperationException("Cannot continue simulator because it is not paused");
				}
				this.stepEnabled = false;
				this.doPause = false;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void DoStep(bool wait)
		{
			Monitor.Enter(this);
			try
			{
				if (this.state == SimulatorState.Running)
				{
					throw new InvalidOperationException("Cannot perform operation 'Step' because simulator is running");
				}
				this.stepEnabled = true;
				switch (this.state)
				{
				case SimulatorState.Stopped:
					this.doWork = true;
					this.thread = new Thread(new ThreadStart(this.Run));
					this.thread.Name = "Simulator";
					this.thread.Start();
					goto IL_8D;
				case SimulatorState.Paused:
					this.doPause = false;
					goto IL_8D;
				}
				throw new ApplicationException("Unknown simulator state");
				IL_8D:;
			}
			finally
			{
				Monitor.Exit(this);
			}
			if (wait)
			{
				while (this.state != SimulatorState.Paused)
				{
					Thread.Sleep(1);
				}
			}
		}
		public void DoStep()
		{
			this.DoStep(true);
		}
		private void Run()
		{
			try
			{
				Clock.ClockMode = ClockMode.Simulation;
				this.state = SimulatorState.Running;
				this.EmitStateChanged();
				this.EmitEnterSimulationEvent();
				foreach (Interval interval in this.intervals)
				{
					if (!this.doWork)
					{
						break;
					}
					Clock.SetDateTime(interval.Begin);
					this.EmitEnterIntervalEvent(interval);
					ArrayList arrayList = new ArrayList();
					this.objectsInInterval = 0;
					foreach (IDataSeries dataSeries in new ArrayList(this.inputSeries))
					{
						int num = dataSeries.IndexOf(interval.Begin, SearchOption.Next);
						int num2 = dataSeries.IndexOf(interval.End.AddMilliseconds(1.0), SearchOption.Prev);
						if (num != -1 && num <= num2)
						{
							QueueEntry queueEntry = new QueueEntry();
							queueEntry.Series = dataSeries;
							queueEntry.Object = (dataSeries[num] as IDataObject);
							queueEntry.CurrentPosition = num + 1;
							queueEntry.EndPosition = num2;
							arrayList.Add(queueEntry);
							int num3 = (queueEntry.Object is Bar) ? 2 : 1;
							this.objectsInInterval += (num2 - num + 1) * num3;
						}
					}
					if (arrayList.Count > 0)
					{
						arrayList.Sort();
						DateTime dateTime = (arrayList[0] as QueueEntry).Object.DateTime;
						DateTime dateTime2 = (this.simulationMode == SimulationMode.MaxSpeed) ? interval.End.AddYears(1) : dateTime;
						DateTime dateTime3 = (this.step == 0) ? DateTime.MaxValue : dateTime.AddSeconds((double)this.step);
						DateTime d = DateTime.Now;
						DateTime dateTime4 = dateTime2;
						while (arrayList.Count > 0 && this.doWork)
						{
							while (true)
							{
								if (this.doPause)
								{
									this.state = SimulatorState.Paused;
									this.EmitStateChanged();
									DateTime now = DateTime.Now;
									while (this.doPause)
									{
										Thread.Sleep(1);
									}
									DateTime now2 = DateTime.Now;
									if (this.doWork)
									{
										if (this.simulationMode == SimulationMode.UserDefinedSpeed)
										{
											d = d.AddTicks(now2.Ticks - now.Ticks);
										}
										if (this.simulationMode == SimulationMode.MaxSpeed && dateTime3 < Clock.Now)
										{
											dateTime3 = Clock.Now.AddSeconds((double)this.step);
										}
										this.state = SimulatorState.Running;
										this.EmitStateChanged();
									}
								}
								if (!this.doWork)
								{
									break;
								}
								bool flag = false;
								QueueEntry queueEntry2 = arrayList[0] as QueueEntry;
								if ((this.simulationMode != SimulationMode.UserDefinedSpeed || !(dateTime2 >= dateTime3)) && (this.simulationMode != SimulationMode.MaxSpeed || !(queueEntry2.Object.DateTime >= dateTime3)))
								{
									goto IL_2D9;
								}
								dateTime3 = dateTime3.AddSeconds((double)this.step);
								if (!this.stepEnabled)
								{
									goto IL_2D9;
								}
								this.doPause = true;
								flag = true;
								IL_4FA:
								if (!flag)
								{
									break;
								}
								continue;
								IL_2D9:
								if (dateTime2 >= queueEntry2.Object.DateTime)
								{
									arrayList.RemoveAt(0);
									if (queueEntry2.Object is Bar || queueEntry2.Object is BarObject)
									{
										if (queueEntry2 is QueueEntry2)
										{
											BarObject barObject = queueEntry2.Object as BarObject;
											QueueEntry2 queueEntry3 = queueEntry2 as QueueEntry2;
											queueEntry3.Bar.High = barObject.Bar.High;
											queueEntry3.Bar.Low = barObject.Bar.Low;
											queueEntry3.Bar.Close = barObject.Bar.Close;
											queueEntry3.Bar.Volume = barObject.Bar.Volume;
											queueEntry3.Bar.OpenInt = barObject.Bar.OpenInt;
											queueEntry3.Bar.EndTime = barObject.Bar.EndTime;
											queueEntry3.Bar.IsComplete = true;
											queueEntry3.Object = queueEntry3.Bar;
										}
										else
										{
											Bar bar = queueEntry2.Object as Bar;
                                            QueueEntry2 queueEntry4 = new QueueEntry2
                                            {
                                                Bar = bar,
                                                Series = queueEntry2.Series,
                                                Object = new BarObject(bar),
                                                CurrentPosition = 1,
                                                EndPosition = 0
                                            };
                                            queueEntry4.Object.DateTime = bar.EndTime;
                                            this.InsertEntry(arrayList, queueEntry4);

											bar.High = bar.Open;
											bar.Low = bar.Open;
											bar.Close = bar.Open;
											bar.Volume = 0L;
											bar.OpenInt = 0L;
											bar.IsComplete = false;
										}
									}
									this.EmitNewObject(queueEntry2.Series, queueEntry2.Object);
									if (queueEntry2.CurrentPosition <= queueEntry2.EndPosition)
									{
										queueEntry2.Object = (queueEntry2.Series[queueEntry2.CurrentPosition] as IDataObject);
										queueEntry2.CurrentPosition++;
										this.InsertEntry(arrayList, queueEntry2);
									}
									flag = (arrayList.Count > 0);
									goto IL_4FA;
								}
								goto IL_4FA;
							}
							if (this.simulationMode == SimulationMode.UserDefinedSpeed)
							{
								Clock.SetDateTime(dateTime2);
								Thread.Sleep(1);
								DateTime now3 = DateTime.Now;
								dateTime2 = dateTime4.Add(new TimeSpan((long)((double)(now3 - d).Ticks * this.speedMultiplier)));
							}
						}
					}
					Clock.FireAllReminders();
					this.EmitLeaveIntervalEvent(interval);
				}
			}
			finally
			{
                Clock.ClockMode = ClockMode.Realtime;
                this.EmitExitSimulationEvent();
				this.state = SimulatorState.Stopped;
				this.EmitStateChanged();
			}
		}
		public override string ToString()
		{
			return this.state.ToString();
		}
		private void EmitNewObject(IDataSeries series, IDataObject obj)
		{
			DateTime dateTime;
			if (obj is Bar)
			{
				Bar bar = obj as Bar;
				dateTime = (bar.IsComplete ? bar.EndTime : bar.BeginTime);
			}
			else
			{
				dateTime = obj.DateTime;
			}
			Clock.SetDateTime(dateTime);
			if (this.NewObject != null)
			{
				this.NewObject(new SeriesObjectEventArgs(series, obj));
			}
		}
		private void EmitErrorEvent(Exception exception)
		{
			if (this.Error != null)
			{
				this.Error(new ExceptionEventArgs(exception));
			}
		}
		private void EmitEnterSimulationEvent()
		{
			if (this.EnterSimulation != null)
			{
				this.EnterSimulation(this, EventArgs.Empty);
			}
		}
		private void EmitExitSimulationEvent()
		{
			if (this.ExitSimulation != null)
			{
				this.ExitSimulation(this, EventArgs.Empty);
			}
		}
		private void EmitStateChanged()
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(this, EventArgs.Empty);
			}
		}
		private void EmitEnterIntervalEvent(Interval interval)
		{
			if (this.EnterInterval != null)
			{
				this.EnterInterval(new IntervalEventArgs(interval));
			}
		}
		private void EmitLeaveIntervalEvent(Interval interval)
		{
			if (this.LeaveInterval != null)
			{
				this.LeaveInterval(new IntervalEventArgs(interval));
			}
		}
		private void InsertEntry(ArrayList queue, QueueEntry entry)
		{
			if (queue.Count == 0)
			{
				queue.Add(entry);
				return;
			}
			bool flag = entry is QueueEntry2;
			int num = entry.CompareTo(queue[0]);
			if (num < 0 || (num == 0 && flag))
			{
				queue.Insert(0, entry);
				return;
			}
			num = entry.CompareTo(queue[queue.Count - 1]);
			if (num > 0 || (num == 0 && !flag))
			{
				queue.Add(entry);
				return;
			}
			DateTime dateTime = entry.Object.DateTime;
			int num2 = 0;
			int num3 = queue.Count - 1;
			int num4;
			while (true)
			{
				num4 = (num3 + num2) / 2;
				DateTime dateTime2 = (queue[num4] as QueueEntry).Object.DateTime;
				DateTime dateTime3 = (queue[num4 + 1] as QueueEntry).Object.DateTime;
				if (flag)
				{
					if (dateTime2 < dateTime && dateTime <= dateTime3)
					{
						break;
					}
					if (dateTime > dateTime3)
					{
						num2 = num4 + 1;
					}
					else
					{
						if (dateTime <= dateTime2)
						{
							num3 = num4;
						}
					}
				}
				else
				{
					if (dateTime2 <= dateTime && dateTime < dateTime3)
					{
						break;
					}
					if (dateTime >= dateTime3)
					{
						num2 = num4 + 1;
					}
					else
					{
						if (dateTime < dateTime2)
						{
							num3 = num4;
						}
					}
				}
			}
			queue.Insert(num4 + 1, entry);
		}
	}
}
