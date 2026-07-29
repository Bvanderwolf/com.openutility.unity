#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    public static class ScriptableSceneMenuItems
    {
        [MenuItem("Assets/OpenUtility/Create Scriptable Scene")]
        public static void CreateScriptableScene()
        {
            Object asset = Selection.activeObject;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string directory = Path.GetDirectoryName(assetPath);
            string variableName = asset.name;
            string assetPathAndName = Path.Combine(directory, $"{variableName}.asset");
            var sceneAsset = ScriptableObject.CreateInstance<ScriptableScene>();
            
            AssetDatabase.CreateAsset(sceneAsset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            
            Selection.activeObject = sceneAsset;
        }

        [MenuItem("Assets/OpenUtility/Create Scriptable Scene", true)]
        private static bool ValidateCreateScriptableScene()
        {
            return (Selection.activeObject is SceneAsset);
        }
    }
}

#endif