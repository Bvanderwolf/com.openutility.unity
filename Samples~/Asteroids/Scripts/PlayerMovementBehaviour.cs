using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class PlayerMovementBehaviour : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField, Min(50.0f)]
        private float _speed = 100.0f;

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                Vector3 translation = Vector3.left * (Time.deltaTime * _speed);
                transform.Translate(translation);
            }
            
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                Vector3 translation = Vector3.right * (Time.deltaTime * _speed);
                transform.Translate(translation);
            }
            
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                Vector3 translation = Vector3.down * (Time.deltaTime * _speed);
                transform.Translate(translation);
            }
            
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                Vector3 translation = Vector3.up * (Time.deltaTime * _speed);
                transform.Translate(translation);
            }
        }
    }
}
