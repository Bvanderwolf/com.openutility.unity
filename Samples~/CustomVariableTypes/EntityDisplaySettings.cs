using System;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Samples.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [Serializable]
    public struct EntityDisplayData
    {
        public Sprite sprite;
        public string text;
    }

    [CreateAssetMenu(fileName = "EntityDisplaySettings", menuName = "Scriptable Objects/EntityDisplaySettings")]
    public class EntityDisplaySettings : ScriptableDictionary<int, EntityDisplayData>
    {
        protected override IDictionary<int, EntityDisplayData> CreateValue(int capacity)
        {
            return (new Dictionary<int, EntityDisplayData>(capacity));
        }
    }
}
