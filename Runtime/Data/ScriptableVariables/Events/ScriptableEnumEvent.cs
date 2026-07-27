using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableEnumEvent : ScriptableVariableEvent<int>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableEnum _variable;

        protected override UnityEvent<int> GetChangedEvent()
        {
            return (_variable != null ? _variable.ValueChanged : null);
        }

        protected override int GetValue()
        {
            return (_variable != null ? _variable.GetValue() : 0);
        }
    }
}
