#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Events.Editor
{
    [CustomEditor(typeof(ScriptableEvent))]
    public class ScriptableEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Invoke"))
                ((ScriptableEvent)target).Invoke();
        }
    }
}

#endif