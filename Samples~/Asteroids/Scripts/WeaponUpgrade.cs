using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "OpenUtility/Samples/WeaponUpgrade")]
    public class WeaponUpgrade : ScriptableItem
    {
        [SerializeField]
        private ScriptablePool _bulletPool;

        [SerializeField]
        private Sprite _bulletSprite;
        
        [Header("Settings")]
        [SerializeField, Min(1.1f)]
        private float _damageMultiplier = 2.0f;

        [SerializeField]
        private float _duration = 10.0f;
        
        public float Duration => _duration;

        public override void OnApply(GameObject playerObject)
        {
            SpaceCraftBehaviour behaviour = playerObject.GetComponent<SpaceCraftBehaviour>();
            if (!behaviour.TryAddWeaponUpgrade(this))
                return;
            
            _bulletPool.InstanceRetrieved += UpgradeSpawnedBullet;
        }

        public override void OnRemove(GameObject playerObject)
        { 
            _bulletPool.InstanceRetrieved -= UpgradeSpawnedBullet;
        }

        private void UpgradeSpawnedBullet(PoolGameObject component)
        {
            BulletBehaviour behaviour = component.GetComponent<BulletBehaviour>();
            behaviour.SetSprite(_bulletSprite);
            behaviour.IncreaseDamage(_damageMultiplier);
        }
    }
}
