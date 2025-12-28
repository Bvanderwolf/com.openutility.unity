using System;
using UnityEngine;

namespace OpenUtility.Data
{
    public class ScriptableBool : ScriptableVariable<bool>
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<bool> { }

        [SerializeField]
        private bool _value;

        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        
        protected bool value { get; private set; }

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

        public override bool GetValue() => value;

        public override void SetValue(bool newValue)
        {
            value = newValue;
            _valueChanged.Invoke(value);
        }
        
        public virtual void SetValueWithoutNotify(bool newValue)
        {
            value = newValue;
        }
    }
}
