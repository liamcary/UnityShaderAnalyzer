using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityShaderAnalyzer
{
	public class ShaderAnalyzerData : ScriptableObject
	{
		[SerializeField] List<ShaderVariantData> _shaderVariants = new List<ShaderVariantData>();

		public void AddVariant(Shader shader, ShaderSnippetData snippet, ShaderCompilerData variant)
		{
			_shaderVariants.Add(new ShaderVariantData(shader, snippet, variant));
		}

		public void AnalyzeAllText(ShaderCompilerPlatform? platform, GraphicsTier? graphicsTier, BuildTarget? buildTarget)
		{
			var variants = GetVariants(platform, graphicsTier, buildTarget);

			foreach (var variant in variants) {
				variant.ClearAnalysis();
				variant.GetTextAnalysis();
			}
		}

		public void AnalyzeAllJson(ShaderCompilerPlatform? platform, GraphicsTier? graphicsTier, BuildTarget? buildTarget)
		{
			var variants = GetVariants(platform, graphicsTier, buildTarget);

			foreach (var variant in variants) {
				variant.ClearAnalysis();
				variant.GetJsonAnalysis();
			}
		}

		public void ClearAllAnalysis()
		{
			foreach (var variant in _shaderVariants) {
				variant.ClearAnalysis();
			}
		}

		IEnumerable<ShaderVariantData> GetVariants(ShaderCompilerPlatform? platform, GraphicsTier? graphicsTier, BuildTarget? buildTarget)
		{
			var variants = _shaderVariants.AsEnumerable();

			if (platform.HasValue) {
				variants = variants.Where(v => v.CompilerPlatform == platform.Value);
			}

			if (graphicsTier.HasValue) {
				variants = variants.Where(v => v.GraphicsTier == graphicsTier.Value);
			}

			if (buildTarget.HasValue) {
				variants = variants.Where(v => v.BuildTarget == buildTarget.Value);
			}

			return variants;
		}
	}
}