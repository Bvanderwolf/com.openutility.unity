using System;
using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "ScriptableEntityData", menuName = "Scriptable Objects/ScriptableEntityData")]
    public class ScriptableEntityData : ScriptableVariable<EntityData> 
    {
        [SerializeField]
        private EntityData _data;
        
        public event Action<EntityData> ValueChanged; 

        public override EntityData GetValue()
        {
            return (_data);
        }

        public override void SetValue(EntityData newValue)
        {
            _data = newValue;
            ValueChanged?.Invoke(newValue);
        }
    }
}
