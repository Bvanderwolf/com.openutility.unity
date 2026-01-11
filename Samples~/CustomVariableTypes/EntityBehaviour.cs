using System;
using OpenUtility.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class EntityBehaviour : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField]
        private EntityDataReference _data;

        [SerializeField]
        private EntityDisplaySettings _settings;

        [Header("UI Elements")]
        [SerializeField]
        private Image _imageRenderer;

        [SerializeField]
        private TMP_Text _nameRenderer;
        
        [SerializeField]
        private RectTransform _playingArea;

        private void OnEnable()
        {
            if (_data.ValueSource == VariableValueSource.Shared)
                ((ScriptableEntityData)_data.SharedVariable).ValueChanged += OnDataChanged;
        }

        private void OnDisable()
        {
            if (_data.ValueSource == VariableValueSource.Shared)
                ((ScriptableEntityData)_data.SharedVariable).ValueChanged -= OnDataChanged;
        }
        
        private void Update()
        {
            MoveEntity();
        }
        
        private void MoveEntity()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            if (!(moveX > 0.5f || moveX < -0.5f || moveZ > 0.5f || moveZ < -0.5f))
                return;

            Vector3 move = new Vector3(moveX, moveZ, 0).normalized;
            float currentSpeed = 500f;

            if (Input.GetKey(KeyCode.LeftShift))
                currentSpeed *= 2; // sprinting doubles the speed

            if (!IsFullyContainedBy((RectTransform)transform, _playingArea))
                move *= -2f; // reverse direction if outside playing area

            transform.Translate(move * currentSpeed * Time.deltaTime, Space.Self);
        }

        private void OnDataChanged(EntityData newValue)
        {
            EntityDisplayData displayData = _settings.GetValue(newValue.id);
            _imageRenderer.sprite = displayData.sprite;
            _nameRenderer.text = newValue.name;
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
