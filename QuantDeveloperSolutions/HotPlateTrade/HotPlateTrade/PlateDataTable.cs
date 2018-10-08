using System;
using System.Data;


class PlateDataTable:DataTable
{
	public PlateDataTable()
	{
		this.Columns.Add(new DataColumn("ID", typeof(int)));
		this.Columns.Add(new DataColumn("Name", typeof(string)));
		this.Columns.Add(new DataColumn("UpLimitCount", typeof(int)));
		this.Columns.Add(new DataColumn("FivePercentCount", typeof(int)));
		this.Columns.Add(new DataColumn("UpCount", typeof(int)));
		this.Columns.Add(new DataColumn("ActiveCount",typeof(int)));
		this.Columns.Add(new DataColumn("Weight", typeof(float)));
	}
}
