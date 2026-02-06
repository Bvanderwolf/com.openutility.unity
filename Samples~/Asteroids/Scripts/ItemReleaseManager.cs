using System.Collections.Generic;
using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class ItemReleaseManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _releaseArea;
        
        [Header("Project References")]
        [SerializeField]
        private PoolGameObjectList _items;
        
        private void FixedUpdate()
        {
            IList<PoolGameObject> list = _items.GetValue();
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                PoolGameObject item = list[i];
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(_releaseArea, item.transform.position))
                    continue;

                item.Release();
            }
        }
        
    }
}
