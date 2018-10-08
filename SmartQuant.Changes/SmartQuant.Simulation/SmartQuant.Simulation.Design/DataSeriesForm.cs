using SmartQuant.Data;
using SmartQuant.Instruments;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace SmartQuant.Simulation.Design
{
	internal class DataSeriesForm : Form
	{
		private IDataSeries[] series;
		private Panel panel1;
		private ListView ltvSeries;
		private Button btnOk;
		private Button btnCancel;
		private ImageList images;
		private IContainer components;
		internal IDataSeries[] Series
		{
			get
			{
				return this.series;
			}
		}
		public DataSeriesForm()
		{
			this.InitializeComponent();
			foreach (IDataSeries dataSeries in DataManager.Server.GetDataSeries())
			{
				this.ltvSeries.Items.Add(new DataSeriesViewItem(dataSeries));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataSeriesForm));
			this.panel1 = new Panel();
			this.btnCancel = new Button();
			this.btnOk = new Button();
			this.ltvSeries = new ListView();
			this.images = new ImageList(this.components);
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOk);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 285);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(432, 40);
			this.panel1.TabIndex = 0;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(336, 8);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(72, 24);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnOk.DialogResult = DialogResult.OK;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new Point(248, 8);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(72, 24);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "Ok";
			this.ltvSeries.Dock = DockStyle.Fill;
			this.ltvSeries.HideSelection = false;
			this.ltvSeries.Location = new Point(0, 0);
			this.ltvSeries.Name = "ltvSeries";
			this.ltvSeries.Size = new Size(432, 285);
			this.ltvSeries.SmallImageList = this.images;
			this.ltvSeries.Sorting = SortOrder.Ascending;
			this.ltvSeries.TabIndex = 1;
			this.ltvSeries.UseCompatibleStateImageBehavior = false;
			this.ltvSeries.View = View.List;
			this.ltvSeries.DoubleClick += new EventHandler(this.ltvSeries_DoubleClick);
			this.ltvSeries.SelectedIndexChanged += new EventHandler(this.ltvSeries_SelectedIndexChanged);
			this.images.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("images.ImageStream");
			this.images.TransparentColor = Color.Transparent;
			this.images.Images.SetKeyName(0, "");
			base.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new Size(5, 13);
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(432, 325);
			base.Controls.Add(this.ltvSeries);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DataSeriesForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Data Series";
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
		private void ltvSeries_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.ltvSeries.SelectedItems.Count > 0)
			{
				ArrayList arrayList = new ArrayList();
				foreach (DataSeriesViewItem dataSeriesViewItem in this.ltvSeries.SelectedItems)
				{
					arrayList.Add(dataSeriesViewItem.Series);
				}
				this.series = (arrayList.ToArray(typeof(IDataSeries)) as IDataSeries[]);
				this.btnOk.Enabled = true;
				return;
			}
			this.series = null;
			this.btnOk.Enabled = false;
		}
		private void ltvSeries_DoubleClick(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
			base.Close();
		}
	}
}
