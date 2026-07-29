using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "Item", menuName = "OpenUtility/Inventory/Item")]
    public class ScriptableItem : ScriptableObject
    {
        [Header("Required")]
        [SerializeField]
        private Sprite _sprite;
        
        [Header("Optional")]
        [SerializeField]
        private Optional<Vector3> _scale;

        [SerializeField]
        private Optional<Vector3> _rotation;

        [SerializeField]
        private Optional<int> _stackLimit;
        
        public Sprite Sprite => _sprite;
        public Vector3 Scale => _scale.GetValueOrDefault(Vector3.one);
        public Vector3 Rotation => _rotation.GetValueOrDefault(Vector3.zero);
        public int StackLimit => _stackLimit.GetValueOrDefault(ScriptableInventory.DEFAULT_STACK_LIMIT);

        public ItemBundle Bundle() => (new(this));
    }
}
