using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public interface IPoolGameObject
    {
        /// <summary>
        /// Should release this instance back to the pool. Should return true if successful.
        /// </summary>
        bool Release();
    }

    /// <summary>
    /// Implement this interface to create a MonoBehaviour that can be pooled using ScriptablePool.
    /// </summary>
    public interface IPoolGameObject<T> : IPoolGameObject where T : MonoBehaviour
    {
        /// <summary>
        /// Called after Awake and before Start when this instance is created by the pool.
        /// </summary>
        void OnCreatedByPool(ScriptablePoolBase<T> pool);
    }
}