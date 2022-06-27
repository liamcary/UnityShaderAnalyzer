using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace UnityShaderAnalyzer
{
	public static class MaliOfflineCompiler
	{
		const string _maliocPath = @"C:\Program Files\Arm\Arm Mobile Studio 2022.0\mali_offline_compiler\malioc.exe";

		public static string GetJsonAnalysis(string filePath)
		{
			string args = $"\"{filePath}\" --format json";

			return RunMalioc(args);
		}

		public static string GetTextAnalysis(string filePath)
		{
			return RunMalioc(filePath);
		}

		static string RunMalioc(string args)
		{
			Debug.LogFormat("Running malioc with args: {0}", args);

			var processStartInfo = new ProcessStartInfo(_maliocPath, args) {
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			var process = Process.Start(processStartInfo);

			string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			Debug.LogFormat("Received {0} characters back from malioc", output.Length);

			return output;
		}
	}
}