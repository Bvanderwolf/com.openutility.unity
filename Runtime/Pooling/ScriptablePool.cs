using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility
{
    [CreateAssetMenu(fileName = "GameObjectPool", menuName = "OpenUtility/Pooling/GameObject Pool", order = 1)]
    public sealed class ScriptablePool : ScriptablePoolBase<PoolGameObject>
    {
        [SerializeField]
        private GameObject _prefab;
        
        protected override PoolGameObject OnCreateInstance() => Instantiate(_prefab).GetComponent<PoolGameObject>();

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
