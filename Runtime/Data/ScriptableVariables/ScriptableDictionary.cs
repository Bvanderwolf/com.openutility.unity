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

        private void RebuildDictionary()
        {
            for (int i = 0; i < _values.Length; i++)
                value[_values[i].key] = _values[i].value;
        }
    }
}