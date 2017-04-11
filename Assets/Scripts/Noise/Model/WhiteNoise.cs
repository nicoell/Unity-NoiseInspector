namespace Noise.Model
{
	using UnityEngine;
	using Random = System.Random;

	public class WhiteNoise : Noise
	{
		private Random m_random;
		private readonly int m_seed;

		public WhiteNoise(int seed = 0)
		{
			m_seed = seed;
			m_random = new Random(m_seed);
		}

		public override string Name { get { return "WhiteNoise"; } }
		public override void Reset() { m_random = new Random(m_seed); }
		protected override string ShaderPath { get { return "Noise/WhiteNoise"; } }
		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct) { return (float) m_random.NextDouble(); }
		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct) { return (float) m_random.NextDouble(); }
		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct) { return (float) m_random.NextDouble(); }
	}
}