#if UNITY_EDITOR

using System;
using UnityEditor;

namespace OpenUtility.Data.Editor
{
    [CustomEditor(typeof(ScriptableEnum<>), true)]
    public class ScriptableEnumEditor : UnityEditor.Editor
    {
        private Type _enumType;

        private void OnEnable()
        {
            _enumType = GetScriptableEnumType(target.GetType());
        }

        public override void OnInspectorGUI()
        {
            if (_enumType == null)
            {
                base.OnInspectorGUI();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                serializedObject.Update();
            
                EditorGUI.BeginDisabledGroup(true);
                SerializedProperty script = serializedObject.FindProperty("m_Script");
                EditorGUILayout.PropertyField(script);
                EditorGUI.EndDisabledGroup();
            
                SerializedProperty value = serializedObject.FindProperty("_value");
                Enum enumValue = (Enum)Enum.ToObject(_enumType, value.intValue);
                Enum newEnumValue = EditorGUILayout.EnumPopup(value.displayName, enumValue);
                if (EditorGUI.EndChangeCheck())
                {
                    int newIntValue = Convert.ToInt32(newEnumValue);
                    value.intValue = newIntValue;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        
        private Type GetScriptableEnumType(Type objectType)
        {
            while (objectType != null)
            {
                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ScriptableEnum<>))
                {
                    return objectType.GetGenericArguments()[0];
                }
                
                objectType = objectType.BaseType;
            }
            return null;
        }
    }
}

#endif