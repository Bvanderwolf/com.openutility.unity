using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "PoolGameObjectList", menuName = "OpenUtility/Pooling/GameObject List", order = 1)]
    public class PoolGameObjectList : ScriptableList<PoolGameObject>
    {
        /// <summary>
        /// Releases all instances in this list.
        /// </summary>
        public void Release()
        {
            if (value.Count == 0)
                return;

            for (int i = value.Count - 1; i >= 0; i--)
                value[i].Release();
        }
    }
}
