using System;
using OpenUtility.Exceptions;
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
            ThrowIf.NotInt(newValue, out int result);
            
            SetValue(result);
        }
        
        public void SetValueWithoutNotify(int newValue)
        {
            value = newValue;
        }
        
        public void SetValueWithoutNotify(string newValue)
        {
            ThrowIf.NotInt(newValue, out int result);
            
            SetValueWithoutNotify(result);
        }
    }
}
