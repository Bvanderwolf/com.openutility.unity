using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility
{
    [CreateAssetMenu(fileName = "GameObjectPool", menuName = "OpenUtility/Pooling/GameObject Pool", order = 1)]
    public sealed class ScriptablePool : ScriptablePoolBase<PoolGameObject>
    {
        [SerializeField]
        private GameObject _prefab;

        protected override PoolGameObject OnCreateInstance()
        {
            GameObject gameObject = Instantiate(_prefab);
            if (!gameObject.TryGetComponent<PoolGameObject>(out var instance))
            {
                Debug.Log($"[{name}] Could not find the {nameof(PoolGameObject)} component on prefab '{_prefab.name}'. It is best practice to add your pooling component beforehand to set serialized fields. Adding it manually now...");

                instance = gameObject.AddComponent<PoolGameObject>();
            }

            return (instance);
        }

        protected override void OnGetInstance(PoolGameObject instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected override void OnReleaseInstance(PoolGameObject instance)
        {
            instance.gameObject.SetActive(false);
        }
    }
}
