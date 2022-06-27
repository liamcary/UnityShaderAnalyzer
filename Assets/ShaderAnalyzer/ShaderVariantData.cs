using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityShaderAnalyzer
{
	[Serializable]
	public class ShaderVariantData
	{
		public Shader Shader => _shader;
		public uint SubshaderIndex => _subshaderIndex;
		public uint PassIndex => _passIndex;
		public ShaderType ShaderType => _shaderType;
		public ShaderCompilerPlatform CompilerPlatform => _compilerPlatform;
		public GraphicsTier GraphicsTier => _graphicsTier;
		public BuildTarget BuildTarget => _buildTarget;
		public string[] ShaderKeywords => _shaderKeywords;
		public string AnalysisJson => _analysisJson;
		public string AnalysisText => _analysisText;

		[SerializeField] Shader _shader;
		[SerializeField] uint _subshaderIndex;
		[SerializeField] uint _passIndex;
		[SerializeField] string _passName;
		[SerializeField] PassType _passType;
		[SerializeField] ShaderType _shaderType;
		[SerializeField] GraphicsTier _graphicsTier;
		[SerializeField] ShaderCompilerPlatform _compilerPlatform;
		[SerializeField] BuildTarget _buildTarget;
		[SerializeField] string[] _shaderKeywords;
		[SerializeField] string _shaderKeywordsCombined;
		[SerializeField] string _shaderRequirementsCombined;
		[SerializeField] string _compiledCode;
		[SerializeField] string _analysisText;
		[SerializeField] string _analysisJson;

		public ShaderVariantData(Shader shader, ShaderSnippetData snippet, ShaderCompilerData variant)
		{
			_shader = shader;
			_subshaderIndex = snippet.pass.SubshaderIndex;
			_passIndex = snippet.pass.PassIndex;
			_passName = snippet.passName;
			_passType = snippet.passType;
			_shaderType = snippet.shaderType;
			_graphicsTier = variant.graphicsTier;
			_compilerPlatform = variant.shaderCompilerPlatform;
			_buildTarget = variant.buildTarget;
			_shaderKeywords = variant.shaderKeywordSet.GetShaderKeywords().Select(sk => sk.name).ToArray();
			_shaderKeywordsCombined = string.Join(';', _shaderKeywords);

			var enumValues = Enum.GetValues(typeof(ShaderRequirements));
			foreach (object value in enumValues) {
				if (((long) variant.shaderRequirements & (long) value) != 0) {
					_shaderRequirementsCombined += value.ToString() + ";";
				}
			}
		}

		public string GetJsonAnalysis()
		{
			if (TryGetFilePath(out string path) && TryCompileCode() && TryWriteCompiledCodeToFile(path)) {
				_analysisJson = MaliOfflineCompiler.GetJsonAnalysis(path);
			} else {
				_analysisJson = null;
			}

			return _analysisJson;
		}

		public string GetTextAnalysis()
		{
			if (TryGetFilePath(out string path) && TryCompileCode() && TryWriteCompiledCodeToFile(path)) {
				_analysisText = MaliOfflineCompiler.GetTextAnalysis(path);
			} else {
				_analysisText = null;
			}

			return _analysisText;
		}

		public void ClearAnalysis()
		{
			_compiledCode = null;
			_analysisJson = null;
			_analysisText = null;
		}

		bool TryGetFilePath(out string path)
		{
			if (ShaderType == ShaderType.Vertex) {
				path = $"{Guid.NewGuid()}.vert";
				return true;
			}

			if (ShaderType == ShaderType.Fragment) {
				path = $"{Guid.NewGuid()}.frag";
				return true;
			}

			path = null;
			return false;
		}

		bool TryCompileCode()
		{
			if (!string.IsNullOrEmpty(_compiledCode)) {
				return true;
			}

			if (_shaderType != ShaderType.Vertex && _shaderType != ShaderType.Fragment) {
				Debug.LogFormat("Can't compile ShaderType {0}", _shaderType.ToString());
				return false;
			}

			var shaderData = ShaderUtil.GetShaderData(_shader);
			var subshader = shaderData.GetSubshader((int) _subshaderIndex);
			var pass = subshader.GetPass((int) _passIndex);

			if (pass == null) {
				Debug.LogFormat("Failed to get pass {0}", _passIndex);
				return false;
			}

			var variantCompileInfo = pass.CompileVariant(_shaderType, _shaderKeywords, _compilerPlatform, _buildTarget, _graphicsTier);
			if (!variantCompileInfo.Success) {
				Debug.LogError("Failed to compile shader variant");

				foreach (var message in variantCompileInfo.Messages) {
					Debug.LogErrorFormat("{0}: {1}. {2}", message.severity, message.message, message.messageDetails);
				}

				return false;
			}

			string code = Encoding.ASCII.GetString(variantCompileInfo.ShaderData).Trim();
			code = code.Replace("#version 310 es", "")
					.Replace("#version 300 es", "")
					.Insert(0, "#version 310 es\n");

			int fragmentStart = code.IndexOf("#ifdef FRAGMENT");
			if (fragmentStart == -1) {
				Debug.LogFormat("Failed to find #ifdef FRAGMENT");
				return false;
			}

			// Take only vertex method OR fragment method
			if (_shaderType == ShaderType.Vertex) {
				code = code.Substring(0, fragmentStart);
			} else if (_shaderType == ShaderType.Fragment) {
				code = code.Substring(fragmentStart);
			}

			// Remove #ifdef VERTEX/FRAGMENT and #endif
			int closingBrace = code.LastIndexOf('}');
			if (closingBrace == -1) {
				Debug.LogFormat("Failed to find last closing brace in code");
			} else {
				code = code.Substring(0, closingBrace + 1).Replace("#ifdef VERTEX", "").Replace("#ifdef FRAGMENT", "");
			}

			_compiledCode = code;
			return true;
		}

		bool TryWriteCompiledCodeToFile(string path)
		{
			if (string.IsNullOrEmpty(_compiledCode)) {
				return false;
			}

			try {
				File.WriteAllText(path, _compiledCode);
			} catch (Exception e) {
				Debug.LogException(e);
				return false;
			}

			return true;
		}
	}
}