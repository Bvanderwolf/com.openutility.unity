#if UNITY_EDITOR

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
    [CustomEditor(typeof(TMP_Dropdown), true)]
    [CanEditMultipleObjects]
    public class DropdownEditor_ScriptableVariable : DropdownEditor
    {
        private static readonly Dictionary<string, BindingData> _bindingDataCache = new Dictionary<string, BindingData>();
        private static readonly Dictionary<string, SelectionData> _selectionDataCache = new Dictionary<string, SelectionData>();
        
        private static Type[] SupportedVariableTypes { get; } = new Type[]
        {
            typeof(ScriptableEnum)
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
            
            Dictionary<string, BindingData> bindingData = GetBindingData();

            var assets = guids.Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<Object>);
            var dictionary = new Dictionary<Type, List<Object>>();

            foreach (var asset in assets)
            {
                var typeOfAsset = asset.GetType();
                if (!SupportedVariableTypes.Any(type => type.IsAssignableFrom(typeOfAsset)))
                    continue;
                
                if (bindingData.All(bd => bd.Value.variableType != typeOfAsset))
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
                if (attribute.TypeOfComponentToBindTo != typeof(TMP_Dropdown))
                    continue;
                
                var valueType = attribute.TypeOfValue;

                Type variableType;
                Type bindingType;
                if (type.IsAssignableFrom(typeof(ScriptableEnum)))
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
            Texture2D variableIcon = (Texture2D)EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            Dictionary<string, SelectionData> selectionData = GetSelectableItems();
            ExtendedDropdownBuilder builder = new ExtendedDropdownBuilder("Select Binding", rect);
            
            var enumItems = selectionData.Where(bd => bd.Key.StartsWith("Int32/")).ToArray();
            builder.StartIndent("Enum");
            for (int i = 0; i < enumItems.Length; i++)
            {
                var item = enumItems[i];
                var path = item.Key;
                var itemName = path.Substring(path.IndexOf('/') + 1);
                
                builder.AddItem(itemName, false, variableIcon, item.Value, OnSelectEnumVariableBinding);
            }
            builder.EndIndent();
            
            var maxItemsPerColumn = Mathf.Max(enumItems.Length);
            var minimumHeight = (maxItemsPerColumn + 3) * 20f;
            var minimumSize = new Vector2(rect.width, minimumHeight);
            builder.AddMinimumSize(minimumSize).GetResult().Show();
        }

        private void OnSelectEnumVariableBinding(object data)
        {
            var selectionData = (SelectionData)data;
            var variableAsset = selectionData.variableAsset;
            var dropdown = (TMP_Dropdown)target;
            
            ScriptableVariableFactory.AssignEnumVariableForDropdown(dropdown, variableAsset);
        }

        private void OnCreateButtonClicked(Rect rect)
        {
            Texture2D variableIcon = (Texture2D)EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            Dictionary<string, BindingData> bindingData = GetBindingData();
            ExtendedDropdownBuilder builder = new ExtendedDropdownBuilder("Create Binding", rect);
            
            var enumItems = bindingData.Where(bd => bd.Key.StartsWith("Int32/")).ToArray();
            builder.StartIndent("Enum");
            for (int i = 0; i < enumItems.Length; i++)
            {
                var item = enumItems[i];
                var itemName = item.Key.Split('/')[1];
                
                builder.AddItem(itemName, false, variableIcon, item.Value, OnCreateEnumVariableBinding);
            }
            builder.EndIndent();

            var maxItemsPerColumn = Mathf.Max(SupportedVariableTypes.Length, enumItems.Length);
            var minimumHeight = (maxItemsPerColumn + 3) * 20f;
            var minimumSize = new Vector2(rect.width, minimumHeight);
            builder.AddMinimumSize(minimumSize).GetResult().Show();
        }

        private void OnCreateEnumVariableBinding(object data)
        {
            var bindingData = (BindingData)data;
            var variableType = bindingData.variableType;
            var dropdown = (TMP_Dropdown)target;
            
            ScriptableVariableFactory.CreateAndAssignEnumVariableForDropdown(dropdown, variableType);
        }
    }
}

#endif