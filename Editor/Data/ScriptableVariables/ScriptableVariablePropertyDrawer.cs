#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableVariable<>), true)]
    public class ScriptableVariablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawPropertyField(position, property, label);
            DrawButton(position, property);
        }

        private void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            float widthAdjustment = property.objectReferenceValue == null ? 20f : 0f;
            Rect fieldRect = new Rect(position.x, position.y, position.width - widthAdjustment, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);
        }
        
        private void DrawButton(Rect position, SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
                return;
            
            GUIContent buttonContent = EditorGUIUtility.IconContent("Toolbar Plus");
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };  
            
            Rect buttonRect = new Rect(position.x + position.width - 18, position.y, 18, position.height);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AssetCreationOptions options = CreateOptions(fieldInfo.FieldType);
                ScriptableVariableFactory.CreateNewAsset(property, fieldInfo.FieldType, OnAssetCreated, options);
            }
        }

        private AssetCreationOptions CreateOptions(Type variableType)
        {
            if (typeof(ScriptableGameObject).IsAssignableFrom(variableType))
            {
                return (new AssetCreationOptions
                {
                    creationMethod = CreateScriptableGameObjectInstance,
                    inheritNameFromTarget = true
                });
            }
            
            return (AssetCreationOptions.Default);
        }
        
        private ScriptableObject CreateScriptableGameObjectInstance(Type scriptableObjectType) => (ScriptableGameObject.FromScene());

        private void OnAssetCreated(Object asset, Object target, string propertyPath)
        {
            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyPath);
            property.objectReferenceValue = asset;
                
            serializedObject.ApplyModifiedProperties();
            serializedObject.Dispose();
        }
    }
}

#endif