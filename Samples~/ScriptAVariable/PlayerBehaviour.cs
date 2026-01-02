using OpenUtility.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private ScriptableString _name;

        [SerializeField]
        private ScriptableInt _health;

        [SerializeField]
        private ScriptableFloat _speed;

        [SerializeField]
        private ScriptableBool _shiftToSprint;

        [Header("Theme")]
        [SerializeField]
        private ScriptableFloat _alpha;

        [SerializeField]
        private ScriptableInt _shadow;

        [Header("UI Elements")]
        [SerializeField]
        private TMP_Text _nameRenderer;

        [SerializeField]
        private RectTransform _playingArea;
        
        [SerializeField]
        private Image _backgroundImage;

        [SerializeField]
        private Image _playerImage;

        private int _startingHealth;

        private void Awake()
        {
            _startingHealth = _health.GetValue();
            OnNameValueChanged(_name.GetValue());
        }

        private void OnEnable()
        {
            _name.ValueChanged.AddListener(OnNameValueChanged);
            _health.ValueChanged.AddListener(OnHealthValueChanged);
            _alpha.ValueChanged.AddListener(OnAlphaValueChanged);
            _shadow.ValueChanged.AddListener(OnShadowValueChanged);
        }

        private void OnDisable()
        {
            _name.ValueChanged.RemoveListener(OnNameValueChanged);
            _health.ValueChanged.RemoveListener(OnHealthValueChanged);
            _alpha.ValueChanged.RemoveListener(OnAlphaValueChanged);
            _shadow.ValueChanged.RemoveListener(OnShadowValueChanged);
        }

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            if (!(moveX > 0.5f || moveX < -0.5f || moveZ > 0.5f || moveZ < -0.5f))
                return;

            Vector3 move = new Vector3(moveX, moveZ, 0).normalized;
            float currentSpeed = _speed.GetValue();

            if (_shiftToSprint.GetValue() && Input.GetKey(KeyCode.LeftShift))
                currentSpeed *= 2; // sprinting doubles the speed

            if (!IsFullyContainedBy((RectTransform)transform, _playingArea))
                move *= -2f; // reverse direction if outside playing area

            transform.Translate(move * currentSpeed * Time.deltaTime, Space.Self);
        }

        private void OnNameValueChanged(string newValue)
        {
            int health = _health.GetValue();
            _nameRenderer.text = $"[{newValue}][{health}/{_startingHealth}]";
        }

        private void OnHealthValueChanged(int newValue)
        {
            string currentName = _name.GetValue();
            _nameRenderer.text = $"[{currentName}][{newValue}/{_startingHealth}]";
        }
        
        private void OnShadowValueChanged(int newValue)
        {
            Color color = _backgroundImage.color;
            color.r = (255 - newValue) / 255f;
            color.g = (255 - newValue) / 255f;
            color.b = (255 - newValue) / 255f;
            
            _backgroundImage.color = color;
        }

        private void OnAlphaValueChanged(float newValue)
        {
            Color color = _playerImage.color;
            color.a = newValue;
            
            _playerImage.color = color;
        }

        private static bool IsFullyContainedBy(RectTransform rect1, RectTransform containerRect)
        {
            Rect r1 = GetWorldRect(rect1);
            Rect r2 = GetWorldRect(containerRect);

            return r1.xMin >= r2.xMin && r1.xMax <= r2.xMax &&
                   r1.yMin >= r2.yMin && r1.yMax <= r2.yMax;
        }

        private static Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            float width = corners[2].x - corners[0].x;
            float height = corners[2].y - corners[0].y;
            return new Rect(corners[0].x, corners[0].y, width, height);
        }
    }
}
