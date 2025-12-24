using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Pooling.Editor
{
    [CustomEditor(typeof(ScriptablePoolBase<>), true)]

    public class ScriptablePoolEditor : UnityEditor.Editor
    {
        private Type _scriptablePoolBaseType;
        private PropertyInfo _poolProperty;

        // ObjectPool<T> reflection
        private PropertyInfo _countAllProp;
        private PropertyInfo _countActiveProp;
        private PropertyInfo _countInactiveProp;

        private SerializedProperty _maxSize;

        private void OnEnable()
        {
            _scriptablePoolBaseType = FindScriptablePoolBaseType(target.GetType());
            if (_scriptablePoolBaseType == null)
                return;

            _poolProperty = _scriptablePoolBaseType.GetProperty(
                "pool",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            _maxSize = serializedObject.FindProperty("_maxSize");

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

            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            DrawRuntimeState();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuntimeState()
        {
            EditorGUILayout.LabelField("Runtime Pool State", EditorStyles.boldLabel);

            var pool = GetPoolInstance();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Pool state is only available in Play Mode.\nThe pool is lazily created on first Get().", MessageType.Info);
                return;
            }

            if (pool == null)
            {
                EditorGUILayout.HelpBox("Pool has not been created yet.", MessageType.Warning);
                return;
            }

            if (!BindObjectPool(pool.GetType()))
            {
                EditorGUILayout.HelpBox($"Unsupported pool type: {pool.GetType().Name}", MessageType.Warning);
                return;
            }

            int countAll = (int)_countAllProp.GetValue(pool);
            int countActive = (int)_countActiveProp.GetValue(pool);
            int countInactive = (int)_countInactiveProp.GetValue(pool);

            EditorGUILayout.LabelField("Created", countAll.ToString());
            EditorGUILayout.LabelField("Active", countActive.ToString());
            EditorGUILayout.LabelField("Inactive", countInactive.ToString());

            if (_maxSize.intValue > 0)
            {
                float fill = Mathf.Clamp01(countInactive / (float)_maxSize.intValue);
                var rect = EditorGUILayout.GetControlRect(false, 18);
                EditorGUI.ProgressBar(rect, fill, $"Inactive / Max ({countInactive}/{_maxSize.intValue})");
            }
        }

        // ---------- Helpers ----------

        private object GetPoolInstance()
        {
            return _poolProperty?.GetValue(target);
        }

        private bool BindObjectPool(Type poolType)
        {
            if (_countAllProp != null && _countAllProp.DeclaringType == poolType)
                return true;

            _countAllProp = poolType.GetProperty("CountAll");
            _countActiveProp = poolType.GetProperty("CountActive");
            _countInactiveProp = poolType.GetProperty("CountInactive");

            return _countAllProp != null &&
                   _countActiveProp != null &&
                   _countInactiveProp != null;
        }

        private static Type FindScriptablePoolBaseType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(ScriptablePoolBase<>))
                    return type;

                type = type.BaseType;
            }

            return null;
        }
    }
}
