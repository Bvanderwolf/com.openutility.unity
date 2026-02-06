using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OpenUtility.Samples.Data
{
    public class AsteroidSpawnManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _spawnArea;

        [SerializeField]
        private RectTransform _spawner;

        [Header("Project References")]
        [SerializeField]
        private ScriptableDifficulty _difficulty;

        [SerializeField]
        private ScriptablePool _asteroidPool;

        [SerializeField]
        private SpriteList _asteroidSprites;

        [Header("Settings")]
        [SerializeField, Tooltip("The speed of spawned asteroids moving down.")]
        private FloatRange _asteroidSpeed;

        [SerializeField]
        private float _spawnerMoveSpeed;

        private const float MIN_SPAWN_TIME = 0.5f;
        private const float MAX_SPAWN_TIME = 2.5f;
        private float _nextSpawnTime;
        
        private float _pingPongTime;

        private void Start()
        {
            _nextSpawnTime = Time.time + MAX_SPAWN_TIME;
            _asteroidPool.SetParent(_spawnArea);
        }

        private void Update()
        {
            UpdateSpawnerPosition();
            CheckForAsteroidSpawn();
        }

        public float GetSpeed()
        {
            float difficulty = _difficulty.GetValue();
            float speed = _asteroidSpeed.GetValue(difficulty);
            
            return (speed);
        }

        public Sprite GetSprite()
        {
            Sprite sprite = _asteroidSprites.GetRandom();

            return (sprite);
        }

        private void UpdateSpawnerPosition()
        {
            _pingPongTime += Time.deltaTime * _spawnerMoveSpeed;
            
            Vector2 position = _spawner.anchoredPosition;
            float width = _spawnArea.rect.width - _spawner.rect.width;
            float x = Mathf.PingPong(_pingPongTime, width);
            
            position.x = x;
            
            _spawner.anchoredPosition = position;
        }

        private void CheckForAsteroidSpawn()
        {
            float time = Time.time;

            if (time < _nextSpawnTime)
                return;

            PoolGameObject component = _asteroidPool.Get();
            component.transform.position = _spawner.position;
            
            AsteroidBehaviour behaviour = component.GetComponent<AsteroidBehaviour>();
            behaviour.OnSpawn(this);

            ResetSpawnTime(time);
        }


        private void ResetSpawnTime(float time)
        {
            _nextSpawnTime = time + Random.Range(MIN_SPAWN_TIME, MAX_SPAWN_TIME);
        }
    }
}
