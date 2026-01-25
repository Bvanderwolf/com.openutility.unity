using UnityEngine;

namespace OpenUtility.Samples.Pooling.Custom
{
    public class EntityBehaviour : MonoBehaviour
    {
        public void Initialize(EntitySettings settings)
        {
            Debug.Log($"Initializing entity with Health: {settings.health}, Speed: {settings.speed}");
        }

        public void DeInitialize()
        {
            Debug.Log("De-initializing entity");
        }
    }
}
