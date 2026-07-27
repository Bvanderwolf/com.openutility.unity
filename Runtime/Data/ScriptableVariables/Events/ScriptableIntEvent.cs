using OpenUtility.Data;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility
{
    public class ScriptableIntEvent : ScriptableVariableEvent<int>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableInt _variable;

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
