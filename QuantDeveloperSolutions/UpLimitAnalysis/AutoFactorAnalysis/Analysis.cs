using System;
using MongoDB.Bson;
using MongoDB.Driver;

public class Analysis{
	private Factor factor;
	private Result result;
	private DateTime beginDate=new DateTime(1970,1,1);
	public DateTime BeginDate{
		get { return this.beginDate;}
		set { this.beginDate=value;}
	}
	private DateTime endDate=new DateTime(2999,12,30);
	public DateTime EndDate{
		get { return this.endDate;}
		set { this.endDate=value;}
	}
	public Analysis(Factor factor,Result result){
		this.factor=factor;
		this.result=result;
	}
	public void Dowith(){
		MongoClient client = new MongoClient("mongodb://localhost:27017");
		MongoServer server = client.GetServer();
		MongoDatabase database = server.GetDatabase("finance");
		
		MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("UpLimitAnalysis");
		
		BsonElement[] eleArray = new BsonElement[3];
		BsonDocument temp=new BsonDocument(new BsonElement[] {new BsonElement("$gte",beginDate.ToString("yyyy-MM-dd")),
			new BsonElement("$lt",endDate.ToString("yyyy-MM-dd")) });	
		eleArray[0]=new BsonElement("date",temp);
		QueryDocument query;
		if (this.factor.Type==ConditionType.Field){
			this.factor.Val=this.factor.MinVal;
			do {
				eleArray[1]=this.factor.ParseField();
				eleArray[2]=null;
				query = new QueryDocument(eleArray);
				long c1=collection.Count(query);
				eleArray[2]=this.result.Parse();
				query = new QueryDocument(eleArray);
				long c2=collection.Count(query);
				Console.WriteLine("Factor[{0}],Resule[All],Count={1}",
					this.factor.ToString(),c1);
				Console.WriteLine("Factor[{0}],Resule[{1}],Count={2}",
					this.factor.ToString(),this.result.ToString(),c2);
				if (c1<=0) break;
				Console.WriteLine("Rate={0}",(double)c2/c1);
				this.factor.Val+=this.factor.Step;
			}while(this.factor.Val<=this.factor.MaxVal);	
		}else if (this.factor.Type==ConditionType.Where){
			eleArray[1]=this.factor.ParseWhere();
			eleArray[2]=null;
			query = new QueryDocument(eleArray);
			long c1=collection.Count(query);
			if (this.result.Type==ConditionType.Where){//由于不能同时有两个$where查询子句
				string oldExp=this.factor.WhereExp;
				this.factor.WhereExp=this.factor.WhereExp+" && "+this.result.WhereExp;
				eleArray[1]=this.factor.ParseWhere();
				this.factor.WhereExp=oldExp;
			}else {
				eleArray[2]=this.result.Parse();
			}
			query = new QueryDocument(eleArray);
			long c2=collection.Count(query);
			Console.WriteLine("Factor[{0}],Result[All],Count={1}",
				this.factor.ToString(),c1);
			Console.WriteLine("Factor[{0}],Result[{1}],Count={2}",
				this.factor.ToString(),this.result.ToString(),c2);
			Console.WriteLine("Rate={0}",(double)c2/c1);
		}
	}
	
}