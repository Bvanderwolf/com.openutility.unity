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
using Object = UnityEngine.Object;

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

        private struct SelectionData
        {
            public Object variableAsset;
            public Type bindingType;
            
            public SelectionData(Object variableAsset, Type bindingType)
            {
                this.variableAsset = variableAsset;
                this.bindingType = bindingType;
            }
        }
        
        private static readonly Dictionary<string, BindingData> _bindingDataCache = new Dictionary<string, BindingData>();
        private static readonly Dictionary<string, SelectionData> _selectionDataCache = new Dictionary<string, SelectionData>();
        
        private static Type[] SupportedVariableTypes { get; } = new Type[]
        {
            typeof(ScriptableString),
            typeof(ScriptableInt),
            typeof(ScriptableFloat)
        };

        [DidReloadScripts]
        private static void ClearBindingDataCache()
        {
            _bindingDataCache.Clear();
            _selectionDataCache.Clear();
        }
        
        private static Dictionary<Type, List<Object>> GetScriptableVariableAssetData() 
        {
            var guids = AssetDatabase.FindAssets("t:ScriptableVariable`1");
            if (guids.Length == 0)
                return (null);

            var assets = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Object>);
            var dictionary = new Dictionary<Type, List<Object>>();

            foreach (var asset in assets)
            {
                var typeOfAsset = asset.GetType();
                if (!ArrayUtility.Contains(SupportedVariableTypes, typeOfAsset))
                    continue;
                
                var attribute = typeOfAsset.GetCustomAttribute<ScriptableVariableBinder>();
                if (attribute != null && attribute.TypeOfComponentToBindTo != typeof(TMP_InputField))
                    continue;
                
                if (!dictionary.TryGetValue(typeOfAsset, out List<Object> list))
                {
                    list = new List<Object>();
                    dictionary[typeOfAsset] = list;
                }
                
                var assignableTypes = dictionary.Keys.Where(t => t.IsAssignableFrom(typeOfAsset));
                foreach (var type in assignableTypes)
                    dictionary[type].Add(asset);
            }

            return (dictionary);
        }
        
        private static Dictionary<string, SelectionData> GetSelectableItems()
        {
            if (_selectionDataCache.Count != 0)
                return (_selectionDataCache);

            Dictionary<string, BindingData> bindingData = GetBindingData();
            Dictionary<Type, List<Object>> assetData = GetScriptableVariableAssetData();
            foreach (KeyValuePair<string, BindingData> dataPoint in bindingData)
            {
                var nameOfOption = dataPoint.Key;
                var data = dataPoint.Value;
                
                if (!assetData.TryGetValue(data.variableType, out List<Object> assets))
                    continue;
                
                foreach (Object asset in assets)
                {
                    string nameOfSubOption = asset.name;
                    string path = $"{nameOfOption}/{nameOfSubOption}";
                    _selectionDataCache.Add(path, new SelectionData(asset, data.bindingType));
                }
            }

            return (_selectionDataCache);
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
            
            var content = new GUIContent("Bind Scriptable Variable");
            var selectButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 11 };
            var selectRect = GUILayoutUtility.GetRect(content, selectButtonStyle);
            selectRect.height = 20;
            selectRect.width -= 24;
            
            if (GUI.Button(selectRect, content, selectButtonStyle))
                OnSelectButtonClicked(selectRect);
            
            var createRect = new Rect(selectRect.xMax + 4, selectRect.y, 20, selectRect.height);
            var buttonContent = EditorGUIUtility.IconContent("Toolbar Plus");
            var buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };  
            
            if (GUI.Button(createRect, buttonContent, buttonStyle))
                OnCreateButtonClicked(selectRect);
        }

        private void OnSelectButtonClicked(Rect rect)
        {
            Dictionary<string, SelectionData> selectionData = GetSelectableItems();
            ExtendedDropdownBuilder builder = new ExtendedDropdownBuilder("Select Binding", rect);

            var stringItems = selectionData.Where(bd => bd.Key.StartsWith("String/")).ToArray();
            builder.StartIndent("String");
            for (int i = 0; i < stringItems.Length; i++)
            {
                var item = stringItems[i];
                var path = item.Key;
                var itemName = path.Substring(path.IndexOf('/') + 1);
                
                builder.AddItem(itemName, false, item.Value, OnSelectStringVariableBinding);
            }
            builder.EndIndent();
            
            var intItems = selectionData.Where(bd => bd.Key.StartsWith("Int32/")).ToArray();
            builder.StartIndent("Int");
            for (int i = 0; i < intItems.Length; i++)
            {
                var item = intItems[i];
                var path = item.Key;
                var itemName = path.Substring(path.IndexOf('/') + 1);
                
                builder.AddItem(itemName, false, item.Value, OnSelectIntegerVariableBinding);
            }
            builder.EndIndent();
            
            var floatItems = selectionData.Where(bd => bd.Key.StartsWith("Single/")).ToArray();
            builder.StartIndent("Float");
            for (int i = 0; i < floatItems.Length; i++)
            {
                var item = floatItems[i];
                var path = item.Key;
                var itemName = path.Substring(path.IndexOf('/') + 1);
                
                builder.AddItem(itemName, false, item.Value, OnSelectFloatVariableBinding);
            }
            builder.EndIndent();
            
            var maxItemsPerColumn = Mathf.Max(stringItems.Length, intItems.Length, floatItems.Length);
            var minimumHeight = (maxItemsPerColumn + 3) * 20f;
            var minimumSize = new Vector2(rect.width, minimumHeight);
            builder.AddMinimumSize(minimumSize).GetResult().Show();
        }
        
        private void OnSelectStringVariableBinding(object data)
        {
            var selectionData = (SelectionData)data;
            var variableAsset = selectionData.variableAsset;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.AssignStringVariableForInputField(inputField, variableAsset);
        }
        
        private void OnSelectIntegerVariableBinding(object data)
        {
            var selectionData = (SelectionData)data;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.AssignIntVariableForInputField(inputField, selectionData.variableAsset, selectionData.bindingType);
        }

        private void OnSelectFloatVariableBinding(object data)
        {
            var selectionData = (SelectionData)data;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.AssignFloatVariableForInputField(inputField, selectionData.variableAsset, selectionData.bindingType);
        }

        private void OnCreateButtonClicked(Rect rect)
        {
            Dictionary<string, BindingData> bindingData = GetBindingData();
            ExtendedDropdownBuilder builder = new ExtendedDropdownBuilder("Create Binding", rect);

            var stringItems = bindingData.Where(bd => bd.Key.StartsWith("String/")).ToArray();
            builder.StartIndent("String");
            for (int i = 0; i < stringItems.Length; i++)
            {
                var item = stringItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnCreateStringVariableBinding);
            }
            builder.EndIndent();
            
            var intItems = bindingData.Where(bd => bd.Key.StartsWith("Int32/")).ToArray();
            builder.StartIndent("Int");
            for (int i = 0; i < intItems.Length; i++)
            {
                var item = intItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnCreateIntegerVariableBinding);
            }
            builder.EndIndent();
            
            var floatItems = bindingData.Where(bd => bd.Key.StartsWith("Single/")).ToArray();
            builder.StartIndent("Float");
            for (int i = 0; i < floatItems.Length; i++)
            {
                var item = floatItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, item.Value, OnCreateFloatVariableBinding);
            }
            builder.EndIndent();

            var maxItemsPerColumn = Mathf.Max(3, stringItems.Length, intItems.Length, floatItems.Length);
            var minimumHeight = (maxItemsPerColumn + 3) * 20f;
            var minimumSize = new Vector2(rect.width, minimumHeight);
            builder.AddMinimumSize(minimumSize).GetResult().Show();
        }

        private void OnCreateStringVariableBinding(object data)
        {
            var bindingData = (BindingData)data;
            var variableType = bindingData.variableType;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignStringVariableForInputField(inputField, variableType);
        }
        
        private void OnCreateIntegerVariableBinding(object data)
        {
            var bindingData = (BindingData)data;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignIntVariableForInputField(inputField, bindingData.variableType, bindingData.bindingType);
        }

        private void OnCreateFloatVariableBinding(object data)
        {
            var bindingData = (BindingData)data;
            var inputField = (TMP_InputField)target;
            
            ScriptableVariableFactory.CreateAndAssignFloatVariableForInputField(inputField, bindingData.variableType, bindingData.bindingType);
        }
    }
}
