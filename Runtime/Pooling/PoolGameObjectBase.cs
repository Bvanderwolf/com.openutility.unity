using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    /// <summary>
    /// Inherit from this class to create a MonoBehaviour that can be pooled using a scriptable pool.
    /// </summary>
    public abstract class PoolGameObjectBase<T> : MonoBehaviour, IPoolGameObject<T> where T : MonoBehaviour
    {
        /// <summary>
        /// The pool that created this instance.
        /// </summary>
        protected ScriptablePoolBase<T> pool { get; private set; }

        /// <summary>
        /// Called after Awake and before Start when this instance is created by the pool.
        /// </summary>
        public virtual void OnCreatedByPool(ScriptablePoolBase<T> scriptablePool)
        {
            pool = scriptablePool;

            Debug.Log($"Created new instance of {typeof(T).Name} by pool {scriptablePool.name}");
        }

        /// <summary>
        /// Releases this instance back to the pool. Returns true if successful.
        /// </summary>
        public virtual bool Release()
        {
            pool.Release(this as T);
            return (true);
        }
    }
}