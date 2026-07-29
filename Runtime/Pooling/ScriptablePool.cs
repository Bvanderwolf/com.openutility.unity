using System;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "GameObjectPool", menuName = "OpenUtility/Pooling/GameObject Pool", order = 1)]
    public sealed class ScriptablePool : ScriptablePoolBase<PoolGameObject>
    {
        [Header("Project References")]
        [SerializeField]
        private GameObject _prefab;

        [SerializeField, Tooltip("An optional list variable for storing references to active instances.")]
        private Optional<PoolGameObjectList> _references;

        public PoolGameObjectList References => _references.GetValueOrDefault();

        public event Action<PoolGameObject> InstanceRetrieved;

        protected override PoolGameObject OnCreateInstance()
        {
            GameObject gameObject;

            if (parameters.HasValue)
            {
                gameObject = Instantiate(_prefab, parameters.Value);
            }
            else if (parent.HasValue)
            {
                gameObject = Instantiate(_prefab, parent.Value);
            }
            else if (position.HasValue && rotation.HasValue)
            {
                gameObject = Instantiate(_prefab, position.Value, rotation.Value);
            }
            else
            {
                gameObject = Instantiate(_prefab);
            }
            
            if (!gameObject.TryGetComponent(out PoolGameObject instance))
            {
                Debug.Log($"[{name}] Could not find the {nameof(PoolGameObject)} component on prefab '{_prefab.name}'. It is best practice to add your pooling component beforehand to set serialized fields. Adding it manually now...");

                instance = gameObject.AddComponent<PoolGameObject>();
            }

            return (instance);
        }

        protected override void OnGetInstance(PoolGameObject instance)
        {
            instance.gameObject.SetActive(true);
            
            if (_references.HasValue)
                _references.Value.Add(instance);
            
            InstanceRetrieved?.Invoke(instance);
        }

        protected override void OnReleaseInstance(PoolGameObject instance)
        {
            instance.gameObject.SetActive(false);
            
            if (position.HasValue && rotation.HasValue)
                instance.transform.SetPositionAndRotation(position.Value, rotation.Value);
            
            if (_references.HasValue)
                _references.Value.Remove(instance);
        }
    }
}
