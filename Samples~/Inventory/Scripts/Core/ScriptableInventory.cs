using System;
using System.Collections.Generic;
using System.Text;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public struct ItemBundle : IEquatable<ItemBundle>, IFormattable
    {
        public static ItemBundle Empty { get; } = new ItemBundle
        {
            item = Optional<ScriptableItem>.None(),
            stackCount = 0
        };
        
        /// <summary>
        /// Whether this bundle is empty.
        /// </summary>
        public bool IsEmpty => !item.HasValue;
        
        /// <summary>
        /// The reference to the item instance in the project.
        /// </summary>
        public Optional<ScriptableItem> item;

        /// <summary>
        /// The current stack count of the item.
        /// </summary>
        public int stackCount;

        /// <summary>
        /// Creates a new instance of an item bundle.
        /// </summary>
        /// <param name="item">The reference to the item instance in the project.</param>
        /// <param name="stackCount">The stack limit of the item.</param>
        public ItemBundle(ScriptableItem item, int stackCount = 1)
        {
            this.item = item;
            this.stackCount = stackCount;
        }

        public ItemBundle Add(ItemBundle other)
        {
            if (other.IsEmpty)
                return (this);

            if (IsEmpty)
                return (other);
            
            if (item.Value != other.item.Value)
                throw new InvalidOperationException("Can't add two item bundles of different item types.");

            return (new ItemBundle(other.item.Value, stackCount + other.stackCount));
        }
        
        public override bool Equals(object other)
        {
            if (other is not ItemBundle casted)
                return false;

            return Equals(casted);
        }

        public bool Equals(ItemBundle other)
        {
            // If this has a value, the other must have a value and they must be equal
            if (item.HasValue)
                return (other.item.HasValue && item.Value == other.item.Value && stackCount == other.stackCount);

            // If this has no value, the other must have no value and the stack limits must be equal
            if (!other.item.HasValue)
                return (stackCount == other.stackCount);

            return (false);
        }

        public override int GetHashCode()
        {
            int itemHash = item.HasValue ? item.Value.GetHashCode() : 0;
            int stackLimitHash = stackCount.GetHashCode();
            return HashCode.Combine(itemHash, stackLimitHash);
        }

        public static bool operator ==(ItemBundle lhs, ItemBundle rhs) => lhs.Equals(rhs);

        public static bool operator !=(ItemBundle lhs, ItemBundle rhs) => !(lhs == rhs);
        
        public string ToString(string format) => ToString(format, null);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");
            builder.Append(item);
            builder.Append(" , (");
            builder.Append(stackCount);
            builder.Append(") }");
            return builder.ToString();
        }

        public static ItemBundle FromSlot(InventorySlot slot)
        {
            if (slot.IsEmpty)
                return (Empty);

            ItemBundle bundle;
            bundle.item = slot.item;
            bundle.stackCount = slot.stackCount;
            return (bundle);
        }
    }
    
     /// <summary>
     /// Represents an slot in an inventory, optionally containg an item. 
     /// </summary>
     [Serializable]
    public struct InventorySlot : IEquatable<InventorySlot>, IFormattable
    {
        public static InventorySlot Empty { get; } = new InventorySlot
        {
            item = Optional<ScriptableItem>.None(),
            stackCount = 0,
            stackLimit = 0
        };
        
        /// <summary>
        /// Whether the inventory item is empty.
        /// </summary>
        public bool IsEmpty => !item.HasValue;
        
        /// <summary>
        /// Whether the item is fully stacked.
        /// </summary>
        public bool ReachedStackLimit => stackCount == stackLimit;

        /// <summary>
        /// The reference to the item instance in the project.
        /// </summary>
        public Optional<ScriptableItem> item;

        /// <summary>
        /// The current stack count of the item.
        /// </summary>
        public int stackCount;

        /// <summary>
        /// The stack limit of the item.
        /// </summary>
        public int stackLimit;

        /// <summary>
        /// Creates a new instance of an inventory item.
        /// </summary>
        /// <param name="item">The reference to the item instance in the project.</param>
        /// <param name="stackLimit">The stack count of the item.</param>
        /// <param name="stackCount">The stack limit of the item.</param>
        public InventorySlot(ScriptableItem item, int stackLimit = 1, int stackCount = 1)
        {
            this.item = item;
            this.stackCount = stackCount;
            this.stackLimit = stackLimit;
        }

        public override bool Equals(object other)
        {
            if (other is not InventorySlot casted)
                return false;

            return Equals(casted);
        }

        public bool Equals(InventorySlot other)
        {
            // If this has a value, the other must have a value and they must be equal
            if (item.HasValue)
                return (other.item.HasValue && item.Value == other.item.Value && stackLimit == other.stackLimit);

            // If this has no value, the other must have no value and the stack limits must be equal
            if (!other.item.HasValue)
                return (stackLimit == other.stackLimit);

            return (false);
        }

        public override int GetHashCode()
        {
            int itemHash = item.HasValue ? item.Value.GetHashCode() : 0;
            int stackLimitHash = stackLimit.GetHashCode();
            return HashCode.Combine(itemHash, stackLimitHash);
        }

        public static bool operator ==(InventorySlot lhs, InventorySlot rhs) => lhs.Equals(rhs);

        public static bool operator !=(InventorySlot lhs, InventorySlot rhs) => !(lhs == rhs);

        public override string ToString() => ToString(null, null);

        public string ToString(string format) => ToString(format, null);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");
            builder.Append(item);
            builder.Append(" , (");
            builder.Append(stackCount);
            builder.Append("/");
            builder.Append(stackLimit);
            builder.Append(") }");
            return builder.ToString();
        }
    }

     /// <summary>
     /// Represents an inventory as a scriptable object asset. It holds inventory slots which can contain stacks
     /// of items. Stack limits of slots can not be overriden or ignored. Inventory size can.
     /// </summary>
    public class ScriptableInventory : ScriptableList<InventorySlot>
    {
        public delegate void ItemTakenAction(int index, ItemBundle bundle);
        public delegate void ItemAddedAction(int index, InventorySlot slot);
        
        /// <summary>
        /// The size of this inventory, in other words, how many items can fit in this inventory.
        /// </summary>
        [Header("Inventory")]
        [SerializeField, Tooltip("The size of this inventory, in other words, how many items can fit in this inventory.")]
        private int _size;

        public event ItemTakenAction ItemTaken;
        public event ItemAddedAction ItemAdded;

        /// <summary>
        /// The size of this inventory, in other words, how many items can fit
        /// in this inventory.
        /// </summary>
        public int Size
        {
            get => _size;
            set
            {
                if (value == _size)
                    return;
                
                _size = value;
                Resize(_size);
            }
        }

        /// <summary>
        /// The amount of actual items in this inventory.
        /// </summary>
        public int ItemCount
        {
            get
            {
                int count = 0;
                
                for (int i = 0; i < value.Count; i++)
                {
                    if (!value[i].IsEmpty)
                        count++;
                }

                return (count);
            }
        }

        /// <summary>
        /// The stack limits used by items in this inventory.
        /// </summary>
        private readonly Dictionary<ScriptableItem, int> _stackLimits = new Dictionary<ScriptableItem, int>();
        
        /// <summary>
        /// The default stack limit for items in the inventory.
        /// </summary>
        public const int DEFAULT_STACK_LIMIT = 1;
        
        /// <summary>
        /// Sets the stack limit for an item in the inventory.
        /// </summary>
        public void SetStackLimit(ScriptableItem item, int stackLimit)
        {
            ThrowIf.UnityObjectNull(item);
            ThrowIf.SmallerThen(stackLimit, 1, "Stack limit of an item can't be smaller than 1.");

            _stackLimits[item] = stackLimit;
        }
        
        /// <summary>
        /// Gets the stack limit for an item in the inventory.
        /// </summary>
        public int GetStackLimit(ScriptableItem item) => _stackLimits.GetValueOrDefault(item, DEFAULT_STACK_LIMIT);
        
        /// <summary>
        /// Switches the positions of two items in the inventory.
        /// </summary>
        public void Switch(int firstIndex, int secondIndex)
        {
            ThrowIf.OutOfBounds(value, firstIndex);
            ThrowIf.OutOfBounds(value, secondIndex);

            InventorySlot firstSlot = value[firstIndex];
            InventorySlot secondSlot = value[secondIndex];

            value[firstIndex] = secondSlot;
            value[secondIndex] = firstSlot;
        }
        
        /// <summary>
        /// Inserts an item at a given index in the inventory. Returns whether the item could be inserted.
        /// Ignore size allows for overflow to new slot.
        /// </summary>
        public bool Insert(int index, ScriptableItem item, int count = 1, bool ignoreSize = false)
        {
            ThrowIf.OutOfBounds(value, index);
            ThrowIf.UnityObjectNull(item);
            ThrowIf.SmallerThen(count, 0);

            InventorySlot slot = value[index];
            
            if (slot.IsEmpty)
            {
                // If the index corresponds to an empty slot, create a new item.
                int stackLimit = GetOrCreateStackLimitForItem(item);
                int stackCount = Mathf.Min(count, stackLimit);
                
                slot = new InventorySlot(item, stackLimit, stackCount);
                
                value[index] = slot;
                return true;
            }
            
            if (slot.ReachedStackLimit && !ignoreSize)
                return false; 
            
            int countToLimit = slot.stackLimit - slot.stackCount;
            if (count > countToLimit && !ignoreSize)
                return false;

            // If the item exists, update its count.
            IncrementItemCount(item, index, count, ignoreSize);
            return true;
        }

        /// <summary>
        /// Inserts an item bundle at a given index in the inventory. Use this if you want to return an item bundle
        /// that you previously took from the inventory. Returns whether the bundle could be returned.
        /// </summary>
        public bool Return(int index, ItemBundle bundle)
        {
            if (bundle.IsEmpty)
                return (false);

            return (Insert(index, bundle.item.Value, bundle.stackCount));
        }
        
        /// <summary>
        /// Adds a new item to the inventory.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="ignoreSize">Whether to ignore the capacity during this operation.</param>
        /// <returns>Whether the item was added.</returns>
        public bool Add(ScriptableItem item, bool ignoreSize) => Add(item, 1, ignoreSize);

        /// <summary>
        /// Adds a new item to the inventory.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="count">The amount of items to add.</param>
        /// <param name="ignoreSize">Whether to ignore the size during this operation.</param>
        /// <returns>Whether the item was added.</returns>
        public bool Add(ScriptableItem item, int count = 1, bool ignoreSize = false)
        {
            // Find the index of an existing item that has not yet reached its stack slimit.
            int index = ((List<InventorySlot>)value).FindIndex( slot => !slot.IsEmpty && slot.item.Value == item && !slot.ReachedStackLimit);
            if (index == -1)
            {
                // If the item doesn't exist yet, add a new item.
                int stackLimit = GetOrCreateStackLimitForItem(item);
                bool couldBeAdded = AddNewItemToContent(item, stackLimit, count, ignoreSize);
                return couldBeAdded;
            }

            // If the item exists, update its count.
            IncrementItemCount(item, index, count, ignoreSize);

            return true;
        }

        /// <summary>
        /// Takes the given item from the inventory. Throws an exception if the item count is smaller then 1 or there
        /// is no slot that has 'count' amount of the item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <returns>The bundle of items removed.</returns>
        public ItemBundle Take(ScriptableItem item, int count = 1)
        {
            ThrowIf.SmallerThen(count, 1);
            
            IList<InventorySlot> slots = GetValue();
            int countAbleToTake = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.IsEmpty || slot.item.Value != item)
                    continue;

                countAbleToTake += slot.stackCount;
            }

            ThrowIf.SmallerThen(count, countAbleToTake);

            ItemBundle bundle = ItemBundle.Empty;
            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.IsEmpty || slot.item.Value != item)
                    continue;

                int countLeftoverToTake = countAbleToTake - bundle.stackCount;
                int portionToTake = Mathf.Min(slot.stackCount, countLeftoverToTake);
                ItemBundle portion = TakeAt(i, portionToTake);
                
                bundle = bundle.Add(portion);
            }
            
            return (bundle);
        }
        
        /// <summary>
        /// Removes an item at a given index. If count is left null, all items at the index are removed.
        /// </summary>
        /// <param name="index">The index to remove the item at</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <returns>The bundle of items removed.</returns>
        public ItemBundle TakeAt(int index, int? count = null)
        {
            ThrowIf.OutOfBounds(value, index);

            InventorySlot slot = value[index];
            ItemBundle bundle = ItemBundle.Empty;
            
            if (slot.IsEmpty)
                return (bundle); // If the slot to remove the item from is empty, return an empty bundle.

            int removeCount = count.GetValueOrDefault();
            if (!count.HasValue || slot.stackCount == removeCount)
            {
                // If the remove count is 0 or all items are to be removed, we empty the slot and return the full bundle.
                value[index] = InventorySlot.Empty;
                
                bundle = ItemBundle.FromSlot(slot);
            }
            else
            {
                // If there is a remove count and it is not the full item count, remove the count of the item.
                bundle = RemoveCountOfItem(slot, index, removeCount);
            }
            
            ItemTaken?.Invoke(index, bundle);

            return (bundle);
        }

        /// <summary>
        /// Removes items at a given indices.
        /// </summary>
        /// <param name="indices">The indices to remove the items at</param>
        /// <param name="count">The amount of items to remove.</param>
        /// <returns>The removed bundles.</returns>
        public ItemBundle[] TakeAt(int[] indices, int? count = null)
        {
            ThrowIf.SystemObjectNull(indices);

            var items = new ItemBundle[indices.Length];

            for (int i = 0; i < indices.Length; i++)
                items[i] = TakeAt(indices[i], count);

            return (items);
        }

        public int GetTotalItemCount(ScriptableItem item)
        {
            IList<InventorySlot> slots = GetValue();
            int count = 0;
            
            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot slot = slots[i];
                if (slot.IsEmpty || slot.item.Value != item)
                    continue;

                count += slot.stackCount;
            }

            return (count);
        }
        
        protected override IList<InventorySlot> CreateValue(int capacity)
        {
            int max = Mathf.Max(capacity, _size);
            if (max == 0)
                return (base.CreateValue(capacity));
            
            var list = base.CreateValue(max);
            if (list.Count == max)
                return (list);

            for (int i = 0; i < max; i++)
                list.Add(InventorySlot.Empty);
            
            return (list);
        }
        
        /// <summary>
        /// Adds a new item to the content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="stackLimit">The stack limit for the item.</param>
        /// <param name="stackCount">The stack count for the item.</param>
        /// <param name="ignoreSize">Whether to ignore the size for this operation.</param>
        /// <returns>Whether the adding succeeded.</returns>
        private bool AddNewItemToContent(ScriptableItem item, int stackLimit, int stackCount, bool ignoreSize)
        {
            int currentSize = value.Count;
            
            // First try filling a default entry.
            for (int i = 0; i < currentSize; i++)
            {
                if (value[i].IsEmpty)
                {
                    // Assign the new item to a default entry.
                    AddItemAtIndex(i);
                    return true;
                }
            }

            if (ignoreSize)
            {
                // If there are no default entries left but we can ignore size, we force a resize and append the new item.
                Resize(currentSize + 1);
                AddItemAtIndex(currentSize);

                return true;
            }

            void AddItemAtIndex(int index)
            {
                if (stackCount > stackLimit)
                {
                    // If the count is greater than the stack limit, assign the limit as count
                    // and add a new item with the left over count.
                    int leftOverCount = stackCount - stackLimit;

                    stackCount = stackLimit;
                    
                    InventorySlot slot = new InventorySlot(item, stackLimit, stackCount);
                    value[index] = slot;

                    Add(item, leftOverCount, ignoreSize);
                }
                else
                {
                    // If the count is not greater than the stack limit, just assign the new item with given info.
                    InventorySlot slot = new InventorySlot(item, stackLimit, stackCount);
                    value[index] = slot;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Increments the amount of an item in the inventory.
        /// </summary>
        /// <param name="item">The existing item to increment.</param>
        /// <param name="index">The index to of the existing item to increment.</param>
        /// <param name="count">The amount to increment.</param>
        /// <param name="ignoreSize">Whether to ignore size during this operation.</param>
        private void IncrementItemCount(ScriptableItem item, int index, int count, bool ignoreSize)
        {
            InventorySlot slot = value[index];
            int countToLimit = slot.stackLimit - slot.stackCount;
            if (count > countToLimit)
            {
                // If the count is greater than the count to limit, assign the count to limit
                // to the existing item's count and add a new item with the left over count.
                int leftOverCount = count - countToLimit;

                slot.stackCount += countToLimit;
                
                value[index] = slot;

                Add(item, leftOverCount, ignoreSize);
            }
            else
            {
                // If the count is not greater than the count to limit, just assign count to the existing item.
                slot.stackCount += count;
                
                value[index] = slot;
            }
        }
        
        /// <summary>
        /// Removes the stack count of an slot and returns the removed bundle.
        /// </summary>
        /// <param name="slot">The slot of which to remove the count.</param>
        /// <param name="indexOfItem">The index of the item.</param>
        /// <param name="removeCount">The amount to remove.</param>
        /// <returns>The bundle of items taken.</returns>
        private ItemBundle RemoveCountOfItem(InventorySlot slot, int indexOfItem, int removeCount)
        {
            ThrowIf.SmallerThen(removeCount, 1, $"Trying to remove a negative amount of items at {indexOfItem}.");

            int itemCount = slot.stackCount;
            
            ThrowIf.GreaterThen(removeCount, itemCount,  $"Trying to retrieve {removeCount} of item {slot.item.Value.name} at {indexOfItem} while it has {slot.stackCount}.");
            
            ScriptableItem item = slot.item.Value;
            int stackLimit = slot.stackLimit;
            int stackCount = itemCount - removeCount;
            slot = new InventorySlot(item, stackLimit, stackCount);
            
            value[indexOfItem] = slot;
            
            return (new ItemBundle(item, removeCount));
        }
        
        /// <summary>
        /// Returns the stack limit for an item, assigning a default
        /// stack limit to the item if none existed yet.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The stack limit for the item.</returns>
        private int GetOrCreateStackLimitForItem(ScriptableItem item)
        {
            if (_stackLimits.TryGetValue(item, out int result)) 
                return (result);

            int stackLimit = item.StackLimit;
            
            _stackLimits.Add(item, stackLimit);
            return (stackLimit);

        }
        
        private void Resize(int newSize)
        {
            int currentSize = value.Count;
            if (newSize == currentSize)
                return;

            var list = (List<InventorySlot>)value;
            if (newSize < currentSize)
            {
                // If the inventory is getting smaller, first make sure all items are moved to the front.
                list.Sort((a, b) =>
                {
                    if (a.IsEmpty)
                        return (b.IsEmpty ? 0 : 1);
                    
                    if (b.IsEmpty)
                        return -1;
                    
                    return 0;
                });

                // If there are more inventory items than the new size can hold, resize to the amount of items in the inventory.
                int itemsInInventory = ItemCount;
                if (itemsInInventory > newSize)
                    newSize = itemsInInventory;

                int index = newSize;
                int count = currentSize - newSize;
                list.RemoveRange(index, count);
            }
            else
            {
                // If the inventory is getting larger, add empty slots.
                int count = newSize - currentSize;
                for (int i = 0; i < count; i++)
                    list.Add(InventorySlot.Empty);
            }

            _size = newSize;
        }
        
        /// <summary>
        /// Returns the string representation of the inventory.
        /// </summary>
        /// <returns>The string representation of the inventory.</returns>
        public override string ToString()
        {
            if (value.Count == 0)
                return string.Empty;

            return string.Join(" , ", value);
        }
    }
}
