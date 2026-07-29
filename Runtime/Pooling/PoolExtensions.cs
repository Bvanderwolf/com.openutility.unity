using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public static class PoolExtensions
    {
        /// <summary>
        /// Releases the GameObject back to its pool if any of its components implement IPoolGameObject.
        /// </summary>
        public static bool Release(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<IPoolGameObject>(out var component))
                return component.TryRelease();

            return (false);
        }
    }
}
