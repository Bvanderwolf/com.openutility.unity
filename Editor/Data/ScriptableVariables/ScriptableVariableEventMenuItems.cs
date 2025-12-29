#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Data.Editor
{
    public static class ScriptableVariableEventMenuItems
    {
        [MenuItem("CONTEXT/Slider/Create Variable")]
        public static void CreateAndAssignVariableForSlider(MenuCommand command)
        {
            var slider = command.context as Slider;
            if (slider == null)
                return;
            
            var serializedObject = new SerializedObject(slider);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            var variableType = typeof(ScriptableFloat);
            
            ScriptableVariableFactory.CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                var scriptableFloat = (ScriptableFloat)asset;
                
                UnityEventTools.AddPersistentListener(slider.onValueChanged, scriptableFloat.SetValue);
            }
        }
        
        [MenuItem("CONTEXT/TMP_InputField/Create String Variable")]
        public static void CreateAndAssignStringVariableForInputField(MenuCommand command)
        {
            var inputField = command.context as TMP_InputField;
            if (inputField == null)
                return;
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            var variableType = typeof(ScriptableString);
            
            ScriptableVariableFactory.CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                var scriptableString = (ScriptableString)asset;
                
                UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableString.SetValue);
            }
        }
        
        [MenuItem("CONTEXT/TMP_InputField/Create Int Variable")]
        public static void CreateAndAssignIntVariableForInputField(MenuCommand command)
        {
            var inputField = command.context as TMP_InputField;
            if (inputField == null)
                return;
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            var variableType = typeof(ScriptableInt);
            
            ScriptableVariableFactory.CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                var scriptableInt = (ScriptableInt)asset;
                
                UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableInt.SetValue);
            }
        }
        
        [MenuItem("CONTEXT/TMP_InputField/Create Float Variable")]
        public static void CreateAndAssignFloatVariableForInputField(MenuCommand command)
        {
            var inputField = command.context as TMP_InputField;
            if (inputField == null)
                return;
            
            var serializedObject = new SerializedObject(inputField);
            var valueChangedProperty = serializedObject.FindProperty("m_OnValueChanged");
            var variableType = typeof(ScriptableFloat);
            
            ScriptableVariableFactory.CreateNewAsset(valueChangedProperty, variableType, OnAssetCreated);
            
            serializedObject.Dispose();
            
            void OnAssetCreated(Object asset, Object target, string propertyName)
            {
                var scriptableFloat = (ScriptableFloat)asset;
                
                UnityEventTools.AddPersistentListener(inputField.onValueChanged, scriptableFloat.SetValue);
            }
        }
    }
}

#endif