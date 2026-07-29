using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public class ReleaseGameObject : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Tooltip("Allowed range from the pool component's start position to release the game object. If a component is set to 0, it will be ignored.")]
        private Optional<Vector3> _range;

        [SerializeField, Tooltip("Timer in seconds to wait before releasing the game object.")]
        private Optional<float> _timer;

        private float _currentTimer;
        private Vector3 _startPosition;

        private void OnEnable()
        {
            _currentTimer = 0.0f;
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (!_timer.HasValue)
                return;

            _currentTimer += Time.deltaTime;
            
            if (_currentTimer < _timer.Value)
                return;

            gameObject.Release();
        }

        private void FixedUpdate()
        {
            if (!_range.TryGetValue(out Vector3 range))
                return;
            
            float allowedX = range.x == 0.0f ? float.PositiveInfinity : range.x;
            float allowedY = range.y == 0.0f ? float.PositiveInfinity : range.y;
            float allowedZ = range.z == 0.0f ? float.PositiveInfinity : range.z;
            Vector3 position = transform.position;
            bool outOfRange = Mathf.Abs(position.x - _startPosition.x) > allowedX ||
                              Mathf.Abs(position.y - _startPosition.y) > allowedY ||
                              Mathf.Abs(position.z - _startPosition.z) > allowedZ;
            
            if (outOfRange)
                gameObject.Release();
        }
    }
}
