using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    /// <summary>
    /// Inherit from this class to create a MonoBehaviour that can be pooled using a async scriptable pool.
    /// </summary>
    public abstract class PoolAsyncGameObjectBase<T> : MonoBehaviour, IPoolAsyncGameObject<T> where T : MonoBehaviour
    {
        /// <summary>
        /// The pool that created this instance.
        /// </summary>
        protected AsyncScriptablePoolBase<T> pool { get; private set; }
        
        /// <summary>
        /// The promise used to create this instance.
        /// </summary>
        protected Promised<T> promise { get; private set; }

        /// <summary>
        /// Called after Awake and before Start when this instance is created by the pool.
        /// </summary>
        public virtual void OnCreatedByPool(AsyncScriptablePoolBase<T> scriptablePool)
        {
            pool = scriptablePool;
        }

        /// <summary>
        /// Called after Awake and before Start and 'OnCreatedByPool' when this instance is created from a promise.
        /// </summary>
        public virtual void OnCreatedByPromise(Promised<T> promiseCreatedFrom)
        {
            promise = promiseCreatedFrom;
        }

        /// <summary>
        /// Releases this instance back to the pool. Returns true if successful.
        /// </summary>
        public virtual bool Release()
        {
            pool.Release(promise);
            return (true);
        }
    }
}