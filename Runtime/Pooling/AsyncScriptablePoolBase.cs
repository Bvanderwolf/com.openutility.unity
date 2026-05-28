using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace OpenUtility.Data.Pooling
{
    public abstract class AsyncScriptablePoolBase<T> : ScriptableObject where T : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Collection checks are performed when an instance is returned back to the pool. An exception will be thrown if the instance is already in the pool. Collection checks are only performed in the Editor.")]
        private bool _collectionCheck;

        [SerializeField, Tooltip("The default capacity the stack will be created with.")]
        private int _defaultCapacity = 10;

        [SerializeField, Tooltip("The maximum size of the pool. When the pool reaches the max size then any further instances returned to the pool will be ignored and can be garbage collected. This can be used to prevent the pool growing to a very large size.")]
        private int _maxSize = 100;
        
        [SerializeField, Tooltip("If the scene the pool is used in, is unloaded, the pool will be cleared automatically. Only set to false if you are certain the pooled game objects will stay alive across scene loads.")]
        private bool _clearOnSceneUnload = true;
        
        /// <summary>
        /// The internal object pool instance.
        /// </summary>
        protected ObjectPool<Promised<T>> pool { get; private set; }
        
        /// <summary>
        /// The parent transform for pooled instances.
        /// </summary>
        protected Optional<Transform> parent { get; private set; }
        
        /// <summary>
        /// The scene this pool is currently being used in.
        /// </summary>
        protected Scene? scene { get; private set; }
        
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Clear();
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void Clear()
        {
            pool?.Clear();
        }

        /// <summary>
        /// Sets the parent transform for pooled instances.
        /// </summary>
        public void SetParent(Transform transform)
        {
            parent = transform;
        }

        public Promised<T> Get()
        {
            pool = GetOrCreatePool();

            bool createsNewInstance = pool.CountInactive == 0;
            Promised<T> promise = pool.Get();
            
            if (parent.HasValue && promise.Value.transform.parent != parent.Value)
                promise.Then(SetParentOfPromisedGameObject);

            bool createdFirstInstance = createsNewInstance && pool.CountAll == 1;
            if (createdFirstInstance && _clearOnSceneUnload)
                promise.Then(SetSceneFromPromisedGameObject);

            if (createsNewInstance)
                promise.Then(CallbackOnCreatedBehaviourIfPossible);
            
            return (promise);
        }

        public PooledObject<Promised<T>> Get(out Promised<T> promise)
        {
            pool = GetOrCreatePool();

            bool createsNewInstance = pool.CountInactive == 0;
            PooledObject<Promised<T>> pooled = pool.Get(out promise);
            
            if (parent.HasValue)
                promise.Then(SetParentOfPromisedGameObject);
            
            bool createdFirstInstance = createsNewInstance && pool.CountAll == 1;
            if (createdFirstInstance && _clearOnSceneUnload)
                promise.Then(SetSceneFromPromisedGameObject);
            
            if (createsNewInstance)
                promise.Then(CallbackOnCreatedBehaviourIfPossible);

            return (pooled);
        }

        public virtual bool Release(Promised<T> promise)
        {
            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object to a pool that hasn't been created yet.");
                return (false);
            }

            pool.Release(promise);
            return (true);
        }

        private ObjectPool<Promised<T>> GetOrCreatePool()
        {
            return (pool ??= new ObjectPool<Promised<T>>(
                OnCreatePromise,
                OnGetPromise,
                OnReleasePromise,
                OnDestroyInstance,
                _collectionCheck,
                _defaultCapacity,
                _maxSize
            ));
        }

        private void OnSceneUnloaded(Scene unloadedScene)
        {
            if (!_clearOnSceneUnload)
                return;
            
            if (!scene.HasValue || unloadedScene != scene.Value)
                return;
            
            scene = null;
            Clear();
        }

        protected abstract Promised<T> OnCreatePromise();

        protected abstract void OnGetPromise(Promised<T> promise);

        protected abstract void OnReleasePromise(Promised<T> promise);

        private void OnDestroyInstance(Promised<T> promise)
        {
            if (promise.HasValue)
            {
                DestroyInstance(promise.Value);
            }
            else
            {
                promise.Then(DestroyInstance);
            }

            void DestroyInstance(T instance)
            {
                if (instance == null)
                    return;
                
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(instance.gameObject);
                }
                else
                {
                    DestroyImmediate(instance.gameObject);
                }
#else
                Destroy(instance.gameObject);
#endif 
            }
        }

        private void SetParentOfPromisedGameObject(T instance)
        {
            if (instance.transform.parent == parent.Value)
                return;
            
            instance.transform.SetParent(parent.Value);
        }

        private void SetSceneFromPromisedGameObject(T instance)
        {
            scene = instance.gameObject.scene;
        }

        private void CallbackOnCreatedBehaviourIfPossible(T instance)
        {
            if (instance is IPoolAsyncGameObject<T> behaviour)
                behaviour.OnCreatedByPool(this);
        }
    }
}
