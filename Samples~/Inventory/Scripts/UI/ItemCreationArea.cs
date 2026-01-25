using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class ItemCreationArea : MonoBehaviour
    {
        [Header("Project Referencs")]
        [SerializeField]
        private ItemList _items;

        [SerializeField]
        private GameObject _entryPrefab;

        [Header("Scene References")]
        [SerializeField]
        private Transform _entryparent;
        
        private void Start()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Item item = _items.GetValue(i);
                GameObject entryObject = Instantiate(_entryPrefab, _entryparent);
                ItemCreationEntry entry = entryObject.GetComponent<ItemCreationEntry>();
                entry.Setup(item);
            }
        }
    }
}
