using UnityEngine;

public static class NoiseUtils
{
	/// <summary>
	/// Perform Spline Interpolation using Unitys AnimationCurves
	/// </summary>
	/// <param name="animationCurve"></param>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="t"></param>
	/// <returns></returns>
	public static float Splerp(ref AnimationCurve animationCurve, float start, float end, float t)
	{
		return start + (end - start) * animationCurve.Evaluate(t);
	}

	private static Texture2D m_perlinHashTexture;
	public static Texture2D PerlinHashTexture
	{
		get
		{
			if (m_perlinHashTexture == null)
			{
				m_perlinHashTexture = new Texture2D(PerlinHash.Length / 2, 1, TextureFormat.RGBA32, false)
				{
					name = "PerlinHashTexture",
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Repeat,
					anisoLevel = 0
				};
				var colors = new Color32[PerlinHash.Length / 2];
				for (int i = 0; i < PerlinHash.Length / 2; i++)
				{
					colors[i] = new Color32((byte) PerlinHash[i], (byte) PerlinHash[i], (byte) PerlinHash[i], 255);
				}
				m_perlinHashTexture.SetPixels32(colors);
				m_perlinHashTexture.Apply();
			}
			return m_perlinHashTexture;
		}
	}


	public static float SmootherStep(float t)
	{
		return t * t * t * (t * (t * 6f - 15f) + 10f);
	}

	public static readonly float Sqr2 = Mathf.Sqrt(2f);

	public static readonly int HashMask = 255;
	public static readonly int[] PerlinHash =
	{
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
		140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
		247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
		57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
		74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
		65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
		200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
		52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
		207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
		218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
		81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
		184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
		140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
		247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
		57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
		74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
		65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
		200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
		52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
		207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
		218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
		81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
		184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
	};

	public static readonly int GradientsMask1D = 1;
	public static readonly float[] Gradients1D =
	{
		1f, -1f
	};

	public static readonly int GradientsMask2D = 7;
	public static readonly Vector2[] Gradients2D =
	{
		new Vector2(1f, 0f),
		new Vector2(-1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(0f, -1f),
		new Vector2(1f, 1f).normalized,
		new Vector2(-1f, 1f).normalized,
		new Vector2(1f, -1f).normalized,
		new Vector2(-1f, -1f).normalized
	};

	public static readonly int GradientsMask3D = 15;
	public static readonly Vector3[] Gradients3D =
	{
		new Vector3(1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3(1f, -1f, 0f),
		new Vector3(-1f, -1f, 0f),
		new Vector3(1f, 0f, 1f),
		new Vector3(-1f, 0f, 1f),
		new Vector3(1f, 0f, -1f),
		new Vector3(-1f, 0f, -1f),
		new Vector3(0f, 1f, 1f),
		new Vector3(0f, -1f, 1f),
		new Vector3(0f, 1f, -1f),
		new Vector3(0f, -1f, -1f),

		new Vector3(1f, 1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3(0f, -1f, 1f),
		new Vector3(0f, -1f, -1f)
	};
}