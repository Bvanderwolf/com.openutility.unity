using UnityEngine;

namespace OpenUtility.Samples.Pooling.Custom
{
    [CreateAssetMenu(fileName = "EntitySettings", menuName = "Scriptable Objects/EntitySettings")]
    public class EntitySettings : ScriptableObject
    {
        public int health;
        public float speed;
    }
}
