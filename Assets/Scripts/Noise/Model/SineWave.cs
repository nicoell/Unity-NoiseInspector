namespace Noise.Model
{
	using UnityEngine;
	using Random = System.Random;

	public class SineWave : Noise
	{
		public SineWave(){}

		public override string Name { get { return "SineWave"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/SineWave"; } }
		protected override Range Range { get { return Range.OneAroundZero; } }
		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
        {
            point *= 2 * Mathf.PI * noiseStruct.frequency;
            return Mathf.Sin(point.x);
        }
		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
        {

            point *= noiseStruct.frequency;
            return Mathf.Sin(point.x) * Mathf.Sin(point.y);
        }
		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
        {
            point *= noiseStruct.frequency;
            return Mathf.Sin(point.x) * Mathf.Sin(point.y) * Mathf.Sin(point.z);
        }
	}
}