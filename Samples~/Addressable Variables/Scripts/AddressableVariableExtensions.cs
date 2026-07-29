#if ENABLE_OPENUTILITY_ADDRESSABLE_SAMPLE

using System;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace OpenUtility.Samples.Data
{
    public static class AddressableVariableExtensions
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AddDefine()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            const string define = "OPENUTILITY_ADDRESSABLE_SAMPLES";

            var activeBuildProfile = UnityEditor.Build.Profile.BuildProfile.GetActiveBuildProfile();

            if (activeBuildProfile != null)
            {
                string[] defines = activeBuildProfile.scriptingDefines;
                if (Array.IndexOf(defines, define) >= 0)
                    return;

                string[] newDefines = new string[defines.Length + 1];
                Array.Copy(defines, newDefines, defines.Length);
                newDefines[defines.Length] = define;
                activeBuildProfile.scriptingDefines = newDefines;
                UnityEditor.EditorUtility.SetDirty(activeBuildProfile);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(activeBuildProfile);
            }
            else
            {
                var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);

                UnityEditor.PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);
                if (Array.IndexOf(defines, define) >= 0)
                    return;

                string[] newDefines = new string[defines.Length + 1];
                Array.Copy(defines, newDefines, defines.Length);
                newDefines[defines.Length] = define;
                UnityEditor.PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefines);
            }
        }
#endif
        
        ///<summary>
        /// Creates a new instance of the game object using the addressables library. If no key is set, uses the game object's name as key.
        /// Returns a promise of the created instance value. If an instance already exists, it will be destroyed before creating a new one.
        /// </summary>
        public static Promised<GameObject> CreateAddressableValueAsync(this ScriptableGameObject gameObject, string key = null, Action<DownloadStatus> progress = null)
        {
            if (gameObject.HasValue)
            {
                Debug.Log($"[{gameObject.name}] Replacing instance '{gameObject.name}' with new instance.");
                
                gameObject.DestroyValue();
            }

            key ??= gameObject.name;

            AsyncOperationHandle<GameObject> operation = Addressables.LoadAssetAsync<GameObject>(key);
            Promised<GameObject> promise = CreatePromiseFromOperation(operation, progress).Then(gameObject.SetValue);

            return (promise);
        }
        
        /// <summary>
        /// Returns the current instance of the game object. If 'instantiateLazy' is set and no instance is set yet,
        /// creates a new instance using the addressables library. If no key is set, uses the game object's name as key.
        /// Returns a promise to the (created) instance value.
        /// </summary>
        public static Promised<GameObject> GetAddressableValueAsync(this ScriptableGameObject gameObject, string key = null, Action<DownloadStatus> progress = null)
        {
            if (gameObject.HasValue || !gameObject.InstantiatedLazily)
                return Promised<GameObject>.FromResult(gameObject.GetValue());

            key ??= gameObject.name;

            AsyncOperationHandle<GameObject> operation = Addressables.LoadAssetAsync<GameObject>(key);
            Promised<GameObject> promise = CreatePromiseFromOperation(operation, progress).Then(gameObject.SetValue);

            return (promise);
        }
        
        public static Promised<SceneInstance> LoadAddressableValueAsync(this ScriptableScene scene, string key = null, Action<DownloadStatus> progress = null)
        {
            key ??= scene.SceneName;

            AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(key);
            Promised<SceneInstance> promise = CreatePromiseFromSceneLoad(operation, progress).Then(OnComplete);

            return (promise);

            void OnComplete(SceneInstance result)
            {
                if (!result.Scene.IsValid())
                    return;
                
                scene.SetValue(result.Scene);
            }
        }
        
        public static Promised<SceneInstance> CreatePromiseFromSceneLoad(AsyncOperationHandle<SceneInstance> operation, Action<DownloadStatus> progress = null)
        {
            Promised<SceneInstance> promise = PromisePool<SceneInstance>.Get();

            WaitFor.Operation(operation, OnComplete, progress);

            return (promise);

            void OnComplete(DataRequestResult<SceneInstance> result)
            {
                if (result.success)
                {
                    promise.Value = result.data;
                }
                else
                {
                    promise.Error = result.error;
                }
            }
        }
        
        /// <summary>
        /// Returns a promise that is fullfilled if given addressable operation is completed.
        /// </summary>
        public static Promised<T> CreatePromiseFromOperation<T>(AsyncOperationHandle<T> operation, Action<DownloadStatus> progress = null)
        {
            Promised<T> promise = PromisePool<T>.Get();

            WaitFor.Operation(operation, OnComplete, progress);

            return (promise);

            void OnComplete(DataRequestResult<T> result)
            {
                if (result.success)
                {
                    promise.Value = result.data;
                }
                else
                {
                    promise.Error = result.error;
                }
            }
        }
    }
}

#endif