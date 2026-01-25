using OpenUtility.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class ItemRemovalArea : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private Sprite _openBinSprite;

        [SerializeField]
        private Sprite _closedBinSprite;

        [Header("Scene References")]
        [SerializeField]
        private Image _binImage;

        [SerializeField]
        private RectTransform _body;

        private bool _bundleEnteredBody = false;

        private void Update()
        {
            if (!ItemBundleUIBehaviour.DraggedInstance.HasValue)
                return;
            
            ItemBundleUIBehaviour bundle = ItemBundleUIBehaviour.DraggedInstance.Value;
            if (bundle.Origin != ItemCreationOrigin.Inventory)
                return;

            bool overlaps = RectTransformUtility.RectangleContainsScreenPoint(_body, Input.mousePosition);
            if (overlaps)
            {
                OnDraggedItemBundleOverlapsBody(bundle);
            }
            else
            {
                OnNoDraggedItemBundleBodyOverlap(bundle);
            }
        }

        private void OnDraggedItemBundleOverlapsBody(ItemBundleUIBehaviour bundle)
        {
            if (_bundleEnteredBody)
                return;

            bundle.OnRemovalAreaEntered(this);
            
            _binImage.sprite = _openBinSprite;
            _bundleEnteredBody = true;
        }

        private void OnNoDraggedItemBundleBodyOverlap(ItemBundleUIBehaviour bundle)
        {
            if (!_bundleEnteredBody)
                return;
            
            bundle.OnRemovalAreaExited(this);

            _binImage.sprite = _closedBinSprite;
            _bundleEnteredBody = false;
        }

        public void OnItemBundleDrop(ItemBundle bundle)
        {
            _binImage.sprite = _closedBinSprite;
            _bundleEnteredBody = false;
        }
    }
}
