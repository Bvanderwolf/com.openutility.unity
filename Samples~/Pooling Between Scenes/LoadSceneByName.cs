using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenUtility.Samples.Pooling.BetweenScenes
{
    public class LoadSceneByName : MonoBehaviour
    {
        [SerializeField]
        private KeyCode _key;

        [SerializeField]
        private string _sceneName;

#if UNITY_EDITOR
        private void OnGUI()
        {
            float x = Screen.width / 2;
            float y = Screen.height / 2;

            GUIStyle style = new GUIStyle(UnityEditor.EditorStyles.label);
            style.fontSize = 50;

            GUI.Label(new Rect(x - 300, y + 230, 1200, 100), $"Press {_key.ToString()} to load the new scene.", style);
        }
#endif
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(_key))
                SceneManager.LoadScene(_sceneName);
        }
    }
}