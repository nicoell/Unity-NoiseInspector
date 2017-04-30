namespace Noise
{
	using System;
	using System.Diagnostics;
	using UnityEngine;

	public static class Generator
	{
		public static void Generate(NoiseStruct noiseStruct, ref Texture2D texture2D, out long computationTime)
		{
			AssertTrue(texture2D.width == texture2D.height);

			int resolution = texture2D.width;
			float stepSize = 1f / resolution;

			// pointXY X=0: left, X=1: right; Y=0: bottom, Y=1: top
			var point00 = new Vector3(-0.5f, -0.5f);
			var point10 = new Vector3(0.5f, -0.5f);
			var point01 = new Vector3(-0.5f, 0.5f);
			var point11 = new Vector3(0.5f, 0.5f);
			if (noiseStruct.worldTransform)
			{
				point00 = noiseStruct.worldTransform.TransformPoint(point00);
				point10 = noiseStruct.worldTransform.TransformPoint(point10);
				point01 = noiseStruct.worldTransform.TransformPoint(point01);
				point11 = noiseStruct.worldTransform.TransformPoint(point11);
			}

			//Reset NoiseModel before using it
			noiseStruct.noiseModel.Reset();

			var stopwatch = Stopwatch.StartNew();

			var colors = new Color32[resolution * resolution];
			for (int y = 0; y < resolution; y++)
			{
				var point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
				var point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
				for (int x = 0; x < resolution; x++)
				{
					var point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
					var color = Color.white * noiseStruct.noiseModel.Value(point * resolution, noiseStruct);
                    color.a = 1f;
					colors[y * resolution + x] = color;
				}
			}
			texture2D.SetPixels32(colors);

			stopwatch.Stop();
			computationTime = stopwatch.ElapsedMilliseconds;

			texture2D.Apply();
		}

		private static void AssertTrue(bool condition)
		{
			if (!condition)
			{
				throw new NoiseGeneratorException();
			}
		}

		private class NoiseGeneratorException : Exception { }
	}
}