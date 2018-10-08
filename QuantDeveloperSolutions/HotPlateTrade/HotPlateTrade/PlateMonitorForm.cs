using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
public class PlateMonitorForm : Form
{
	private PlateMonitor plateMonitor=null;
	public PlateMonitorForm()
	{
		InitializeComponent();
	}
	protected override void Dispose(bool disposing)
	{
		if (this.plateMonitor!=null){
			this.plateMonitor.RemoveMonitorForm();
			this.plateMonitor=null;
		}
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}
	private void Form1_Load(object sender, EventArgs e)
	{
		
	}
	private delegate void RefreshDataDelegate(DataGridView dgv, DataTable dt);
	private void RefreshPlateData(DataGridView dgv, DataTable dt)
	{
		if (this.InvokeRequired)
		{
			this.Invoke(new RefreshDataDelegate(this.RefreshPlateData), dgv, dt);
		}
		else
		{
			dgv.DataSource = dt;
			dgv.Columns[0].HeaderText = "编号";
			dgv.Columns[0].Visible = false;
			dgv.Columns[1].HeaderText = "名称";
			dgv.Columns[2].HeaderText = "涨停";
			dgv.Columns[3].HeaderText = "5%以上";
			dgv.Columns[4].HeaderText = "上涨";
			dgv.Columns[5].HeaderText = "活跃";
			dgv.Columns[6].HeaderText = "权值";
			dgv.Columns[1].Width = 80;
			dgv.Columns[2].Width = 68;
			dgv.Columns[3].Width = 68;
			dgv.Columns[4].Width = 68;
			dgv.Columns[5].Width = 68;
			dgv.Columns[6].Width = 68;
		}
	}
	public void ShowPlateStatistic(PlateMonitor plateMonitor){
		this.plateMonitor=plateMonitor;
		DataTable dtIndustryPlates = new PlateDataTable();
		DataTable dtConceptPlates = new PlateDataTable();
		DataTable dtAreaPlates = new PlateDataTable();
		foreach (Plate curPlate in plateMonitor.Plates)
		{
			DataRow dr = null;
			switch (curPlate.Type)
			{
				case 1:
					dr = dtAreaPlates.NewRow();
					break;
				case 2:
					dr = dtIndustryPlates.NewRow();
					break;
				case 3:
					dr = dtConceptPlates.NewRow();
					break;
			}
			dr["ID"] = curPlate.ID;
			dr["Name"] = curPlate.Name;
			dr["UpLimitCount"] = curPlate.UpLimitCount;
			dr["FivePercentCount"] = curPlate.FivePercentCount;
			dr["UpCount"] = curPlate.UpCount;
			dr["ActiveCount"] = curPlate.ActiveCount;
			dr["Weight"] = curPlate.Weight;
			switch (curPlate.Type)
			{
				case 1:
					dtAreaPlates.Rows.Add(dr);
					break;
				case 2:
					dtIndustryPlates.Rows.Add(dr);
					break;
				case 3:
					dtConceptPlates.Rows.Add(dr);
					break;
			}
		}
		DataRow dr1 = dtAreaPlates.NewRow();
		dr1["ID"] = -1;
		dr1["Name"] = "全部";
		dr1["UpLimitCount"] = plateMonitor.UpLimitCountOfAll;
		dr1["FivePercentCount"] = plateMonitor.FivePercentCountOfAll;
		dr1["UpCount"] = plateMonitor.UpCountOfAll;
		dr1["ActiveCount"] = plateMonitor.ActiveCountOfAll;
		dr1["Weight"] = plateMonitor.WeightOfAll;
		dtAreaPlates.Rows.Add(dr1);
		dtAreaPlates.DefaultView.Sort = "UpLimitCount DESC";
		dr1 = dtIndustryPlates.NewRow();
		dr1["ID"] = -1;
		dr1["Name"] = "全部";
		dr1["UpLimitCount"] = plateMonitor.UpLimitCountOfAll;
		dr1["FivePercentCount"] = plateMonitor.FivePercentCountOfAll;
		dr1["UpCount"] = plateMonitor.UpCountOfAll;
		dr1["ActiveCount"] = plateMonitor.ActiveCountOfAll;
		dr1["Weight"] = plateMonitor.WeightOfAll;
		dtIndustryPlates.Rows.Add(dr1);
		dtIndustryPlates.DefaultView.Sort = "UpLimitCount DESC";
		dr1 = dtConceptPlates.NewRow();
		dr1["ID"] = -1;
		dr1["Name"] = "全部";
		dr1["UpLimitCount"] = plateMonitor.UpLimitCountOfAll;
		dr1["FivePercentCount"] = plateMonitor.FivePercentCountOfAll;
		dr1["UpCount"] = plateMonitor.UpCountOfAll;
		dr1["ActiveCount"] = plateMonitor.ActiveCountOfAll;
		dr1["Weight"] = plateMonitor.WeightOfAll;
		dtConceptPlates.Rows.Add(dr1);
		dtConceptPlates.DefaultView.Sort = "UpLimitCount DESC";
		this.RefreshPlateData(this.dgvAreas, dtAreaPlates);
		this.RefreshPlateData(this.dgvIndustries, dtIndustryPlates);
		this.RefreshPlateData(this.dgvConcepts, dtConceptPlates);	
	}
	private System.ComponentModel.IContainer components = null;
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.dgvIndustries = new System.Windows.Forms.DataGridView();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.dgvConcepts = new System.Windows.Forms.DataGridView();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.dgvAreas = new System.Windows.Forms.DataGridView();
			
		
		this.tabControl1.SuspendLayout();
		this.tabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.dgvIndustries)).BeginInit();
		this.tabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.dgvConcepts)).BeginInit();
		this.tabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).BeginInit();
		this.SuspendLayout();
		// 
		// tabControl1
		// 
		this.tabControl1.Controls.Add(this.tabPage1);
		this.tabControl1.Controls.Add(this.tabPage2);
		this.tabControl1.Controls.Add(this.tabPage3);
		this.tabControl1.Location = new System.Drawing.Point(12, 31);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(476, 364);
		this.tabControl1.TabIndex = 2;
		// 
		// tabPage1
		// 
		this.tabPage1.Controls.Add(this.dgvIndustries);
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(468, 338);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "行业板块";
		this.tabPage1.UseVisualStyleBackColor = true;
		// 
		// dgvIndustries
		// 
		this.dgvIndustries.AllowUserToAddRows = false;
		this.dgvIndustries.AllowUserToDeleteRows = false;
		this.dgvIndustries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvIndustries.Location = new System.Drawing.Point(3, 3);
		this.dgvIndustries.Name = "dgvIndustries";
		this.dgvIndustries.ReadOnly = true;
		this.dgvIndustries.RowHeadersVisible = false;
		this.dgvIndustries.RowTemplate.Height = 23;
		this.dgvIndustries.Size = new System.Drawing.Size(460, 330);
		this.dgvIndustries.TabIndex = 0;
	
		// 
		// tabPage2
		// 
		this.tabPage2.Controls.Add(this.dgvConcepts);
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(468, 338);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "概念板块";
		this.tabPage2.UseVisualStyleBackColor = true;
		// 
		// dgvConcepts
		// 
		this.dgvConcepts.AllowUserToAddRows = false;
		this.dgvConcepts.AllowUserToDeleteRows = false;
		this.dgvConcepts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvConcepts.Location = new System.Drawing.Point(3, 3);
		this.dgvConcepts.Name = "dgvConcepts";
		this.dgvConcepts.ReadOnly = true;
		this.dgvConcepts.RowHeadersVisible = false;
		this.dgvConcepts.RowTemplate.Height = 23;
		this.dgvConcepts.Size = new System.Drawing.Size(460, 330);
		this.dgvConcepts.TabIndex = 0;
	
		// 
		// tabPage3
		// 
		this.tabPage3.Controls.Add(this.dgvAreas);
		this.tabPage3.Location = new System.Drawing.Point(4, 22);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Size = new System.Drawing.Size(468, 338);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "地区板块";
		this.tabPage3.UseVisualStyleBackColor = true;
		// 
		// dgvAreas
		// 
		this.dgvAreas.AllowUserToAddRows = false;
		this.dgvAreas.AllowUserToDeleteRows = false;
		this.dgvAreas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dgvAreas.Location = new System.Drawing.Point(3, 3);
		this.dgvAreas.Name = "dgvAreas";
		this.dgvAreas.ReadOnly = true;
		this.dgvAreas.RowHeadersVisible = false;
		this.dgvAreas.RowTemplate.Height = 23;
		this.dgvAreas.Size = new System.Drawing.Size(460, 330);
		this.dgvAreas.TabIndex = 0;
	
		// 
		// MainForm
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(496, 448);
		
		this.Controls.Add(this.tabControl1);
	
		this.Name = "MainForm";
		this.Text = "板块监控";
		this.Load += new System.EventHandler(this.Form1_Load);
		this.tabControl1.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.dgvIndustries)).EndInit();
		this.tabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.dgvConcepts)).EndInit();
		this.tabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)(this.dgvAreas)).EndInit();
		this.ResumeLayout(false);
		this.PerformLayout();

	}

	private System.Windows.Forms.TabControl tabControl1;
	private System.Windows.Forms.TabPage tabPage1;
	private System.Windows.Forms.DataGridView dgvIndustries;
	private System.Windows.Forms.TabPage tabPage2;
	private System.Windows.Forms.DataGridView dgvConcepts;
	private System.Windows.Forms.TabPage tabPage3;
	private System.Windows.Forms.DataGridView dgvAreas;

}