using SmartQuant.FIX;
using SmartQuant.Instruments;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class OrderEntryDetailsForm : Form
	{
		private OrderEntry entry;
		private IContainer components;
		private Panel panel1;
		private GroupBox groupBox1;
		private Label label8;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label label3;
		private Label label2;
		private Label label1;
		private DateTimePicker dtpDateTime;
		private ComboBox cbxInstruments;
		private ComboBox cbxSides;
		private ComboBox cbxOrdTypes;
		private NumericUpDown nudOrderQty;
		private NumericUpDown nudStopPx;
		private NumericUpDown nudPrice;
		private TextBox tbxText;
		private Button btnCancel;
		private Button btnOK;
		public OrderEntry Entry
		{
			get
			{
				return this.entry;
			}
			set
			{
				this.entry = value;
				if (this.entry == null)
				{
					this.entry = new OrderEntry();
					this.entry.Enabled = true;
					this.Text = "New Entry";
					this.dtpDateTime.Value = DateTime.Now;
					if (this.cbxInstruments.Items.Count > 0)
					{
						this.cbxInstruments.SelectedIndex = 0;
					}
					this.cbxSides.SelectedIndex = 0;
					this.cbxOrdTypes.SelectedIndex = 0;
					return;
				}
				this.Text = "Edit Entry";
				this.dtpDateTime.Value = this.entry.DateTime;
				this.cbxInstruments.SelectedItem = this.entry.Instrument;
				this.cbxSides.SelectedItem = this.entry.Side;
				this.cbxOrdTypes.SelectedItem = this.entry.OrdType;
				this.nudPrice.Value = (decimal)this.entry.Price;
				this.nudStopPx.Value = (decimal)this.entry.StopPx;
				this.nudOrderQty.Value = (decimal)this.entry.OrderQty;
				this.tbxText.Text = this.entry.Text;
			}
		}
		public OrderEntryDetailsForm()
		{
			this.InitializeComponent();
			this.dtpDateTime.CustomFormat = string.Format("{0} {1}", DateTimeFormatInfo.CurrentInfo.ShortDatePattern, DateTimeFormatInfo.CurrentInfo.LongTimePattern);
			this.cbxInstruments.BeginUpdate();
			this.cbxInstruments.Items.Clear();
			foreach (Instrument instrument in InstrumentManager.Instruments)
			{
				this.cbxInstruments.Items.Add(new InstrumentItem(instrument));
			}
			this.cbxInstruments.EndUpdate();
			this.cbxSides.BeginUpdate();
			this.cbxSides.Items.Clear();
			this.cbxSides.Items.Add(Side.Buy);
			this.cbxSides.Items.Add(Side.Sell);
			this.cbxSides.Items.Add(Side.SellShort);
			this.cbxSides.EndUpdate();
			this.cbxOrdTypes.BeginUpdate();
			this.cbxOrdTypes.Items.Clear();
			this.cbxOrdTypes.Items.Add(OrdType.Market);
			this.cbxOrdTypes.Items.Add(OrdType.Limit);
			this.cbxOrdTypes.Items.Add(OrdType.Stop);
			this.cbxOrdTypes.Items.Add(OrdType.StopLimit);
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (base.DialogResult == DialogResult.OK)
			{
				this.entry.DateTime = this.dtpDateTime.Value;
				this.entry.Instrument = ((this.cbxInstruments.SelectedItem == null) ? null : (this.cbxInstruments.SelectedItem as InstrumentItem).Instrument);
				this.entry.Side = (Side)this.cbxSides.SelectedItem;
				this.entry.OrdType = (OrdType)this.cbxOrdTypes.SelectedItem;
				switch (this.entry.OrdType)
				{
				case OrdType.Market:
					this.entry.Price = 0.0;
					this.entry.StopPx = 0.0;
					break;
				case OrdType.Limit:
					this.entry.Price = (double)this.nudPrice.Value;
					this.entry.StopPx = 0.0;
					break;
				case OrdType.Stop:
					this.entry.Price = 0.0;
					this.entry.StopPx = (double)this.nudStopPx.Value;
					break;
				case OrdType.StopLimit:
					this.entry.Price = (double)this.nudPrice.Value;
					this.entry.StopPx = (double)this.nudStopPx.Value;
					break;
				}
				this.entry.OrderQty = (double)this.nudOrderQty.Value;
				this.entry.Text = this.tbxText.Text.Trim();
			}
			base.OnFormClosing(e);
		}
		private void UpdateDecimalPlaces()
		{
			int decimalPlaces;
			if (this.cbxInstruments.SelectedItem == null)
			{
				decimalPlaces = 2;
			}
			else
			{
				string priceDisplay = (this.cbxInstruments.SelectedItem as InstrumentItem).Instrument.PriceDisplay;
				if (priceDisplay != null && priceDisplay.Length > 1)
				{
					if (!int.TryParse(priceDisplay.Substring(1), out decimalPlaces))
					{
						decimalPlaces = 2;
					}
				}
				else
				{
					decimalPlaces = 2;
				}
			}
			this.nudPrice.DecimalPlaces = decimalPlaces;
			this.nudStopPx.DecimalPlaces = decimalPlaces;
		}
		private void OrderTypeChanged()
		{
			switch ((OrdType)this.cbxOrdTypes.SelectedItem)
			{
			case OrdType.Market:
				this.nudPrice.Enabled = false;
				this.nudStopPx.Enabled = false;
				return;
			case OrdType.Limit:
				this.nudPrice.Enabled = true;
				this.nudStopPx.Enabled = false;
				return;
			case OrdType.Stop:
				this.nudPrice.Enabled = false;
				this.nudStopPx.Enabled = true;
				return;
			case OrdType.StopLimit:
				this.nudPrice.Enabled = true;
				this.nudStopPx.Enabled = true;
				return;
			default:
				return;
			}
		}
		private void cbxInstruments_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateDecimalPlaces();
		}
		private void cbxOrdTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OrderTypeChanged();
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
			this.panel1 = new Panel();
			this.btnCancel = new Button();
			this.btnOK = new Button();
			this.groupBox1 = new GroupBox();
			this.tbxText = new TextBox();
			this.nudOrderQty = new NumericUpDown();
			this.nudStopPx = new NumericUpDown();
			this.nudPrice = new NumericUpDown();
			this.cbxOrdTypes = new ComboBox();
			this.cbxSides = new ComboBox();
			this.cbxInstruments = new ComboBox();
			this.dtpDateTime = new DateTimePicker();
			this.label8 = new Label();
			this.label7 = new Label();
			this.label6 = new Label();
			this.label5 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.nudOrderQty).BeginInit();
			((ISupportInitialize)this.nudStopPx).BeginInit();
			((ISupportInitialize)this.nudPrice).BeginInit();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOK);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(4, 223);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(298, 44);
			this.panel1.TabIndex = 0;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(156, 12);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(64, 22);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnOK.DialogResult = DialogResult.OK;
			this.btnOK.Location = new Point(86, 12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new Size(64, 22);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "Ok";
			this.btnOK.UseVisualStyleBackColor = true;
			this.groupBox1.Controls.Add(this.tbxText);
			this.groupBox1.Controls.Add(this.nudOrderQty);
			this.groupBox1.Controls.Add(this.nudStopPx);
			this.groupBox1.Controls.Add(this.nudPrice);
			this.groupBox1.Controls.Add(this.cbxOrdTypes);
			this.groupBox1.Controls.Add(this.cbxSides);
			this.groupBox1.Controls.Add(this.cbxInstruments);
			this.groupBox1.Controls.Add(this.dtpDateTime);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = DockStyle.Fill;
			this.groupBox1.Location = new Point(4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(298, 219);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.tbxText.Location = new Point(88, 184);
			this.tbxText.Name = "tbxText";
			this.tbxText.Size = new Size(188, 20);
			this.tbxText.TabIndex = 15;
			this.nudOrderQty.Location = new Point(88, 160);
			NumericUpDown arg_4B2_0 = this.nudOrderQty;
			int[] array = new int[4];
			array[0] = 1000000;
			arg_4B2_0.Maximum = new decimal(array);
			this.nudOrderQty.Name = "nudOrderQty";
			this.nudOrderQty.Size = new Size(94, 20);
			this.nudOrderQty.TabIndex = 14;
			this.nudOrderQty.TextAlign = HorizontalAlignment.Right;
			this.nudStopPx.Location = new Point(88, 136);
			NumericUpDown arg_526_0 = this.nudStopPx;
			int[] array2 = new int[4];
			array2[0] = 1000000;
			arg_526_0.Maximum = new decimal(array2);
			this.nudStopPx.Name = "nudStopPx";
			this.nudStopPx.Size = new Size(94, 20);
			this.nudStopPx.TabIndex = 13;
			this.nudStopPx.TextAlign = HorizontalAlignment.Right;
			this.nudPrice.Location = new Point(88, 112);
			NumericUpDown arg_597_0 = this.nudPrice;
			int[] array3 = new int[4];
			array3[0] = 1000000;
			arg_597_0.Maximum = new decimal(array3);
			this.nudPrice.Name = "nudPrice";
			this.nudPrice.Size = new Size(94, 20);
			this.nudPrice.TabIndex = 12;
			this.nudPrice.TextAlign = HorizontalAlignment.Right;
			this.cbxOrdTypes.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxOrdTypes.FormattingEnabled = true;
			this.cbxOrdTypes.Location = new Point(88, 88);
			this.cbxOrdTypes.Name = "cbxOrdTypes";
			this.cbxOrdTypes.Size = new Size(96, 21);
			this.cbxOrdTypes.TabIndex = 11;
			this.cbxOrdTypes.SelectedIndexChanged += new EventHandler(this.cbxOrdTypes_SelectedIndexChanged);
			this.cbxSides.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxSides.FormattingEnabled = true;
			this.cbxSides.Location = new Point(88, 64);
			this.cbxSides.Name = "cbxSides";
			this.cbxSides.Size = new Size(96, 21);
			this.cbxSides.TabIndex = 10;
			this.cbxInstruments.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxInstruments.FormattingEnabled = true;
			this.cbxInstruments.Location = new Point(88, 40);
			this.cbxInstruments.Name = "cbxInstruments";
			this.cbxInstruments.Size = new Size(96, 21);
			this.cbxInstruments.Sorted = true;
			this.cbxInstruments.TabIndex = 9;
			this.cbxInstruments.SelectedIndexChanged += new EventHandler(this.cbxInstruments_SelectedIndexChanged);
			this.dtpDateTime.Format = DateTimePickerFormat.Custom;
			this.dtpDateTime.Location = new Point(88, 16);
			this.dtpDateTime.Name = "dtpDateTime";
			this.dtpDateTime.Size = new Size(188, 20);
			this.dtpDateTime.TabIndex = 8;
			this.label8.Location = new Point(16, 184);
			this.label8.Name = "label8";
			this.label8.Size = new Size(66, 20);
			this.label8.TabIndex = 7;
			this.label8.Text = "Text";
			this.label8.TextAlign = ContentAlignment.MiddleLeft;
			this.label7.Location = new Point(16, 160);
			this.label7.Name = "label7";
			this.label7.Size = new Size(66, 20);
			this.label7.TabIndex = 6;
			this.label7.Text = "OrderQty";
			this.label7.TextAlign = ContentAlignment.MiddleLeft;
			this.label6.Location = new Point(16, 136);
			this.label6.Name = "label6";
			this.label6.Size = new Size(66, 20);
			this.label6.TabIndex = 5;
			this.label6.Text = "StopPx";
			this.label6.TextAlign = ContentAlignment.MiddleLeft;
			this.label5.Location = new Point(16, 112);
			this.label5.Name = "label5";
			this.label5.Size = new Size(66, 20);
			this.label5.TabIndex = 4;
			this.label5.Text = "Price";
			this.label5.TextAlign = ContentAlignment.MiddleLeft;
			this.label4.Location = new Point(16, 88);
			this.label4.Name = "label4";
			this.label4.Size = new Size(66, 20);
			this.label4.TabIndex = 3;
			this.label4.Text = "OrdType";
			this.label4.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.Location = new Point(16, 64);
			this.label3.Name = "label3";
			this.label3.Size = new Size(66, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Side";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new Size(66, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Instrument";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label1.Location = new Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(66, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "DateTime";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(306, 267);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "OrderEntryDetailsForm";
			base.Padding = new Padding(4, 4, 4, 0);
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "OrderEntryDetailsForm";
			this.panel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.nudOrderQty).EndInit();
			((ISupportInitialize)this.nudStopPx).EndInit();
			((ISupportInitialize)this.nudPrice).EndInit();
			base.ResumeLayout(false);
		}
	}
}
