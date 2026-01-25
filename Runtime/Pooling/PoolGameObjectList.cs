using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "PoolGameObjectList", menuName = "OpenUtility/Pooling/GameObject List", order = 1)]
    public class PoolGameObjectList : ScriptableList<PoolGameObject>
    {
    }
}
