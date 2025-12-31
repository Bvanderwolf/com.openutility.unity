using System;
using UnityEngine;

namespace OpenUtility.Data
{
    [CreateAssetMenu(fileName = "ScriptableBool", menuName = "OpenUtility/Scriptable Variable/Float")]
    public class ScriptableFloat : ScriptableVariable<float>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<float> { }

        [SerializeField]
        private float _value;
        
        [Header("Optional")]
        [SerializeField]
        private Optional<string> _playerPref;

        [Header("Events")]
        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected float value { get; private set; }

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


        public override float GetValue() => value;

        public override void SetValue(float newValue)
        {
            SetValueWithoutNotify(newValue);
            OnValueChanged(newValue);
        }

        public void SetValueWithoutNotify(float newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
        }

        private void SetValueInternal(float newValue) => value = newValue;

        private void OnValueChanged(float newValue) => _valueChanged.Invoke(newValue);
        
        private void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue)
                return;

            var key = _playerPref.Value;
            PlayerPrefs.SetFloat(key, value);
        }

        private void SetValueFromPlayerPref(float defaultValue)
        {
            var key = _playerPref.Value;
            var data = PlayerPrefs.GetFloat(key, defaultValue);
            SetValueWithoutNotify(data);
        }

        public override string ToString() => value.ToString();
    }
}
