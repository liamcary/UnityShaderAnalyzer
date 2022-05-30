using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace VRCade.Editor.ShaderAnalysis
{
	public static class ShaderAnalyzerPathHelper
	{
		public static string GetShaderName(Shader shader)
		{
			return shader.name.Replace('/', '-');
		}

		public static string GetUnityTempDirectory()
		{
			string projectRoot = Directory.GetParent(Application.dataPath).ToString();
			return Path.Combine(projectRoot, "Temp");
		}

		public static string GetUnityTempDirectory(Shader shader)
		{
			string directory = GetUnityTempDirectory();
			string subfolder = GetShaderName(shader);

			return Path.Combine(directory, subfolder);
		}

		public static string GetAnalysisDirectory()
		{
			string projectRoot = Directory.GetParent(Application.dataPath).ToString();
			return Path.Combine(projectRoot, "ShaderAnalyzerCache");
		}

		public static string GetAnalysisDirectory(Shader shader, ShaderCompilerPlatform platform)
		{
			string directory = GetAnalysisDirectory();
			string subfolder = GetShaderName(shader);

			return Path.Combine(directory, subfolder, platform.ToString());
		}

		public static string GetCompiledShaderUnityTempPath(Shader shader)
		{
			string directory = GetUnityTempDirectory();
			string fileName = $"Compiled-{GetShaderName(shader)}.shader";

			return Path.Combine(directory, fileName);
		}

		public static string GetCompiledShaderCopyFilePath(Shader shader, ShaderCompilerPlatform platform)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"Compiled-{GetShaderName(shader)}-{platform}.shader";

			return Path.Combine(directory, fileName);
		}

		public static string GetVariantKeywordsFilePath(Shader shader, ShaderCompilerPlatform platform, int variantIndex)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"{variantIndex:D3}_keywords.txt";

			return Path.Combine(directory, fileName);
		}

		public static string GetVertexShaderFilePath(Shader shader, ShaderCompilerPlatform platform, int variantIndex)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"{variantIndex:D3}_vert.vert";

			return Path.Combine(directory, fileName);
		}

		public static string GetVertexShaderAnalysisFilePath(Shader shader, ShaderCompilerPlatform platform, int variantIndex)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"{variantIndex:D3}_vert_analysis.json";

			return Path.Combine(directory, fileName);
		}

		public static string GetFragmentShaderFilePath(Shader shader, ShaderCompilerPlatform platform, int variantIndex)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"{variantIndex:D3}_frag.frag";

			return Path.Combine(directory, fileName);
		}

		public static string GetFragmentShaderAnalysisFilePath(Shader shader, ShaderCompilerPlatform platform, int variantIndex)
		{
			string directory = GetAnalysisDirectory(shader, platform);
			string fileName = $"{variantIndex:D3}_frag_analysis.json";

			return Path.Combine(directory, fileName);
		}
	}
}