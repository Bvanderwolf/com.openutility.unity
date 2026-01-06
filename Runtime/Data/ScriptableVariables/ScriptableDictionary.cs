using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data
{
    /// <summary>
    /// A base class for creating scriptable dictionary variables of various key-value types.
    /// </summary>
    public abstract class ScriptableDictionary<TKey, TValue> : ScriptableVariable<IDictionary<TKey, TValue>>
    {
        [Serializable]
        private struct KeyValuePair
        {
            public TKey key;
            public TValue value;
        }

        [Header("State")]
        [SerializeField]
        private KeyValuePair[] _values = Array.Empty<KeyValuePair>();
        
        protected IDictionary<TKey, TValue> value { get; private set; }

        protected virtual void OnEnable()
        {
            value ??= CreateValue(_values.Length);
            RebuildDictionary();
        }

        protected virtual void OnValidate()
        {
            OnEnable();
        }

        protected abstract IDictionary<TKey, TValue> CreateValue(int capacity);

        public override IDictionary<TKey, TValue> GetValue() => value;
        public TValue GetValue(TKey key) => value[key];

        public override void SetValue(IDictionary<TKey, TValue> newValue) => value = newValue;
        public void SetValue(TKey key, TValue newValue) => value[key] = newValue;

        public void Add(TKey key, TValue newValue)
        {
#if UNITY_EDITOR
            Array.Resize(ref _values, _values.Length + 1);
            _values[^1] = new KeyValuePair { key = key, value = newValue };
#endif
            value.Add(key, newValue);
        }
        
        public void Remove(TKey key)
        {
#if UNITY_EDITOR
            int index = Array.FindIndex(_values, pair => EqualityComparer<TKey>.Default.Equals(pair.key, key));
            if (index >= 0)
            {
                for (int i = index; i < _values.Length - 1; i++)
                    _values[i] = _values[i + 1];
                Array.Resize(ref _values, _values.Length - 1);
            }
#endif
            value.Remove(key);
        }

        public TValue this[TKey key]
        {
            get => GetValue(key);
            set
            {
                if (this.value.ContainsKey(key))
                {
                    SetValue(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        private void RebuildDictionary()
        {
            for (int i = 0; i < _values.Length; i++)
                value[_values[i].key] = _values[i].value;
        }
    }
}