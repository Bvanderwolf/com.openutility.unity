using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public enum ListChangeEvent
    {
        /// <summary>
        /// An entry has been added to the list. Expected contextual argument should be the new entry.
        /// </summary>
        ADDITION,
        
        /// <summary>
        /// An entry has been removed from the list. Expected contextual argument should be the removed entry.
        /// </summary>
        REMOVAL,
        
        /// <summary>
        /// The list has been cleared. Expected contextual argument should be null.
        /// </summary>
        CLEARED
    }
    
    [CreateAssetMenu(fileName = "PoolGameObjectList", menuName = "OpenUtility/Pooling/GameObject List", order = 1)]
    public class PoolGameObjectList : ScriptableList<PoolGameObject>
    {
        public event Action<ListChangeEvent, PoolGameObject> Changed;

        public override void Add(PoolGameObject newValue)
        {
            base.Add(newValue);
            
            Changed?.Invoke(ListChangeEvent.ADDITION, newValue);
        }

        public override void Remove(PoolGameObject valueToRemove)
        {
            base.Remove(valueToRemove);
            
            Changed?.Invoke(ListChangeEvent.REMOVAL, valueToRemove);
        }

        public override void RemoveAt(int index)
        {
            PoolGameObject instance = GetValue(index);
            
            base.RemoveAt(index);
            
            Changed?.Invoke(ListChangeEvent.REMOVAL, instance);
        }

        public override void Clear()
        {
            base.Clear();
            
            Changed?.Invoke(ListChangeEvent.CLEARED, null);
        }

        /// <summary>
        /// Releases all instances in this list.
        /// </summary>
        public void Release()
        {
            IList<PoolGameObject> instance = GetValue();
            
            if (instance.Count == 0)
                return;

            for (int i = instance.Count - 1; i >= 0; i--)
                instance[i].Release();
        }
    }
}
