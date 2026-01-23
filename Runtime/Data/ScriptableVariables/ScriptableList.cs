using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data
{
    public abstract class ScriptableList<T> : ScriptableVariable<IList<T>>
    {
        [Header("State")]
        [SerializeField, Tooltip("The values used to start the list with.")]
        private List<T> _values = new List<T>();
        
        protected IList<T> value { get; private set; }
        
        /// <summary>
        /// The number of elements contained in the list.
        /// </summary>
        public int Count => value.Count;

        protected virtual void OnEnable()
        {
            value ??= CreateValue(_values.Count);
            
            Copy(_values, value);   
        }

        private void OnValidate()
        {
            OnEnable();
        }

        /// <summary>
        /// Returns the collection instance to use for storing values. By default, uses the internal array.
        /// Override for custom collection types.
        /// </summary>
        /// <param name="capacity">The capacity determined by the serialized internal array.</param>
        protected virtual IList<T> CreateValue(int capacity) => _values;

        public override IList<T> GetValue() => value;

        public T GetValue(int index) => value[index];

        public override void SetValue(IList<T> newValue) => value = newValue;

        public void SetValue(int index, T newValue) => value[index] = newValue;
        
        public void Add(T newValue)
        {
            value.Add(newValue);
        }

        public void Remove(T valueToRemove)
        {
            value.Remove(valueToRemove);
        }
        
        public void RemoveAt(int index)
        {
            value.RemoveAt(index);
        }
        
        public T this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }
        
        private static void Copy(List<T> source, IList<T> destination)
        {
            if (ReferenceEquals(source, destination))
                return;
            
            if (destination is List<T> concrete)
                concrete.Capacity = source.Capacity;
            
            destination.Clear();
            for (int i = 0; i < source.Count; i++)
                destination.Add(source[i]);
        }
    }
}
