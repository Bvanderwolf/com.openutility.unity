using OpenUtility.Data.Pooling;
using OpenUtility.Logging;
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
            var instance = Instantiate(_prefab).GetComponent<PoolGameObject>();
            
            WarnIf.SystemObjectNull(instance, $"Failed to create instance of PoolGameObject from prefab '{_prefab.name}'. Make sure the prefab has a PoolGameObject component attached to it.");

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
