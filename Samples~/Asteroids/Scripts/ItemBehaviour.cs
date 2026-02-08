using System;
using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using OpenUtility.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public interface IAffectPlayer
    {
        void OnSpawn(ItemBehaviour item);
        void OnApply(GameObject playerObject);
        void OnRemove(GameObject playerObject);
    }
    
    public class ItemBehaviour : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _player;
        
        [Header("Scene References")]
        [SerializeField]
        private Image _image;
        
        private IAffectPlayer _effect;
        private const float BASE_SPEED_MODIFIER = 150f;
        private Vector2 direction = Vector2.down;

        private void Update()
        {
            Vector3 translation = direction * (BASE_SPEED_MODIFIER * Time.deltaTime);
            transform.Translate(translation);
        }

        private void FixedUpdate()
        {
            if (!_player.HasValue)
                return;

            GameObject player = _player.GetValue();
            RectTransform playerRect = (RectTransform)player.transform;
            RectTransform itemRect = (RectTransform)transform;
            if (!itemRect.Overlaps(playerRect))
                return;
            
            _effect.OnApply(player);
            gameObject.Release();
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
        
        public void OnSpawn(ItemSpawnManager manager)
        {
            _effect = manager.GetEffect();
            _effect.OnSpawn(this);
        }
    }
}
