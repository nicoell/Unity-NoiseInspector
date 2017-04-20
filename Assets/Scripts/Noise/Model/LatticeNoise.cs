namespace Noise.Model
{
	using System;
	using UnityEngine;

	public class LatticeNoise : Noise
	{
		protected const int HashMask = 255;

		public override string Name { get { return "LatticeNoise"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/LatticeNoise"; } }

		protected override Range Range { get { return Range.OneGreaterZero; } }

		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points
			int ix = Mathf.FloorToInt(point.x);
			ix &= HashMask;
			return NoiseUtils.PerlinHash[ix] * (1f / HashMask);
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points
			int ix = Mathf.FloorToInt(point.x);
			int iy = Mathf.FloorToInt(point.y);
			ix &= HashMask;
			iy &= HashMask;
			return NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[ix] + iy] * (1f / HashMask);
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points
			int ix = Mathf.FloorToInt(point.x);
			int iy = Mathf.FloorToInt(point.y);
			int iz = Mathf.FloorToInt(point.z);
			ix &= HashMask;
			iy &= HashMask;
			iz &= HashMask;
			return NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[ix] + iy] + iz] * (1f / HashMask);
		}
	}
}