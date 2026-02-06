using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "SpriteList", menuName = "OpenUtility/Samples/SpriteList")]
    public class SpriteList : ScriptableList<Sprite>
    {
        public Sprite GetRandom()
        {
            if (value.Count == 0)
                return (null);

            int index = Random.Range(0, value.Count);
            return (GetValue(index));
        }
    }
}
