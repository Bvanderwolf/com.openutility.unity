using System;
using OpenUtility.Data;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    [Serializable]
    public class EntityDataReference : ScriptableVariableReference<EntityData>
    {
        [SerializeField]
        private ScriptableEntityData _value;
        
        protected override ScriptableVariable<EntityData> GetScriptableVariable()
        {
            return (_value);
        }
    }
}