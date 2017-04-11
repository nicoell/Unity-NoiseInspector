using System;
using System.Collections.Generic;
using Noise.Model;

namespace Noise
{
	public static class Presets
    {
        public static readonly IDictionary<string, Model.Noise> PresetNoises = new Dictionary<string, Model.Noise>();
        public static readonly Model.Noise WhiteNoise = RegisterPreset(new WhiteNoise(42));
        public static readonly Model.Noise LatticeNoise = RegisterPreset(new LatticeNoise());
        public static readonly Model.Noise ValueNoise = RegisterPreset(new ValueNoise());

        private static Model.Noise RegisterPreset(Model.Noise noiseModel)
        {
            try
            {
                PresetNoises.Add(noiseModel.Name, noiseModel);
            } catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
            return noiseModel;
        }
    }
}