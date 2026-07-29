using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "PlayerHeal", menuName = "OpenUtility/Samples/PlayerHeal")]
    public class PlayerHeal : ScriptablePlayerItem
    {
        [SerializeField]
        private ScriptableInt _health;

        [Header("Settings")]
        [SerializeField, Min(1)]
        private int _amount;

        public override void OnApply(GameObject playerObject)
        {
            _health.Increment(_amount);
        }
    }
}
