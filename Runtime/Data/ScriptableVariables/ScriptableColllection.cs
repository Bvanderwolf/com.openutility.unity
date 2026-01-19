using System;
using System.Collections.Generic;
using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility
{
    public abstract class ScriptableColllection<T> : ScriptableVariable<ICollection<T>>
    {
        [Header("State")]
        [SerializeField]
        private T[] _values = Array.Empty<T>();
        
        protected ICollection<T> value { get; private set; }

        protected virtual void OnEnable()
        {
            value ??= CreateValue(_values.Length);
            
            if (value is not T[])
                RebuildCollection(value, _values);
        }

        private void OnValidate()
        {
            OnEnable();
        }

        protected virtual ICollection<T> CreateValue(int capacity) => new T[capacity];

        public override ICollection<T> GetValue() => value;

        public T GetValue(int index)
        {
            switch (value)
            {
                case T[] array:
                    return array[index];
                
                case List<T> list:
                    return list[index];
                
                default:
                    throw new NotSupportedException($"GetValue(int index) is not supported for collection type {value.GetType().FullName}.");
            }
        }

        public override void SetValue(ICollection<T> newValue) => value = newValue;

        public void SetValue(int index, T newValue)
        {
            switch (value)
            {
                case T[] array:
                    array[index] = newValue;
                    break;
                case IList<T> list:
                    list[index] = newValue;
                    break;
                
                default:
                    throw new NotSupportedException($"SetValue(int index, T newValue) is not supported for collection type {value.GetType().FullName}.");
            }
        }
        
        public void Add(T newValue)
        {
#if UNITY_EDITOR
            Array.Resize(ref _values, _values.Length + 1);
            _values[^1] = newValue;
#endif
            value.Add(newValue);
        }

        public void Remove(T valueToRemove)
        {
#if UNITY_EDITOR
            int index = Array.FindIndex(_values, item => EqualityComparer<T>.Default.Equals(item, valueToRemove));
            if (index >= 0)
            {
                for (int i = index; i < _values.Length - 1; i++)
                    _values[i] = _values[i + 1];
                Array.Resize(ref _values, _values.Length - 1);
            }
#endif
            value.Remove(valueToRemove);
        }
        
        public void RemoveAt(int index)
        {
            if (value is T[] array)
            {
                for (int i = index; i < _values.Length - 1; i++)
                    _values[i] = _values[i + 1];
                Array.Resize(ref _values, _values.Length - 1);
            }
            else if (value is List<T> list)
            {
                list.RemoveAt(index);
            }
            else
            {
                throw new NotSupportedException($"RemoveAt(int index) is not supported for collection type {value.GetType().FullName}.");
            }
        }
        
        public T this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        public void Resize(int newSize)
        {
            switch (value)
            {
                case T[] array:
#if UNITY_EDITOR
                    Array.Resize(ref _values, newSize);
#endif
                    Array.Resize(ref array, newSize);
                    break;
                case IList<T>:
                    Array.Resize(ref _values, newSize);
                    RebuildCollection(value, _values);
                    break;
                
                default:
                    throw new NotSupportedException($"SetValue(int index, T newValue) is not supported for collection type {value.GetType().FullName}.");
            }
        }
        
        private static void RebuildCollection(ICollection<T> collection, T[] values)
        {
            collection.Clear();
            foreach (T value in values)
                collection.Add(value);
        }
    }
}
