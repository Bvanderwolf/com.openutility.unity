#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using OpenUtility.Editor;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    [CustomEditor(typeof(ScriptableScene))]
    public class ScriptableSceneEditor : UnityEditor.Editor
    {
        private SerializedProperty _scriptProperty;
        private SerializedProperty _sceneNameProperty;
        private SerializedProperty _assetGuidProperty;
        private const string SCENE_NAME_PROPERTY_NAME = "_sceneName";

        private string[] _assetPaths;
        private readonly string[] _searchInFolders = new string[] { "Assets" };

        private void OnEnable()
        {
            _scriptProperty = serializedObject.FindProperty("m_Script");
            _sceneNameProperty = serializedObject.FindProperty(SCENE_NAME_PROPERTY_NAME);
            _assetGuidProperty = serializedObject.FindProperty("_assetGuid");
            
            SetSceneNameFromAssetInfo();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_scriptProperty);
            EditorGUI.EndDisabledGroup();

            OnScenePropertyGUI();
            DrawPropertiesExcluding(serializedObject, SCENE_NAME_PROPERTY_NAME, "m_Script");
            
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            
            OnInfoBoxGUI();
        }

        private void OnInfoBoxGUI()
        {
            if (string.IsNullOrEmpty(_sceneNameProperty.stringValue))
                return;
            
            EditorGUILayout.Space();

            string scenePath = GetSelectedSceneAssetPath();
            
            var buildScenes = EditorBuildSettings.scenes;
            for (int i = 0; i < buildScenes.Length; i++)
            {
                string path = buildScenes[i].path;
                if (!string.IsNullOrEmpty(scenePath))
                {
                    if (path == scenePath)
                        return;
                    
                    continue;
                }

                string sceneName = Path.GetFileNameWithoutExtension(path);
                if (sceneName == _sceneNameProperty.stringValue)
                    return;
            }
            
            EditorGUILayout.HelpBox("The scene is not in the build settings.", MessageType.Warning);

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(scenePath)))
            {
                if (GUILayout.Button("Add Scene To Build Settings"))
                    AddSceneToBuildSettings(scenePath);
            }
        }

        private void OnScenePropertyGUI()
        {
            EditorGUILayout.BeginHorizontal();
            var label = new GUIContent("Scene");
            EditorGUILayout.PrefixLabel(label);

            var content = new GUIContent(_sceneNameProperty.stringValue);
            var rect = GUILayoutUtility.GetRect(content, "MiniPullDown");
            bool dropdown = EditorGUI.DropdownButton(rect, content, FocusType.Passive);
            EditorGUILayout.EndHorizontal();
            
            if (!dropdown) 
                return;
            
            var builder = new ExtendedDropdownBuilder("Scene", rect);
            string[] paths = GetSceneAssetPaths();
            
            Array.Sort(paths, (lhs, rhs) => string.Compare(Path.GetFileName(lhs), Path.GetFileName(rhs), StringComparison.OrdinalIgnoreCase));

            for (int i = 0; i < paths.Length; i++)
            {
                string sceneName = Path.GetFileNameWithoutExtension(paths[i]);
                builder.AddItem(sceneName, false, paths[i], OnSceneItemClicked);
            }

            var result = builder.GetResult();
            result.Show();
        }

        private void OnSceneItemClicked(object item)
        {
            string path = (string)item;
            string sceneName = Path.GetFileNameWithoutExtension(path);
            string guid = AssetDatabase.AssetPathToGUID(path);
            
            _sceneNameProperty.stringValue = sceneName;
            _assetGuidProperty.stringValue = guid;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SetSceneNameFromAssetInfo()
        {
            string[] paths = GetSceneAssetPaths();
            if (paths.Length == 0)
                return;
            
            if (string.IsNullOrEmpty(_sceneNameProperty.stringValue))
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(paths[i]);
                    if (sceneName != target.name)
                        continue;

                    string guid = AssetDatabase.AssetPathToGUID(paths[i]);
                
                    _sceneNameProperty.stringValue = sceneName;
                    _assetGuidProperty.stringValue = guid;
                
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(_assetGuidProperty.stringValue))
            {
                string path = paths.FirstOrDefault(p => AssetDatabase.AssetPathToGUID(p) == _assetGuidProperty.stringValue);
                if (string.IsNullOrEmpty(path))
                {
                    _assetGuidProperty.stringValue = string.Empty;
                    _sceneNameProperty.stringValue = string.Empty;
                }
                else
                {
                    string sceneName = Path.GetFileNameWithoutExtension(path);
                    if (sceneName == _sceneNameProperty.stringValue)
                        return;
                    
                    Debug.Log($"Found scene asset with matching guid but different name. Updating scene name to match asset name. Asset Path: {path}", target);
                    
                    _sceneNameProperty.stringValue = sceneName;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private string GetSelectedSceneAssetPath()
        {
            if (!string.IsNullOrEmpty(_assetGuidProperty.stringValue))
            {
                string path = AssetDatabase.GUIDToAssetPath(_assetGuidProperty.stringValue);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }

            if (string.IsNullOrEmpty(_sceneNameProperty.stringValue))
                return string.Empty;

            return GetSceneAssetPaths()
                .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == _sceneNameProperty.stringValue)
                ?? string.Empty;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return;

            var scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Any(scene => scene.path == scenePath))
                return;

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private string[] GetSceneAssetPaths()
        {
            if (_assetPaths != null)
                return (_assetPaths);
            
            var guids = AssetDatabase.FindAssets("t:SceneAsset", _searchInFolders);
            if (guids.Length == 0)
                return (Array.Empty<string>());
            
            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            _assetPaths = paths;

            return (paths);
        }
    }
}

#endif