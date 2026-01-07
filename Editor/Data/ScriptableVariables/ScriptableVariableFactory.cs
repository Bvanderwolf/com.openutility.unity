#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using OpenUtility.Exceptions;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    public static class ScriptableVariableFactory
    {
        public delegate void AssetCreatedCallback(Object asset, Object target, string propertyPath);
        
        private class AssetCreationCallback : EndNameEditAction
        {
            private Object _target;
            private string _propertyPath;
            private Type _variableType;
            private AssetCreatedCallback _callback;

            public void Setup(Object target, string propertyPath, Type scriptableObjectType, AssetCreatedCallback callback)
            {
                _target = target;
                _propertyPath = propertyPath;
                _variableType = scriptableObjectType;
                _callback = callback;
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = CreateInstance(_variableType);
                
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();

                ProjectWindowUtil.ShowCreatedAsset(asset);
                
                _callback?.Invoke(asset, _target, _propertyPath);
            }
        }
        
        public static void AssignIntVariableForSlider(Slider slider, Object variableAsset, Type bindingType)
        {
            var scriptableInt = (ScriptableInt)variableAsset;
            var scriptableIntBinder = (SliderIntBinding)slider.gameObject.AddComponent(bindingType);
            var serializedBinder = new SerializedObject(scriptableIntBinder);
            var variableProperty = serializedBinder.FindProperty("_variable");

            variableProperty.objectReferenceValue = scriptableInt;

            serializedBinder.ApplyModifiedProperties();
            serializedBinder.Dispose();
                
            UnityEventTools.AddPersistentListener(slider.onValueChanged, scriptableIntBinder.SetValue);
        }
        
        public static void CreateAndAssignIntVariableForSlider(Slider slider, Type variableType, Type bindingType)
        {
            ThrowIf.NotDerivedFrom<ScriptableInt>(variableType);
            ThrowIf.NotDerivedFrom<SliderIntBinding>(bindingType);
            
            var serializedObject = new SerializedObject(slider);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignIntVariableForSlider((Slider)target, asset, bindingType);
            }
        }
        
        public static void AssignDropdownToEnumVariableEvent(TMP_Dropdown dropdown, Object variableAsset)
        {
            var scriptableEnum = (ScriptableEnum)variableAsset;
            var scriptableEvent = dropdown.gameObject.AddComponent<ScriptableEnumEvent>();
            var serializedEvent = new SerializedObject(scriptableEvent);
            var variableProperty = serializedEvent.FindProperty("_variable");

            var enumValueType = ScriptableEnumEditor.GetEnumValueType(scriptableEnum.GetType());
            var enumValueNames = Enum.GetNames(enumValueType).ToList();
            dropdown.ClearOptions();
            dropdown.AddOptions(enumValueNames);
            
            var enumIntValue = scriptableEnum.GetValue();
            dropdown.SetValueWithoutNotify(enumIntValue);
            
            variableProperty.objectReferenceValue = scriptableEnum;
            
            serializedEvent.ApplyModifiedProperties();
            serializedEvent.Dispose();
            
            UnityEventTools.AddPersistentListener(scriptableEvent.ValueChanged, dropdown.SetValueWithoutNotify);
        }
        
        public static void CreateEnumVariableAndAssignDropdownToEvent(TMP_Dropdown dropdown, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableEnum>(variableType);
            
            CreateNewAsset(dropdown, variableType, OnAssetCreated);
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignDropdownToEnumVariableEvent(dropdown, asset);
            }
        }

        public static void AssignEnumVariableToDropdownEvent(TMP_Dropdown dropdown, Object variableAsset)
        {
            var scriptableEnum = (ScriptableEnum)variableAsset;
            
            UnityEventTools.AddPersistentListener(dropdown.onValueChanged, scriptableEnum.SetValue);
        }
        
        public static void CreateAndAssignEnumVariableToDropdownEvent(TMP_Dropdown dropdown, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableEnum>(variableType);
            
            var serializedObject = new SerializedObject(dropdown);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignEnumVariableToDropdownEvent(dropdown, asset);
            }
        }
        
        public static void AssignToggleToBoolVariableEvent(Toggle toggle, Object variableAsset)
        {
            var scriptableBool = (ScriptableBool)variableAsset;
            var scriptableEvent = toggle.gameObject.AddComponent<ScriptableBoolEvent>();
            var serializedEvent = new SerializedObject(scriptableEvent);
            var variableProperty = serializedEvent.FindProperty("_variable");

            var boolValue = scriptableBool.GetValue();
            toggle.SetIsOnWithoutNotify(boolValue);
            
            variableProperty.objectReferenceValue = scriptableBool;
            
            serializedEvent.ApplyModifiedProperties();
            serializedEvent.Dispose();
            
            UnityEventTools.AddPersistentListener(scriptableEvent.ValueChanged, toggle.SetIsOnWithoutNotify);
        }
        
        public static void CreateBoolVariableAndAssignToggleToEvent(Toggle toggle, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableBool>(variableType);
            
            CreateNewAsset(toggle, variableType, OnAssetCreated);
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignToggleToBoolVariableEvent(toggle, asset);
            }
        }

        public static void AssignBoolVariableToToggleEvent(Toggle toggle, Object variableAsset)
        {
            var scriptableBool = (ScriptableBool)variableAsset;
            
            UnityEventTools.AddPersistentListener(toggle.onValueChanged, scriptableBool.SetValue);
        }
        
        public static void CreateAndAssignBoolVariableToToggleEvent(Toggle toggle, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableBool>(variableType);
            
            var serializedObject = new SerializedObject(toggle);
            var valueChangedProperty = serializedObject.FindProperty("onValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignBoolVariableToToggleEvent(toggle, asset);
            }
        }
        
        public static void AssignFloatVariableToSliderEvent(Slider slider, Object variableAsset)
        {
            var scriptableFloat = (ScriptableFloat)variableAsset;
                
            UnityEventTools.AddPersistentListener(slider.onValueChanged, scriptableFloat.SetValue);
        }
        
        public static void CreateAndAssignFloatVariableToSliderEvent(Slider slider, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableFloat>(variableType);
            
            var serializedObject = new SerializedObject(slider);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignFloatVariableToSliderEvent(slider, asset);
            }
        }
        
        public static void AssignStringVariableToInputFieldEvent(TMP_InputField inputField, Object variableAsset)
        {
            var scriptableString = (ScriptableString)variableAsset;
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableString.SetValue);
        }
        
        public static void CreateAndAssignStringVariableToInputFieldEvent(TMP_InputField inputField, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableString>(variableType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignStringVariableToInputFieldEvent(inputField, asset);
            }
        }

        public static void AssignIntVariableToInputFieldEvent(TMP_InputField inputField, Object variableAsset, Type bindingType)
        {
            var scriptableInt = (ScriptableInt)variableAsset;
            var scriptableIntBinder = (InputFieldIntBinding)inputField.gameObject.AddComponent(bindingType);
            var serializedBinder = new SerializedObject(scriptableIntBinder);
            var variableProperty = serializedBinder.FindProperty("_variable");

            variableProperty.objectReferenceValue = scriptableInt;

            serializedBinder.ApplyModifiedProperties();
            serializedBinder.Dispose();
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableIntBinder.SetValue);
        }
        
        public static void CreateAndAssignIntVariableToInputFieldEvent(TMP_InputField inputField, Type variableType, Type bindingType)
        {
            ThrowIf.NotDerivedFrom<ScriptableInt>(variableType);
            ThrowIf.NotDerivedFrom<InputFieldIntBinding>(bindingType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignIntVariableToInputFieldEvent((TMP_InputField)target, asset, bindingType);
            }
        }

        public static void AssignFloatVariableToInputFieldEvent(TMP_InputField inputField, Object variableAsset, Type bindingType)
        {
            var scriptableFloat = (ScriptableFloat)variableAsset;
            var scriptableFloatBinding = (InputFieldFloatBinding)inputField.gameObject.AddComponent(bindingType);
            var serializedBinder = new SerializedObject(scriptableFloatBinding);
            var variableProperty = serializedBinder.FindProperty("_variable");

            variableProperty.objectReferenceValue = scriptableFloat;

            serializedBinder.ApplyModifiedProperties();
            serializedBinder.Dispose();
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableFloatBinding.SetValue);
        }
        
        public static void CreateAndAssignFloatVariableToInputFieldEvent(TMP_InputField inputField, Type variableType, Type bindingType)
        {
            ThrowIf.NotDerivedFrom<ScriptableFloat>(variableType);
            ThrowIf.NotDerivedFrom<InputFieldFloatBinding>(bindingType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                AssignFloatVariableToInputFieldEvent((TMP_InputField)target, asset, bindingType);
            }
        }
        
        public static void CreateNewAsset(Object targetObject, Type variableType, AssetCreatedCallback callback)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                if (targetObject is Component component)
                {
                    Scene scene = component.gameObject.scene;
                    
                    if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                    {
                        path = Path.GetDirectoryName(scene.path);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(component.gameObject);
                        path = prefab == null ? "Assets" : Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab));
                    }
                }
                else
                {
                    path = "Assets";
                }
            }
            else if (!Directory.Exists(path)) 
            {
                path = Path.GetDirectoryName(path);
            }
            
            ScriptableObject newVariable = ScriptableObject.CreateInstance(variableType);

            AssetCreationCallback action = ScriptableObject.CreateInstance<AssetCreationCallback>();
            action.Setup(targetObject, null, variableType, callback);
            
            string defaultName = $"New{variableType.Name}.asset";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{defaultName}");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action, 
                assetPathAndName,
                AssetPreview.GetMiniThumbnail(newVariable),
                null);
        }
        
        public static void CreateNewAsset(SerializedProperty property, Type variableType, AssetCreatedCallback callback)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                if (property.serializedObject.targetObject is Component component)
                {
                    Scene scene = component.gameObject.scene;
                    
                    if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                    {
                        path = Path.GetDirectoryName(scene.path);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(component.gameObject);
                        path = prefab == null ? "Assets" : Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab));
                    }
                }
                else
                {
                    path = "Assets";
                }
            }
            else if (!Directory.Exists(path)) 
            {
                path = Path.GetDirectoryName(path);
            }
            
            ScriptableObject newVariable = ScriptableObject.CreateInstance(variableType);

            Object target = property.serializedObject.targetObject;
            AssetCreationCallback action = ScriptableObject.CreateInstance<AssetCreationCallback>();
            action.Setup(target, property.propertyPath, variableType, callback);
            
            string defaultName = $"New{variableType.Name}.asset";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{defaultName}");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action, 
                assetPathAndName,
                AssetPreview.GetMiniThumbnail(newVariable),
                null);
        }
    }

}

#endif