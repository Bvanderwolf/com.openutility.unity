using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OpenUtility.Data.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableVariable<>), true)]
    public class ScriptableVariablePropertyDrawer : PropertyDrawer
    {
        private class AssetCreationCallback : EndNameEditAction
        {
            private Object _editor;
            private string _propertyName;
            private Type _variableType;

            public void Setup(Object editor, string propertyName, Type scriptableObjectType)
            {
                _editor = editor;
                _propertyName = propertyName;
                _variableType = scriptableObjectType;
            }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                ScriptableObject asset = CreateInstance(_variableType);
                
                AssetDatabase.CreateAsset(asset, pathName);
                AssetDatabase.SaveAssets();

                ProjectWindowUtil.ShowCreatedAsset(asset);
                
                SerializedObject serializedObject = new SerializedObject(_editor);
                SerializedProperty property = serializedObject.FindProperty(_propertyName);
                property.objectReferenceValue = asset;
                
                serializedObject.ApplyModifiedProperties();
                serializedObject.Dispose();
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawPropertyField(position, property, label);
            DrawButton(position, property);
        }

        private void DrawPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            float widthAdjustment = property.objectReferenceValue == null ? 20f : 0f;
            Rect fieldRect = new Rect(position.x, position.y, position.width - widthAdjustment, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);
        }
        
        private void DrawButton(Rect position, SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
                return;
            
            GUIContent buttonContent = EditorGUIUtility.IconContent("Toolbar Plus");
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter
            };  
            
            Rect buttonRect = new Rect(position.x + position.width - 18, position.y, 18, position.height);
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
                CreateNewScriptableObjectAsset(property);
        }

        private void CreateNewScriptableObjectAsset(SerializedProperty property)
        {
            // Determine the path (usually the currently selected folder)
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                if (property.serializedObject.targetObject is MonoBehaviour monoBehaviour)
                {
                    // check if the MonoBehaviour is part of a scene
                    Scene scene = monoBehaviour.gameObject.scene;
                    
                    if (scene.IsValid() && !string.IsNullOrEmpty(scene.path))
                    {
                        path = Path.GetDirectoryName(scene.path);
                    }
                    else
                    {
                        GameObject prefab = PrefabUtility.GetNearestPrefabInstanceRoot(monoBehaviour.gameObject);
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
            
            Type variableType = fieldInfo.FieldType;
            ScriptableObject newVariable = ScriptableObject.CreateInstance(variableType);

            Object editor = property.serializedObject.targetObject;
            AssetCreationCallback action = ScriptableObject.CreateInstance<AssetCreationCallback>();
            action.Setup(editor, property.name, variableType);
            
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