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
		protected abstract Range Range { get; }

		public float Value(Vector3 point, NoiseStruct noiseStruct)
		{
			float value;
			switch (noiseStruct.renderType)
			{
				case RenderType.Render1D:
					value = Value1D(point, noiseStruct);
					break;
				case RenderType.Render2D:
					value = Value2D(point, noiseStruct);
					break;
				case RenderType.Render3D:
					value = Value3D(point, noiseStruct);
					break;
				default:
					throw new ArgumentOutOfRangeException("noiseStruct", noiseStruct.renderType, "Given RenderType not supported.");
			}
			if (noiseStruct.range != Range)
			{
				switch (noiseStruct.range) {
					case Range.OneGreaterZero:
						//Convert to OneGreaterZero
						value = (value / 2f) + 0.5f;
						break;
					case Range.OneAroundZero:
						//Convert to OneAroundZero
						value = (value - 0.5f) * 2f;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return value * noiseStruct.amplitude;
		}

		protected abstract float Value1D(Vector3 point, NoiseStruct noiseStruct);
		protected abstract float Value2D(Vector3 point, NoiseStruct noiseStruct);
		protected abstract float Value3D(Vector3 point, NoiseStruct noiseStruct);

		public virtual void UpdateNoise(Renderer renderer, NoiseStruct noiseStruct)
		{
			//TODO: Seperate this and only call when needed
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
			if (noiseStruct.range == Range)
			{
				renderer.sharedMaterial.DisableKeyword("RANGE_ADJUST");
				renderer.sharedMaterial.EnableKeyword("RANGE_NOADJUST");
			} else
			{
				renderer.sharedMaterial.EnableKeyword("RANGE_ADJUST");
				renderer.sharedMaterial.DisableKeyword("RANGE_NOADJUST");
			}
			renderer.GetPropertyBlock(NoisePropertyBlock);

			UpdateNoisePropertyBlock(noiseStruct);

			renderer.SetPropertyBlock(NoisePropertyBlock);
		}

		protected virtual void UpdateNoisePropertyBlock(NoiseStruct noiseStruct)
		{
			NoisePropertyBlock.SetTexture("_Splerp", noiseStruct.splerpTexture);
			NoisePropertyBlock.SetTexture("_PerlinHash", NoiseUtils.PerlinHashTexture);
			// TODO: worldTransform does not contain proper object position... why?
			NoisePropertyBlock.SetMatrix("_WorldTransform",
				noiseStruct.worldTransform ? noiseStruct.worldTransform.localToWorldMatrix : Matrix4x4.identity);
			NoisePropertyBlock.SetFloat("_Resolution", noiseStruct.resolution);
			NoisePropertyBlock.SetFloat("_Frequency", noiseStruct.frequency);
			NoisePropertyBlock.SetFloat("_Amplitude", noiseStruct.amplitude);
		}
	}
}