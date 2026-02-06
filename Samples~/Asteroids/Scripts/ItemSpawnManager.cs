using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class ItemSpawnManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _spawnArea;

        [SerializeField]
        private RectTransform _spawner;
        
        [Header("Project References")]
        [SerializeField]
        private ScriptablePool _itemPool;
        
        [SerializeField]
        private ScriptableDifficulty _difficulty;

        [SerializeField]
        private ScriptableItem[] _items;
        
        [Header("Settings")]
        [SerializeField]
        private FloatRange _spawnInterval;

        private const float SPAWNER_MOVE_SPEED = 100f;
        
        private float _nextSpawnTime;
        private float _pingPongTime;

        private void Start()
        {
            _nextSpawnTime = Time.time + _spawnInterval.From;
            _itemPool.SetParent(_spawnArea);
        }
        
        private void Update()
        {
            UpdateSpawnerPosition();
            CheckForItemSpawn();
        }

        public IAffectPlayer GetEffect()
        {
            int length = _items.Length;
            int index = Random.Range(0, length);

            return (_items[index]);
        }
        
        private void UpdateSpawnerPosition()
        {
            _pingPongTime += Time.deltaTime * SPAWNER_MOVE_SPEED;
            
            Vector2 position = _spawner.anchoredPosition;
            float width = _spawnArea.rect.width - _spawner.rect.width;
            float x = Mathf.PingPong(_pingPongTime, width);
            
            position.x = x;
            
            _spawner.anchoredPosition = position;
        }

        private void CheckForItemSpawn()
        {
            float time = Time.time;

            if (time < _nextSpawnTime)
                return;

            PoolGameObject component = _itemPool.Get();
            component.transform.position = _spawner.position;
            
            ItemBehaviour behaviour = component.GetComponent<ItemBehaviour>();
            behaviour.OnSpawn(this);

            ResetSpawnTime(time);
        }


        private void ResetSpawnTime(float time)
        {
            float difficulty = _difficulty.GetValue();
            _nextSpawnTime = time + _spawnInterval.GetValue(difficulty);
        }
    }
}
