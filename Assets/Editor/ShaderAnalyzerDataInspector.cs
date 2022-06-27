using UnityEditor;
using UnityEngine;

namespace UnityShaderAnalyzer
{
	[CustomEditor(typeof(ShaderAnalyzerData))]
	public class ShaderAnalyzerDataInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button("Generate Android Analyses - Text")) {
				var data = (ShaderAnalyzerData) target;
				data.AnalyzeAllText(UnityEditor.Rendering.ShaderCompilerPlatform.GLES3x, UnityEngine.Rendering.GraphicsTier.Tier1, BuildTarget.Android);
			}

			if (GUILayout.Button("Generate Android Analyses - Json")) {
				var data = (ShaderAnalyzerData) target;
				data.AnalyzeAllJson(UnityEditor.Rendering.ShaderCompilerPlatform.GLES3x, UnityEngine.Rendering.GraphicsTier.Tier1, BuildTarget.Android);
			}

			if (GUILayout.Button("Clear Analyses")) {
				var data = (ShaderAnalyzerData) target;
				data.ClearAllAnalysis();
			}
		}
	}
}