using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class OrderEntryListEditorForm : Form
	{
		private IContainer components;
		private Panel panel;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private ListView ltvEntries;
		private ImageList imgToolbar;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private ColumnHeader columnHeader4;
		private ColumnHeader columnHeader5;
		private ColumnHeader columnHeader6;
		private ColumnHeader columnHeader7;
		private ColumnHeader columnHeader8;
		private Panel panel1;
		private Button btnCancel;
		private Button btnOk;
		private ToolStrip toolStrip;
		private ToolStripButton tsbNewEntry;
		private ContextMenuStrip ctxEntries;
		private ToolStripMenuItem ctxEntries_New;
		private ToolStripMenuItem ctxEntries_Edit;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem ctxEntries_Remove;
		private Dictionary<OrderEntry, OrderEntryViewItem> entryItems;
		public OrderEntryList Entries
		{
			get
			{
				OrderEntryList orderEntryList = new OrderEntryList();
				foreach (OrderEntry current in this.entryItems.Keys)
				{
					orderEntryList.Add(current);
				}
				return orderEntryList;
			}
			set
			{
				this.ltvEntries.BeginUpdate();
				this.ltvEntries.Items.Clear();
				foreach (OrderEntry entry in value)
				{
					this.AddEntry(entry);
				}
				this.ltvEntries.ListViewItemSorter = new OrderEntryListComparer();
				this.ltvEntries.Sort();
				this.ltvEntries.EndUpdate();
			}
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(OrderEntryListEditorForm));
			this.imgToolbar = new ImageList(this.components);
			this.panel = new Panel();
			this.panel1 = new Panel();
			this.btnCancel = new Button();
			this.btnOk = new Button();
			this.tabControl1 = new TabControl();
			this.tabPage1 = new TabPage();
			this.ltvEntries = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.columnHeader2 = new ColumnHeader();
			this.columnHeader3 = new ColumnHeader();
			this.columnHeader4 = new ColumnHeader();
			this.columnHeader5 = new ColumnHeader();
			this.columnHeader6 = new ColumnHeader();
			this.columnHeader7 = new ColumnHeader();
			this.columnHeader8 = new ColumnHeader();
			this.ctxEntries = new ContextMenuStrip(this.components);
			this.ctxEntries_New = new ToolStripMenuItem();
			this.ctxEntries_Edit = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.ctxEntries_Remove = new ToolStripMenuItem();
			this.toolStrip = new ToolStrip();
			this.tsbNewEntry = new ToolStripButton();
			this.panel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.ctxEntries.SuspendLayout();
			this.toolStrip.SuspendLayout();
			base.SuspendLayout();
			this.imgToolbar.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imgToolbar.ImageStream");
			this.imgToolbar.TransparentColor = Color.Transparent;
			this.imgToolbar.Images.SetKeyName(0, "document_add.png");
			this.panel.Controls.Add(this.panel1);
			this.panel.Dock = DockStyle.Bottom;
			this.panel.Location = new Point(0, 253);
			this.panel.Name = "panel";
			this.panel.Size = new Size(730, 46);
			this.panel.TabIndex = 1;
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOk);
			this.panel1.Dock = DockStyle.Right;
			this.panel1.Location = new Point(525, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(205, 46);
			this.panel1.TabIndex = 2;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(126, 12);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(62, 22);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnOk.DialogResult = DialogResult.OK;
			this.btnOk.Location = new Point(58, 12);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(62, 22);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Dock = DockStyle.Fill;
			this.tabControl1.Location = new Point(0, 25);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(730, 228);
			this.tabControl1.TabIndex = 2;
			this.tabPage1.Controls.Add(this.ltvEntries);
			this.tabPage1.Location = new Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new Padding(3);
			this.tabPage1.Size = new Size(722, 202);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Order Entries";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.ltvEntries.CheckBoxes = true;
			this.ltvEntries.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeader1,
				this.columnHeader2,
				this.columnHeader3,
				this.columnHeader4,
				this.columnHeader5,
				this.columnHeader6,
				this.columnHeader7,
				this.columnHeader8
			});
			this.ltvEntries.ContextMenuStrip = this.ctxEntries;
			this.ltvEntries.Dock = DockStyle.Fill;
			this.ltvEntries.FullRowSelect = true;
			this.ltvEntries.GridLines = true;
			this.ltvEntries.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.ltvEntries.HideSelection = false;
			this.ltvEntries.Location = new Point(3, 3);
			this.ltvEntries.Name = "ltvEntries";
			this.ltvEntries.ShowGroups = false;
			this.ltvEntries.ShowItemToolTips = true;
			this.ltvEntries.Size = new Size(716, 196);
			this.ltvEntries.TabIndex = 0;
			this.ltvEntries.UseCompatibleStateImageBehavior = false;
			this.ltvEntries.View = View.Details;
			this.ltvEntries.ItemChecked += new ItemCheckedEventHandler(this.ltvEntries_ItemChecked);
			this.columnHeader1.Text = "DateTime";
			this.columnHeader1.Width = 137;
			this.columnHeader2.Text = "Instrument";
			this.columnHeader2.TextAlign = HorizontalAlignment.Right;
			this.columnHeader2.Width = 77;
			this.columnHeader3.Text = "Side";
			this.columnHeader3.TextAlign = HorizontalAlignment.Right;
			this.columnHeader3.Width = 64;
			this.columnHeader4.Text = "OrdType";
			this.columnHeader4.TextAlign = HorizontalAlignment.Right;
			this.columnHeader4.Width = 73;
			this.columnHeader5.Text = "Price";
			this.columnHeader5.TextAlign = HorizontalAlignment.Right;
			this.columnHeader5.Width = 66;
			this.columnHeader6.Text = "StopPx";
			this.columnHeader6.TextAlign = HorizontalAlignment.Right;
			this.columnHeader6.Width = 70;
			this.columnHeader7.Text = "OrderQty";
			this.columnHeader7.TextAlign = HorizontalAlignment.Right;
			this.columnHeader7.Width = 69;
			this.columnHeader8.Text = "Text";
			this.columnHeader8.Width = 128;
			this.ctxEntries.Items.AddRange(new ToolStripItem[]
			{
				this.ctxEntries_New,
				this.ctxEntries_Edit,
				this.toolStripSeparator1,
				this.ctxEntries_Remove
			});
			this.ctxEntries.Name = "ctxEntries";
			this.ctxEntries.Size = new Size(125, 76);
			this.ctxEntries.Opening += new CancelEventHandler(this.ctxEntries_Opening);
			this.ctxEntries_New.Image = (Image)componentResourceManager.GetObject("ctxEntries_New.Image");
			this.ctxEntries_New.Name = "ctxEntries_New";
			this.ctxEntries_New.Size = new Size(124, 22);
			this.ctxEntries_New.Text = "New...";
			this.ctxEntries_New.Click += new EventHandler(this.ctxEntries_New_Click);
			this.ctxEntries_Edit.Image = (Image)componentResourceManager.GetObject("ctxEntries_Edit.Image");
			this.ctxEntries_Edit.Name = "ctxEntries_Edit";
			this.ctxEntries_Edit.Size = new Size(124, 22);
			this.ctxEntries_Edit.Text = "Edit...";
			this.ctxEntries_Edit.Click += new EventHandler(this.ctxEntries_Edit_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(121, 6);
			this.ctxEntries_Remove.Image = (Image)componentResourceManager.GetObject("ctxEntries_Remove.Image");
			this.ctxEntries_Remove.Name = "ctxEntries_Remove";
			this.ctxEntries_Remove.Size = new Size(124, 22);
			this.ctxEntries_Remove.Text = "Remove";
			this.ctxEntries_Remove.Click += new EventHandler(this.ctxEntries_Remove_Click);
			this.toolStrip.Items.AddRange(new ToolStripItem[]
			{
				this.tsbNewEntry
			});
			this.toolStrip.Location = new Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new Size(730, 25);
			this.toolStrip.TabIndex = 3;
			this.toolStrip.Text = "toolStrip1";
			this.tsbNewEntry.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.tsbNewEntry.Image = (Image)componentResourceManager.GetObject("tsbNewEntry.Image");
			this.tsbNewEntry.ImageTransparentColor = Color.Magenta;
			this.tsbNewEntry.Name = "tsbNewEntry";
			this.tsbNewEntry.Size = new Size(23, 22);
			this.tsbNewEntry.Text = "toolStripButton1";
			this.tsbNewEntry.ToolTipText = "New Entry";
			this.tsbNewEntry.Click += new EventHandler(this.tsbNewEntry_Click);
			base.AcceptButton = this.btnOk;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(730, 299);
			base.Controls.Add(this.tabControl1);
			base.Controls.Add(this.toolStrip);
			base.Controls.Add(this.panel);
			base.MinimizeBox = false;
			base.Name = "OrderEntryListEditorForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Simulation Execution Service - Order Entries";
			this.panel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ctxEntries.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
		public OrderEntryListEditorForm()
		{
			this.InitializeComponent();
			this.entryItems = new Dictionary<OrderEntry, OrderEntryViewItem>();
		}
		private void AddEntry(OrderEntry entry)
		{
			OrderEntryViewItem value = new OrderEntryViewItem(entry);
			this.ltvEntries.Items.Add(value);
			this.entryItems.Add(entry, value);
		}
		private void RemoveEntry(OrderEntry entry)
		{
			OrderEntryViewItem orderEntryViewItem = this.entryItems[entry];
			this.entryItems.Remove(entry);
			orderEntryViewItem.Remove();
		}
		private void NewEntry()
		{
			OrderEntryDetailsForm orderEntryDetailsForm = new OrderEntryDetailsForm();
			orderEntryDetailsForm.Entry = null;
			if (orderEntryDetailsForm.ShowDialog(this) == DialogResult.OK)
			{
				this.AddEntry(orderEntryDetailsForm.Entry);
			}
			orderEntryDetailsForm.Dispose();
		}
		private void EditEntry(OrderEntry entry)
		{
			OrderEntryDetailsForm orderEntryDetailsForm = new OrderEntryDetailsForm();
			orderEntryDetailsForm.Entry = entry;
			if (orderEntryDetailsForm.ShowDialog(this) == DialogResult.OK)
			{
				this.entryItems[entry].UpdateValues();
				this.ltvEntries.Sort();
			}
			orderEntryDetailsForm.Dispose();
		}
		private void tsbNewEntry_Click(object sender, EventArgs e)
		{
			this.NewEntry();
		}
		private void ctxEntries_Opening(object sender, CancelEventArgs e)
		{
			int count = this.ltvEntries.SelectedItems.Count;
			if (count == 0)
			{
				this.ctxEntries_New.Enabled = true;
				this.ctxEntries_Edit.Enabled = false;
				this.ctxEntries_Remove.Enabled = false;
				return;
			}
			if (count == 1)
			{
				this.ctxEntries_New.Enabled = true;
				this.ctxEntries_Edit.Enabled = true;
				this.ctxEntries_Remove.Enabled = true;
				return;
			}
			this.ctxEntries_New.Enabled = true;
			this.ctxEntries_Edit.Enabled = false;
			this.ctxEntries_Remove.Enabled = true;
		}
		private void ctxEntries_New_Click(object sender, EventArgs e)
		{
			this.NewEntry();
		}
		private void ctxEntries_Edit_Click(object sender, EventArgs e)
		{
			OrderEntry entry = (this.ltvEntries.SelectedItems[0] as OrderEntryViewItem).Entry;
			this.EditEntry(entry);
		}
		private void ctxEntries_Remove_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure to remove selected entries?", "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				List<OrderEntry> list = new List<OrderEntry>();
				foreach (OrderEntryViewItem orderEntryViewItem in this.ltvEntries.SelectedItems)
				{
					list.Add(orderEntryViewItem.Entry);
				}
				foreach (OrderEntry current in list)
				{
					this.RemoveEntry(current);
				}
			}
		}
		private void ltvEntries_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			OrderEntryViewItem orderEntryViewItem = (OrderEntryViewItem)e.Item;
			orderEntryViewItem.Entry.Enabled = orderEntryViewItem.Checked;
			orderEntryViewItem.UpdateValues();
		}
	}
}
