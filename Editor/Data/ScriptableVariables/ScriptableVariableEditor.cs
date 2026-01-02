#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    [CustomEditor(typeof(ScriptableString))]
    public class ScriptableStringEditor : ScriptableVariableEditor<ScriptableString> { }
    
    [CustomEditor(typeof(ScriptableInt))]
    public class ScriptableIntEditor : ScriptableVariableEditor<ScriptableInt> { }
    
    [CustomEditor(typeof(ScriptableFloat))]
    public class ScriptableFloatEditor : ScriptableVariableEditor<ScriptableFloat> { }
    
    [CustomEditor(typeof(ScriptableBool))]
    public class ScriptableBoolEditor : ScriptableVariableEditor<ScriptableBool> { }
    
    public class ScriptableVariableEditor<T> : UnityEditor.Editor
    {
        private SerializedProperty _valueProperty;
        private PropertyInfo _runtimeValueInfo;
        
        private void OnEnable()
        {
            _valueProperty = serializedObject.FindProperty("_value");
            _runtimeValueInfo = typeof(T).GetProperty("value", BindingFlags.NonPublic | BindingFlags.Instance);
            EditorApplication.update += RepaintWhilePlaying;
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintWhilePlaying;
        }
        
        private void RepaintWhilePlaying()
        {
            if (Application.isPlaying)
                Repaint();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUILayout.PropertyField(_valueProperty, new GUIContent("Default"));
            DrawRuntimeValue();
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "_value");

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuntimeValue()
        {
            if (!Application.isPlaying)
                return;

            if (_runtimeValueInfo == null)
            {
                Debug.LogWarning($"Could not find runtime value info for {target.GetType().Name}. Make sure to name the variable 'value'.");
                return;
            }

            EditorGUI.BeginDisabledGroup(true);
            object value = _runtimeValueInfo.GetValue(target);
            switch (value)
            {
                case bool boolean:
                    EditorGUILayout.Toggle("Current", boolean);
                    break;
                
                case float single:
                    EditorGUILayout.FloatField("Current", single);
                    break;
                
                case int integer:
                    EditorGUILayout.IntField("Current", integer);
                    break;
                
                case string str:
                    EditorGUILayout.TextField("Current", str);
                    break;
                
                default:
                    EditorGUILayout.LabelField("Current", "N/A");
                    break;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

#endif