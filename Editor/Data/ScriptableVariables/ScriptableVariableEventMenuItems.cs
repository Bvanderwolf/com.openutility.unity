#if UNITY_EDITOR

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
    }
}

#endif