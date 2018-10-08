using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Accord;
using Accord.Statistics.Models.Regression.Linear;
using ZedGraph;
public class GraphShowForm:Form
{
	public GraphShowForm()
	{
		InitializeComponent();
	}
	public void ShowGraph(double[] inputs,double[] outputs,
		double[] regInputs1,double[] regOutputs1,
		double[] regInputs2,double[] regOutputs2){
		GraphPane myPane = this.zedGraphControl1.GraphPane;
		myPane.CurveList.Clear();
		// Set the titles
		myPane.Title.IsVisible = false;
		myPane.Chart.Border.IsVisible = false;
		myPane.XAxis.Title.Text = "X";
		myPane.YAxis.Title.Text = "Y";
		myPane.XAxis.IsAxisSegmentVisible = true;
		myPane.YAxis.IsAxisSegmentVisible = true;
		myPane.XAxis.MinorGrid.IsVisible = false;
		myPane.YAxis.MinorGrid.IsVisible = false;
		myPane.XAxis.MinorTic.IsOpposite = false;
		myPane.XAxis.MajorTic.IsOpposite = false;
		myPane.YAxis.MinorTic.IsOpposite = false;
		myPane.YAxis.MajorTic.IsOpposite = false;
		myPane.XAxis.Scale.MinGrace = 0;
		myPane.XAxis.Scale.MaxGrace = 0;
		myPane.XAxis.Scale.Max = 50;
		//myPane.XAxis.Scale.Min = -10;
		myPane.YAxis.Scale.MinGrace = 0;
		myPane.YAxis.Scale.MaxGrace = 0;
		//myPane.YAxis.Scale.Min = -10;
		myPane.YAxis.Scale.Max = 50;
		
		LineItem myCurve;
		
		PointPairList list1 = new PointPairList(inputs,outputs);
		myCurve = myPane.AddCurve("points", list1, Color.Blue, SymbolType.Circle);
		myCurve.Line.IsVisible = false;
		myCurve.Symbol.Fill = new Fill(Color.Blue);
		if (regInputs1!=null&&regOutputs1!=null) {
			PointPairList list2 = new PointPairList(regInputs1, regOutputs1);
			myCurve = myPane.AddCurve("regression 1", list2, Color.Red, SymbolType.Circle);
			myCurve.Line.IsAntiAlias = true;
			myCurve.Line.IsVisible = true;
			myCurve.Symbol.IsVisible = false;
		}
		if (regInputs2!=null&&regOutputs2!=null) {
			PointPairList list3 = new PointPairList(regInputs2, regOutputs2);
			myCurve = myPane.AddCurve("regression 2", list3, Color.DarkGreen, SymbolType.Circle);
			myCurve.Line.IsAntiAlias = true;
			myCurve.Line.IsVisible = true;
			myCurve.Symbol.IsVisible = false;
		}

		this.zedGraphControl1.AxisChange();
		this.zedGraphControl1.Invalidate();
	}
	/// <summary>
	/// 必需的设计器变量。
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// 清理所有正在使用的资源。
	/// </summary>
	/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows 窗体设计器生成的代码

	/// <summary>
	/// 设计器支持所需的方法 - 不要修改
	/// 使用代码编辑器修改此方法的内容。
	/// </summary>
	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
		this.SuspendLayout();
		
		// 
		// zedGraphControl1
		// 
		this.zedGraphControl1.Location = new System.Drawing.Point(12, 12);
		this.zedGraphControl1.Name = "zedGraphControl1";
		this.zedGraphControl1.ScrollGrace = 0D;
		this.zedGraphControl1.ScrollMaxX = 0D;
		this.zedGraphControl1.ScrollMaxY = 0D;
		this.zedGraphControl1.ScrollMaxY2 = 0D;
		this.zedGraphControl1.ScrollMinX = 0D;
		this.zedGraphControl1.ScrollMinY = 0D;
		this.zedGraphControl1.ScrollMinY2 = 0D;
		this.zedGraphControl1.Size = new System.Drawing.Size(606, 304);
		this.zedGraphControl1.TabIndex = 1;
		this.zedGraphControl1.UseExtendedPrintDialog = true;
		// 
		// Form1
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(722, 328);
		this.Controls.Add(this.zedGraphControl1);
		this.Name = "Form1";
		this.Text = "Form1";
		this.ResumeLayout(false);

	}

	#endregion
	private ZedGraph.ZedGraphControl zedGraphControl1;
}