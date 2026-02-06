using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    /// <summary>
    /// Returns a floating point number (0-1) indicating a difficulty percentage. Increases with time. Optionally
    /// use a range to map 0-1 to a different range.
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableDifficulty", menuName = "OpenUtility/Samples/ScriptableDifficulty")]
    public class ScriptableDifficulty : ScriptableFloat
    {
        [Header("Difficulty Settings")]
        [SerializeField, Tooltip("The amount of time (in seconds) till the maximum difficulty is reached.")]
        private float _timeAtMaximum = 300;

        [SerializeField, Tooltip("An optional curve to apply to the difficulty.")]
        private Optional<AnimationCurve> _curve;

        [SerializeField, Tooltip("An optional range to map 0-1 to.")]
        private Optional<FloatRange> _range;
        
        private float _timeOnEnable;
        private float _timeOffsetOnEnable;
        private float _timeOnLastRetrieval;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            float percentage = _range.HasValue ? _range.Value.GetPercentage(value) : Mathf.InverseLerp(0.0f, 1.0f, value);

            if (_curve.HasValue)
                percentage = _curve.Value.Evaluate(percentage);

            _timeOnEnable = Time.time;
            _timeOffsetOnEnable = Mathf.Lerp(_timeOnEnable, _timeAtMaximum, percentage);
        }

        public float GetValue(float time)
        {
            if (time == _timeOnLastRetrieval)
                return (value);
            
            _timeOnLastRetrieval = time;
            
            float timeWithOffset = _timeOnLastRetrieval + _timeOffsetOnEnable;
            float percentage = Mathf.InverseLerp(_timeOnEnable, _timeAtMaximum, timeWithOffset);
            
            float newValue = percentage;

            if (_curve.HasValue)
                newValue = _curve.Value.Evaluate(percentage);

            if (_range.HasValue)
                newValue = _range.Value.MapFrom01(percentage);

            SetValueWithoutNotify(newValue);

            return (newValue);
        }

        public override float GetValue() => GetValue(Time.time);
    }
}
