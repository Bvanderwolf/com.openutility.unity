using UnityEngine;

namespace OpenUtility.Data
{
    public static class NoiseUtility
    {
        public static float GetPerlinNoiseValue(float value, float speed, float amplitude)
        {
            float noise = Mathf.PerlinNoise1D(Time.time * speed); // 0..1
            float centeredNoise = (noise - 0.5f) * 2f; // -1..1

            float noiseValue = value + centeredNoise * amplitude;
            noiseValue = Mathf.Max(noiseValue, 0.01f); // Ensure it's always positive

            return (noiseValue);
        }
        
        public static float GetPerlinNoiseValue(float value, float speed, float amplitude, float offset)
        {
            float noise = Mathf.PerlinNoise(Time.time * speed, offset); // 0..1
            float centeredNoise = (noise - 0.5f) * 2f; // -1..1

            float noiseValue = value + centeredNoise * amplitude;
            noiseValue = Mathf.Max(noiseValue, 0.01f); // Ensure it's always positive

            return (noiseValue);
        }
    }
}
