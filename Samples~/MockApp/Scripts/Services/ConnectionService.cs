using System;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenUtility.Samples.Data
{
    public class ConnectionService : MockAppService
    {
        public bool IsConnected
        {
            get
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                    return false;

                return CheckedConnection && _isConnected;
            }
        }

        public bool CheckedConnection { get; private set; } = false;
        public event Action<bool> ConnectionChanged;

        private bool _isConnected = false;

        public void CheckConnection()
        {
            WaitFor.Connection(OnConnectionCheckComplete);
        }

        private void OnConnectionCheckComplete(RequestResult result)
        {
            if (result.success)
            {
                Debug.Log("Connection check successful.");
            }
            else
            {
                Debug.LogWarning($"Failed connection check: {result.error}");
            }

            bool previousConnectionStatus = _isConnected;

            _isConnected = result.success;
            CheckedConnection = true;

            if (previousConnectionStatus != _isConnected)
                ConnectionChanged?.Invoke(_isConnected);
        }
    }
}
