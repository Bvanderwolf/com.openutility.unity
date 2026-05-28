using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "PoolAsyncGameObjectList", menuName = "OpenUtility/Pooling/Async GameObject List", order = 1)]
    public class PoolAsyncGameObjectList : ScriptableList<PoolAsyncGameObject>
    {
        public event Action<ListChangeEvent, PoolAsyncGameObject> Changed;

        public override void Add(PoolAsyncGameObject newValue)
        {
            base.Add(newValue);
            
            Changed?.Invoke(ListChangeEvent.ADDITION, newValue);
        }

        public override void Remove(PoolAsyncGameObject valueToRemove)
        {
            base.Remove(valueToRemove);
            
            Changed?.Invoke(ListChangeEvent.REMOVAL, valueToRemove);
        }

        public override void RemoveAt(int index)
        {
            PoolAsyncGameObject instance = GetValue(index);
            
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
            IList<PoolAsyncGameObject> instance = GetValue();
            
            if (instance.Count == 0)
                return;

            for (int i = instance.Count - 1; i >= 0; i--)
                instance[i].Release();
        }
    }

}