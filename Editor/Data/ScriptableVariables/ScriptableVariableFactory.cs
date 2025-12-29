#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    public static class ScriptableVariableFactory
    {
        public delegate void AssetCreatedCallback(Object asset, Object target, string propertyName);
        
        private class AssetCreationCallback : EndNameEditAction
        {
            private Object _target;
            private string _propertyName;
            private Type _variableType;
            private AssetCreatedCallback _callback;

            public void Setup(Object target, string propertyName, Type scriptableObjectType, AssetCreatedCallback callback)
            {
                _target = target;
                _propertyName = propertyName;
                _variableType = scriptableObjectType;
                _callback = callback;
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = CreateInstance(_variableType);
                
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();

                ProjectWindowUtil.ShowCreatedAsset(asset);
                
                _callback?.Invoke(asset, _target, _propertyName);
            }
        }
        
        public static void CreateNewAsset(SerializedProperty property, Type variableType, AssetCreatedCallback callback)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                if (property.serializedObject.targetObject is Component component)
                {
                    Scene scene = component.gameObject.scene;
                    
                    if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                    {
                        path = Path.GetDirectoryName(scene.path);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(component.gameObject);
                        path = prefab == null ? "Assets" : Path.GetDirectoryName(AssetDatabase.GetAssetPath(prefab));
                    }
                }
                else
                {
                    path = "Assets";
                }
            }
            else if (!Directory.Exists(path)) 
            {
                path = Path.GetDirectoryName(path);
            }
            
            ScriptableObject newVariable = ScriptableObject.CreateInstance(variableType);

            Object target = property.serializedObject.targetObject;
            AssetCreationCallback action = ScriptableObject.CreateInstance<AssetCreationCallback>();
            action.Setup(target, property.name, variableType, callback);
            
            string defaultName = $"New{variableType.Name}.asset";
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{defaultName}");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action, 
                assetPathAndName,
                AssetPreview.GetMiniThumbnail(newVariable),
                null);
        }
    }

}

#endif