using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data
{
    /// <summary>
    /// A base class for creating scriptable list variables of various key-value types.
    /// </summary>
    public abstract class ScriptableList<T> : ScriptableVariable<IList<T>>
    {
        [Header("State")]
        [SerializeField, Tooltip("The values used to start the list with.")]
        private List<T> _values = new List<T>();

        [Header("Settigs")]
        [SerializeField, Tooltip("Optionally determine capacity by this value instead of the 'values' property")]
        private Optional<int> _capacity;

        [SerializeField, Tooltip("Remove mono behaviours from the list if they are destroyed, leaving no null references.")]
        private bool _cleanupOnDestroy;

        [field:NonSerialized]
        protected IList<T> value { get; private set; }
        
        /// <summary>
        /// The number of elements contained in the list.
        /// </summary>
        public int Count => value.Count;

        protected virtual void OnEnable()
        {
            value ??= CreateValue(_capacity.HasValue ? _capacity.Value : _values.Count);
            
            Copy(_values, value);   
        }

        private void OnValidate()
        {
            OnEnable();
        }

        /// <summary>
        /// Returns the collection instance to use for storing values. By default, uses the serialized list property.
        /// Override for custom collection types.
        /// </summary>
        /// <param name="capacity">The capacity determined by the serialized list property in the inspector.</param>
        protected virtual IList<T> CreateValue(int capacity)
        {
#if UNITY_EDITOR
            if (capacity == _values.Count) 
                return (new List<T>(_values));
            
            var list = new List<T>(capacity);
            list.AddRange(_values);
            return (list);

#else
            if (capacity == _values.Count)
                 return (_values);
                
            _values.Capacity = capacity;
            return (_values);
#endif
        }

        public override IList<T> GetValue() => value;

        public T GetValue(int index) => value[index];

        public override void SetValue(IList<T> newValue) => value = newValue;

        public void SetValue(int index, T newValue) => value[index] = newValue;
        
        public void Add(T newValue)
        {
            if (_cleanupOnDestroy && newValue is MonoBehaviour behaviour)
            {
                behaviour.destroyCancellationToken.Register(OnDestroy, behaviour);

                void OnDestroy(object instance) => Remove((T)instance);
            }
            
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
            
            destination.Clear();
            
            if (destination is List<T> concrete)
                concrete.Capacity = source.Capacity;
            
            for (int i = 0; i < source.Count; i++)
                destination.Add(source[i]);
        }
    }
}
