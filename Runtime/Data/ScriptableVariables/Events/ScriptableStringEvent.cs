using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableStringEvent : ScriptableVariableEvent<string>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableString _variable;

        protected override UnityEvent<string> GetChangedEvent()
        {
            return (_variable != null ? _variable.ValueChanged : null);
        }

        protected override string GetValue()
        {
            return (_variable != null ? _variable.GetValue() : null);
        }
    }
}
