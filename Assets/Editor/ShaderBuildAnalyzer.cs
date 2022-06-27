using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;

namespace UnityShaderAnalyzer
{
	public class ShaderBuildAnalyzer : IPreprocessShaders, IPostprocessBuildWithReport
	{
		public int callbackOrder => 1;

		ShaderAnalyzerData _data;

		public void OnPostprocessBuild(BuildReport report)
		{
			if (_data != null) {
				string filename = $"Assets/ShaderVariants-{DateTime.Now:yy-MM-dd HH:mm:ss}.asset";
				AssetDatabase.CreateAsset(_data, filename);
			}

			_data = null;
		}

		public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
		{
			if (_data == null) {
				_data = ScriptableObject.CreateInstance<ShaderAnalyzerData>();
			}

			foreach (var d in data) {
				_data.AddVariant(shader, snippet, d);
			}
		}
	}
}