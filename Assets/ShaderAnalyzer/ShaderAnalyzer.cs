using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace VRCade.Editor.ShaderAnalysis
{
	public static class ShaderAnalyzer
	{
		const string _maliocPath = @"C:\Program Files\Arm\Arm Mobile Studio 2022.0\mali_offline_compiler\malioc.exe";

		const string _variantSeparator = "Keywords: ";
		const string _endif = "#endif";
		const string _vertexStart = "#ifdef VERTEX";
		const string _fragmentStart = "#ifdef FRAGMENT";

		public static void Compile(Shader shader, ShaderCompilerPlatform platform)
		{
			OpenCompiledShader(shader, platform);
		}

		public static void Parse(Shader shader, ShaderCompilerPlatform platform)
		{
			string shaderName = ShaderAnalyzerPathHelper.GetShaderName(shader);
			string analysisDirectory = ShaderAnalyzerPathHelper.GetAnalysisDirectory(shader, platform);

			// Create analyzer directory or clear past analysis
			if (Directory.Exists(analysisDirectory)) {
				foreach (string file in Directory.GetFiles(analysisDirectory)) {
					File.Delete(file);
				}
			} else {
				Directory.CreateDirectory(analysisDirectory);
			}

			string tempFilePath = ShaderAnalyzerPathHelper.GetCompiledShaderUnityTempPath(shader);
			string copyPath = ShaderAnalyzerPathHelper.GetCompiledShaderCopyFilePath(shader, ShaderCompilerPlatform.GLES3x);

			// Copy temp file to shader subfolder in analyzer directory
			File.Copy(tempFilePath, copyPath, true);

			// Split shader file text into blocks for each variant
			string compiledShader = File.ReadAllText(copyPath);

			int start = compiledShader.IndexOf(_variantSeparator);
			compiledShader = compiledShader.Substring(start);

			string[] variants = compiledShader.Split(new string[] { _variantSeparator }, StringSplitOptions.RemoveEmptyEntries);
			Debug.LogFormat("{0} variants found in shader {1}", variants.Length, shaderName);

			for (int i = 0; i < variants.Length; ++i) {
				// Parse keywords defining the shader variant
				int endOfFirstLine = variants[i].IndexOf('\n');
				if (endOfFirstLine == -1) {
					Debug.LogErrorFormat("Variant {0} has no end of line: {1}", i, variants[i]);
					continue;
				}

				string keywords = variants[i].Substring(0, endOfFirstLine).Trim().Replace(' ', ';');

				Debug.LogFormat("Variant {0} has keywords: {1}", i, keywords);

				// Find vertex method
				int vertStart = variants[i].IndexOf(_vertexStart) + _vertexStart.Length;
				int fragStart = variants[i].IndexOf(_fragmentStart) + _fragmentStart.Length;

				int vertEnd = variants[i].LastIndexOf(_endif, fragStart);
				int fragEnd = variants[i].LastIndexOf(_endif);

				Debug.LogFormat("Vert from {0} to {1}, Frag from {2} to {3}", vertStart, vertEnd, fragStart, fragEnd);

				if (fragEnd == -1) {
					fragEnd = variants[i].Length - 1;
				}

				string vertex = variants[i][vertStart..vertEnd].Trim();
				string fragment = variants[i][fragStart..fragEnd].Trim();

				string keywordsPath = ShaderAnalyzerPathHelper.GetVariantKeywordsFilePath(shader, platform, i);
				File.WriteAllText(keywordsPath, keywords);

				string vertexShaderPath = ShaderAnalyzerPathHelper.GetVertexShaderFilePath(shader, platform, i);
				File.WriteAllText(vertexShaderPath, vertex);

				string fragmentShaderPath = ShaderAnalyzerPathHelper.GetFragmentShaderFilePath(shader, platform, i);
				File.WriteAllText(fragmentShaderPath, fragment);

				string vertexAnalysisPath = ShaderAnalyzerPathHelper.GetVertexShaderAnalysisFilePath(shader, platform, i);
				Analyze(vertexShaderPath, vertexAnalysisPath);

				string fragmentAnalysisPath = ShaderAnalyzerPathHelper.GetFragmentShaderAnalysisFilePath(shader, platform, i);
				Analyze(fragmentShaderPath, fragmentAnalysisPath);
			}
		}

		static void Analyze(string shaderPath, string outputPath)
		{
			string args = $"\"{shaderPath}\" --format json";

			Debug.LogFormat("Running malioc with args: {0}", args);

			var processStartInfo = new ProcessStartInfo(_maliocPath, args) {
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

			var process = Process.Start(processStartInfo);

			string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			Debug.LogFormat("Writing {0} characters to {1}", output.Length, outputPath);

			File.WriteAllText(outputPath, output);
		}

		static void OpenCompiledShader(Shader shader, ShaderCompilerPlatform platform)
		{
			var type = typeof(ShaderUtil);
			var method = type.GetMethod("OpenCompiledShader", BindingFlags.NonPublic | BindingFlags.Static);

			int mask = 1 << (int) platform;

#if UNITY_2021_2_OR_NEWER
			// Signature 2020.2+: extern internal static void OpenCompiledShader(Shader shader, int mode, int externPlatformsMask, bool includeAllVariants, bool preprocessOnly, bool stripLineDirectives); 
			method.Invoke(null, new object[] { shader, (int) PlatformModes.Custom, mask, false, false, false });
#elif UNITY_2020_1_OR_NEWER
			// Signature 2020.1: extern internal static void OpenCompiledShader(Shader shader, int mode, int externPlatformsMask, bool includeAllVariants, bool preprocessOnly); 
			method.Invoke(null, new object[] { shader, (int) PlatformModes.Custom, mask, false, false, });
#else 
			// Signature 2019.4: extern internal static void OpenCompiledShader(Shader shader, int mode, int externPlatformsMask, bool includeAllVariants); 
			method.Invoke(null, new object[] { shader, (int) PlatformModes.Custom, mask, false });
#endif
		}

		enum PlatformModes
		{
			CurrentGraphicsDevice,
			CurrentBuildPlatform,
			AllPlatforms,
			Custom
		}
	}
}