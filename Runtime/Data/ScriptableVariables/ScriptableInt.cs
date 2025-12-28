using System;
using UnityEngine;

namespace OpenUtility.Data
{
    public class ScriptableInt : ScriptableVariable<int>
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<int> { }

        [SerializeField]
        private int _value;

        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        
        protected int value { get; private set; }

        protected virtual void OnEnable()
        {
            SetValueWithoutNotify(_value);
        }
        
        protected void OnValidate()
        {
            if (Application.isPlaying)
                SetValue(_value);
            else
                SetValueWithoutNotify(_value);
        }

        public override int GetValue() => value;

        public override void SetValue(int newValue)
        {
            value = newValue;
            _valueChanged.Invoke(value);
        }

        public virtual void SetValue(string newValue)
        {
            if (!int.TryParse(newValue, out int result))
                throw new FormatException($"The provided string '{newValue}' is not a valid integer.");
            
            SetValue(result);
        }
        
        public void SetValueWithoutNotify(int newValue)
        {
            value = newValue;
        }
        
        public void SetValueWithoutNotify(string newValue)
        {
            if (!int.TryParse(newValue, out int result))
                throw new FormatException($"The provided string '{newValue}' is not a valid integer.");
            
            SetValueWithoutNotify(result);
        }
    }
}
