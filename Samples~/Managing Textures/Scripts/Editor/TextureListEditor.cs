using System;
using System.IO;
using System.Linq;
using OpenUtility.Data;
using OpenUtility.Data.Editor;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Samples.Data.Editor
{
    [CustomEditor(typeof(TextureList))]
    public class TextureListEditor : ScriptableListEditor
    {
        private SerializedProperty _sasTokenProperty;
        private SerializedProperty _domainProperty;
        private SerializedProperty _cacheDirectoryProperty;

        private int _fileCacheCount;
        private float _totalCacheSize;
        private readonly string[] _extensions = new[] { ".jpg", ".png", ".tga", ".exr" };
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _sasTokenProperty = serializedObject.FindProperty("_sasToken");
            _domainProperty = serializedObject.FindProperty("_domain");
            _cacheDirectoryProperty = serializedObject.FindProperty("_cacheDirectory");

            ReInitializeFileCacheInfo();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
                return;
            
            OnDownloadInfoGUI();
            OnFileInfoGUI();
        }

        private void OnDownloadInfoGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Download Info", EditorStyles.boldLabel);

            if (_domainProperty.FindPropertyRelative("_hasValue").boolValue)
            {
                string domain = GetValueFromSerializedStringReferenceProperty(_domainProperty);
                string sasToken = GetValueFromSerializedStringReferenceProperty(_sasTokenProperty);
                bool sasTokenHasValue = _sasTokenProperty.FindPropertyRelative("_hasValue").boolValue;
                string url = sasTokenHasValue ? $"{domain}/[fileName]{sasToken}" : $"{domain}/[fileName]";
                EditorGUILayout.HelpBox($"Textures added from file will be downloaded from: {url}.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Textures added from file will be downloaded expecting 'fileName' to be the complete URL.\nSet a domain to prepend to 'fileName' when downloading textures.", MessageType.Info);
            }
        }

        private void OnFileInfoGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("File Info", EditorStyles.boldLabel);
            
            bool usesCacheDirectory = _cacheDirectoryProperty.FindPropertyRelative("_hasValue").boolValue;
            if (usesCacheDirectory)
            {
                string subdirectory = GetValueFromSerializedStringReferenceProperty(_cacheDirectoryProperty);
                EditorGUILayout.HelpBox($"Textures added from file will be cached in {Path.Combine(Application.persistentDataPath, subdirectory)}.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"Textures added from file will be cached in {TextureList.DefaultCacheDirectory}.\nSet a cache directory to organize cached textures.", MessageType.Info);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Files in Cache", EditorStyles.boldLabel);

            EditorGUILayout.LabelField($"Number of files in cache: {_fileCacheCount}");
            EditorGUILayout.LabelField($"Total cache size: {_totalCacheSize:F2} MB");
            
            if (_fileCacheCount > 0 && GUILayout.Button("Clear Cache"))
                ClearCache();

            if (_fileCacheCount > 0 && GUILayout.Button("Open Cache"))
            {
                string path = usesCacheDirectory 
                    ? Path.Combine(Application.persistentDataPath, GetValueFromSerializedStringReferenceProperty(_cacheDirectoryProperty)) 
                    : TextureList.DefaultCacheDirectory;
                
                EditorUtility.RevealInFinder(path);
            }
        }

        private void ReInitializeFileCacheInfo()
        {
            string cacheDirectory = TextureList.DefaultCacheDirectory;
            if (_cacheDirectoryProperty.FindPropertyRelative("_hasValue").boolValue)
            {
                string subdirectory = GetValueFromSerializedStringReferenceProperty(_cacheDirectoryProperty);
                cacheDirectory = Path.Combine(Application.persistentDataPath, subdirectory);
            }

            if (Directory.Exists(cacheDirectory))
            {
                string[] files = Directory.GetFiles(cacheDirectory);
                _fileCacheCount = 0;
                _totalCacheSize = 0f;

                foreach (string file in files)
                {
                    if (!_extensions.Contains(Path.GetExtension(file)))
                        continue;
                    
                    FileInfo fileInfo = new FileInfo(file);
                    _totalCacheSize += fileInfo.Length;
                    _fileCacheCount++;
                }

                _totalCacheSize /= (1024f * 1024f); // convert to MB
            }
            else
            {
                _fileCacheCount = 0;
                _totalCacheSize = 0f;
            }
        }

        private string GetValueFromSerializedStringReferenceProperty(SerializedProperty property)
        {
            SerializedProperty value = property.FindPropertyRelative("_value");
            SerializedProperty valueSource = value.FindPropertyRelative("_valueSource");

            switch ((VariableValueSource)valueSource.enumValueIndex)
            {
                case VariableValueSource.Local:
                    return (value.FindPropertyRelative("_localValue").stringValue);
                
                case VariableValueSource.Shared:
                    return (value.FindPropertyRelative("_variable")?.FindPropertyRelative("_value")?.stringValue ?? string.Empty);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueSource));
            }
        }

        private void ClearCache()
        {
            TextureList textureList = (TextureList)target;
            textureList.ClearFileCache();
            
            ReInitializeFileCacheInfo();
        }
    }
}