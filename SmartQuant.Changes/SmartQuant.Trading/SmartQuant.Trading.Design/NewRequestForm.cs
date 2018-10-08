using SmartQuant.Data;
using SmartQuant.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartQuant.Trading.Design
{
	internal class NewRequestForm : Form
	{
		private Panel pnlBar;

		private Label label2;

		private Label label3;

		private Panel panel1;

		private Label label1;

		private ComboBox cbxDataType;

		private Button btnOk;

		private Button btnCancel;

		private ComboBox cbxBarType;

		private NumericUpDown nudBarSize;

		private Container components;

		public string Request
		{
			get
			{
				string result = null;
				switch ((DataManager.EDataSeries)this.cbxDataType.SelectedItem)
				{
				case DataManager.EDataSeries.Daily:
					result = "Daily";
					break;
				case DataManager.EDataSeries.Trade:
					result = "Trade";
					break;
				case DataManager.EDataSeries.Quote:
					result = "Quote";
					break;
				case DataManager.EDataSeries.Bar:
					result = string.Concat(new object[]
					{
						"Bar",
						'.',
						((BarType)this.cbxBarType.SelectedItem).ToString(),
						'.',
						((long)this.nudBarSize.Value).ToString()
					});
					break;
				case DataManager.EDataSeries.MarketDepth:
					result = "Depth";
					break;
				case DataManager.EDataSeries.Fundamental:
					result = "Fund";
					break;
				case DataManager.EDataSeries.CorporateAction:
					result = "Corp";
					break;
				}
				return result;
			}
		}

		public NewRequestForm()
		{
			this.InitializeComponent();
			foreach (DataManager.EDataSeries eDataSeries in Enum.GetValues(typeof(DataManager.EDataSeries)))
			{
				this.cbxDataType.Items.Add(eDataSeries);
			}
			this.cbxDataType.SelectedIndex = 0;
			foreach (BarType barType in Enum.GetValues(typeof(BarType)))
			{
				this.cbxBarType.Items.Add(barType);
			}
			this.cbxBarType.SelectedIndex = 0;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.pnlBar = new Panel();
			this.label2 = new Label();
			this.cbxBarType = new ComboBox();
			this.label3 = new Label();
			this.nudBarSize = new NumericUpDown();
			this.panel1 = new Panel();
			this.label1 = new Label();
			this.cbxDataType = new ComboBox();
			this.btnOk = new Button();
			this.btnCancel = new Button();
			this.pnlBar.SuspendLayout();
			((ISupportInitialize)this.nudBarSize).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.pnlBar.Controls.Add(this.nudBarSize);
			this.pnlBar.Controls.Add(this.label3);
			this.pnlBar.Controls.Add(this.cbxBarType);
			this.pnlBar.Controls.Add(this.label2);
			this.pnlBar.Location = new Point(8, 40);
			this.pnlBar.Name = "pnlBar";
			this.pnlBar.Size = new Size(280, 24);
			this.pnlBar.TabIndex = 6;
			this.label2.Dock = DockStyle.Left;
			this.label2.Location = new Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new Size(56, 24);
			this.label2.TabIndex = 3;
			this.label2.Text = "Bar type";
			this.label2.TextAlign = ContentAlignment.MiddleCenter;
			this.cbxBarType.Dock = DockStyle.Left;
			this.cbxBarType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxBarType.Location = new Point(56, 0);
			this.cbxBarType.Name = "cbxBarType";
			this.cbxBarType.Size = new Size(88, 21);
			this.cbxBarType.TabIndex = 5;
			this.label3.Dock = DockStyle.Left;
			this.label3.Location = new Point(144, 0);
			this.label3.Name = "label3";
			this.label3.Size = new Size(56, 24);
			this.label3.TabIndex = 6;
			this.label3.Text = "Bar type";
			this.label3.TextAlign = ContentAlignment.MiddleCenter;
			this.nudBarSize.Dock = DockStyle.Fill;
			this.nudBarSize.Location = new Point(200, 0);
			NumericUpDown arg_2A5_0 = this.nudBarSize;
			int[] array = new int[4];
			array[0] = 10000000;
			arg_2A5_0.Maximum = new decimal(array);
			this.nudBarSize.Name = "nudBarSize";
			this.nudBarSize.Size = new Size(80, 20);
			this.nudBarSize.TabIndex = 7;
			this.nudBarSize.TextAlign = HorizontalAlignment.Right;
			this.panel1.Controls.Add(this.cbxDataType);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new Point(8, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(176, 24);
			this.panel1.TabIndex = 7;
			this.label1.Dock = DockStyle.Left;
			this.label1.Location = new Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new Size(56, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Data type";
			this.label1.TextAlign = ContentAlignment.MiddleCenter;
			this.cbxDataType.Dock = DockStyle.Fill;
			this.cbxDataType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxDataType.Location = new Point(56, 0);
			this.cbxDataType.Name = "cbxDataType";
			this.cbxDataType.Size = new Size(120, 21);
			this.cbxDataType.TabIndex = 2;
			this.cbxDataType.SelectedIndexChanged += new EventHandler(this.cbxDataType_SelectedIndexChanged);
			this.btnOk.DialogResult = DialogResult.OK;
			this.btnOk.Location = new Point(144, 80);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(72, 24);
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "Ok";
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(224, 80);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(72, 24);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "Cancel";
			base.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(314, 111);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.pnlBar);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NewRequestForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "New Request";
			this.pnlBar.ResumeLayout(false);
			((ISupportInitialize)this.nudBarSize).EndInit();
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void cbxDataType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.pnlBar.Enabled = ((DataManager.EDataSeries)this.cbxDataType.SelectedItem == DataManager.EDataSeries.Bar);
		}
	}
}
