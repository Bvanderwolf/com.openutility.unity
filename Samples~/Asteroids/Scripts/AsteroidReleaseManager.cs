using System.Collections.Generic;
using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class AsteroidReleaseManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _releaseArea;
        
        [Header("Project References")]
        [SerializeField]
        private PoolGameObjectList _asteroids;

        private void FixedUpdate()
        {
            IList<PoolGameObject> list = _asteroids.GetValue();
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                PoolGameObject asteroid = list[i];
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(_releaseArea, asteroid.transform.position))
                    continue;

                asteroid.Release();
            }
        }
    }
}
