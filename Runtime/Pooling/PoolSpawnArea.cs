using System;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public class PoolSpawnArea : MonoBehaviour
    {
        [Serializable]
        private class SpawnEvent : UnityEngine.Events.UnityEvent<GameObject> { }
        
        [Header("Project References")]
        [SerializeField, Tooltip("The pool to spawn game objects from.")]
        private ScriptablePool _pool;

        [Header("Scene References")]
        [SerializeField]
        private Optional<Transform> _parent;

        [Header("Events")]
        [SerializeField]
        private SpawnEvent _spawnEvent;

        public ScriptablePool Pool => _pool;

        private void Awake()
        {
            if (_parent.HasValue)
                _pool.SetParent(_parent.Value);
        }

        public void Spawn()
        {
            var component = _pool.Get();
            component.transform.position = _parent.TryGetValue(out Transform parent) ? parent.position : transform.position;
            
            _spawnEvent?.Invoke(component.gameObject);
        }

        public void Spawn(out PoolGameObject instance)
        {
            instance = _pool.Get();
            instance.transform.position = _parent.TryGetValue(out Transform parent) ? parent.position : transform.position;
            
            _spawnEvent?.Invoke(instance.gameObject);
        }
    }
}
