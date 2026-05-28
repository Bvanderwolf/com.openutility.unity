using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    /// <summary>
    /// Implement this interface to create a MonoBehaviour that can be pooled using AsyncScriptablePool.
    /// </summary>
    public interface IPoolAsyncGameObject<T> : IPoolGameObject where T : MonoBehaviour
    {
        /// <summary>
        /// Called after Awake and before Start when this instance is created by the pool.
        /// </summary>
        void OnCreatedByPool(AsyncScriptablePoolBase<T> pool);
    }
}
