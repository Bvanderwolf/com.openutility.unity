using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class ItemCreationEntry : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private GameObject _itemBundlePrefab;
        
        [Header("Scene References")]
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _title;

        [SerializeField]
        private GameObject _frame;

        public ScriptableItem Item { get; private set; }

        public void Setup(ScriptableItem item)
        {
            _icon.sprite = item.Sprite;
            _title.text = item.name;
            
            Item = item;
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            _frame.SetActive(true);
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            _frame.SetActive(false);
        }

        public void OnBeginDrag(BaseEventData eventData)
        {
            Transform body = GetComponentInParent<ScrollRect>().transform.parent;
            GameObject bundle = Instantiate(_itemBundlePrefab, body);

            PointerEventData pointerData = (PointerEventData)eventData;
            RectTransform bundleRect = bundle.GetComponent<RectTransform>();
            bundleRect.position = pointerData.position;

            ItemBundleUIBehaviour behaviour = bundle.GetComponent<ItemBundleUIBehaviour>();
            behaviour.OnBeginDrag((PointerEventData)eventData);
            behaviour.SetupFromCreationEntry(this);
        }
    }
}
