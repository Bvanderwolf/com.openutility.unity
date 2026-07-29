using System;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public class AsyncPoolSpawnArea : MonoBehaviour
    {
        [Serializable]
        private class SpawnEvent : UnityEngine.Events.UnityEvent<GameObject> { }
        
        [Header("Project References")]
        [SerializeField, Tooltip("The pool to spawn game objects from.")]
        private AsyncScriptablePool _pool;

        [Header("Scene References")]
        [SerializeField]
        private Optional<Transform> _parent;

        [Header("Events")]
        [SerializeField]
        private SpawnEvent _spawnEvent;

        public AsyncScriptablePool Pool => _pool;

        private void Awake()
        {
            if (_parent.HasValue)
            {
                Transform parent = _parent.Value;
                
                _pool.SetParent(parent);
                _pool.SetPositionAndRotation(parent.position, parent.rotation);
            }
            else
            {
                _pool.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }

        public void Spawn()
        {
            _pool.Get().Then(OnSpawned).Catch(p => Debug.LogError(p.Error));
        }

        private void OnSpawned(PoolAsyncGameObject component)
        {
            component.transform.position = _parent.TryGetValue(out Transform parent) ? parent.position : transform.position;
            
            _spawnEvent?.Invoke(component.gameObject);
        }
    }
}
