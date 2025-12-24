using UnityEngine;

namespace OpenUtility.Data
{
    public delegate float EasingFunction(float percentage);

    public static class EasingFunctions
    {
        // Linear
        public static readonly EasingFunction noEase = value => value;

        // Sine
        public static readonly EasingFunction easeInSine = value => 1 - Mathf.Cos((value * Mathf.PI) / 2);
        public static readonly EasingFunction easeOutSine = value => Mathf.Sin((value * Mathf.PI) / 2);
        public static readonly EasingFunction easeInOutSine = value => -(Mathf.Cos(Mathf.PI * value) - 1) / 2;

        // Quad
        public static readonly EasingFunction easeInQuad = value => value * value;
        public static readonly EasingFunction easeOutQuad = value => 1 - (1 - value) * (1 - value);
        public static readonly EasingFunction easeInOutQuad = value => value < 0.5f ? 2 * value * value : 1 - Mathf.Pow(-2 * value + 2, 2) / 2;

        // Cubic
        public static readonly EasingFunction easeInCubic = value => value * value * value;
        public static readonly EasingFunction easeOutCubic = value => 1 - Mathf.Pow(1 - value, 3);
        public static readonly EasingFunction easeInOutCubic = value => value < 0.5f ? 4 * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 3) / 2;

        // Quart
        public static readonly EasingFunction easeInQuart = value => value * value * value * value;
        public static readonly EasingFunction easeOutQuart = value => 1 - Mathf.Pow(1 - value, 4);
        public static readonly EasingFunction easeInOutQuart = value => value < 0.5f ? 8 * value * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 4) / 2;

        // Expo
        public static readonly EasingFunction easeInExpo = value => value == 0 ? 0 : Mathf.Pow(2, 10 * value - 10);
        public static readonly EasingFunction easeOutExpo = value => value == 1 ? 1 : 1 - Mathf.Pow(2, -10 * value);
        public static readonly EasingFunction easeInOutExpo = value => value == 0 ? 0 : value == 1 ? 1 : value < 0.5f ? Mathf.Pow(2, 20 * value - 10) / 2 : (2 - Mathf.Pow(2, -20 * value + 10)) / 2;

        // Circ
        public static readonly EasingFunction easeInCirc = value => 1 - Mathf.Sqrt(1 - Mathf.Pow(value, 2));
        public static readonly EasingFunction easeOutCirc = value => Mathf.Sqrt(1 - Mathf.Pow(value - 1, 2));
        public static readonly EasingFunction easeInOutCirc = value => value < 0.5f ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * value, 2))) / 2 : (Mathf.Sqrt(1 - Mathf.Pow(-2 * value + 2, 2)) + 1) / 2;

        // Back (De "overshoot")
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1;
        public static readonly EasingFunction easeInBack = value => c3 * value * value * value - c1 * value * value;
        public static readonly EasingFunction easeOutBack = value => 1 + c3 * Mathf.Pow(value - 1, 3) + c1 * Mathf.Pow(value - 1, 2);
        public static readonly EasingFunction easeInOutBack = value => value < 0.5f ? (Mathf.Pow(2 * value, 2) * ((c2 + 1) * 2 * value - c2)) / 2 : (Mathf.Pow(2 * value - 2, 2) * ((c2 + 1) * (value * 2 - 2) + c2) + 2) / 2;

        // Bounce
        public static readonly EasingFunction easeOutBounce = value => {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (value < 1 / d1) return n1 * value * value;
            if (value < 2 / d1) return n1 * (value -= 1.5f / d1) * value + 0.75f;
            if (value < 2.5f / d1) return n1 * (value -= 2.25f / d1) * value + 0.9375f;
            return n1 * (value -= 2.625f / d1) * value + 0.984375f;
        };
        public static readonly EasingFunction easeInBounce = value => 1 - easeOutBounce(1 - value);

        public static EasingFunction GetFunction(this EasingType easing)
        {
            return easing switch
            {
                EasingType.NONE => noEase,
                EasingType.EASE_IN_SINE => easeInSine,
                EasingType.EASE_OUT_SINE => easeOutSine,
                EasingType.EASE_IN_OUT_SINE => easeInOutSine,
                EasingType.EASE_IN_QUAD => easeInQuad,
                EasingType.EASE_OUT_QUAD => easeOutQuad,
                EasingType.EASE_IN_OUT_QUAD => easeInOutQuad,
                EasingType.EASE_IN_CUBIC => easeInCubic,
                EasingType.EASE_OUT_CUBIC => easeOutCubic,
                EasingType.EASE_IN_OUT_CUBIC => easeInOutCubic,
                EasingType.EASE_IN_EXPO => easeInExpo,
                EasingType.EASE_OUT_EXPO => easeOutExpo,
                EasingType.EASE_IN_OUT_EXPO => easeInOutExpo,
                EasingType.EASE_IN_BACK => easeInBack,
                EasingType.EASE_OUT_BACK => easeOutBack,
                EasingType.EASE_IN_OUT_BACK => easeInOutBack,
                EasingType.EASE_OUT_BOUNCE => easeOutBounce,
                EasingType.EASE_IN_BOUNCE => easeInBounce,
                _ => noEase
            };
        }
    }

    public enum EasingType
    {
        [InspectorName("None (Linear)")] NONE,
        [InspectorName("Sine/In")] EASE_IN_SINE,
        [InspectorName("Sine/Out")] EASE_OUT_SINE,
        [InspectorName("Sine/InOut")] EASE_IN_OUT_SINE,
        [InspectorName("Quad/In")] EASE_IN_QUAD,
        [InspectorName("Quad/Out")] EASE_OUT_QUAD,
        [InspectorName("Quad/InOut")] EASE_IN_OUT_QUAD,
        [InspectorName("Cubic/In")] EASE_IN_CUBIC,
        [InspectorName("Cubic/Out")] EASE_OUT_CUBIC,
        [InspectorName("Cubic/InOut")] EASE_IN_OUT_CUBIC,
        [InspectorName("Expo/In")] EASE_IN_EXPO,
        [InspectorName("Expo/Out")] EASE_OUT_EXPO,
        [InspectorName("Expo/InOut")] EASE_IN_OUT_EXPO,
        [InspectorName("Back/In")] EASE_IN_BACK,
        [InspectorName("Back/Out")] EASE_OUT_BACK,
        [InspectorName("Back/InOut")] EASE_IN_OUT_BACK,
        [InspectorName("Bounce/In")] EASE_IN_BOUNCE,
        [InspectorName("Bounce/Out")] EASE_OUT_BOUNCE
    }
}
