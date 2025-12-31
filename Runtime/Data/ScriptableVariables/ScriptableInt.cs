using System;
using UnityEngine;

namespace OpenUtility.Data
{
    [CreateAssetMenu(fileName = "ScriptableBool", menuName = "OpenUtility/Scriptable Variable/Int")]
    public class ScriptableInt : ScriptableVariable<int>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<int> { }

        [SerializeField]
        private int _value;

        [Header("Optional")]
        [SerializeField]
        private Optional<string> _playerPref;
        
        [Header("Events")]
        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected int value { get; private set; }

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

        protected void OnValidate()
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

        public override int GetValue() => value;

        public override void SetValue(int newValue)
        {
            SetValueWithoutNotify(newValue);
            OnValueChanged(newValue);
        }

        public void SetValueWithoutNotify(int newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
        }

        private void SetValueFromPlayerPref(int defaultValue)
        {
            var key = _playerPref.Value;
            var data = PlayerPrefs.GetInt(key, defaultValue);
            SetValueInternal(data);
        }

        private void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue)
                return;

            var key = _playerPref.Value;
            PlayerPrefs.SetInt(key, value);
        }
        
        private void SetValueInternal(int newValue) => value = newValue;

        private void OnValueChanged(int newValue) => _valueChanged.Invoke(newValue);

        public override string ToString() => value.ToString();
    }
}
