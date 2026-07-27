using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableBoolEvent : ScriptableVariableEvent<bool>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableBool _variable;

        protected override UnityEvent<bool> GetChangedEvent()
        {
            return (_variable != null) ? _variable.ValueChanged : null;
        }

        protected override bool GetValue()
        {
            return (_variable != null) ? _variable.GetValue() : false;
        }
    }
}
