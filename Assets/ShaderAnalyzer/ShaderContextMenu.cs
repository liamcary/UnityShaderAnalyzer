using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using VRCade.Editor.ShaderAnalysis;

namespace VRCade.Editor
{
	[InitializeOnLoad]
	public static class ShaderContextMenu
	{
		const string _prefix = "CONTEXT/Shader/";
		const string _compileShaderMenu = _prefix + nameof(CompileShader);
		const string _parseShaderMenu = _prefix + nameof(ParseShader);

		static ShaderContextMenu()
		{
		}

		[MenuItem(_compileShaderMenu)]
		static void CompileShader(MenuCommand command)
		{
			var shader = (Shader) command.context;
			ShaderAnalyzer.Compile(shader, ShaderCompilerPlatform.GLES3x);
		}

		[MenuItem(_parseShaderMenu)]
		static void ParseShader(MenuCommand command)
		{
			var shader = (Shader) command.context;
			ShaderAnalyzer.Parse(shader, ShaderCompilerPlatform.GLES3x);
		}
	}
}