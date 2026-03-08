using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "PoolGameObjectList", menuName = "OpenUtility/Pooling/GameObject List", order = 1)]
    public class PoolGameObjectList : ScriptableList<PoolGameObject>
    {
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
