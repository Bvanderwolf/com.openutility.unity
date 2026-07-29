using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class InventoryUIManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableInventory _inventory;

        [SerializeField]
        private ScriptablePool _slotPool;
        
        [Header("Scene References")]
        [SerializeField]
        private Transform _grid;

        private void Awake()
        {
            _slotPool.SetParent(_grid);
        }

        private void Start()
        {
            for (int i = 0; i < _inventory.Size; i++)
            {
                PoolGameObject component = _slotPool.Get();
                InventorySlotUIBehaviour behaviour = component.GetComponent<InventorySlotUIBehaviour>();

                InventorySlot slot = _inventory.GetValue(i);
                behaviour.SetupFromInventorySlot(slot);
            }
        }
    }
}
