using OpenUtility.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenUtility.Samples.Data
{
    /// <summary>
    /// Used to create and hold a reference to a GameObject instance in the scene.
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableGameObject", menuName = "OpenUtility/Samples/ScriptableGameObject")]
    public class ScriptableGameObject : ScriptableVariable<GameObject>
    {
        [SerializeField]
        private GameObject _prefab;
        
        public bool HasValue => _instance.HasValue;

        private Optional<GameObject> _instance;

        private Optional<Transform> _parent;

        /// <summary>
        /// The scene the game object instance is currently being used in.
        /// </summary>
        private Scene? _scene;

        private void OnEnable()
        {
#if UNITY_EDITOR
            _instance = Optional<GameObject>.None();
#endif
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void SetParent(Transform parent)
        {
            _parent = parent;
        }

        public GameObject CreateValue()
        {
            if (_instance.HasValue)
            {
                Debug.Log($"[{name}] Replacing instance '{_instance.Value.name}' with new instance.");
                
                Destroy(_instance.Value);
            }
            
            _instance = _parent.HasValue ? Instantiate(_prefab, _parent.Value) : Instantiate(_prefab);
            _scene = _instance.Value.scene;

            return (_instance.Value);
        }
        
        public void DestroyValue()
        {
            if (!_instance.HasValue)
                return;
            
            Destroy(_instance.Value);
            
            _instance = Optional<GameObject>.None();
            _scene = null;
        }

        public override GameObject GetValue() => (_instance.GetValueOrDefault());

        private void OnSceneUnloaded(Scene unloadedScene)
        {
            if (_scene.HasValue && unloadedScene == _scene.Value)
                _instance = Optional<GameObject>.None();
        }
    }
}
