using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Samples.Pooling.Custom
{
    public class EntitySpawner : MonoBehaviour
    {
        [SerializeField]
        private EntityPool _pool;

        private List<EntityBehaviour> _entities = new List<EntityBehaviour>();

        private void Start()
        {
            _pool.SetParent(transform);
        }
        
#if UNITY_EDITOR
        private void OnGUI()
        {
            float x = Screen.width / 2;
            float y = Screen.height / 2;
        
            GUIStyle style = new GUIStyle(UnityEditor.EditorStyles.label);
            style.fontSize = 50;
        
            GUI.Label(new Rect(x - 300, y + 10, 1200, 100), "Press TAB to spawn an entity.", style);
            GUI.Label(new Rect(x - 300, y + 120, 1200, 100), "Press BACKSPACE to release an entity.", style);
        }
#endif
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                _entities.Add(_pool.Get());

            if (Input.GetKeyDown(KeyCode.Backspace) && _entities.Count > 0)
            {
                _pool.Release(_entities[0]);
                _entities.RemoveAt(0);
            }
        }
    }
}