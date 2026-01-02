using System;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(string), DisplayName = "Default String Binding")]
    [CreateAssetMenu(fileName = "ScriptableBool", menuName = "OpenUtility/Scriptable Variable/String")]
    public class ScriptableString : ScriptableVariable<string>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<string> { }

        [Header("State")]
        [SerializeField]
        private string _value;
        
        [Header("Optional")]
        [SerializeField, Space]
        private Optional<string> _playerPref;

        [Header("Events")]
        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected string value { get; private set; }

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

        public override string GetValue() => value;

        public override void SetValue(string newValue)
        {
            SetValueWithoutNotify(newValue);
            OnValueChanged(newValue);
        }

        public void SetValueWithoutNotify(string newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
        }

        private void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue) 
                return;
            
            var key = _playerPref.Value;
            PlayerPrefs.SetString(key, value);
        }

        private void SetValueFromPlayerPref(string defaultValue)
        {
            var key = _playerPref.Value;
            var data = PlayerPrefs.GetString(key, defaultValue);
            SetValueInternal(data);
        }

        private void SetValueInternal(string newValue) => value = newValue;

        private void OnValueChanged(string newValue) => _valueChanged.Invoke(newValue);

        public override string ToString() => value;
    }
}
