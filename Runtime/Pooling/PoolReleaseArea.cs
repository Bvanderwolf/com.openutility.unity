using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    public class PoolReleaseArea : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField, Tooltip("The pool to release the game object to.")]
        private ScriptablePool _pool;

        private readonly List<Component> _lookup = new(10);

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponentsInParent(false, _lookup);

            foreach (Component component in _lookup)
            {
                if (component is not PoolGameObject poolComponent)
                    continue;

                _pool.Release(poolComponent);
                break;
            }
            
            _lookup.Clear();
        }

        private void OnCollisionEnter(Collision collision)
        {
            collision.body.GetComponentsInParent(false, _lookup);

            foreach (Component component in _lookup)
            {
                if (component is not PoolGameObject poolComponent)
                    continue;

                _pool.Release(poolComponent);
                break;
            }
            
            _lookup.Clear();
        }
    }
}
