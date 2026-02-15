#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    public static class ScriptableGameObjectMenuItems
    {
        [MenuItem("Assets/OpenUtility/Share", false)]
        private static void ExecuteShareOnPrefabOrScriptAsset()
        {
            Object selected = Selection.activeObject;
            if (selected == null)
                return;

            if (selected is MonoScript script)
            {
                Type typeOfComponent = script.GetClass();
                if (typeOfComponent == null || !typeof(Component).IsAssignableFrom(typeOfComponent))
                    return;
                
                GameObject instance = new GameObject(script.name);
                instance.AddComponent(typeOfComponent);
                instance.AddComponent<ShareGameObject>();

                string assetPath = ScriptableVariableFactory.GetAssetPathForNewVariable(selected);
                string assetPathAndName = $"{assetPath}/{instance.name}.prefab";
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, assetPathAndName);
                
                CreateVariableForPrefab(prefab);
                
                Object.DestroyImmediate(instance);
            }
            else if (PrefabUtility.GetPrefabAssetType(selected) != PrefabAssetType.NotAPrefab)
            {
                GameObject prefab = (GameObject)selected;
                
                CreateVariableForPrefab(prefab);
            }

            void CreateVariableForPrefab(GameObject prefab)
            {
               if (!prefab.TryGetComponent(out ShareGameObject component))
                    component = prefab.AddComponent<ShareGameObject>();
               
               AssetCreationOptions options = new AssetCreationOptions
                {
                    creationMethod = ScriptableObject.CreateInstance,
                    inheritNameFromTarget = true
                };
            
                ScriptableVariableFactory.CreateNewAsset(component, typeof(ScriptableGameObject), OnAssetCreated, options);

                void OnAssetCreated(Object asset, Object target, string propertyPath)
                {
                    SerializedObject serializedObject = new SerializedObject(target);
                    SerializedProperty property = serializedObject.FindProperty("_variable");
                    property.objectReferenceValue = asset;
                
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Dispose();

                    serializedObject = new SerializedObject(asset);
                    property = serializedObject.FindProperty("_prefab");
                    property.objectReferenceValue = ((ShareGameObject)target).gameObject;
                    
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Dispose();
                }
            }
        }

        [MenuItem("GameObject/OpenUtility/Share", false, 3)]
        private static void ExcecuteShareOnGameObjectInstance()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
                return;

            if (!selected.TryGetComponent(out ShareGameObject component))
                component = selected.AddComponent<ShareGameObject>();

            AssetCreationOptions options = new AssetCreationOptions
            {
                creationMethod = ScriptableObject.CreateInstance,
                inheritNameFromTarget = true
            };
            
            ScriptableVariableFactory.CreateNewAsset(component, typeof(ScriptableGameObject), OnAssetCreated, options);

            void OnAssetCreated(Object asset, Object target, string propertyPath)
            {
                SerializedObject serializedObject = new SerializedObject(target);
                SerializedProperty property = serializedObject.FindProperty("_variable");
                property.objectReferenceValue = asset;
                
                serializedObject.ApplyModifiedProperties();
                serializedObject.Dispose();
            }
        }

        [MenuItem("GameObject/OpenUtility/Share", true)]
        private static bool ValidateShareOnGameObjectInstance()
        {
            if (Selection.count == 0)
                return (false);
            
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
                return (false);

            return (true);
        }

        [MenuItem("Assets/OpenUtility/Share", true)]
        private static bool ValidateShareOnPrefabAsset()
        {
            if (Selection.count == 0)
                return (false);
            
            Object selected = Selection.activeObject;
            if (selected == null)
                return (false);

            bool isScriptAsset = selected is MonoScript script && script.GetClass() != null && typeof(Component).IsAssignableFrom(script.GetClass());
            bool isPrefabAsset = PrefabUtility.GetPrefabAssetType(selected) != PrefabAssetType.NotAPrefab;
            
            return isScriptAsset || isPrefabAsset;
        }
    }
}

#endif