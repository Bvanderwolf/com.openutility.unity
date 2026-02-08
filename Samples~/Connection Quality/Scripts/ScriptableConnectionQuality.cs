using System;
using System.Threading;
using System.Threading.Tasks;
using OpenUtility.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenUtility.Samples.Data
{
    /// <summary>
    /// Holds the estimated connection quality based on ping time in milliseconds to a specified target.
    /// Is -1 if no connection could be established.
    /// </summary>
    public class ScriptableConnectionQuality : ScriptableInt
    {
        /// <summary>
        /// The type of target to ping. 
        /// </summary>
        private enum PingTargetType
        {
            /// <summary>
            /// Target is an IP address or hostname.
            /// </summary>
            [InspectorName("host")]
            HOST,
            
            /// <summary>
            /// Target is a full URL.
            /// </summary>
            [InspectorName("url")]
            URL
        }

        [Header("Settings")]
        [SerializeField, Tooltip("The type of target to ping. host: IP address or hostname. url: full URL.")]
        private PingTargetType _targetType = PingTargetType.HOST;
        
        [SerializeField]
        private string _hostOrUrl = "8.8.8.8";
        
        private const int PING_TIMEOUT_MS = 1000;
        private const string TIMEOUT_MESSAGE = "Ping request timed out.";
        private const string FAILURE_MESSAGE = "Ping request failed.";
        private const string UNSUPPORTED_TARGET_MESSAGE = "Unsupported ping target type.";

        
        public async Task RefreshAsync(int timeoutMs = PING_TIMEOUT_MS, CancellationToken cancellationToken = default)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("No internet connection available. Cannot perform ping. Setting value to -1.");
                SetValue(-1);
                return;
            }
            
            Task<DataRequestResult<int>> task;

            switch (_targetType)
            {
                case PingTargetType.HOST:
                    task = RunIcmpPingAsync(_hostOrUrl, timeoutMs, cancellationToken);
                    break;
                
                case PingTargetType.URL:
                    task = RunHttpPingAsync(_hostOrUrl, timeoutMs, cancellationToken);
                    break;
                
                default:
                    task = Task.FromResult(DataRequestResult<int>.CreateError(UNSUPPORTED_TARGET_MESSAGE));
                    break;
            }

            try
            {
                Debug.Log($"Starting ping to {_hostOrUrl}...");
                
                DataRequestResult<int> result = await task;
                if (result.success)
                {
                    int elapsedMs = result.data;
                    
                    Debug.Log($"Received ping response in {elapsedMs} ms.");
                    
                    SetValue(elapsedMs);
                }
                else
                {
                    Debug.LogError($"{result.error}. Setting value to -1.");
                    
                    SetValue(-1);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Ping operation was canceled. Keeping last known value.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        private static async Task<DataRequestResult<int>> RunIcmpPingAsync(string host, int timeoutMs, CancellationToken cancellationToken)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return (DataRequestResult<int>.CreateError("ICMP ping is not supported on WebGL builds."));
#else
            
            var ping = new Ping(host);
            float realtimeSinceStart = Time.realtimeSinceStartup;

            while (!ping.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();

                float elapsedMs = (Time.realtimeSinceStartup - realtimeSinceStart) * 1000f;
                if (elapsedMs >= timeoutMs) 
                    return (DataRequestResult<int>.CreateError(TIMEOUT_MESSAGE));

                await Task.Yield();
            }
            
            if (ping.time == -1)
                return (DataRequestResult<int>.CreateError(FAILURE_MESSAGE));

            return (DataRequestResult<int>.CreateSuccess(ping.time));
#endif
        }
        
        private static async Task<DataRequestResult<int>> RunHttpPingAsync(string url, int timeoutMs, CancellationToken cancellationToken)
        {
            using var request = UnityWebRequest.Head(url);
            request.timeout = Mathf.CeilToInt(timeoutMs / 1000f);

            float realtimeSinceStart = Time.realtimeSinceStartup;

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success) 
                return (DataRequestResult<int>.CreateError(request.error));

            float elapsed = Time.realtimeSinceStartup - realtimeSinceStart;
            int elapsedMs = Mathf.CeilToInt(elapsed * 1000f);
            return (DataRequestResult<int>.CreateSuccess(elapsedMs));
        }
    }
}
