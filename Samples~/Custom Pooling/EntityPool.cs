using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Pooling.Custom
{
    [CreateAssetMenu(fileName = "EntityPool", menuName = "Scriptable Objects/Pools/EntityPool")]
    public class EntityPool : ScriptablePoolBase<EntityBehaviour>
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private EntitySettings _settings;
    
        protected override EntityBehaviour OnCreateInstance() => Instantiate(_prefab).GetComponent<EntityBehaviour>();

        protected override void OnGetInstance(EntityBehaviour instance)
        {
            instance.gameObject.SetActive(true);
            instance.Initialize(_settings);
        }

        protected override void OnReleaseInstance(EntityBehaviour instance)
        {
            instance.DeInitialize();
            instance.gameObject.SetActive(false);
        }
    }
}
