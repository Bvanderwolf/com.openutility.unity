using System;
using UnityEngine;

namespace OpenUtility.Data
{
    [CreateAssetMenu(fileName = "ScriptableVector2", menuName = "OpenUtility/Scriptable Variable/Vector2")]
    public class ScriptableVector2 : ScriptableVariable<Vector2>, ICanLoadValueFromPlayerPrefs
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<Vector2> { }

        [Header("State")]
        [SerializeField]
        private Vector2 _value;
        
        [Header("Optional")]
        [SerializeField]
        private Optional<string> _playerPref;

        [Header("Event")]
        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        public Optional<string> PlayerPref => _playerPref;
        
        protected Vector2 value { get; private set; }

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
        
        public override Vector2 GetValue() => value;

        public override void SetValue(Vector2 newValue)
        {
            SetValueInternal(newValue);
            SetPlayerPrefIfNeeded();
            OnValueChanged(newValue);
        }

        public virtual void SetValueWithoutNotify(Vector2 newValue)
        {
            SetValueInternal(newValue);
        }

        protected void SetValueInternal(Vector2 newValue) => value = newValue;

        protected void OnValueChanged(Vector2 newValue) => _valueChanged?.Invoke(newValue);
        
        protected void SetPlayerPrefIfNeeded()
        {
            if (!_playerPref.HasValue)
                return;

            var key = _playerPref.Value;
            var xkey = $"{key}_X";
            var ykey = $"{key}_Y";
            
            PlayerPrefs.SetFloat(xkey, value.x);
            PlayerPrefs.SetFloat(ykey, value.y);
        }

        private void SetValueFromPlayerPref(Vector2 defaultValue)
        {
            var key = _playerPref.Value;
            var xkey = $"{key}_X";
            var ykey = $"{key}_Y";
            
            var x = PlayerPrefs.GetFloat(xkey, defaultValue.x);
            var y = PlayerPrefs.GetFloat(ykey, defaultValue.y);
            var data = new Vector2(x, y);
            SetValueWithoutNotify(data);
        }

        public override string ToString() => value.ToString();
        
        public static implicit operator Vector2(ScriptableVector2 reference) => reference.GetValue();
    }
}
