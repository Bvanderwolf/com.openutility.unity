using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public enum ConnectionQuality
    {
        Excellent = 50,
        Good = 100,
        Fair = 200,
        Poor = 201
    }
    
    public class ConnectionQualityIndicator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float _refreshInterval = 10f;
        
        [SerializeField]
        private bool _beginOnStart = true;
        
        [Header("References")]
        [SerializeField]
        private ScriptableConnectionQuality _connection;
        
        [SerializeField]
        private ConnectionQualitySpriteList _sprites;

        [SerializeField]
        private Image _image;

        private bool _shouldUpdateIndicator = false;
        private float _timeTillNextRefresh = 0f;

        private Task _refreshTask;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnEnable()
        {
            _connection.ValueChanged.AddListener(OnConnectionQualityChanged);
        }

        private void OnDisable()
        {
            _connection.ValueChanged.RemoveListener(OnConnectionQualityChanged);
        }

        private void Start()
        {
            if (_beginOnStart)
                Begin();
        }

        private void Update()
        {
            if (!_shouldUpdateIndicator)
                return;

            if (_refreshTask != null && !_refreshTask.IsCompleted)
                return;

            _timeTillNextRefresh += Time.unscaledDeltaTime;
            if (_timeTillNextRefresh < _refreshInterval)
                return;

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _refreshTask = _connection.RefreshAsync(cancellationToken: _cancellationTokenSource.Token);
            _timeTillNextRefresh = 0.0f;
        }

        public void Begin()
        {
            Debug.Log("Beginning connection quality indicator updates.");
            
            _shouldUpdateIndicator = true;
            _timeTillNextRefresh = 0.0f;
        }

        public void End()
        {
            Debug.Log("Ending connection quality indicator updates.");
            
            _shouldUpdateIndicator = false;
            _cancellationTokenSource?.Cancel();
        }

        private void OnConnectionQualityChanged(int newElapsedMs)
        {
            if (newElapsedMs == -1)
                return;
            
            var quality = GetConnectionQualityFromElapsedMs(newElapsedMs);
            var sprite = _sprites.GetValue(quality);
            
            _image.sprite = sprite;
        }
        
        private static ConnectionQuality GetConnectionQualityFromElapsedMs(int elapsedMs)
        {
            int[] values = (int[])Enum.GetValues(typeof(ConnectionQuality));

            for (int i = 0; i < values.Length; i++)
                if (elapsedMs < values[i])
                    return ((ConnectionQuality)values[i]);

            return (ConnectionQuality.Poor);
        }
    }
}
