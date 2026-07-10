using System;
using System.Globalization;
using UnityEngine;

namespace OpenUtility.Data
{
    [CreateAssetMenu(fileName = "ScriptableDouble", menuName = "OpenUtility/Scriptable Variable/Double")]
    public class ScriptableDouble : ScriptableVariable<double>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<double> { }

        [Header("State")]
        [SerializeField]
        private double _value;

        [Header("Optional")]
        [SerializeField]
        private Optional<string> _playerPref;
        
        [Header("Event")]
        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected double value { get; private set; }

        protected virtual void OnEnable()
        {
            if (_playerPref.HasValue)
            {
                SetValueFromPlayerPref(_value);
            }
            else
            {
                SetValueWithoutNotify(_value);
            }
        }

        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                SetValue(_value);
            }
            else
            {
                SetValueWithoutNotify(_value);
            }
        }

        public override double GetValue() => value;

        public override void SetValue(double newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
            OnValueChanged(newValue);
        }

        public virtual void SetValueWithoutNotify(double newValue)
        {
            SetValueInternal(newValue);
        }

        public void Increment() => Increment(1d);
        public void Increment(double increment) => SetValue(GetValue() + increment);
        public void Increment(float increment) => SetValue(GetValue() + increment);
        
        public void Decrement() => Decrement(1d);
        public void Decrement(double decrement) => SetValue(GetValue() - decrement);

        protected void SetValueInternal(double newValue) => value = newValue;

        protected void OnValueChanged(double newValue) => _valueChanged?.Invoke(newValue);

        private void SetValueFromPlayerPref(double defaultValue)
        {
            var key = _playerPref.Value;
            var culture = CultureInfo.InvariantCulture;
            var data = double.Parse(PlayerPrefs.GetString(key, defaultValue.ToString(culture)), culture);
            SetValueInternal(data);
        }

        private void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue)
                return;

            var key = _playerPref.Value;
            var culture = CultureInfo.InvariantCulture;
            PlayerPrefs.SetString(key, value.ToString(culture));
        }

        public override string ToString() => value.ToString(CultureInfo.InvariantCulture);
        
        public static implicit operator double(ScriptableDouble scriptableInt) => scriptableInt.GetValue();
    }
}
