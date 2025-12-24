using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace OpenUtility.Data.Pooling
{
    /// <summary>
    /// Inherit from this class to create a scriptable object asset that manages a pool of MonoBehaviour instances.
    /// Implement IPoolGameObject{T} or inherit from PoolGameObject{T} to get a reference to the pool on creation.
    /// </summary>
    public abstract class ScriptablePoolBase<T> : ScriptableObject where T : MonoBehaviour
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
        protected ObjectPool<T> pool { get; private set; }
        
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

        public T Get()
        {
            pool = GetOrCreatePool();

            bool createsNewInstance = pool.CountInactive == 0;
            T instance = pool.Get();
            
            if (parent.HasValue)
                instance.transform.SetParent(parent.Value);

            bool createdFirstInstance = createsNewInstance && pool.CountAll == 1;
            if (createdFirstInstance && _clearOnSceneUnload)
                scene = instance.gameObject.scene;

            if (createsNewInstance && instance is IPoolGameObject<T> behaviour)
                behaviour.OnCreatedByPool(this);

            return (instance);
        }

        public PooledObject<T> Get(out T instance)
        {
            pool = GetOrCreatePool();

            bool createsNewInstance = pool.CountInactive == 0;
            PooledObject<T> pooled = pool.Get(out instance);
            
            if (parent.HasValue)
                instance.transform.SetParent(parent.Value);
            
            bool createdFirstInstance = createsNewInstance && pool.CountAll == 1;
            if (createdFirstInstance && _clearOnSceneUnload)
                scene = instance.gameObject.scene;

            if (createsNewInstance && instance is IPoolGameObject<T> behaviour)
                behaviour.OnCreatedByPool(this);

            return (pooled);
        }

        public virtual bool Release(T element)
        {
            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object to a pool that hasn't been created yet.");
                return (false);
            }

            pool.Release(element);
            return (true);
        }

        private ObjectPool<T> GetOrCreatePool()
        {
            return (pool ??= new ObjectPool<T>(
                OnCreateInstance,
                OnGetInstance,
                OnReleaseInstance,
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

        protected abstract T OnCreateInstance();
        protected abstract void OnGetInstance(T instance);
        protected abstract void OnReleaseInstance(T instance);

        protected virtual void OnDestroyInstance(T instance)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(instance);
            }
            else
            {
                DestroyImmediate(instance);
            }
#else
            Destroy(instance);
#endif
        }
    }
}
