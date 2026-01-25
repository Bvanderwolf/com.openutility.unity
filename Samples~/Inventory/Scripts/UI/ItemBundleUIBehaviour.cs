using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public enum ItemCreationOrigin
    {
        None,
        Inventory,
        World
    }
    
    public class ItemBundleUIBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public static Optional<ItemBundleUIBehaviour> DraggedInstance { get; private set; }

        [Header("Project References")]
        [SerializeField]
        private Inventory _inventory;

        [SerializeField]
        private PoolGameObjectList _slotList;

        [Header("Scene References")]
        [SerializeField]
        private RectTransform _itemFrame;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _stackCount;

        public ItemBundle Bundle { get; private set; }
        public ItemCreationOrigin Origin { get; set; }

        private InventorySlotUIBehaviour _originalSlot;
        private Optional<ItemRemovalArea> _hoveredRemovalArea;
        private Optional<InventorySlotUIBehaviour> _hoveredSlot;

        private void Update()
        {
            if (!DraggedInstance.HasValue)
                return;

            if (DraggedInstance.Value != this)
                return;

            for (int i = 0; i < _slotList.Count; i++)
            {
                RectTransform rect = (RectTransform)_slotList.GetValue(i).transform;
                bool overlaps = RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
                if (overlaps)
                {
                    OnInventorySlotRectangleOverlaps(rect);
                }
                else
                {
                    OnNoInventorySlotRectangleOverlap(rect);
                }
            }
        }

        private void OnInventorySlotRectangleOverlaps(RectTransform rect)
        {
            if (_hoveredSlot.HasValue) 
                return;
            
            InventorySlotUIBehaviour slot = rect.GetComponent<InventorySlotUIBehaviour>();
            if (slot == _originalSlot || !slot.CanAcceptBundle(Bundle))
                return;
            
            slot.OnDraggedItemBundleEnter(this);

            _stackCount.text = string.Empty;
            _hoveredSlot = slot;
        }

        private void OnNoInventorySlotRectangleOverlap(RectTransform rect)
        {
            if (!_hoveredSlot.HasValue) 
                return;
            
            InventorySlotUIBehaviour slot = rect.GetComponent<InventorySlotUIBehaviour>();
            if (_hoveredSlot.Value != slot)
                return;

            slot.OnDraggedItemBundleExit(this);
                        
            _stackCount.text = Bundle.stackCount == 1 ? string.Empty : Bundle.stackCount.ToString();
            _hoveredSlot = Optional<InventorySlotUIBehaviour>.None();
        }

        public void SetupFromSlot(InventorySlotUIBehaviour slot)
        {
            ItemBundle bundle = slot.CurrentBundleTaken;
            Item item = bundle.item.Value;

            _itemFrame.localScale = item.Scale;
            _itemFrame.localEulerAngles = item.Rotation;
            _icon.sprite = item.Sprite;
            _stackCount.text = bundle.stackCount == 1 ? string.Empty : bundle.stackCount.ToString();

            _originalSlot = slot;

            Bundle = bundle;
            Origin = ItemCreationOrigin.Inventory;
        }
        
        public void SetupFromCreationEntry(ItemCreationEntry entry)
        {
            Item item = entry.Item;
            
            _itemFrame.localScale = item.Scale;
            _itemFrame.localEulerAngles = item.Rotation;
            _icon.sprite = item.Sprite;
            _stackCount.text = string.Empty;

            Bundle = item.Bundle();
            Origin = ItemCreationOrigin.World;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            eventData.pointerDrag = gameObject;
            
            DraggedInstance = this;
        }

        public void OnDrag(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_hoveredSlot.HasValue && _hoveredSlot.Value != _originalSlot && _hoveredSlot.Value.CanAcceptBundle(Bundle))
            {
                if (Origin == ItemCreationOrigin.Inventory)
                    _originalSlot.OnLoseBundleTaken();
                
                _hoveredSlot.Value.OnItemBundleDrop(Bundle);
            }
            else if (_hoveredRemovalArea.HasValue)
            {
                if (Origin == ItemCreationOrigin.Inventory)
                    _originalSlot.OnLoseBundleTaken();
                
                _hoveredRemovalArea.Value.OnItemBundleDrop(Bundle);
            }

            if (Origin == ItemCreationOrigin.Inventory)
            {
                _originalSlot.OnReturnBundleTaken();
                _originalSlot = null;
            }
            
            DraggedInstance = Optional<ItemBundleUIBehaviour>.None();
            Bundle = default;
            Origin = ItemCreationOrigin.None;
            
            Destroy(gameObject);
        }
        
        
        public void OnRemovalAreaEntered(ItemRemovalArea area)
        {
            _hoveredRemovalArea = area;
        }
        
        public void OnRemovalAreaExited(ItemRemovalArea area)
        {
            if (_hoveredRemovalArea.HasValue && _hoveredRemovalArea.Value != area)
                return;
            
            _hoveredRemovalArea = null;
        }
    }
}
