using System.Collections.Generic;
using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class BulletReleaseManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _releaseArea;
        
        [Header("Project References")]
        [SerializeField]
        private PoolGameObjectList _bullets;

        private void FixedUpdate()
        {
            IList<PoolGameObject> list = _bullets.GetValue();
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                PoolGameObject bullet = list[i];
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(_releaseArea, bullet.transform.position))
                    continue;

                bullet.Release();
            }
        }
    }
}
