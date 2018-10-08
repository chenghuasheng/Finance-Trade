using System;
using Accord.Statistics.Models.Regression.Linear;
public class PatternRecognition{
	private PolynomialLeastSquares pls;
	private OrdinaryLeastSquares ols;
	private GraphShowForm showForm=null;
	public PatternRecognition(GraphShowForm showForm){
		this.showForm=showForm;
		this.pls=new PolynomialLeastSquares();
		this.pls.Degree=2;
		this.ols=new OrdinaryLeastSquares();
	}
	public RecognitionState Recognition(double[] inputs,double[] outputs){
		RecognitionState ret= new RecognitionState();
		if (inputs.Length>5){
			/*一次线性回归*/
			SimpleLinearRegression regression = this.ols.Learn(inputs, outputs,null);
			double k =regression.Slope;
			const double TAN45=1.0;
			const double TAN15=0.2679492;
			double k1=Math.Abs(k);
			if (k1>TAN45) ret.Slope=SlopeState.Steep;
			else if (k1>TAN15) ret.Slope=SlopeState.moderate;
			else ret.Slope=SlopeState.gentle;
			/*二次线性回归*/
			PolynomialRegression poly = this.pls.Learn(inputs, outputs,null);
			double a=poly.Weights[0];
			double b=poly.Weights[1];
			if (k>0&&a>0) ret.Shape=ShapeState.Rise;
			else if (k>0&&a<0) ret.Shape=ShapeState.FallAfterRise;
			else if (k<0&&a<0) ret.Shape=ShapeState.Fall;
			else if (k<0&&a>0) ret.Shape=ShapeState.RiseAfterFall;
			double last=inputs[inputs.Length-1];
			double s=2*a*last+b;
			double s1=Math.Abs(s);
			if (s1>TAN45) ret.Speed=SpeedState.Rapid;
			else if (s1>TAN15) ret.Speed=SpeedState.Steady;
			else ret.Speed=SpeedState.Slow;
			/*显示图形*/
			if (this.showForm!=null) {
				double[] outputs2=regression.Transform(inputs);
				double[] outputs3=poly.Transform(inputs);
				this.showForm.ShowGraph(inputs,outputs,inputs,outputs2,inputs,outputs3);
			}
			Console.WriteLine("k={0},a={1},b={2}",k,a,b);
		}
		return ret;
	}
}

public enum SlopeState{
	Undefined=0,
	Steep=1,//陡峭
	moderate,//适中
	gentle//平缓
}
public enum SpeedState{
	Undefined=0,
	Rapid=1,//急速
	Steady,//平稳
	Slow//缓慢
}
public enum ShapeState{
	Undefined=0,
	Rise=1,//上升
	FallAfterRise,//上升中回落
	Fall,//下落
	RiseAfterFall//下落中回升
}
public class RecognitionState{
	public SlopeState Slope;
	public SpeedState Speed;
	public ShapeState Shape;
}