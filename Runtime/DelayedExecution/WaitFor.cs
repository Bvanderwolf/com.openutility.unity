using System;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using OpenUtility.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace OpenUtility.DelayedExecution
{
    public static class WaitFor
    {
        private class SecondsComparer : IEqualityComparer<float>
        {
            public bool Equals(float a, float b) => Mathf.Approximately(a, b);

            public int GetHashCode(float obj) => obj.GetHashCode();
        }

        private static readonly Dictionary<float, WaitForSeconds> _awaitSecondsCache = new(new SecondsComparer());

        private static readonly Dictionary<float, WaitForSecondsRealtime> _awaitSecondsRealtimeCache = new(new SecondsComparer());

        private static readonly WaitForFixedUpdate _awaitFixedUpdateInstance = new WaitForFixedUpdate();
        private static readonly WaitForEndOfFrame _awaitEndOfFrameInstance = new WaitForEndOfFrame();

        private static Optional<ActionAwaiter> _awaiter;

        public static WaitForFixedUpdate FixedUpdate => _awaitFixedUpdateInstance;
        public static WaitForEndOfFrame EndOfFrame => _awaitEndOfFrameInstance;

        /// <summary>
        /// Issues the scrolling of the scroll view and invokes the callback when complete.
        /// </summary>
        public static YieldInstruction Scroll(ScrollRect scrollView, ScrollOptions options = default, Action callback = null)
        {
            ThrowIf.Null(scrollView);
            
            return (GetOrCreateAwaiter().WaitForScroll(scrollView, options, callback));
        }

        /// <summary>
        /// Issues the localization request and invokes the callback when complete.
        /// </summary>
        public static YieldInstruction Localization(LocalizedString localizedString, Action<string> callback = null)
        {
            ThrowIf.Null(localizedString);

            return (GetOrCreateAwaiter().WaitForLocalization(localizedString, callback));
        }
        
        /// <summary>
        /// Issues the web request and invokes the callback when complete.
        /// </summary>
        public static YieldInstruction WebRequest(UnityWebRequest request, Action<UnityWebRequest> callback = null)
        {
            ThrowIf.Null(request);
            
            return (GetOrCreateAwaiter().WaitForWebRequest(request, callback));
        }
        
        /// <summary>
        /// Invokes the callback when the operation is complete.
        /// </summary>
        public static YieldInstruction Operation(AsyncOperation operation, Action<AsyncOperation> callback = null)
        {
            ThrowIf.Null(operation);
            
            return (GetOrCreateAwaiter().WaitForOperation(operation, callback));
        }
        
        /// <summary>
        /// Invokes the callback when the operation is complete.
        /// </summary>
        public static YieldInstruction Operation<T>(AsyncOperationBase<T> operation, Action<AsyncOperationBase<T>> callback = null)
        {
            ThrowIf.Null(operation);
            
            return (GetOrCreateAwaiter().WaitForOperation(operation, callback));
        }
        
        /// <summary>
        /// Invokes the callback when the operation is complete, with optional progress updates.
        /// </summary>
        public static YieldInstruction Operation(AsyncOperationHandle operation, Action<RequestResult> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operation);
            
            return (GetOrCreateAwaiter().WaitForOperation(operation, callback, progress));
        }
        
        /// <summary>
        /// Invokes the callback when the operation is complete, with optional progress updates.
        /// </summary>
        public static YieldInstruction Operation<T>(AsyncOperationHandle<T> operation, Action<DataRequestResult<T>> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operation);
            
            return (GetOrCreateAwaiter().WaitForOperation(operation, callback, progress));
        }
        
        /// <summary>
        /// Invokes the callback when the operations are complete, with optional progress updates.
        /// </summary>
        public static YieldInstruction Operations<T>(AsyncOperationHandle<T>[] operations, Action<DataRequestResult<T[]>> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operations);
            
            return (GetOrCreateAwaiter().WaitForOperations(operations, callback, progress));
        }

        public static WaitForSeconds Seconds(float seconds)
        {
            if (_awaitSecondsCache.TryGetValue(seconds, out var instance))
                return (instance);

            instance = new WaitForSeconds(seconds);
            _awaitSecondsCache[seconds] = instance;

            return (instance);
        }

        public static WaitForSecondsRealtime RealtimeSeconds(float seconds)
        {
            if (_awaitSecondsRealtimeCache.TryGetValue(seconds, out var instance))
                return (instance);

            instance = new WaitForSecondsRealtime(seconds);
            _awaitSecondsRealtimeCache[seconds] = instance;

            return (instance);
        }
        
        private static ActionAwaiter GetOrCreateAwaiter()
        {
            if (_awaiter.HasValue)
                return (_awaiter.Value);
            
            GameObject instance = new GameObject("~WaitFor.ActionAwaiter");
            ActionAwaiter awaiter = instance.AddComponent<ActionAwaiter>();
            Object.DontDestroyOnLoad(instance);

            _awaiter = awaiter;
            
            return (awaiter);
        }
    }
}
