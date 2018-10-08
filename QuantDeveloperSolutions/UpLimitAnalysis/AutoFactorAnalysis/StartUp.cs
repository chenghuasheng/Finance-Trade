using System;

public class StartUp
{
	static void Main(string[] args)
	{
		// TO DO: Add your code here
		Console.WriteLine("因素分析：");
		Factor factor=new Factor();
		factor.Type=ConditionType.Field;
		//factor.Field="turnoverrate";
		//factor.Field="voldivlast5avgvol";
		//factor.Field="bidsizedivvol";
		//factor.Field="flowashare";
		factor.Type=ConditionType.Where;
		factor.WhereExp="this.open==this.close";
		
		factor.RelationExp="<=";
		factor.MinVal=0.01;
		factor.MaxVal=1;
		factor.Step=0.01;
		Result result=new Result();
		result.Type=ConditionType.Where;
		result.WhereExp="this.nextopen<this.nextclose|| (this.nextopen==this.nextclose&&this.nextlow<this.nexthigh)";
		Analysis analysis=new Analysis(factor,result);
		analysis.BeginDate=new DateTime(2017,1,1);
		analysis.EndDate=new DateTime(2017,4,1);
		analysis.Dowith();
		Console.WriteLine("除去因素：");
		factor.Type=ConditionType.Where;
		factor.WhereExp="1";
		analysis=new Analysis(factor,result);
		analysis.BeginDate=new DateTime(2017,1,1);
		analysis.EndDate=new DateTime(2017,4,1);
		analysis.Dowith();
	}
}