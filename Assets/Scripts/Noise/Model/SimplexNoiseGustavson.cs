using UnityEditorInternal.VersionControl;

namespace Noise.Model
{
	using System;
	using UnityEngine;

	public class SimplexNoiseGustavson : Noise
	{
		public override string Name { get { return "SimplexNoiseGustavson"; } }
		public override void Reset() { }
		protected override string ShaderPath { get { return "Noise/SimplexNoiseGustavson"; } }

		protected override Range Range { get { return Range.OneAroundZero; } }

		private static readonly float Skew2 = 0.5f * (Mathf.Sqrt(3f) - 1f);
		private static readonly float Unskew2 = (3f - Mathf.Sqrt(3f)) / 6f;
		private static readonly float Skew3 = 1f/3f;
		private static readonly float Unskew3 = 1f/6f;

		protected override float Value1D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;

			return 0;
		}

		protected override float Value2D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;

			float n0, n1, n2; // Noise contributions form the three corners

			// Skew simplex triangle grid to rightangle isosceles triangles, two of which form a square
			float skew = (point.x + point.y) * Skew2;
			// Find integer lattice points in skewed space
			int i0 = Mathf.FloorToInt(point.x + skew);
			int j0 = Mathf.FloorToInt(point.y + skew);

			float unskew = (i0 + j0) * Unskew2;
			// Unskew integer lattice points in skewed space back into simplex grid
			// This is now the origin point of our simplex cell, called first corner
			//var corner0 = new Vector2(i0 - unskew, j0 - unskew);
			// Relative coordinates from first corner to input point within simplex cell
			var t0 = new Vector2(point.x - i0 + unskew, point.y - j0 + unskew);

			// For the 2D case, the simplex shape is a triangle.
			// Determine which simplex we are in.
			int i1, j1;         // Offsets for second corner of simplex cell in skewed space
			int i2 = 1, j2 = 1; // Offsets for third corner of simplex cell in skewed space

			if (t0.x > t0.y)	//lower triangle
			{
				i1 = 1;
				j1 = 0;
			}
			else				// upper triangle
			{
				i1 = 0;
				j1 = 1;
			}

			var t1 = new Vector2(t0.x - i1 + Unskew2, t0.y - j1 + Unskew2);
			var t2 = new Vector2(t0.x - i2 + 2 * Unskew2, t0.y - j2 + 2 * Unskew2);

			// Work out the hashed gradient indices gi of the four simplex corners
			int ii = i0 & NoiseUtils.HashMask;
			int jj = j0 & NoiseUtils.HashMask;
			int gi0 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[jj     ] + ii] & NoiseUtils.GradientsMask3D; // Gradient index for first corner
			int gi1 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[jj + j1] + ii + i1] & NoiseUtils.GradientsMask3D; // Gradient index for second corner
			int gi2 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[jj + j2] + ii + i2] & NoiseUtils.GradientsMask3D; // Gradient index for third corner

			// Calculate the contribution c from the four corners
			float c0 = 0.5f - (t0.x * t0.x + t0.y * t0.y);
			if (c0 < 0) n0 = 0.0f;
			else n0 = Mathf.Pow(c0, 4) * dot(NoiseUtils.Gradients3D[gi0], t0);

			float c1 = 0.5f - t1.x * t1.x - t1.y * t1.y;
			if (c1 < 0) n1 = 0.0f;
			else n1 = Mathf.Pow(c1, 4) * dot(NoiseUtils.Gradients3D[gi1], t1);

			float c2 = 0.5f - t2.x * t2.x - t2.y * t2.y;
			if (c2 < 0) n2 = 0.0f;
			else n2 = Mathf.Pow(c2, 4) * dot(NoiseUtils.Gradients3D[gi2], t2);

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to return values in the interval [-1,1].
			return 70f * (n0 + n1 + n2);
		}

		protected override float Value3D(Vector3 point, NoiseStruct noiseStruct)
		{
			point *= noiseStruct.frequency;

			float n0, n1, n2, n3; // Noise contributions form the four corners

			// Factor to skew simplex tetrahedron grid to unit cube grid
			float skew = (point.x + point.y + point.z) * Skew3;
			// Find integer lattice points in skewed space
			int i0 = Mathf.FloorToInt(point.x + skew);
			int j0 = Mathf.FloorToInt(point.y + skew);
			int k0 = Mathf.FloorToInt(point.z + skew);
			// Factor to unskew back to simplex tetrahedron grid
			float unskew = (i0 + j0 + k0) * Unskew3;
			// Unskew integer lattice points in skewed space back into simplex grid
			// This is now the origin point of our simplex cell, called first corner
			//var corner0 = new Vector3(i0 - unskew, j0 - unskew, k0 - unskew);
			// Relative coordinates from first corner to input point within simplex cell
			var t0 = new Vector3(point.x - i0 + unskew, point.y - j0 + unskew, point.z - k0 + unskew);

			// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
			// Determine which simplex we are in.
			int i1, j1, k1;				// Offsets for second corner of simplex cell in skewed space
			int i2, j2, k2;				// Offsets for third corner of simplex cell in skewed space
			int i3 = 1, j3 = 1, k3 = 1; // Offsets for fourth corner of simplex cell in skewed space
			if (t0.x >= t0.y)
			{
				if (t0.y >= t0.z)			// x > y > z
				{
					i1 = 1; j1 = 0; k1 = 0;
					i2 = 1; j2 = 1; k2 = 0;
				} else if (t0.x >= t0.z)	// x > z > y
				{
					i1 = 1; j1 = 0; k1 = 0;
					i2 = 1; j2 = 0; k2 = 1;
				} else						// z > x > y
				{
					i1 = 0; j1 = 0; k1 = 1;
					i2 = 1; j2 = 0; k2 = 1;
				}
			} else // t0.x < t0.y
			{
				if (t0.y < t0.z)			// z > y > x
				{
					i1 = 0; j1 = 0; k1 = 1;
					i2 = 0; j2 = 1; k2 = 1;
				} else if (t0.x < t0.z)		// y > z > x
				{
					i1 = 0; j1 = 1; k1 = 0;
					i2 = 0; j2 = 1; k2 = 1;
				} else						// y > x > z
				{
					i1 = 0; j1 = 1; k1 = 0;
					i2 = 1; j2 = 1; k2 = 0;
				}
			}

			var t1 = new Vector3(t0.x - i1 + Unskew3, t0.y - j1 + Unskew3, t0.z - k1 + Unskew3);
			var t2 = new Vector3(t0.x - i2 + 2*Unskew3, t0.y - j2 + 2*Unskew3, t0.z - k2 + 2*Unskew3);
			var t3 = new Vector3(t0.x - i3 + 3*Unskew3, t0.y - j3 + 3*Unskew3, t0.z - k3 + 3*Unskew3);

			// Work out the hashed gradient indices gi of the four simplex corners
			int ii = i0 & NoiseUtils.HashMask;
			int jj = j0 & NoiseUtils.HashMask;
			int kk = k0 & NoiseUtils.HashMask;
			int gi0 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[kk     ] + jj     ] + ii] & NoiseUtils.GradientsMask3D; // Gradient index for first corner
			int gi1 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[kk + k1] + jj + j1] + ii + i1] & NoiseUtils.GradientsMask3D; // Gradient index for second corner
			int gi2 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[kk + k2] + jj + j2] + ii + i2] & NoiseUtils.GradientsMask3D; // Gradient index for third corner
			int gi3 = NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[NoiseUtils.PerlinHash[kk + k3] + jj + j3] + ii + i3] & NoiseUtils.GradientsMask3D; // Gradient index for forth corner

			// Calculate the contribution c from the four corners
			float c0 = 0.6f - t0.x * t0.x - t0.y * t0.y - t0.z * t0.z;
			if (c0 < 0) n0 = 0.0f;
			else n0 = Mathf.Pow(c0, 4) * Vector3.Dot(NoiseUtils.Gradients3D[gi0], t0);

			float c1 = 0.6f - t1.x * t1.x - t1.y * t1.y - t1.z * t1.z;
			if (c1 < 0) n1 = 0.0f;
			else n1 = Mathf.Pow(c1, 4) * Vector3.Dot(NoiseUtils.Gradients3D[gi1], t1);

			float c2 = 0.6f - t2.x * t2.x - t2.y * t2.y - t2.z * t2.z;
			if (c2 < 0) n2 = 0.0f;
			else n2 = Mathf.Pow(c2, 4) * Vector3.Dot(NoiseUtils.Gradients3D[gi2], t2);

			float c3 = 0.6f - t3.x * t3.x - t3.y * t3.y - t3.z * t3.z;
			if (c3 < 0) n3 = 0.0f;
			else n3 = Mathf.Pow(c3, 4) * Vector3.Dot(NoiseUtils.Gradients3D[gi3], t3);

			// Add contributions from each corner to get the final noise value.
			// The result is scaled to return values in the interval [-1,1].
			return 8.0f * (n0 + n1 + n2 + n3);
		}
		
		private static float dot(Vector3 grad, Vector3 dir) { return Vector3.Dot(grad, dir); }
		private static float dot(Vector3 grad, Vector2 dir) { return grad.x * dir.x + grad.y * dir.y;  }
	}
}