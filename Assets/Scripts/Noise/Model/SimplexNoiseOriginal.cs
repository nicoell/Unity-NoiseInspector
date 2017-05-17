namespace Noise.Model
{
	using System;
	using UnityEngine;

	public class SimplexNoiseOriginal : Noise
	{
		public override string Name { get { return "SimplexNoiseOriginal"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/SimplexNoiseOriginal"; } }

		protected override Range Range { get { return Range.OneAroundZero; } }

		private static readonly double Skew3 = 1.0 / 3.0;
		private static readonly double Unskew3 = 1.0 / 6.0;
		private static double u, v, w;
		private static int i, j, k;
		private static int[] A = { 0, 0, 0 };

		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;

			return 0;
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;

			return 0;
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;
		
			//Skew Point
			double skew = (point.x + point.y + point.z) * Skew3;
			
			//Determine surrounding unit cube with integer coorindates in skewed space
			i = (int)Math.Floor(point.x + skew);
			j = (int)Math.Floor(point.y + skew);
			k = (int)Math.Floor(point.z + skew);
			double unskew = (i + j + k) * Unskew3;
			u = point.x - i + unskew;
			v = point.y - j + unskew;
			w = point.z - k + unskew;
			A = new[] {0, 0, 0};
			int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
			int lo = u < w ? u < v ? 0 : 1 : v < w ? 1 : 2;

			return (float) (8.0 * (K(hi) + K(3 - hi - lo) + K(lo) + K(0)));
		}

		private double K(int a)
		{
			double s = (A[0] + A[1] + A[2]) * Unskew3;
			double x = u - A[0] + s;
			double y = v - A[1] + s;
			double z = w - A[2] + s;

			double t = 0.6 - x * x - y * y - z * z;
			int h = Shuffle(i + A[0], j + A[1], k + A[2]);
			A[a]++;
			if (t < 0) return 0;
			int b5 = h >> 5 & 1;
			int b4 = h >> 4 & 1;
			int b3 = h >> 3 & 1;
			int b2 = h >> 2 & 1;
			int b = h & 3;

			double p = b == 1 ? x : b == 2 ? y : z;
			double q = b == 1 ? y : b == 2 ? z : x;
			double r = b == 1 ? z : b == 2 ? x : y;

			if (b5 == b3) p = -p;
			if (b5 == b4) q = -q;
			if (b5 != (b4 ^ b3)) r = -r;

			t *= t;
			return t * t * (p + (b == 0 ? q + r : b2 == 0 ? q : r));

		}

		private int Shuffle(int _i, int _j, int _k)
		{
			return B(_i, _j, _k, 0) + B(_j, _k, _i, 1) + B(_k, _i, _j, 2) + B(_i, _j, _k, 3) +
			       B(_j, _k, _i, 4) + B(_k, _i, _j, 5) + B(_i, _j, _k, 6) + B(_j, _k, _i, 7);
		}

		private int B(int _i, int _j, int _k, int b) { return NoiseUtils.Tn[B(_i, b) << 2 | B(_j, b) << 1 | B(_k, b)]; }

		private int B(int n, int b) { return n >> b & 1; }
	}
}