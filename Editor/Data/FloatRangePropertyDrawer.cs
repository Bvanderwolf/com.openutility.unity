#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedProperty fromProperty = property.FindPropertyRelative("_from");
            SerializedProperty toProperty = property.FindPropertyRelative("_to");
            
            Rect rect = EditorGUI.PrefixLabel(position, label);
            
            OnDrawProperty(rect, fromProperty);
            
            rect.x += rect.width * 0.5f + EditorGUIUtility.standardVerticalSpacing;
            
            OnDrawProperty(rect, toProperty);
            
            EditorGUI.EndProperty();
        }

        private void OnDrawProperty(Rect position, SerializedProperty property)
        {
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float fieldWidth = position.width * 0.25f;
            Rect fromLabelRect = new Rect(position.x, position.y, fieldWidth, position.height);
            Rect fromFieldRect = new Rect(fromLabelRect.xMax + spacing, position.y, fieldWidth - (spacing * 2.0f), position.height);
            EditorGUI.LabelField(fromLabelRect, property.displayName);
            EditorGUI.PropertyField(fromFieldRect, property, GUIContent.none);
        }
    }
}

#endif