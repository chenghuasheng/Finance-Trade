using System;
using MongoDB.Bson;
using MongoDB.Driver;

public class Result{
	private ConditionType type;
	public ConditionType Type{
		get { return this.type;}
		set { this.type=value;}
	}
	private string field;
	public string Field{
		get { return this.field;}
		set { this.field=value;}
	}
	private string relationExp;
	public string RelationExp{
		get { return this.relationExp;}
		set { this.relationExp=value;}
	}
	private double val;
	public double Val{
		get { return this.val;}
		set { this.val=value;}
	}

	private string whereExp;
	public string WhereExp{
		get { return this.whereExp;}
		set { this.whereExp=value;}
	}
	public Result(){
		this.type=ConditionType.Field;
		this.relationExp=">=";
	}
	public BsonElement ParseField(){
		if (this.relationExp=="="){
			BsonElement ele=new BsonElement(this.field,this.val);
			return ele;
		}
		string rela;
		switch(this.relationExp){
			case ">":
				rela="$gt";
				break;
			case ">=":
				rela="$gte";
				break;
			case "<":
				rela="$lt";
				break;
			case "<=":
				rela="$lte";
				break;
			case "!=":
				rela="$ne";
				break;
			default:
				rela="$e";
				break;
		}
		BsonDocument doc=new BsonDocument(rela,this.val);
		BsonElement ele1=new BsonElement(this.field,doc);
		return ele1;
	}
	public BsonElement ParseWhere(){
		return new BsonElement("$where",this.whereExp);
	}
	public BsonElement Parse(){
		switch(this.type){
			case ConditionType.Field:
				return this.ParseField();
			case ConditionType.Where:
				return this.ParseWhere();
		}
		return null;
	}
	public override string ToString(){
		switch(this.type){
			case ConditionType.Field:
				return string.Format("field:{0},relation:{1},val:{2}"
					,this.field,this.relationExp,this.val);
			case ConditionType.Where:
				return string.Format("where:{0}",this.whereExp);
		}
		return null;	
	}
}

public enum ConditionType{
	Field,
	Where
}