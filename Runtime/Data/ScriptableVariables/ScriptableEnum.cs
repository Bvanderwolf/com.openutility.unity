using System;
using UnityEngine;

namespace OpenUtility.Data
{
    public abstract class ScriptableEnum : ScriptableInt
    {
        public T GetEnumValue<T>() => (T)Enum.ToObject(typeof(T), GetValue());
    }
    
    /// <summary>
    /// A base class for creating ScriptableObjects that hold enum values.
    /// </summary>
    public abstract class ScriptableEnum<T> : ScriptableEnum where T : Enum
    {
        [Serializable]
        public class EnumValueChangedEvent : UnityEngine.Events.UnityEvent<T> { }
        
        [Serializable]
        public class StringValueChangedEvent : UnityEngine.Events.UnityEvent<string> { }
        
        [SerializeField]
        private EnumValueChangedEvent _enumValueChanged;
        
        [SerializeField]
        private StringValueChangedEvent _stringValueChanged;
        
        public EnumValueChangedEvent EnumValueChanged => _enumValueChanged;
        
        public StringValueChangedEvent StringValueChanged => _stringValueChanged;

        public T GetEnumValue() => GetEnumValue<T>();

        public override void SetValue(int newValue)
        {
            base.SetValue(newValue);
            OnEnumValueChanged(newValue);
            OnStringValueChanged(newValue);
        }

        public override string ToString() => GetEnumValue().ToString();

        private void OnEnumValueChanged(int newValue)
        {
            if (_enumValueChanged == null)
                return;

            var enumValue = (T)Enum.ToObject(typeof(T), newValue);
            _enumValueChanged.Invoke(enumValue);
        }

        private void OnStringValueChanged(int newValue)
        {
            if (_stringValueChanged == null)
                return;
            
            var enumValue = (T)Enum.ToObject(typeof(T), newValue);
            var stringValue = enumValue.ToString();
            _stringValueChanged.Invoke(stringValue);
        }
        
        public static implicit operator T(ScriptableEnum<T> scriptableEnum)
        {
            return scriptableEnum.GetEnumValue();
        }
    }
}
