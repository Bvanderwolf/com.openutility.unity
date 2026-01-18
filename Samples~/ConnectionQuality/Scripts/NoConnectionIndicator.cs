using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class NoConnectionIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private ScriptableConnectionQuality _connection;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Sprite _sprite;

        private void OnEnable()
        {
            _connection.ValueChanged.AddListener(OnConnectionQualityChanged);
        }

        private void OnDisable()
        {
            _connection.ValueChanged.RemoveListener(OnConnectionQualityChanged);
        }

        private void OnConnectionQualityChanged(int newElapsedMs)
        {
            if (newElapsedMs != -1)
                return;

            _image.sprite = _sprite;
        }
    }
}
