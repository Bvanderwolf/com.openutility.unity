using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public static class PoolExtensions
    {
        /// <summary>
        /// Releases the GameObject back to its pool if any of its components implement IPoolGameObject.
        /// </summary>
        public static void Release(this GameObject gameObject) => gameObject.GetComponent<IPoolGameObject>()?.Release();
    }
}
