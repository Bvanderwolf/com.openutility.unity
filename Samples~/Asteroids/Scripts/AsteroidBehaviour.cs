using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using OpenUtility.Hierarchy;
using OpenUtility.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class AsteroidBehaviour : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _player;
        
        [Header("Scene References")]
        [SerializeField]
        private Image _image;

        [Header("Settings")]
        [SerializeField]
        private int _health = 4;

        [SerializeField, Min(1)]
        private int _damage = 1;

        private float _speed = 1f;
        private const float BASE_SPEED_MODIFIER = 150f;
        private Vector2 direction = Vector2.down;

        private int _initialHealth;

        private void Awake()
        {
            _initialHealth = _health;
        }

        private void OnEnable()
        {
            _health = _initialHealth;
            _image.SetVisible();
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }

        private void FixedUpdate()
        {
            if (!_player.HasValue)
                return;

            RectTransform playerRect = (RectTransform)_player.GetValue().transform;
            RectTransform asteroidRect = (RectTransform)transform;
            if (!asteroidRect.Overlaps(playerRect))
                return;
            
            gameObject.Release();
            
            SpaceCraftBehaviour behaviour = _player.GetValue().GetComponent<SpaceCraftBehaviour>();
            if (!behaviour.TryDestroy(this))
                return;

            _player.DestroyValue();
        }

        public int GetDamage() => _damage;

        public void OnSpawn(AsteroidSpawnManager manager)
        {
            SetSpeed(manager);
            SetSprite(manager);
        }

        public bool TryDestroy(BulletBehaviour bullet)
        {
            int damage = bullet.GetDamage();

            _health -= damage;
            
            float newTransparency = ((float)_health / _initialHealth) * 255f;
            _image.SetTransparency(newTransparency);

            return (_health <= 0);
        }

        private void Update()
        {
            Vector3 translation = direction * (BASE_SPEED_MODIFIER * _speed * Time.deltaTime);
            transform.Translate(translation);
        }

        private void SetSpeed(AsteroidSpawnManager manager)
        {
            _speed = manager.GetSpeed();
        }
        
        private void SetSprite(AsteroidSpawnManager manager)
        {
            _image.sprite = manager.GetSprite();
            _image.SetNativeSize();

            transform.localScale *= 0.5f;
        }
    }
}
