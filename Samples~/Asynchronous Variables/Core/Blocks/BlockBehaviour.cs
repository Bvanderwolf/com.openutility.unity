using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Samples.Data
{
    public class BlockBehaviour : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent _collided;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _collided?.Invoke();
        }
    }
}
