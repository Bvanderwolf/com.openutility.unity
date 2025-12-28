using System;
using UnityEngine;

namespace OpenUtility.Data
{
    public class ScriptableString : ScriptableVariable<string>
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<string> { }

        [SerializeField]
        private string _value;

        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        
        protected string value { get; private set; }

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

        public override string GetValue() => value;

        public override void SetValue(string newValue)
        {
            value = newValue;
            _valueChanged.Invoke(value);
        }
        
        public void SetValueWithoutNotify(string newValue)
        {
            value = newValue;
        }
    }
}
