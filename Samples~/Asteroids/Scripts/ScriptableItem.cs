using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public abstract class ScriptableItem : ScriptableObject, IAffectPlayer
    {
        [Header("Project References")]
        [SerializeField]
        private Sprite _sprite;

        public virtual void OnSpawn(ItemBehaviour item)
        {
            item.SetSprite(_sprite);
        }

        public abstract void OnApply(GameObject playerObject);
        
        public virtual void OnRemove(GameObject playerObject) { }
    }
}
