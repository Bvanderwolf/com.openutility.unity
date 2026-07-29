using System;
using System.Collections.Generic;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace OpenUtility.Data
{
    [CreateAssetMenu(fileName = "ScriptableScene", menuName = "Scriptable Objects/ScriptableScene")]
    public class ScriptableScene : ScriptableVariable<Scene>
    {
        [Serializable]
        public class LoadedEvent : UnityEvent<Scene> { }

        [Header("Settings")]
        [SerializeField, Tooltip("The name of the scene in the build settings")]
        private string _sceneName;

        [SerializeField, Tooltip("Should the scene be created automatically when no instance is set yet?")]
        private bool _loadLazily;

        [SerializeField, HideInInspector]
        private string _assetGuid;

        [Header("Optional")]
        [SerializeField, Tooltip("The mode to load the scene with by default if no function parameters are provided. This can still be overridden by providing parameters to the loading functions.")]
        private Optional<LoadSceneMode> _loadMode;

        [SerializeField, Tooltip("The parameters to load the scene with by default if no function parameters are provided. This can still be overridden by providing parameters to the loading functions.")]
        private Optional<LoadSceneParameters> _loadParameters;

        public event Action<Scene> Loaded; 
        public event Action Unloaded; 
        
        public string SceneName => _sceneName;
        public bool IsLoaded => _scene.HasValue && _scene.Value.isLoaded;
        public bool IsActiveScene => _scene.HasValue && _scene.Value == SceneManager.GetActiveScene();

        private Scene? _scene;

        private void OnEnable()
        {
            Scene scene = GetSceneByName();
            
            if (scene.IsValid())
                AssignLoadedScene(scene);
        }

        /// <summary>
        /// Returns components of type T from the root game objects of the scene.
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            if (!_scene.HasValue)
                return (null);

            Scene scene = _scene.Value;
            if (!scene.IsValid())
            {
                Debug.LogWarning("Trying to get a component from the scene reference while it has an invalid reference to a scene. Make sure to load the scene using this instance before trying to get a component from it.", this);
                return (null);
            }
            
            if (!scene.isLoaded)
            {
                Debug.LogWarning("Trying to get a component from the scene reference while the referenced scene is not loaded. Make sure to load the scene using this instance before trying to get a component from it.", this);
                return (null);
            }
            
            using var pooled = ListPool<GameObject>.Get(out List<GameObject> rootGameObjects);
            scene.GetRootGameObjects(rootGameObjects);
            
            foreach (GameObject gameObject in rootGameObjects)
                if (gameObject.TryGetComponent(out T component))
                    return (component);

            return (null);
        }

        /// <summary>
        /// Sets this scene as the active scene.
        /// </summary>
        public void SetActive()
        {
            if (!IsLoaded)
            {
                Debug.LogWarning($"[{name}] Can't set this scene as active scene as its not loaded.");
                return;
            }

            SceneManager.SetActiveScene(_scene.Value);
        }

        /// <summary>
        /// Loads and assigns the value of the scene using set load mode and/or load parameters.
        /// </summary>
        public void Load() => LoadValue();
        
        /// <summary>
        /// Loads and assigns the value of the scene asynchronously using set load mode and/or load parameters.
        /// </summary>
        public void LoadAsync() => LoadValueAsync();

        /// <summary>
        /// Loads and assigns the value of the scene using set load mode and/or load parameters.
        /// Returns the result.
        /// </summary>
        public Scene LoadValue()
        {
            bool usesLoadSceneMode = _loadMode.TryGetValue(out LoadSceneMode loadSceneMode);
            bool usesLoadSceneParameters = _loadParameters.TryGetValue(out LoadSceneParameters parameters);
            
            if (!usesLoadSceneMode && !usesLoadSceneParameters)
            {
                SceneManager.LoadScene(_sceneName);
            
                _scene = SceneManager.GetActiveScene();

                return (_scene.Value);
            }

            parameters.loadSceneMode = loadSceneMode;
            
            SceneManager.LoadScene(_sceneName, parameters);
            
            _scene = SceneManager.GetSceneByName(_sceneName);
            
            Loaded?.Invoke(_scene.Value);

            return (_scene.Value);
        }
        
        /// <summary>
        /// Loads and assigns the value of the scene using given load mode.
        /// Returns the result.
        /// </summary>
        public Scene LoadValue(LoadSceneMode mode)
        {
            SceneManager.LoadScene(_sceneName, mode);
            
            _scene = SceneManager.GetSceneByName(_sceneName);
            
            Loaded?.Invoke(_scene.Value);

            return (_scene.Value);
        }
        
        /// <summary>
        /// Loads and assigns the value of the scene using given load parameters.
        /// Returns the result.
        /// </summary>
        public Scene LoadValue(LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(_sceneName, parameters);
            
            _scene = SceneManager.GetSceneByName(_sceneName);
            
            Loaded?.Invoke(_scene.Value);

            return (_scene.Value);
        }
        
        /// <summary>
        /// Returns a promise to the scene that is loaded and assigned asynchronously using set load mode and/or load parameters.
        /// </summary>
        public Promised<Scene> LoadValueAsync(Action<float> progress = null)
        {
            bool usesLoadSceneMode = _loadMode.TryGetValue(out LoadSceneMode loadSceneMode);
            bool usesLoadSceneParameters = _loadParameters.TryGetValue(out LoadSceneParameters parameters);

            Promised<Scene> promise;
            
            if (!usesLoadSceneMode && !usesLoadSceneParameters)
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName);
                promise = Promised<Scene>.FromSceneLoad(operation, SceneManager.GetActiveScene, progress).Then(AssignLoadedScene);
            }
            else
            {
                if (usesLoadSceneMode)
                    parameters.loadSceneMode = loadSceneMode;
                
                AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName, parameters);
                promise = Promised<Scene>.FromSceneLoad(operation, GetSceneByName, progress).Then(AssignLoadedScene);
            }

            if (Loaded != null)
                promise.Then(Loaded.Invoke);

            return (promise);
        }

        /// <summary>
        /// Returns a promise to the scene that is loaded and assigned asynchronously using given load mode.
        /// </summary>
        public Promised<Scene> LoadValueAsync(LoadSceneMode mode, Action<float> progress = null)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName, mode);
            Promised<Scene> promise = Promised<Scene>.FromSceneLoad(operation, GetSceneByName, progress).Then(AssignLoadedScene);
            
            if (Loaded != null)
                promise.Then(Loaded.Invoke);
            
            return (promise);
        }

        /// <summary>
        /// Returns a promise to the scene that is loaded and assigned asynchronously using given load parameters.
        /// </summary>
        public Promised<Scene> LoadValueAsync(LoadSceneParameters parameters, Action<float> progress = null)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneName, parameters);
            Promised<Scene> promise = Promised<Scene>.FromSceneLoad(operation, GetSceneByName, progress).Then(AssignLoadedScene);
            
            if (Loaded != null)
                promise.Then(Loaded.Invoke);
            
            return (promise);
        }

        public void UnloadValue() => UnloadValueAsync();

        /// <summary>
        /// Unloads the scene. Optionally add callbacks or use the returned yield instruction for coroutines.
        /// </summary>
        public YieldInstruction UnloadValueAsync(Action callback = null, Action<float> progress = null)
        {
            if (!_scene.HasValue)
            {
                Debug.LogWarning("Trying to unload scene reference while it doesn't have a reference to a scene. Make sure to load the scene using this instance before unloading it.", this);
                return (null);
            }

            Scene scene = _scene.Value;
            if (!scene.IsValid())
            {
                Debug.LogWarning("Trying to unload scene reference while it has an invalid reference to a scene. Make sure to load the scene using this instance before unloading it.", this);
                return (null);
            }

            if (!scene.isLoaded)
            {
                Debug.LogWarning("Trying to unload scene reference while the referenced scene is not loaded. Make sure to load the scene using this instance before unloading it.", this);
                return (null);
            }
            
            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
            return (WaitFor.Operation(operation, OnComplete, progress));

            void OnComplete(AsyncOperation finishedOperation)
            {
                _scene = null;

                Unloaded?.Invoke();
                callback?.Invoke();
            }
        }

        /// <summary>
        /// Returns the value of the scene. If lazy loading is set, uses the set load mode and load parameters to
        /// load the returned value if it is not set yet.
        /// </summary>
        public override Scene GetValue()
        {
            Scene scene;
            
            if (_scene.HasValue)
            {
                scene = _scene.Value;
                
                if (!scene.IsValid())
                    Debug.LogWarning("Detected invalid scene reference. Make sure to unload the scene using the 'UnloadValue' method to prevent this from happening.", this);
                
                return (scene);
            }

            scene = SceneManager.GetSceneByName(_sceneName);
            if (scene.IsValid())
            {
                _scene = scene;
                
                return (scene);
            }

            return _loadLazily ? LoadValue() : scene;
        }

        /// <summary>
        /// Returns the value of the scene. If lazy loading is set, uses the given load mode to load the returned value
        /// if it is not set yet.
        /// </summary>
        public Scene GetValue(LoadSceneMode mode)
        {
            Scene scene;
            
            if (_scene.HasValue)
            {
                scene = _scene.Value;
                
                if (!scene.IsValid())
                    Debug.LogWarning("Detected invalid scene reference. Make sure to unload the scene using the 'UnloadValue' method to prevent this from happening.", this);
                
                return (scene);
            }

            scene = SceneManager.GetSceneByName(_sceneName);
            if (scene.IsValid())
            {
                _scene = scene;
                
                return (scene);
            }

            return _loadLazily ? LoadValue(mode) : scene;
        }

        /// <summary>
        /// Returns the value of the scene. If lazy loading is set, uses the given load parameters to load the returned value
        /// if it is not set yet.
        /// </summary>
        public Scene GetValue(LoadSceneParameters parameters)
        {
            Scene scene;
            
            if (_scene.HasValue)
            {
                scene = _scene.Value;
                
                if (!scene.IsValid())
                    Debug.LogWarning("Detected invalid scene reference. Make sure to unload the scene using the 'UnloadValue' method to prevent this from happening.", this);

                return (scene);
            }

            scene = GetSceneByName();
            if (scene.IsValid())
            {
                _scene = scene;
                
                return (scene);
            }

            return _loadLazily ? LoadValue(parameters) : scene;
        }

        /// <summary>
        /// Returns a promise to the current scene value. If lazy loading is set, it is loaded and assigned asynchronously
        /// using set load mode and/or load parameters if it is not set yet.
        /// </summary>
        public Promised<Scene> GetValueAsync(Action<float> progress = null)
        {
            bool usesLoadSceneMode = _loadMode.TryGetValue(out LoadSceneMode loadSceneMode);
 
            _loadParameters.TryGetValue(out LoadSceneParameters parameters);

            if (usesLoadSceneMode)
                parameters.loadSceneMode = loadSceneMode;

            return (GetValueAsync(parameters, progress));
        }

        /// <summary>
        /// Returns a promise to the current scene value. If lazy loading is set, it is loaded and assigned asynchronously
        /// using given load mode if it is not set yet.
        /// </summary>
        public Promised<Scene> GetValueAsync(LoadSceneMode mode, Action<float> progress = null)
        {
            _loadParameters.TryGetValue(out LoadSceneParameters parameters);

            parameters.loadSceneMode = mode;

            return (GetValueAsync(parameters, progress));
        }

        /// <summary>
        /// Returns a promise to the current scene value. If lazy loading is set, it is loaded and assigned asynchronously
        /// using given load parameters if it is not set yet.
        /// </summary>
        public Promised<Scene> GetValueAsync(LoadSceneParameters parameters, Action<float> progress = null)
        {
            Scene scene;
            Promised<Scene> promise;
            
            if (_scene.HasValue)
            {
                scene = _scene.Value;
                
                if (!scene.IsValid())
                    Debug.LogWarning("Detected invalid scene reference. Make sure to unload the scene using the 'UnloadValue' method to prevent this from happening.", this);

                promise = Promised<Scene>.FromResult(scene);
                progress?.Invoke(1.0f);
                
                return (promise);
            }

            scene = SceneManager.GetSceneByName(_sceneName);
            if (scene.IsValid())
            {
                _scene = scene;
                
                promise = Promised<Scene>.FromResult(scene);
                progress?.Invoke(1.0f);
                
                return (promise);
            }

            if (_loadLazily)
            {
                promise = LoadValueAsync(parameters, progress);
            }
            else
            {
                promise = Promised<Scene>.FromResult(scene);
                progress?.Invoke(1.0f);
            }

            return (promise);
        }

        /// <summary>
        /// Sets the given scene as the current value.
        /// Use this (for example) if your scene is the first to load and you want to store the reference to it.
        /// </summary>
        public override void SetValue(Scene newValue)
        {
            if (_scene.HasValue && _scene.Value.IsValid())
            {
                Debug.LogWarning($"Trying to set scene reference to '{newValue.name}' while it already has a valid reference to '{_scene.Value.name}'. Make sure to unload the previous scene using this instance before setting a new value.", this);
                return;
            }

            _scene = newValue;
        }

        private void AssignLoadedScene(Scene scene)
        {
            _scene = scene;
        }
        
        private Scene GetSceneByName() => SceneManager.GetSceneByName(_sceneName);
    }
}
