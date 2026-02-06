using System;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using OpenUtility.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class BulletBehaviour : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private Image _image;
        
        [Header("Project References")]
        [SerializeField]
        private PoolGameObjectList _asteroids;

        [SerializeField]
        private ScriptableStreak _streak;

        [SerializeField]
        private ScriptableInt _score;

        [Header("Settings")]
        [SerializeField, Min(50.0f)]
        private float _speed = 100f;
        
        [SerializeField, Min(1.0f)]
        private float _damage;
        
        private float _damageMultiplier = 1.0f;
        private Sprite _defaultSprite;

        private void Awake()
        {
            _defaultSprite = _image.sprite;
        }

        private void OnDisable()
        {
            _image.sprite = _defaultSprite;
            _damageMultiplier = 1.0f;
        }

        private void Update()
        {
            Vector3 translation = Vector3.up * (Time.deltaTime * _speed);
            transform.Translate(translation);
        }

        private void FixedUpdate()
        {
            Optional<PoolGameObject> asteroid = FindOverlappingAsteroid();
            if (!asteroid.HasValue)
                return;

            AsteroidBehaviour behaviour = asteroid.Value.GetComponent<AsteroidBehaviour>();
            if (behaviour.TryDestroy(this))
            {
                _score.Increment();
                
                asteroid.Value.Release();
            }
            else
            {
                _streak.KeepAlive();
            }

            gameObject.Release();
        }

        public int GetDamage()
        {
            return (Mathf.RoundToInt(_damage * _damageMultiplier));
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void IncreaseDamage(float multiplier)
        {
            _damageMultiplier += multiplier;
        }

        private Optional<PoolGameObject> FindOverlappingAsteroid()
        {
            IList<PoolGameObject> list = _asteroids.GetValue();
            RectTransform thisTransform = (RectTransform)transform;
            
            for (int i = 0; i < list.Count; i++)
            {
                PoolGameObject asteroid = list[i];
                RectTransform asteroidTransform = (RectTransform)asteroid.transform;
                if (!thisTransform.Overlaps(asteroidTransform))
                    continue;

                return (asteroid);
            }

            return (Optional<PoolGameObject>.None());
        }
    }
}
