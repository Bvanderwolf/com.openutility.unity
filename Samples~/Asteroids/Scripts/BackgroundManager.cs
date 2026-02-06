using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class BackgroundManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField]
        private RectTransform _top;

        [SerializeField]
        private RectTransform _bottom;

        private const float SCROLL_SPEED = 75.0f;

        private Vector2 _topPosition;
        private Vector2 _resetPosition;

        private void Awake()
        {
            _topPosition = _top.position;
            
            _resetPosition = _bottom.position;
            _resetPosition.y -= _bottom.rect.height;
        }

        private void Update()
        {
            float delta = Time.deltaTime * SCROLL_SPEED;
            Vector3 translation = Vector3.down * delta;
            
            _top.Translate(translation);
            _bottom.Translate(translation);
        }

        private void LateUpdate()
        {
            if (_top.position.y <= _resetPosition.y)
                _top.position = _topPosition;

            if (_bottom.position.y <= _resetPosition.y)
                _bottom.position = _topPosition;
        }
    }
}
