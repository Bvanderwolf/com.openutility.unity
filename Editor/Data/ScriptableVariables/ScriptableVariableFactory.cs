#if UNITY_EDITOR

using System;
using System.IO;
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
        public delegate void AssetCreatedCallback(Object asset, Object target, string propertyName);
        
        private class AssetCreationCallback : EndNameEditAction
        {
            private Object _target;
            private string _propertyName;
            private Type _variableType;
            private AssetCreatedCallback _callback;

            public void Setup(Object target, string propertyName, Type scriptableObjectType, AssetCreatedCallback callback)
            {
                _target = target;
                _propertyName = propertyName;
                _variableType = scriptableObjectType;
                _callback = callback;
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = CreateInstance(_variableType);
                
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();

                ProjectWindowUtil.ShowCreatedAsset(asset);
                
                _callback?.Invoke(asset, _target, _propertyName);
            }
        }
        
        public static void AssignIntVariableForSlider(Slider slider, Object variableAsset, Type bindingType)
        {
            var scriptableInt = (ScriptableInt)variableAsset;
            var scriptableIntBinder = (Slider_ScriptableIntBinding)slider.gameObject.AddComponent(bindingType);
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
            ThrowIf.NotDerivedFrom<Slider_ScriptableIntBinding>(bindingType);
            
            var serializedObject = new SerializedObject(slider);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                AssignIntVariableForInputField((TMP_InputField)target, asset, bindingType);
            }
        }
        
        public static void AssignFloatVariableForSlider(Slider slider, Object variableAsset)
        {
            var scriptableFloat = (ScriptableFloat)variableAsset;
                
            UnityEventTools.AddPersistentListener(slider.onValueChanged, scriptableFloat.SetValue);
        }
        
        public static void CreateAndAssignFloatVariableForSlider(Slider slider, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableFloat>(variableType);
            
            var serializedObject = new SerializedObject(slider);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                AssignFloatVariableForSlider(slider, asset);
            }
        }
        
        public static void AssignStringVariableForInputField(TMP_InputField inputField, Object variableAsset)
        {
            var scriptableString = (ScriptableString)variableAsset;
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableString.SetValue);
        }
        
        public static void CreateAndAssignStringVariableForInputField(TMP_InputField inputField, Type variableType)
        {
            ThrowIf.NotDerivedFrom<ScriptableString>(variableType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                AssignStringVariableForInputField(inputField, asset);
            }
        }

        public static void AssignIntVariableForInputField(TMP_InputField inputField, Object variableAsset, Type bindingType)
        {
            var scriptableInt = (ScriptableInt)variableAsset;
            var scriptableIntBinder = (TMP_InputField_ScriptableIntBinding)inputField.gameObject.AddComponent(bindingType);
            var serializedBinder = new SerializedObject(scriptableIntBinder);
            var variableProperty = serializedBinder.FindProperty("_variable");

            variableProperty.objectReferenceValue = scriptableInt;

            serializedBinder.ApplyModifiedProperties();
            serializedBinder.Dispose();
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableIntBinder.SetValue);
        }
        
        public static void CreateAndAssignIntVariableForInputField(TMP_InputField inputField, Type variableType, Type bindingType)
        {
            ThrowIf.NotDerivedFrom<ScriptableInt>(variableType);
            ThrowIf.NotDerivedFrom<TMP_InputField_ScriptableIntBinding>(bindingType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                AssignIntVariableForInputField((TMP_InputField)target, asset, bindingType);
            }
        }

        public static void AssignFloatVariableForInputField(TMP_InputField inputField, Object variableAsset, Type bindingType)
        {
            var scriptableFloat = (ScriptableFloat)variableAsset;
            var scriptableFloatBinding = (TMP_InputField_ScriptableFloatBinding)inputField.gameObject.AddComponent(bindingType);
            var serializedBinder = new SerializedObject(scriptableFloatBinding);
            var variableProperty = serializedBinder.FindProperty("_variable");

            variableProperty.objectReferenceValue = scriptableFloat;

            serializedBinder.ApplyModifiedProperties();
            serializedBinder.Dispose();
                
            UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableFloatBinding.SetValue);
        }
        
        public static void CreateAndAssignFloatVariableForInputField(TMP_InputField inputField, Type variableType, Type bindingType)
        {
            ThrowIf.NotDerivedFrom<ScriptableFloat>(variableType);
            ThrowIf.NotDerivedFrom<TMP_InputField_ScriptableFloatBinding>(bindingType);
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            
            CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                AssignFloatVariableForInputField((TMP_InputField)target, asset, bindingType);
            }
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
            action.Setup(target, property.name, variableType, callback);
            
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