using OpenUtility.DelayedExecution;
using OpenUtility.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class InventorySlotUIBehaviour : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableInventory _inventory;

        [SerializeField]
        private GameObject _itemBundlePrefab;

        [Header("Scene References")]
        [SerializeField]
        private RectTransform _itemFrame;

        [SerializeField]
        private GameObject _emptyItemFrame;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _stackCount;

        public ItemBundle CurrentBundleTaken => _currentBundleTaken.GetValueOrDefault();
        
        private readonly Vector2 emptyFrameSize = new Vector2(48f, 48f);

        private ItemBundle? _currentBundleTaken;
        private bool _currentBundleDrag;
        
        public bool CanAcceptBundle(ItemBundle bundle)
        {
            int index = transform.GetSiblingIndex();
            InventorySlot slot = _inventory.GetValue(index);
            if (slot.IsEmpty)
                return true;

            if (slot.item.Value != bundle.item.Value)
                return false;

            if (slot.stackCount + bundle.stackCount > slot.stackLimit)
                return false;

            return true;
        }

        public void SetupFromInventorySlot()
        {
            int index = transform.GetSiblingIndex();
            InventorySlot slot = _inventory.GetValue(index);
            
            SetupFromInventorySlot(slot);
        }

        public void SetupFromInventorySlot(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                _itemFrame.localScale = Vector3.one;
                _itemFrame.localEulerAngles = Vector3.zero;
                
                _icon.sprite = null;
                _icon.gameObject.SetActive(false);
                
                _stackCount.text = string.Empty;
                _stackCount.gameObject.SetActive(false);

                _emptyItemFrame.SetActive(true);

                SetFrameSize(emptyFrameSize);
            }
            else
            {
                ScriptableItem item = slot.item.Value;

                _itemFrame.localScale = item.Scale;
                _itemFrame.localEulerAngles = item.Rotation;

                _icon.gameObject.SetActive(true);
                _icon.sprite = item.Sprite;

                _stackCount.gameObject.SetActive(true);
                SetStackCountForActiveSlot(slot);

                _emptyItemFrame.SetActive(false);

                SetFrameSize(Vector2.zero);
            }
            
            _icon.SetVisible();
            _stackCount.SetVisible();
        }

        public void OnDraggedItemBundleEnter(ItemBundleUIBehaviour component)
        {
            int index = transform.GetSiblingIndex();
            InventorySlot slot = _inventory.GetValue(index);
            if (slot.IsEmpty)
            {
                OnOptionalItemBundleDrop(component.Bundle);
            }
            else
            {
                OnOptionalItemBundleIncrement(component.Bundle);
            }
        }

        public void OnDraggedItemBundleExit(ItemBundleUIBehaviour bundle)
        {
            SetupFromInventorySlot();
        }

        public void OnPointerDown(BaseEventData eventData)
        {
            bool leftShiftPressed = Keyboard.current.leftShiftKey.isPressed;
            int index = transform.GetSiblingIndex();
            int? count = leftShiftPressed ? null : 1;
            _currentBundleTaken = _inventory.TakeAt(index, count);

            InventorySlot slot = _inventory.GetValue(index);
            if (slot.IsEmpty)
            {
                WaitFor.Condition(this, IsBundleDragged, SetupFromInventorySlot);
            }
            else
            {
                SetupFromInventorySlot(slot);
            }
            
            bool IsBundleDragged(InventorySlotUIBehaviour instance) => instance._currentBundleDrag;
        }

        public void OnPointerUp(BaseEventData eventData)
        {
            if (_currentBundleDrag)
                return;

            OnReturnBundleTaken();
        }

        public void OnBeginDrag(BaseEventData eventData)
        {
            Transform body = transform.parent.parent;
            GameObject bundle = Instantiate(_itemBundlePrefab, body);
            bundle.transform.position = transform.position;

            ItemBundleUIBehaviour behaviour = bundle.GetComponent<ItemBundleUIBehaviour>();
            behaviour.OnBeginDrag((PointerEventData)eventData);
            behaviour.SetupFromSlot(this);

            _currentBundleDrag = true;
        }

        private void OnOptionalItemBundleDrop(ItemBundle bundle)
        {
            ScriptableItem item = bundle.item.Value;

            _itemFrame.localScale = item.Scale;
            _itemFrame.localEulerAngles = item.Rotation;
            
            _icon.sprite = item.Sprite;
            _icon.SetTransparency(100f);
            _icon.gameObject.SetActive(true);
            
            _stackCount.SetTransparency(100f);
            _stackCount.gameObject.SetActive(true);
            SetStackCountForActiveSlot(bundle);

            _emptyItemFrame.SetActive(false);
            
            SetFrameSize(Vector2.zero);
        }

        private void OnOptionalItemBundleIncrement(ItemBundle bundle)
        {
            int index = transform.GetSiblingIndex();
            InventorySlot slot = _inventory.GetValue(index);
            if (slot.item.Value != bundle.item.Value) 
                return;

            if (slot.stackCount + bundle.stackCount > slot.stackLimit)
                return;
            
            var current = ItemBundle.FromSlot(slot);
            var newBundle = current.Add(bundle);
            OnOptionalItemBundleDrop(newBundle);
        }

        public void OnItemBundleDrop(ItemBundle bundle)
        {
            int index = transform.GetSiblingIndex();
            
            _inventory.Return(index, bundle);
            
            SetupFromInventorySlot();
        }

        public void OnLoseBundleTaken()
        {
            _currentBundleTaken = null;
            _currentBundleDrag = false;
        }

        public void OnReturnBundleTaken()
        {
            if (!_currentBundleTaken.HasValue)
                return;

            ItemBundle bundle = _currentBundleTaken.Value;
            int index = transform.GetSiblingIndex();
           
            _inventory.Return(index, bundle);

            InventorySlot slot = _inventory.GetValue(index);
            SetupFromInventorySlot(slot);

            _currentBundleTaken = null;
            _currentBundleDrag = false;
        }

        private void SetFrameSize(Vector2 size)
        {
            _itemFrame.sizeDelta = size;
        }
        
        private void SetStackCountForActiveSlot(ItemBundle bundle)
        {
            int index = transform.GetSiblingIndex();
            InventorySlot slot = _inventory.GetValue(index);
            int potential = bundle.stackCount;
            int max = slot.IsEmpty ? _inventory.GetStackLimit(bundle.item.Value) : slot.stackLimit;
            
            _stackCount.text = $"{potential}/{max}";
        }

        private void SetStackCountForActiveSlot(InventorySlot slot)
        {
            int current = slot.stackCount;
            if (current == 1)
            {
                _stackCount.text = string.Empty;
            }
            else
            {
                int max = slot.stackLimit;
                _stackCount.text = $"{current}/{max}";
            }
        }
    }
}
