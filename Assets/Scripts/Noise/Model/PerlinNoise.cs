namespace Noise.Model
{
	using System;
	using UnityEngine;

	public class PerlinNoise : Noise
	{
		public override string Name { get { return "PerlinNoise"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/PerlinNoise"; } }

		protected override Range Range { get { return Range.OneAroundZero; } }

		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int i0X = Mathf.FloorToInt(point.x);
			float t0 = point.x - i0X;
			float t1 = t0 - 1f;
			i0X &= NoiseUtils.HashMask;
			int i1X = i0X + 1;

			int h0 = NoiseUtils.PerlinHash[i0X];
			int h1 = NoiseUtils.PerlinHash[i1X];

			float g0 = NoiseUtils.Gradients1D[h0 & NoiseUtils.GradientsMask1D];
			float g1 = NoiseUtils.Gradients1D[h1 & NoiseUtils.GradientsMask1D];

			float v0 = g0 * t0;
			float v1 = g1 * t1;

			//t0 = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0);
			t0 = NoiseUtils.SmootherStep(t0);
			return Mathf.Lerp(v0, v1, t0) * 2f;
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int i0X = Mathf.FloorToInt(point.x);
			int i0Y = Mathf.FloorToInt(point.y);
			var t0 = new Vector2(point.x - i0X, point.y - i0Y);
			var t1 = new Vector2(t0.x - 1f, t0.y - 1f);
			i0X &= NoiseUtils.HashMask;
			i0Y &= NoiseUtils.HashMask;
			int i1X = i0X + 1;
			int i1Y = i0Y + 1;

			int h0 = NoiseUtils.PerlinHash[i0X];
			int h1 = NoiseUtils.PerlinHash[i1X];
			int h00 = NoiseUtils.PerlinHash[h0 + i0Y];
			int h01 = NoiseUtils.PerlinHash[h0 + i1Y];
			int h10 = NoiseUtils.PerlinHash[h1 + i0Y];
			int h11 = NoiseUtils.PerlinHash[h1 + i1Y];

			var g00 = NoiseUtils.Gradients2D[h00 & NoiseUtils.GradientsMask2D];
			var g01 = NoiseUtils.Gradients2D[h01 & NoiseUtils.GradientsMask2D];
			var g10 = NoiseUtils.Gradients2D[h10 & NoiseUtils.GradientsMask2D];
			var g11 = NoiseUtils.Gradients2D[h11 & NoiseUtils.GradientsMask2D];

			float v00 = Vector2.Dot(g00, new Vector2(t0.x, t0.y));
			float v01 = Vector2.Dot(g01, new Vector2(t0.x, t1.y));
			float v10 = Vector2.Dot(g10, new Vector2(t1.x, t0.y));
			float v11 = Vector2.Dot(g11, new Vector2(t1.x, t1.y));
			
			//t0.x = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.x);
			//t0.y = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.y);
			t0.x = NoiseUtils.SmootherStep(t0.x);
			t0.y = NoiseUtils.SmootherStep(t0.y);
			return
				Mathf.Lerp(Mathf.Lerp(v00, v10, t0.x),
					Mathf.Lerp(v01, v11, t0.x), t0.y) * NoiseUtils.Sqr2;
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
			//Calculate Integer Lattice Points and interpolants t
			int i0X = Mathf.FloorToInt(point.x);
			int i0Y = Mathf.FloorToInt(point.y);
			int i0Z = Mathf.FloorToInt(point.z);
			//Get point relative to wavelet center
			var t0 = new Vector3(point.x - i0X, point.y - i0Y, point.z - i0Z);
			var t1 = new Vector3(t0.x - 1f, t0.y - 1f, t0.z -1f);
			i0X &= NoiseUtils.HashMask;
			i0Y &= NoiseUtils.HashMask;
			i0Z &= NoiseUtils.HashMask;
			int i1X = i0X + 1;
			int i1Y = i0Y + 1;
			int i1Z = i0Z + 1;

			// Fold x,y,z using permuation table
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

			var g000 = NoiseUtils.Gradients3D[h000 & NoiseUtils.GradientsMask3D];
			var g001 = NoiseUtils.Gradients3D[h001 & NoiseUtils.GradientsMask3D];
			var g010 = NoiseUtils.Gradients3D[h010 & NoiseUtils.GradientsMask3D];
			var g011 = NoiseUtils.Gradients3D[h011 & NoiseUtils.GradientsMask3D];
			var g100 = NoiseUtils.Gradients3D[h100 & NoiseUtils.GradientsMask3D];
			var g101 = NoiseUtils.Gradients3D[h101 & NoiseUtils.GradientsMask3D];
			var g110 = NoiseUtils.Gradients3D[h110 & NoiseUtils.GradientsMask3D];
			var g111 = NoiseUtils.Gradients3D[h111 & NoiseUtils.GradientsMask3D];

			float v000 = Vector3.Dot(g000, new Vector3(t0.x, t0.y, t0.z));
			float v001 = Vector3.Dot(g001, new Vector3(t0.x, t0.y, t1.z));
			float v010 = Vector3.Dot(g010, new Vector3(t0.x, t1.y, t0.z));
			float v011 = Vector3.Dot(g011, new Vector3(t0.x, t1.y, t1.z));
			float v100 = Vector3.Dot(g100, new Vector3(t1.x, t0.y, t0.z));
			float v101 = Vector3.Dot(g101, new Vector3(t1.x, t0.y, t1.z));
			float v110 = Vector3.Dot(g110, new Vector3(t1.x, t1.y, t0.z));
			float v111 = Vector3.Dot(g111, new Vector3(t1.x, t1.y, t1.z));

			//t0.x = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.x);
			//t0.y = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.y);
			//t0.z = NoiseUtils.Splerp(ref noiseStruct.splerp, 0f, 1f, t0.z);
			
			//Compute the dropoff
			t0.x = NoiseUtils.SmootherStep(t0.x);
			t0.y = NoiseUtils.SmootherStep(t0.y);
			t0.z = NoiseUtils.SmootherStep(t0.z);
			return Mathf.Lerp(
				       Mathf.Lerp(Mathf.Lerp(v000, v100, t0.x),
					       Mathf.Lerp(v010, v110, t0.x), t0.y),
				       Mathf.Lerp(Mathf.Lerp(v001, v101, t0.x),
					       Mathf.Lerp(v011, v111, t0.x), t0.y),
					   t0.z);
		}
	}
}