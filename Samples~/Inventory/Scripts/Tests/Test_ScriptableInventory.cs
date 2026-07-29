using NUnit.Framework;
using OpenUtility.Samples.Data;
using UnityEditor;
using UnityEngine;

public class Test_ScriptableInventory
{
    private SerializedObject _serializedObject;
    
    [Test]
    public void Test_ScriptableInventory_Size_Get_Default()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        
        // Act
        int actual = instance.Size;
        int expected = 0;
        
        // Assert
        Assert.AreEqual(actual, expected);
    }
    
    [Test]
    public void Test_ScriptableInventory_Size_Increase_Check_Size()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        
        // Act
        int newSize = 5;
        instance.Size = newSize;
        
        int expected = newSize;
        int actual = instance.Size;
        
        // Assert
        Assert.AreEqual(actual, expected);
    }
    
    [Test]
    public void Test_ScriptableInventory_Size_Increase_Check_ListCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        
        // Act
        int newSize = 5;
        instance.Size = newSize;
        
        int expected = newSize;
        int actual = instance.GetValue().Count;
        
        // Assert
        Assert.AreEqual(actual, expected);
    }
    
    [Test]
    public void Test_ScriptableInventory_Size_Decrease_Check_Size()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        _serializedObject = new SerializedObject(instance);
        _serializedObject.FindProperty("_size").intValue = 5;
        _serializedObject.ApplyModifiedProperties();
        
        // Act
        int newSize = 2;
        instance.Size = newSize;
        
        int expected = newSize;
        int actual = instance.Size;
        
        // Assert
        Assert.AreEqual(actual, expected);
    }
    
    [Test]
    public void Test_ScriptableInventory_Size_Decrease_Check_ListCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        _serializedObject = new SerializedObject(instance);
        _serializedObject.FindProperty("_size").intValue = 5;
        _serializedObject.ApplyModifiedProperties();
        
        // Act
        int newSize = 2;
        instance.Size = newSize;
        
        int expected = newSize;
        int actual = instance.GetValue().Count;
        
        // Assert
        Assert.AreEqual(actual, expected);
    }

    [Test]
    public void Test_ScriptableInventory_ItemCount_Get_Default()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
         
        // Act
        int actual = instance.ItemCount;
        int expected = 0;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_After_Adding_Single_Item()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        instance.Add(item, ignoreSize: false);
        int actual = instance.ItemCount;
        int expected = 1;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_After_Adding_Multiple_Different_Items()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        instance.Add(item1, ignoreSize: false);
        instance.Add(item2, ignoreSize: false);
        instance.Add(item3, ignoreSize: false);
        int actual = instance.ItemCount;
        int expected = 3;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_Stacking_Same_Item_Does_Not_Increase_Count()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 10);
         
        // Act
        instance.Add(item, count: 1, ignoreSize: false);
        int countAfterFirstAdd = instance.ItemCount;
        instance.Add(item, count: 3, ignoreSize: false);
        int actual = instance.ItemCount;
        int expected = 1;
         
        // Assert
        Assert.AreEqual(countAfterFirstAdd, 1);
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_After_Removing_Item()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item, ignoreSize: false);
         
        // Act
        instance.TakeAt(0);
        int actual = instance.ItemCount;
        int expected = 0;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_Multiple_Items_After_Removing_One()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1, ignoreSize: false);
        instance.Add(item2, ignoreSize: false);
         
        // Act
        instance.TakeAt(0);
        int actual = instance.ItemCount;
        int expected = 1;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_ItemCount_Fills_Capacity()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        int capacity = 3;
        instance.Size = capacity;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        instance.Add(item1, ignoreSize: false);
        instance.Add(item2, ignoreSize: false);
        instance.Add(item3, ignoreSize: false);
        int actual = instance.ItemCount;
        int expected = capacity;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
    
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    public void Test_ScriptableInventory_SetStackLimit_And_Get(int value)
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        int stackLimit = value;
         
        // Act
        instance.SetStackLimit(item, stackLimit);
        int actual = instance.GetStackLimit(item);
        int expected = stackLimit;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_With_Large_Value()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        int stackLimit = 999;
         
        // Act
        instance.SetStackLimit(item, stackLimit);
        int actual = instance.GetStackLimit(item);
        int expected = 999;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_Update_Existing()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
         
        // Act
        int firstStackLimit = instance.GetStackLimit(item);
        instance.SetStackLimit(item, 10);
        int actual = instance.GetStackLimit(item);
        int expected = 10;
         
        // Assert
        Assert.AreEqual(firstStackLimit, 5);
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_Different_Items()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        int stackLimit1 = 3;
        int stackLimit2 = 7;
         
        // Act
        instance.SetStackLimit(item1, stackLimit1);
        instance.SetStackLimit(item2, stackLimit2);
        int actual1 = instance.GetStackLimit(item1);
        int actual2 = instance.GetStackLimit(item2);
         
        // Assert
        Assert.AreEqual(actual1, stackLimit1);
        Assert.AreEqual(actual2, stackLimit2);
    }
     
    [Test]
    public void Test_ScriptableInventory_GetStackLimit_Default_For_Unset_Item()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        int actual = instance.GetStackLimit(item);
        int expected = ScriptableInventory.DEFAULT_STACK_LIMIT;
         
        // Assert
        Assert.AreEqual(actual, expected);
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_Null_Item_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
         
        // Act & Assert
        Assert.Throws<System.NullReferenceException>(() => instance.SetStackLimit(null, 5));
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_Zero_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.SetStackLimit(item, 0));
    }
     
    [Test]
    public void Test_ScriptableInventory_SetStackLimit_Negative_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.SetStackLimit(item, -5));
    }

    [Test]
    public void Test_ScriptableInventory_Switch_Two_Items()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1, ignoreSize: false);
        instance.Add(item2, ignoreSize: false);
         
        // Act
        instance.Switch(0, 1);
         
        // Assert
        Assert.AreEqual(instance.GetValue()[0].item.Value, item2);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_With_Empty_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item, ignoreSize: false);
         
        // Act
        instance.Switch(0, 1);
         
        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item);
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Two_Empty_Slots()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act
        instance.Switch(0, 1);
         
        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.IsTrue(instance.GetValue()[1].IsEmpty);
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Same_Index()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item, ignoreSize: false);
         
        // Act
        instance.Switch(0, 0);
         
        // Assert
        Assert.AreEqual(instance.GetValue()[0].item.Value, item);
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Preserves_Stack_Count()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item1, 10);
        instance.SetStackLimit(item2, 10);
        instance.Insert(0, item1, count: 3);
        instance.Insert(1, item2, count: 5);
         
        // Act
        instance.Switch(0, 1);
         
        // Assert
        Assert.AreEqual(instance.GetValue()[0].stackCount, 5);
        Assert.AreEqual(instance.GetValue()[1].stackCount, 3);
        Assert.AreEqual(instance.GetValue()[0].item.Value, item2);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_First_Index_Out_Of_Bounds_Negative()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Switch(-1, 0));
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_First_Index_Out_Of_Bounds_Too_Large()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Switch(5, 0));
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Second_Index_Out_Of_Bounds_Negative()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Switch(0, -1));
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Second_Index_Out_Of_Bounds_Too_Large()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Switch(0, 5));
    }
     
    [Test]
    public void Test_ScriptableInventory_Switch_Multiple_Times()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1, ignoreSize: false);
        instance.Add(item2, ignoreSize: false);
        instance.Add(item3, ignoreSize: false);
         
        // Act
        instance.Switch(0, 1);
        instance.Switch(1, 2);
         
        // Assert
        Assert.AreEqual(instance.GetValue()[0].item.Value, item2);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item3);
        Assert.AreEqual(instance.GetValue()[2].item.Value, item1);
    }

    [Test]
    public void Test_ScriptableInventory_Insert_Into_Empty_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        bool result = instance.Insert(0, item, count: 1);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].item.Value, item);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Multiple_Count()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
         
        // Act
        bool result = instance.Insert(0, item, count: 5);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(5, instance.GetValue()[0].stackCount);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_At_Different_Indices()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        instance.Insert(1, item1, count: 1);
        instance.Insert(3, item2, count: 1);
         
        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item1);
        Assert.IsTrue(instance.GetValue()[2].IsEmpty);
        Assert.AreEqual(instance.GetValue()[3].item.Value, item2);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Increments_Existing_Within_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 10);
        instance.Insert(0, item, count: 3);
         
        // Act
        bool result = instance.Insert(0, item, count: 2);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 5);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Returns_False_When_Stack_Limit_Reached()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);
        instance.Insert(0, item, count: 3);
         
        // Act
        bool result = instance.Insert(0, item, count: 1);
         
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 3);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Default_Count_Is_One()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        bool result = instance.Insert(0, item);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Null_Item_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
         
        // Act & Assert
        Assert.Throws<System.NullReferenceException>(() => instance.Insert(0, null));
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Index_Out_Of_Bounds_Negative()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Insert(-1, item));
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Index_Out_Of_Bounds_Too_Large()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Insert(5, item));
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Different_Items_Same_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item1, count: 1);
         
        // Act
        bool result = instance.Insert(0, item2, count: 1);
         
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(instance.GetValue()[0].item.Value, item1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Uses_Stack_Limit_From_Dictionary()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 7);
         
        // Act
        instance.Insert(0, item, count: 1);
         
        // Assert
        Assert.AreEqual(instance.GetValue()[0].stackLimit, 7);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Sequential_Inserts()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        instance.Insert(0, item1, count: 1);
        instance.Insert(1, item2, count: 1);
        instance.Insert(2, item3, count: 1);
         
        // Assert
        Assert.AreEqual(instance.ItemCount, 3);
        Assert.AreEqual(instance.GetValue()[0].item.Value, item1);
        Assert.AreEqual(instance.GetValue()[1].item.Value, item2);
        Assert.AreEqual(instance.GetValue()[2].item.Value, item3);
    }

    [Test]
    public void Test_ScriptableInventory_Insert_With_Zero_Count()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        bool result = instance.Insert(0, item, count: 0);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 0);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_With_Negative_Count()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act & Assert - Should throw or handle gracefully
        Assert.Throws<System.Exception>(() => instance.Insert(0, item, count: -5));
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Count_Exceeds_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);
         
        // Act
        bool result = instance.Insert(0, item, count: 5);
         
        // Assert - Should cap at stack limit
        Assert.IsTrue(result);
        Assert.AreEqual(3, instance.GetValue()[0].stackCount);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Count_Equals_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
         
        // Act
        bool result = instance.Insert(0, item, count: 5);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 5);
        Assert.IsTrue(instance.GetValue()[0].ReachedStackLimit);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Increment_That_Exceeds_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 1;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 2);
         
        // Act - Try to add more than stack limit allows
        bool result = instance.Insert(0, item, count: 4);
         
        // Assert - Should NOT increment beyond stack limit
        Assert.IsFalse(result);
        Assert.AreEqual(2, instance.GetValue()[0].stackCount);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Increment_With_Ignore_Capacity_Large_Amount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 1;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        int stackLimit = 5;
        instance.SetStackLimit(item, stackLimit);
        instance.Insert(0, item, count: 2);
         
        // Act
        int stackCount = 100;
        bool result = instance.Insert(0, item, count: stackCount, ignoreSize: true);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(stackLimit, instance.GetValue()[0].stackCount);
        Assert.AreEqual(stackCount / stackLimit + 1, instance.Size);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_All_Slots_Full_With_Same_Item()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 3;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 2);
        instance.Insert(0, item, count: 2);
        instance.Insert(1, item, count: 2);
        instance.Insert(2, item, count: 2);
         
        // Act - All slots are full with the same item
        int actual = instance.ItemCount;
         
        // Assert
        Assert.AreEqual(actual, 3);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Into_Full_Inventory_Ignore_Capacity_True()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 2;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item1, count: 1);
        instance.Insert(1, item2, count: 1);
         
        // Act - Inventory is full, but we use ignoreCapacity
        bool result = instance.Insert(0, item1, count: 1, ignoreSize: true);
         
        // Assert - Should overflow to new slot
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 1);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_At_Boundary_Index_Zero()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        bool result = instance.Insert(0, item);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].item.Value, item);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_At_Boundary_Index_Last()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        int size = 5;
        instance.Size = size;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
         
        // Act
        bool result = instance.Insert(size - 1, item);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[size - 1].item.Value, item);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Stack_Limit_One_Default()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        // Don't set stack limit, should use default
         
        // Act
        bool result = instance.Insert(0, item, count: 1);
         
        // Assert - Default stack limit is 1
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackLimit, ScriptableInventory.DEFAULT_STACK_LIMIT);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Into_Slot_With_Different_Item_Type()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item1, 2);
        instance.Insert(0, item1, count: 2);
         
        // Act - Try to insert different item into occupied slot
        bool result = instance.Insert(0, item2, count: 1);
         
        // Assert - Should fail
        Assert.IsFalse(result);
        Assert.AreEqual(item1, instance.GetValue()[0].item.Value);
        Assert.AreEqual(2, instance.GetValue()[0].stackCount);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Very_Large_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 999999);
         
        // Act
        bool result = instance.Insert(0, item, count: 50000);
         
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 50000);
    }
     
    [Test]
    public void Test_ScriptableInventory_Insert_Partial_Increment_With_IgnoreCapacity_False()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 3);
         
        // Act - Try to add 4 when only 2 space left, ignoreCapacity=false
        bool result = instance.Insert(0, item, count: 2);
         
        // Assert - Should succeed since 2 <= 2 (space available)
        Assert.IsTrue(result);
        Assert.AreEqual(instance.GetValue()[0].stackCount, 5);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Empty_Bundle_Returns_False()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 1;
        
        // Act
        bool actual = instance.Return(0, ItemBundle.Empty);
        
        // Assert
        Assert.IsFalse(actual);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Into_Empty_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 2;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 2);
        ItemBundle bundle = new ItemBundle(item, 2);
        
        // Act
        bool actual = instance.Return(1, bundle);
        
        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(item, instance.GetValue()[1].item.Value);
        Assert.AreEqual(2, instance.GetValue()[1].stackCount);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Into_Slot_With_Same_Item_Increments()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 3;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 10);
        instance.Insert(0, item, count: 2);
        ItemBundle bundle = new ItemBundle(item, 3);
        
        // Act
        bool actual = instance.Return(0, bundle);
        
        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(5, instance.GetValue()[0].stackCount);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Into_Slot_With_Different_Item_Returns_False()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 3;
        ScriptableItem firstItem = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem secondItem = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, firstItem, count: 1);
        ItemBundle bundle = new ItemBundle(secondItem, 1);
        
        // Act
        bool actual = instance.Return(0, bundle);
        
        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(firstItem, instance.GetValue()[0].item.Value);
        Assert.AreEqual(1, instance.GetValue()[0].stackCount);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Into_Reached_Stack_Limit_Returns_False()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 3;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 2);
        instance.Insert(0, item, count: 2);
        ItemBundle bundle = new ItemBundle(item, 1);
        
        // Act
        bool actual = instance.Return(0, bundle);
        
        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(2, instance.GetValue()[0].stackCount);
    }
    
    [Test]
    public void Test_ScriptableInventory_Return_Non_Empty_Bundle_Out_Of_Bounds_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 3;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        ItemBundle bundle = new ItemBundle(item, 1);
        
        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.Return(3, bundle));
    }


    [Test]
    public void Test_ScriptableInventory_Add_Returns_True_When_Space_Available()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act
        bool actual = instance.Add(item);

        // Assert
        Assert.IsTrue(actual);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Places_Item_In_First_Empty_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act
        instance.Add(item);

        // Assert
        Assert.AreEqual(item, instance.GetValue()[0].item.Value);
        Assert.AreEqual(1, instance.GetValue()[0].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Default_Count_Is_One()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act
        instance.Add(item);

        // Assert
        Assert.AreEqual(1, instance.GetValue()[0].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_With_Count_Sets_StackCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 10);

        // Act
        instance.Add(item, count: 4);

        // Assert
        Assert.AreEqual(4, instance.GetValue()[0].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Same_Item_Stacks_On_Existing_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 10);
        instance.Add(item, count: 2);

        // Act
        instance.Add(item, count: 3);

        // Assert
        Assert.AreEqual(5, instance.GetValue()[0].stackCount);
        Assert.AreEqual(1, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Count_Exceeds_Stack_Limit_Spills_To_New_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);

        // Act
        instance.Add(item, count: 5);

        // Assert
        Assert.AreEqual(3, instance.GetValue()[0].stackCount);
        Assert.AreEqual(item, instance.GetValue()[1].item.Value);
        Assert.AreEqual(2, instance.GetValue()[1].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Fills_Existing_Slot_Before_Creating_New_One()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Add(item, count: 3);

        // Act
        instance.Add(item, count: 1);

        // Assert
        Assert.AreEqual(4, instance.GetValue()[0].stackCount);
        Assert.AreEqual(1, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Returns_False_When_Inventory_Full()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 2;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1);
        instance.Add(item2);

        // Act
        bool actual = instance.Add(item3);

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(2, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_IgnoreSize_True_Expands_Inventory()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 1;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1);

        // Act
        bool actual = instance.Add(item2, ignoreSize: true);

        // Assert
        Assert.IsTrue(actual);
        Assert.AreEqual(2, instance.ItemCount);
        Assert.AreEqual(item2, instance.GetValue()[1].item.Value);
    }

    [Test]
    public void Test_ScriptableInventory_Add_IgnoreSize_False_Does_Not_Expand_Inventory()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 1;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1);

        // Act
        bool actual = instance.Add(item2, ignoreSize: false);

        // Assert
        Assert.IsFalse(actual);
        Assert.AreEqual(1, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Multiple_Different_Items_Each_Gets_Own_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item3 = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act
        instance.Add(item1);
        instance.Add(item2);
        instance.Add(item3);

        // Assert
        Assert.AreEqual(3, instance.ItemCount);
        Assert.AreEqual(item1, instance.GetValue()[0].item.Value);
        Assert.AreEqual(item2, instance.GetValue()[1].item.Value);
        Assert.AreEqual(item3, instance.GetValue()[2].item.Value);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Uses_Default_Stack_Limit()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act
        instance.Add(item);

        // Assert
        Assert.AreEqual(ScriptableInventory.DEFAULT_STACK_LIMIT, instance.GetValue()[0].stackLimit);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Count_Exceeds_Stack_Limit_On_New_Item_Caps_And_Spills()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);

        // Act - Add 7 items with stack limit 3: slot 0 = 3, slot 1 = 3, slot 2 = 1
        instance.Add(item, count: 7);

        // Assert
        Assert.AreEqual(3, instance.GetValue()[0].stackCount);
        Assert.AreEqual(3, instance.GetValue()[1].stackCount);
        Assert.AreEqual(1, instance.GetValue()[2].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Add_Null_Item_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;

        // Act & Assert
        Assert.Throws<System.ArgumentNullException>(() => instance.Add(null));
    }

    [Test]
    public void Test_ScriptableInventory_Take_Returns_Correct_Item_Reference()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item);

        // Act
        ItemBundle bundle = instance.Take(item);

        // Assert
        Assert.AreEqual(item, bundle.item.Value);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Default_Count_Takes_All_From_Single_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item);

        // Act
        ItemBundle bundle = instance.Take(item);

        // Assert
        Assert.AreEqual(1, bundle.stackCount);
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Clears_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item);

        // Act
        instance.Take(item, count: 1);

        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.AreEqual(0, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Decreases_ItemCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1);
        instance.Add(item2);

        // Act
        instance.Take(item1, count: 1);

        // Assert
        Assert.AreEqual(1, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Count_Zero_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item);

        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.Take(item, count: 0));
    }

    [Test]
    public void Test_ScriptableInventory_Take_Count_Negative_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item);

        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.Take(item, count: -3));
    }

    [Test]
    public void Test_ScriptableInventory_Take_Count_Less_Than_Available_Throws()
    {
        // Arrange - 5 items in inventory
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Add(item, count: 5);

        // Act & Assert - requesting fewer than available is an error
        Assert.Throws<System.Exception>(() => instance.Take(item, count: 3));
    }

    [Test]
    public void Test_ScriptableInventory_Take_Count_Exactly_Matches_Available()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Add(item, count: 5);

        // Act
        ItemBundle bundle = instance.Take(item, count: 5);

        // Assert
        Assert.AreEqual(5, bundle.stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Count_Greater_Than_Available_Takes_All()
    {
        // Arrange - 3 items but requesting 5
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);
        instance.Add(item, count: 3);

        // Act - count=5 is not less than countAbleToTake=3, so no throw; takes all 3
        ItemBundle bundle = instance.Take(item, count: 5);

        // Assert
        Assert.AreEqual(3, bundle.stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Item_Not_In_Inventory_Returns_Empty_Bundle()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();

        // Act - count=1 is not less than countAbleToTake=0, no throw; loop finds nothing
        ItemBundle bundle = instance.Take(item, count: 1);

        // Assert
        Assert.IsTrue(bundle.IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Collects_From_Multiple_Slots()
    {
        // Arrange - same item spread across two slots
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);
        instance.Add(item, count: 3); // slot 0: 3
        instance.Add(item, count: 2); // slot 1: 2 (spills to new slot)

        // Act - take all 5 across both slots
        ItemBundle bundle = instance.Take(item, count: 5);

        // Assert
        Assert.AreEqual(5, bundle.stackCount);
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.IsTrue(instance.GetValue()[1].IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_Take_Other_Items_Unaffected()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Add(item1);
        instance.Add(item2);

        // Act
        instance.Take(item1, count: 1);

        // Assert
        Assert.AreEqual(item2, instance.GetValue()[1].item.Value);
        Assert.AreEqual(1, instance.GetValue()[1].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Null_Count_Takes_All_And_Empties_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 3);

        // Act
        instance.TakeAt(0);

        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Null_Count_Returns_Full_Bundle()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 3);

        // Act
        ItemBundle bundle = instance.TakeAt(0);

        // Assert
        Assert.AreEqual(item, bundle.item.Value);
        Assert.AreEqual(3, bundle.stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Partial_Count_Reduces_StackCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 5);

        // Act
        instance.TakeAt(0, count: 2);

        // Assert
        Assert.AreEqual(3, instance.GetValue()[0].stackCount);
        Assert.IsFalse(instance.GetValue()[0].IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Partial_Count_Returns_Correct_Bundle()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 5);

        // Act
        ItemBundle bundle = instance.TakeAt(0, count: 2);

        // Assert
        Assert.AreEqual(item, bundle.item.Value);
        Assert.AreEqual(2, bundle.stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Count_Equals_StackCount_Empties_Slot()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 4);

        // Act
        ItemBundle bundle = instance.TakeAt(0, count: 4);

        // Assert
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
        Assert.AreEqual(4, bundle.stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Count_Zero_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item);

        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.TakeAt(0, count: 0));
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Count_Negative_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item);

        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.TakeAt(0, count: -1));
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Count_Greater_Than_StackCount_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 3);
        instance.Insert(0, item, count: 2);

        // Act & Assert
        Assert.Throws<System.Exception>(() => instance.TakeAt(0, count: 5));
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Empty_Slot_Returns_Empty_Bundle()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;

        // Act
        ItemBundle bundle = instance.TakeAt(0);

        // Assert
        Assert.IsTrue(bundle.IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Index_Out_Of_Bounds_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;

        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.TakeAt(5));
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Index_Negative_Throws()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;

        // Act & Assert
        Assert.Throws<System.IndexOutOfRangeException>(() => instance.TakeAt(-1));
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Does_Not_Affect_Other_Slots()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item1 = ScriptableObject.CreateInstance<ScriptableItem>();
        ScriptableItem item2 = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item1);
        instance.Insert(1, item2);

        // Act
        instance.TakeAt(0);

        // Assert
        Assert.AreEqual(item2, instance.GetValue()[1].item.Value);
        Assert.AreEqual(1, instance.GetValue()[1].stackCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Null_Count_Decreases_ItemCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item);

        // Act
        instance.TakeAt(0);

        // Assert
        Assert.AreEqual(0, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_Partial_Count_Does_Not_Decrease_ItemCount()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.SetStackLimit(item, 5);
        instance.Insert(0, item, count: 3);

        // Act - only take 1 of 3, slot stays occupied
        instance.TakeAt(0, count: 1);

        // Assert
        Assert.AreEqual(1, instance.ItemCount);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_At_Boundary_Index_Zero()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        instance.Size = 5;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(0, item);

        // Act
        ItemBundle bundle = instance.TakeAt(0);

        // Assert
        Assert.AreEqual(item, bundle.item.Value);
        Assert.IsTrue(instance.GetValue()[0].IsEmpty);
    }

    [Test]
    public void Test_ScriptableInventory_TakeAt_At_Boundary_Index_Last()
    {
        // Arrange
        ScriptableInventory instance = ScriptableObject.CreateInstance<ScriptableInventory>();
        int size = 5;
        instance.Size = size;
        ScriptableItem item = ScriptableObject.CreateInstance<ScriptableItem>();
        instance.Insert(size - 1, item);

        // Act
        ItemBundle bundle = instance.TakeAt(size - 1);

        // Assert
        Assert.AreEqual(item, bundle.item.Value);
        Assert.IsTrue(instance.GetValue()[size - 1].IsEmpty);
    }
    [TearDown]
    public void Test_Teardown()
    {
        _serializedObject?.Dispose();
        _serializedObject = null;
    }
}
