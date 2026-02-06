using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class SpaceCraftBehaviour : MonoBehaviour
    {
        private readonly struct ManagedWeaponUpgrade
        {
            public readonly WeaponUpgrade upgrade;
            private readonly float _startTime;

            public ManagedWeaponUpgrade(WeaponUpgrade upgrade)
            {
                this.upgrade = upgrade;
                _startTime = Time.time;
            }

            public bool CheckTime(float time) => time > _startTime + upgrade.Duration;
        }
        
        [Header("Scene References")]
        [SerializeField]
        private Transform[] _bulletSpawnAreas;
        
        [Header("Project References")]
        [SerializeField]
        private ScriptablePool _bulletPool;
        
        [SerializeField]
        private ScriptableInt _health;

        [SerializeField]
        private ScriptableStreak _streak;
        
        [Header("Settings")]
        [SerializeField]
        private float _spawnFrequency = 0.5f;
        
        private float _currentSpawnTime;

        private readonly List<ManagedWeaponUpgrade> _timers = new List<ManagedWeaponUpgrade>();

        private void Start()
        {
            _bulletPool.SetParent(transform.parent);
        }

        private void Update()
        {
            float time = Time.time;
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                ManagedWeaponUpgrade timer = _timers[i];

                if (!timer.CheckTime(time))
                    continue;
                
                timer.upgrade.OnRemove(gameObject);
                
                _timers.RemoveAt(i);
            }
        }

        private void FixedUpdate()
        {
            _currentSpawnTime += Time.deltaTime;

            if (_currentSpawnTime < _spawnFrequency)
                return;

            _currentSpawnTime = 0.0f;
            
            SpawnBullets();
        }
        
        public bool TryAddWeaponUpgrade(WeaponUpgrade upgrade)
        {
            for (int i = 0; i < _timers.Count; i++)
            {
                if (_timers[i].upgrade == upgrade)
                    return (false);
            }
            
            _timers.Add(new ManagedWeaponUpgrade(upgrade));
            
            return (true);
        }

        public bool TryDestroy(AsteroidBehaviour asteroid)
        {
            int damage = asteroid.GetDamage();

            _health.Decrement(damage);
            _streak.ResetStreak();

            return (_health.GetValue() <= 0);
        }

        private void SpawnBullets()
        {
            for (int i = 0; i < _bulletSpawnAreas.Length; i++)
            {
                Transform area = _bulletSpawnAreas[i];
                Vector3 position = area.position;

                PoolGameObject component = _bulletPool.Get();
                component.transform.position = position;
            }
        }
    }
}
