using System;
using System.Diagnostics;

public static class CMDAgent{
	public static void RunPythonScript(string cmd,string args){
		ProcessStartInfo startInfo=new ProcessStartInfo();
		startInfo.FileName=@"python.exe";
		startInfo.Arguments=string.Format("{0} {1}",cmd,args);
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardOutput = true;
		startInfo.RedirectStandardInput = true;
		startInfo.RedirectStandardError = true;
		startInfo.CreateNoWindow = true;
		using(Process process=Process.Start(startInfo)){
			process.BeginOutputReadLine(); 
			process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
			process.WaitForExit(); 
		}
	}
	private static void OutputDataReceived(object sender, DataReceivedEventArgs e){
		Console.WriteLine(e.Data);
	}
}