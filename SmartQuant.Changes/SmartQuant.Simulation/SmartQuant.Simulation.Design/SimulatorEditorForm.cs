using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class SimulatorEditorForm : Form
	{
		private Panel panel1;
		private Container components;
		private Button btnStart;
		private Button btnStop;
		private Button btnClose;
		private PropertyGrid grdSimulator;
		private Button btnPause;
		private Simulator simulator;
		public SimulatorEditorForm(Simulator simulator)
		{
			this.InitializeComponent();
			this.simulator = simulator;
			this.grdSimulator.SelectedObject = simulator;
			this.UpdateStatus();
			simulator.EnterSimulation += new EventHandler(this.OnEnterSimulation);
			simulator.ExitSimulation += new EventHandler(this.OnExitSimulation);
			simulator.StateChanged += new EventHandler(this.OnStateChanged);
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			this.simulator.EnterSimulation -= new EventHandler(this.OnEnterSimulation);
			this.simulator.ExitSimulation -= new EventHandler(this.OnExitSimulation);
			this.simulator.StateChanged -= new EventHandler(this.OnStateChanged);
			base.OnClosing(e);
		}
		private void InitializeComponent()
		{
			this.panel1 = new Panel();
			this.btnPause = new Button();
			this.btnClose = new Button();
			this.btnStop = new Button();
			this.btnStart = new Button();
			this.grdSimulator = new PropertyGrid();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.btnPause);
			this.panel1.Controls.Add(this.btnClose);
			this.panel1.Controls.Add(this.btnStop);
			this.panel1.Controls.Add(this.btnStart);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 263);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(458, 56);
			this.panel1.TabIndex = 0;
			this.btnPause.Location = new Point(104, 16);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new Size(80, 24);
			this.btnPause.TabIndex = 3;
			this.btnPause.Text = "Pause";
			this.btnPause.Click += new EventHandler(this.btnPause_Click);
			this.btnClose.DialogResult = DialogResult.Cancel;
			this.btnClose.Location = new Point(376, 16);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new Size(72, 24);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnStop.Location = new Point(192, 16);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new Size(80, 24);
			this.btnStop.TabIndex = 1;
			this.btnStop.Text = "Stop";
			this.btnStop.Click += new EventHandler(this.btnStop_Click);
			this.btnStart.Location = new Point(16, 16);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new Size(80, 24);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new EventHandler(this.btnStart_Click);
			this.grdSimulator.CommandsVisibleIfAvailable = true;
			this.grdSimulator.Dock = DockStyle.Fill;
			this.grdSimulator.LargeButtons = false;
			this.grdSimulator.LineColor = SystemColors.ScrollBar;
			this.grdSimulator.Location = new Point(0, 0);
			this.grdSimulator.Name = "grdSimulator";
			this.grdSimulator.Size = new Size(458, 263);
			this.grdSimulator.TabIndex = 3;
			this.grdSimulator.Text = "propertyGrid1";
			this.grdSimulator.ViewBackColor = SystemColors.Window;
			this.grdSimulator.ViewForeColor = SystemColors.WindowText;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.CancelButton = this.btnClose;
			base.ClientSize = new Size(458, 319);
			base.Controls.Add(this.grdSimulator);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SimulatorEditorForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Simulator settings";
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
		private void btnStart_Click(object sender, EventArgs e)
		{
			if (this.simulator.CurrentState == SimulatorState.Stopped)
			{
				this.simulator.Start(false);
				return;
			}
			this.simulator.Continue();
		}
		private void btnPause_Click(object sender, EventArgs e)
		{
			this.simulator.Pause();
		}
		private void btnStop_Click(object sender, EventArgs e)
		{
			this.simulator.Stop(false);
		}
		private void OnEnterSimulation(object sender, EventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(this.OnEnterSimulation), new object[]
				{
					sender,
					e
				});
				return;
			}
			this.UpdateStatus();
		}
		private void OnExitSimulation(object sender, EventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(this.OnExitSimulation), new object[]
				{
					sender,
					e
				});
				return;
			}
			this.UpdateStatus();
		}
		private void OnStateChanged(object sender, EventArgs e)
		{
			if (base.InvokeRequired)
			{
				base.Invoke(new EventHandler(this.OnStateChanged), new object[]
				{
					sender,
					e
				});
				return;
			}
			this.UpdateStatus();
		}
		private void UpdateStatus()
		{
			switch (this.simulator.CurrentState)
			{
			case SimulatorState.Stopped:
				this.btnStart.Enabled = true;
				this.btnPause.Enabled = false;
				this.btnStop.Enabled = false;
				this.btnStart.Text = "Start";
				return;
			case SimulatorState.Running:
				this.btnStart.Enabled = false;
				this.btnPause.Enabled = true;
				this.btnStop.Enabled = true;
				this.btnStart.Text = "Start";
				return;
			case SimulatorState.Paused:
				this.btnStart.Enabled = true;
				this.btnPause.Enabled = false;
				this.btnStop.Enabled = true;
				this.btnStart.Text = "Continue";
				return;
			default:
				return;
			}
		}
	}
}
