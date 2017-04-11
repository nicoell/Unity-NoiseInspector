namespace Noise.Model
{
	using System;
	using UnityEngine;

	public abstract class Noise
	{
		public abstract string Name { get; }
		public abstract void Reset();
		public Shader NoiseShader { get { return Shader.Find(ShaderPath); } }
		protected abstract string ShaderPath { get; }
		private MaterialPropertyBlock m_noisePropertyBlock;
		protected MaterialPropertyBlock NoisePropertyBlock
		{
			get { return m_noisePropertyBlock ?? (m_noisePropertyBlock = new MaterialPropertyBlock()); }
			set { m_noisePropertyBlock = value; }
		}

		public float Value(Vector3 point, NoiseStruct noiseStruct)
		{
			switch (noiseStruct.renderType)
			{
				case RenderType.Render1D:
					return Value1D(point, noiseStruct) * noiseStruct.amplitude;
				case RenderType.Render2D:
					return Value2D(point, noiseStruct) * noiseStruct.amplitude;
				case RenderType.Render3D:
					return Value3D(point, noiseStruct) * noiseStruct.amplitude;
				default:
					throw new ArgumentOutOfRangeException("noiseStruct", noiseStruct.renderType, "Given RenderType not supported.");
			}
		}

		protected abstract float Value1D(Vector3 point, NoiseStruct noiseStruct);
		protected abstract float Value2D(Vector3 point, NoiseStruct noiseStruct);
		protected abstract float Value3D(Vector3 point, NoiseStruct noiseStruct);

		public virtual void UpdateNoise(Renderer renderer, NoiseStruct noiseStruct)
		{
			switch (noiseStruct.renderType)
			{
				case RenderType.Render1D:
					renderer.sharedMaterial.EnableKeyword("RENDERTYPE_1D");
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_2D");
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_3D");
					break;
				case RenderType.Render2D:
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_1D");
					renderer.sharedMaterial.EnableKeyword("RENDERTYPE_2D");
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_3D");
					break;
				case RenderType.Render3D:
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_1D");
					renderer.sharedMaterial.DisableKeyword("RENDERTYPE_2D");
					renderer.sharedMaterial.EnableKeyword("RENDERTYPE_3D");
					break;
				default:
					throw new ArgumentOutOfRangeException("noiseStruct", noiseStruct.renderType, "Given RenderType not supported.");
			}
			renderer.GetPropertyBlock(NoisePropertyBlock);

			UpdateNoisePropertyBlock(noiseStruct);

			renderer.SetPropertyBlock(NoisePropertyBlock);
		}

		protected virtual void UpdateNoisePropertyBlock(NoiseStruct noiseStruct)
		{
			NoisePropertyBlock.SetTexture("_Splerp", noiseStruct.splerpTexture);
			NoisePropertyBlock.SetTexture("_PerlinHash", NoiseUtils.PerlinHashTexture);
			NoisePropertyBlock.SetFloat("_Resolution", noiseStruct.resolution);
			NoisePropertyBlock.SetFloat("_Frequency", noiseStruct.frequency);
			NoisePropertyBlock.SetFloat("_Amplitude", noiseStruct.amplitude);
		}
	}
}