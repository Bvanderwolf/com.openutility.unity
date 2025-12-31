using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using OpenUtility.Editor;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TMP_InputField), true)]
    public class TMP_InputFieldEditor_ScriptableVariable : TMP_InputFieldEditor
    {
        private struct BindingData
        {
            public Type variableType;
            public Type bindingType;

            public BindingData(Type variableType, Type bindingType)
            {
                this.variableType = variableType;
                this.bindingType = bindingType;
            }
        }
        
        private static readonly Dictionary<string, BindingData> _bindingDataCache = new Dictionary<string, BindingData>();

        [DidReloadScripts]
        private static void ClearBindingDataCache() => _bindingDataCache.Clear();
        
        private static UnityEngine.Object[] GetScriptableVariableAssets() // TODO: implementeren voor selecteren van bestaande variabelen
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableVariable`1");
            if (guids.Length == 0)
                return (null);

            return null;
        }

        private static Dictionary<string, BindingData> GetBindingData()
        {
            if (_bindingDataCache.Count != 0)
                return (_bindingDataCache);

            TypeCache.TypeCollection collection = TypeCache.GetTypesWithAttribute<ScriptableVariableBinder>();
            foreach (Type type in collection)
            {
                var attribute = type.GetCustomAttribute<ScriptableVariableBinder>();
                if (attribute.TypeOfComponentToBindTo != typeof(TMP_InputField))
                    continue;
                
                var valueType = attribute.TypeOfValue;

                Type variableType;
                Type bindingType;
                if (type.IsAssignableFrom(typeof(ScriptableString)))
                {
                    variableType = type;
                    bindingType = null;
                }
                else
                {
                    variableType = attribute.TypeOfScriptableVariable;
                    bindingType = type;
                }
                
                string nameOfOption = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(valueType.Name);
                string nameOfSubOption = attribute.DisplayName ?? bindingType?.Name ?? variableType.Name;
                string path = $"{nameOfOption}/{nameOfSubOption}";
                
                _bindingDataCache.Add(path, new BindingData(variableType, bindingType));
            }

            return (_bindingDataCache);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scriptable Variables", new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 });
            
            GUIContent content = new GUIContent("Bind Scriptable Variable");
            GUIStyle style = new GUIStyle(GUI.skin.button) { fontSize = 11 };
            Rect rect = GUILayoutUtility.GetRect(content, style);
            rect.height *= 1.25f;
            
            if (GUI.Button(rect, content, style))
                OnBindScriptableVariableButtonClicked(rect, content);
        }

        private void OnBindScriptableVariableButtonClicked(Rect rect, GUIContent content)
        {
            Dictionary<string, BindingData> bindingData = GetBindingData();
            ExtendedDropdownBuilder builder = new ExtendedDropdownBuilder(content.text, rect);

            var stringItems = bindingData.Where(bd => bd.Key.StartsWith("String/")).ToArray();
            builder.StartIndent("String");
            for (int i = 0; i < stringItems.Length; i++)
            {
                var item = stringItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnStringVariableSelected);
            }
            builder.EndIndent();
            
            var intItems = bindingData.Where(bd => bd.Key.StartsWith("Int32/")).ToArray();
            builder.StartIndent("Int");
            for (int i = 0; i < intItems.Length; i++)
            {
                var item = intItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnIntegerVariableSelected);
            }
            builder.EndIndent();
            
            var floatItems = bindingData.Where(bd => bd.Key.StartsWith("Single/")).ToArray();
            builder.StartIndent("Float");
            for (int i = 0; i < floatItems.Length; i++)
            {
                var item = floatItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnFloatVariableSelected);
            }
            builder.EndIndent();
            
            builder.GetResult().Show();
        }

        private void OnStringVariableSelected(object data)
        {
            BindingData bindingData = (BindingData)data;
            Type variableType = bindingData.variableType;
            TMP_InputField inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignStringVariableForInputField(inputField, variableType);
        }
        
        private void OnIntegerVariableSelected(object data)
        {
            BindingData bindingData = (BindingData)data;
            TMP_InputField inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignIntVariableForInputField(inputField, bindingData.variableType, bindingData.bindingType);
        }

        private void OnFloatVariableSelected(object data)
        {
            BindingData bindingData = (BindingData)data;
            TMP_InputField inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignFloatVariableForInputField(inputField, bindingData.variableType, bindingData.bindingType);
        }
    }
}
