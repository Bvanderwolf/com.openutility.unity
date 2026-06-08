using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public class PoolSpawnArea : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField, Tooltip("The pool to spawn game objects from.")]
        private ScriptablePool _pool;

        [Header("Scene References")]
        [SerializeField]
        private Optional<Transform> _parent;

        private void Awake()
        {
            if (_parent.HasValue)
                _pool.SetParent(_parent.Value);
        }

        public void Spawn()
        {
            _pool.Get();
        }
    }
}
