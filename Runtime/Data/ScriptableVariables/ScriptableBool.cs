using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(Toggle), typeof(bool), DisplayName = "Default Bool Binding")]
    [CreateAssetMenu(fileName = "ScriptableBool", menuName = "OpenUtility/Scriptable Variable/Bool")]
    public class ScriptableBool : ScriptableVariable<bool>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<bool> { }

        [Header("State")]
        [SerializeField]
        private bool _value;

        [Header("Optional")]
        [SerializeField]
        private Optional<string> _playerPref;

        [Header("Events")]
        [SerializeField, Space]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected bool value { get; private set; }

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

        public override bool GetValue() => value;

        public override void SetValue(bool newValue)
        {
            SetValueWithoutNotify(newValue);
            OnValueChanged(newValue);
        }
        
        public virtual void SetValueWithoutNotify(bool newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
        }

        private void SetValueInternal(bool newValue) => value = newValue;

        private void OnValueChanged(bool newValue) => _valueChanged.Invoke(newValue);

        private void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue)
                return;
            
            var key = _playerPref.Value;
            var data = value ? 1 : 0;
            PlayerPrefs.SetInt(key, data);
        }

        private void SetValueFromPlayerPref(bool defaultValue)
        {
            var defaultIntValue = defaultValue ? 1 : 0;
            var key = _playerPref.Value;
            var data = PlayerPrefs.GetInt(key, defaultIntValue);
            var newValue = data != 0;
            SetValueInternal(newValue);
        }

        public override string ToString() => value.ToString();
    }
}
