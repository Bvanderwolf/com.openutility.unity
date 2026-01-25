using System.Collections.Generic;
using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
   
    public class ConnectionQualitySpriteList : ScriptableDictionary<ConnectionQuality, Sprite>
    {
        protected override IDictionary<ConnectionQuality, Sprite> CreateValue(int capacity)
        {
            return new Dictionary<ConnectionQuality, Sprite>(capacity);
        }
    }
}
