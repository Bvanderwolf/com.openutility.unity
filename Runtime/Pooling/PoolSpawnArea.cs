using System.Collections;
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

        [Header("Settings")]
        [SerializeField]
        private IntReference _spawnCount;

        [SerializeField]
        private FloatReference _spawnDelay;

        private Coroutine _spawnRoutine;

        private void Awake()
        {
            if (_parent.HasValue)
                _pool.SetParent(_parent.Value);
        }

        public void Spawn() => Spawn(_spawnCount, _spawnDelay);

        public void Spawn(int spawnCount, float spawnDelay)
        {
            if (_spawnRoutine != null)
                StopCoroutine(_spawnRoutine);
            
            if (spawnCount > 1 && spawnDelay > 0f)
            {
                _spawnRoutine = StartCoroutine(SpawnRoutine(spawnCount, spawnDelay));
            }
            else
            {
                for (int i = 0; i < spawnCount; i++)
                    _pool.Get();
            }
        }

        private IEnumerator SpawnRoutine(int amount, float delay)
        {
            for (int i = 0; i < amount; i++)
            {
                _pool.Get();

                float endTime = Time.time + delay;
                while (Time.time < endTime)
                {
                    if (Application.exitCancellationToken.IsCancellationRequested)
                        yield break;

                    yield return null;
                }
            }
        }
    }
}
