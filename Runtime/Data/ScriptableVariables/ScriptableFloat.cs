using System;
using UnityEngine;

namespace OpenUtility.Data
{
    public class ScriptableFloat : ScriptableVariable<float>
    {
        [Serializable]
        public class ChangedEvent : UnityEngine.Events.UnityEvent<float> { }

        [SerializeField]
        private float _value;

        [SerializeField]
        private ChangedEvent _valueChanged;

        public ChangedEvent ValueChanged => _valueChanged;
        
        protected float value { get; private set; }

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

        public override float GetValue() => value;

        public override void SetValue(float newValue)
        {
            value = newValue;
            _valueChanged.Invoke(value);
        }
        
        public virtual void SetValue(string newValue)
        {
            if (!float.TryParse(newValue, out float result))
                throw new FormatException($"The provided string '{newValue}' is not a valid float.");
            
            SetValue(result);
        }
        
        public void SetValueWithoutNotify(float newValue)
        {
            value = newValue;
        }
        
        public void SetValueWithoutNotify(string newValue)
        {
            if (!float.TryParse(newValue, out float result))
                throw new FormatException($"The provided string '{newValue}' is not a valid float.");
            
            SetValueWithoutNotify(result);
        }
    }
}
