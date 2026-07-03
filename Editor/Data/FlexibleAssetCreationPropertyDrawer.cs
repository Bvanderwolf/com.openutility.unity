#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    [CustomPropertyDrawer(typeof(FlexibleAssetCreationAttribute))]
    public class FlexibleAssetCreationPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawPropertyField(position, property, label);
            DrawButton(position, property);
        }

        private void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            float widthAdjustment = IsAssignable(property) ? 20f : 0f;
            Rect fieldRect = new Rect(position.x, position.y, position.width - widthAdjustment, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);
        }

        private bool IsAssignable(SerializedProperty property)
        {
            if (!typeof(ScriptableObject).IsAssignableFrom(fieldInfo.FieldType))
                return (false);
            
            if (property.objectReferenceValue != null)
                return (false);

            return (true);
        }
        
        private void DrawButton(Rect position, SerializedProperty property)
        {
            if (!IsAssignable(property))
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
                ScriptableVariableFactory.CreateNewAsset(property, fieldInfo.FieldType, OnAssetCreated, AssetCreationOptions.Default);
        }

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