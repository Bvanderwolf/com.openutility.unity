#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    [CustomEditor(typeof(ScriptableString), true)]
    public class ScriptableStringEditor : ScriptableVariableEditor { }
    
    [CustomEditor(typeof(ScriptableInt), true)]
    public class ScriptableIntEditor : ScriptableVariableEditor { }
    
    [CustomEditor(typeof(ScriptableFloat), true)]
    public class ScriptableFloatEditor : ScriptableVariableEditor { }
    
    [CustomEditor(typeof(ScriptableBool),true)]
    public class ScriptableBoolEditor : ScriptableVariableEditor { }
    
    [CustomEditor(typeof(ScriptableVector2), true)]
    public class ScriptableVector2Editor : ScriptableVariableEditor { }
    
    [CustomEditor(typeof(ScriptableDouble), true)]
    public class ScriptableDoubleEditor : ScriptableVariableEditor { }
    
    public class ScriptableVariableEditor : UnityEditor.Editor
    {
        private SerializedProperty _valueProperty;
        private SerializedProperty _playerPrefProperty;
        
        private void OnEnable()
        {
            _valueProperty = serializedObject.FindProperty("_value");
            _playerPrefProperty = serializedObject.FindProperty("_playerPref");
            
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

            DrawDefaultValue();
            DrawRuntimeValue();
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "_value");

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            
            DrawOptionalPlayerPrefGUI();
        }

        private void DrawDefaultValue()
        {
            EditorGUILayout.PropertyField(_valueProperty, new GUIContent("Default"));
        }

        private void SetValueWithNotify(object value)
        {
            switch (target)
            {
                case ScriptableBool boolean:
                    boolean.SetValue((bool)value); 
                    break;
                
                case ScriptableFloat single:
                    single.SetValue((float)value);
                    break;
                
                case ScriptableInt integer:
                    integer.SetValue((int)value);
                    break;
                
                case ScriptableString str:
                    str.SetValue((string)value);
                    break;
                
                case ScriptableVector2 vector2:
                    vector2.SetValue((Vector2)value);
                    break;
                
                case ScriptableDouble doubleValue:
                    doubleValue.SetValue((double)value);
                    break;
            }
        }

        private void DrawRuntimeValue()
        {
            if (!Application.isPlaying)
                return;

            object value = null;
            
            EditorGUI.BeginChangeCheck();
            switch (target)
            {
                case ScriptableBool boolean:
                    value = EditorGUILayout.Toggle("Current", boolean);
                    break;
                
                case ScriptableFloat single:
                    value = EditorGUILayout.FloatField("Current", single);
                    break;
                
                case ScriptableInt integer:
                    value = EditorGUILayout.IntField("Current", integer);
                    break;
                
                case ScriptableString str:
                    value = EditorGUILayout.TextField("Current", str);
                    break;
                
                case ScriptableVector2 vector2:
                    value = EditorGUILayout.Vector2Field("Current", vector2);
                    break;
                
                case ScriptableDouble doubleValue:
                    value = EditorGUILayout.DoubleField("Current", doubleValue);
                    break;
                
                default:
                    EditorGUILayout.LabelField("Current", "N/A");
                    break;
            }
            
            if (EditorGUI.EndChangeCheck())
                SetValueWithNotify(value);
            
            EditorGUILayout.HelpBox("Updating the current value will trigger change events.", MessageType.Info);
        }

        private void DrawOptionalPlayerPrefGUI()
        {
            if (_playerPrefProperty == null)
                return;

            SerializedProperty value = _playerPrefProperty.FindPropertyRelative("_value");
            string key = value.stringValue;

            if (string.IsNullOrEmpty(key))
                return;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("PlayerPref Info", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(true);
            switch (target)
            {
                case ScriptableBool:
                    bool boolValue = PlayerPrefs.GetInt(key) == 1;
                    EditorGUILayout.Toggle("Stored", boolValue);
                    break;
                
                case ScriptableFloat:
                    float floatValue = PlayerPrefs.GetFloat(key);
                    EditorGUILayout.FloatField("Stored", floatValue);
                    break;
                
                case ScriptableInt:
                    int intValue = PlayerPrefs.GetInt(key);
                    EditorGUILayout.IntField("Stored", intValue);
                    break;
                
                case ScriptableString:
                    string stringValue = PlayerPrefs.GetString(key);
                    EditorGUILayout.TextField("Stored", stringValue);
                    break;
                
                case ScriptableVector2:
                    var xkey = $"{key}_x";
                    var ykey = $"{key}_y";
                    var x = PlayerPrefs.GetFloat(xkey);
                    var y = PlayerPrefs.GetFloat(ykey);
                    var vector2Value = new Vector2(x, y);
                    EditorGUILayout.Vector2Field("Stored", vector2Value);
                    break;
                
                default:
                    EditorGUILayout.LabelField("Stored", "N/A");
                    break;
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Delete PlayerPref"))
            {
                DeletePlayerPref(key);
                Repaint();
            }
        }

        private void DeletePlayerPref(string key)
        {
            switch (target)
            {
                case ScriptableVector2:
                    PlayerPrefs.DeleteKey($"{key}_X");
                    PlayerPrefs.DeleteKey($"{key}_Y");
                    break;
                
                default:
                    PlayerPrefs.DeleteKey(key);
                    break;
            }
        }
    }
}

#endif