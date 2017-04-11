namespace Noise.Model
{
	using UnityEngine;

	public class ValueNoise : LatticeNoise
	{
		public override string Name { get { return "Value Noise"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/ValueNoise"; } }

		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int ix0 = Mathf.FloorToInt(point.x);
			float t = point.x - ix0;
			ix0 &= HashMask;
			int ix1 = ix0 + 1;

			int h0 = NoiseUtils.PerlinHash[ix0];
			int h1 = NoiseUtils.PerlinHash[ix1];
			return NoiseUtils.Splerp(ref noiseStruct.splerp, h0, h1, t) * (1f / HashMask);
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int ix0 = Mathf.FloorToInt(point.x);
			int iy0 = Mathf.FloorToInt(point.y);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			ix0 &= HashMask;
			iy0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			int h0 = NoiseUtils.PerlinHash[ix0];
			int h1 = NoiseUtils.PerlinHash[ix1];
			int h00 = NoiseUtils.PerlinHash[h0 + iy0];
			int h01 = NoiseUtils.PerlinHash[h0 + iy1];
			int h10 = NoiseUtils.PerlinHash[h1 + iy0];
			int h11 = NoiseUtils.PerlinHash[h1 + iy1];

			return
				NoiseUtils.Splerp(ref noiseStruct.splerp, NoiseUtils.Splerp(ref noiseStruct.splerp, h00, h10, tx),
					NoiseUtils.Splerp(ref noiseStruct.splerp, h01, h11, tx), ty) * (1f / HashMask);
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int ix0 = Mathf.FloorToInt(point.x);
			int iy0 = Mathf.FloorToInt(point.y);
			int iz0 = Mathf.FloorToInt(point.z);
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			ix0 &= HashMask;
			iy0 &= HashMask;
			iz0 &= HashMask;
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;

			int h0 = NoiseUtils.PerlinHash[ix0];
			int h1 = NoiseUtils.PerlinHash[ix1];
			int h00 = NoiseUtils.PerlinHash[h0 + iy0];
			int h01 = NoiseUtils.PerlinHash[h0 + iy1];
			int h10 = NoiseUtils.PerlinHash[h1 + iy0];
			int h11 = NoiseUtils.PerlinHash[h1 + iy1];
			int h000 = NoiseUtils.PerlinHash[h00 + iz0];
			int h001 = NoiseUtils.PerlinHash[h00 + iz1];
			int h010 = NoiseUtils.PerlinHash[h01 + iz0];
			int h011 = NoiseUtils.PerlinHash[h01 + iz1];
			int h100 = NoiseUtils.PerlinHash[h10 + iz0];
			int h101 = NoiseUtils.PerlinHash[h10 + iz1];
			int h110 = NoiseUtils.PerlinHash[h11 + iz0];
			int h111 = NoiseUtils.PerlinHash[h11 + iz1];

			return NoiseUtils.Splerp(ref noiseStruct.splerp,
				       NoiseUtils.Splerp(ref noiseStruct.splerp, NoiseUtils.Splerp(ref noiseStruct.splerp, h000, h100, tx),
					       NoiseUtils.Splerp(ref noiseStruct.splerp, h010, h110, tx), ty),
				       NoiseUtils.Splerp(ref noiseStruct.splerp, NoiseUtils.Splerp(ref noiseStruct.splerp, h001, h101, tx),
					       NoiseUtils.Splerp(ref noiseStruct.splerp, h011, h111, tx), ty),
				       tz) * (1f / HashMask);
		}
	}
}