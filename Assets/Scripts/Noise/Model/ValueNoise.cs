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
			int i0X = Mathf.FloorToInt(point.x);
			float t = point.x - i0X;
			i0X &= HashMask;
			int i1X = i0X + 1;

			int h0 = NoiseUtils.PerlinHash[i0X];
			int h1 = NoiseUtils.PerlinHash[i1X];

			t = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t);
			return Mathf.Lerp(h0, h1, t) * (1f / HashMask);
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int i0X = Mathf.FloorToInt(point.x);
			int i0Y = Mathf.FloorToInt(point.y);
			var t0 = new Vector2(point.x - i0X, point.y - i0Y);
			i0X &= HashMask;
			i0Y &= HashMask;
			int i1X = i0X + 1;
			int i1Y = i0Y + 1;

			int h0 = NoiseUtils.PerlinHash[i0X];
			int h1 = NoiseUtils.PerlinHash[i1X];
			int h00 = NoiseUtils.PerlinHash[h0 + i0Y];
			int h01 = NoiseUtils.PerlinHash[h0 + i1Y];
			int h10 = NoiseUtils.PerlinHash[h1 + i0Y];
			int h11 = NoiseUtils.PerlinHash[h1 + i1Y];

			t0.x = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.x);
			t0.y = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.y);
			return
				Mathf.Lerp(Mathf.Lerp(h00, h10, t0.x),
					Mathf.Lerp(h01, h11, t0.x), t0.y) * (1f / HashMask);
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int i0X = Mathf.FloorToInt(point.x);
			int i0Y = Mathf.FloorToInt(point.y);
			int i0Z = Mathf.FloorToInt(point.z);
			var t0 = new Vector3(point.x - i0X, point.y - i0Y, point.z - i0Z);
			i0X &= HashMask;
			i0Y &= HashMask;
			i0Z &= HashMask;
			int i1X = i0X + 1;
			int i1Y = i0Y + 1;
			int i1Z = i0Z + 1;

			int h0 = NoiseUtils.PerlinHash[i0X];
			int h1 = NoiseUtils.PerlinHash[i1X];
			int h00 = NoiseUtils.PerlinHash[h0 + i0Y];
			int h01 = NoiseUtils.PerlinHash[h0 + i1Y];
			int h10 = NoiseUtils.PerlinHash[h1 + i0Y];
			int h11 = NoiseUtils.PerlinHash[h1 + i1Y];
			int h000 = NoiseUtils.PerlinHash[h00 + i0Z];
			int h001 = NoiseUtils.PerlinHash[h00 + i1Z];
			int h010 = NoiseUtils.PerlinHash[h01 + i0Z];
			int h011 = NoiseUtils.PerlinHash[h01 + i1Z];
			int h100 = NoiseUtils.PerlinHash[h10 + i0Z];
			int h101 = NoiseUtils.PerlinHash[h10 + i1Z];
			int h110 = NoiseUtils.PerlinHash[h11 + i0Z];
			int h111 = NoiseUtils.PerlinHash[h11 + i1Z];

			t0.x = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.x);
			t0.y = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.y);
			t0.z = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.z);
			return Mathf.Lerp(
					   Mathf.Lerp(Mathf.Lerp(h000, h100, t0.x),
					       Mathf.Lerp(h010, h110, t0.x), t0.y),
				       Mathf.Lerp(Mathf.Lerp(h001, h101, t0.x),
					       Mathf.Lerp(h011, h111, t0.x), t0.y),
					   t0.z) * (1f / HashMask);
		}
	}
}