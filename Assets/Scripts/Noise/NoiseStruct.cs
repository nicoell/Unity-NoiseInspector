namespace Noise
{
	using UnityEngine;

	public enum RenderType
	{
		Render1D,
		Render2D,
		Render3D
	}

	public enum Range
	{
		OneAroundZero, OneGreaterZero
	}

	public struct NoiseStruct
	{
		public Model.Noise noiseModel;
		public AnimationCurve splerp;
		public Texture2D splerpTexture;
		public Range range;
		public RenderType renderType;
		public int resolution;
		public float frequency;
		public float amplitude;
		public Transform worldTransform;

		public NoiseStruct(
			Model.Noise noiseModel,
			AnimationCurve splerp,
			Texture2D splerpTexture,
			Range range = Range.OneAroundZero,
			RenderType renderType = RenderType.Render2D,
			int resolution = 512,
			float frequency = 1.0f,
			float amplitude = 1.0f,
			Transform worldTransform = null
		)
		{
			this.noiseModel = noiseModel;
			this.splerp = splerp;
			this.splerpTexture = splerpTexture;
			this.range = range;
			this.renderType = renderType;
			this.resolution = resolution;
			this.frequency = frequency;
			this.amplitude = amplitude;
			this.worldTransform = worldTransform;
		}
	}
}