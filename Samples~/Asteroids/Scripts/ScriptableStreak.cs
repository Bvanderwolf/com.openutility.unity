using OpenUtility.Data;
using TMPro;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    /// <summary>
    /// The result of a streak continuation.
    /// </summary>
    public enum StreakContinuation : int
    {
        /// <summary>
        /// The streak has reached its ceiling.
        /// </summary>
        REACHED_CEILING = 0,
        
        /// <summary>
        /// The streak is on cooldown still.
        /// </summary>
        ON_COOLDOWN = 1,
        
        /// <summary>
        /// The interval for the streak has been missed.
        /// </summary>
        MISSED_INTERVAL = 2,
        
        /// <summary>
        /// The streak was successfully incremented.
        /// </summary>
        SUCCESFULL = 3
    }
    
    /// <summary>
    /// Represents a continueable streak as a ScriptableObject.
    /// </summary>
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(int), typeof(ScriptableStreak), BindingGoal.DetermineValue, DisplayName = "Streak")]
    [CreateAssetMenu(fileName = "ScriptableStreak", menuName = "OpenUtility/Samples/ScriptableStreak")]
    public class ScriptableStreak : ScriptableInt
    {
        /// <summary>
        /// An optional ceiling for the streak.
        /// </summary>
        [Header("Streak Settings")]
        [SerializeField, Tooltip("An optional ceiling for the streak.")]
        private Optional<int> _ceiling;

        /// <summary>
        /// An optional cooldown after being reset.
        /// </summary>
        [SerializeField, Tooltip("An optional cooldown after being reset.")]
        private Optional<float> _cooldown;

        /// <summary>
        /// If a continuation is attempted after this time has passed
        /// since the last continuation, the streak is reset.
        /// </summary>
        [SerializeField, Tooltip("Optionally, if a continuation is attempted after this time has passed since the last continuation, the streak is reset.")]
        private Optional<float> _interval;

        /// <summary>
        /// Whether the streak has a ceiling it can hit.
        /// </summary>
        public bool HasCeiling => _ceiling.HasValue;

        /// <summary>
        /// Whether the streak has a cooldown after being reset.
        /// </summary>
        public bool HasCooldown => _cooldown.HasValue;

        /// <summary>
        /// Whether the streak has a required interval time between continuations.
        /// </summary>
        public bool HasInterval => _interval.HasValue;
        
        private float _lastContinueTime;
        private float _lastResetTime;

        /// <summary>
        /// Resets the streak to 0.
        /// </summary>
        public void ResetStreak()
        {
            _lastResetTime = Time.time;
            SetValue(0);
        }

        /// <summary>
        /// Does not continue the streak, but updates the last continue time to prevent the streak from being reset if it has an interval.
        /// </summary>
        public void KeepAlive()
        {
            if (!_interval.HasValue)
                return;

            float time = Time.time;
            float timeSinceLastContinue = time - _lastContinueTime;
            
            if (timeSinceLastContinue > _interval.Value)
                return;

            UpdateContinueTime(time);
        }

        /// <summary>
        /// Continues the streak, incrementing the current streak number if successful.
        /// </summary>
        /// <returns>The streak continuation result.</returns>
        public StreakContinuation Continue()
        {
            if (_ceiling.HasValue && value == _ceiling.Value)
                return StreakContinuation.REACHED_CEILING;

            float time = Time.time;
            
            if (_cooldown.HasValue && value == 0)
            {
                // If the streak has a cooldown and is reset, check whether the streak can start again.
                float timeSinceLastReset = time - _lastResetTime;
                
                if (timeSinceLastReset < _cooldown.Value)
                {
                    UpdateContinueTime(time);
                    return StreakContinuation.ON_COOLDOWN;
                }
            }

            if (_interval.HasValue)
            {
                // If the streak has an interval, check whether the streak can continue.
                float timeSinceLastContinue = time - _lastContinueTime;
                
                if (timeSinceLastContinue > _interval.Value)
                {
                    UpdateContinueTime(time);
                    ResetStreak();
                    return StreakContinuation.MISSED_INTERVAL;
                }
            }

            UpdateContinueTime(time);
            SetValue(value + 1);

            return StreakContinuation.SUCCESFULL;
        }
        
        private void UpdateContinueTime(float time)
        {
            _lastContinueTime = time;
        }
    }
}