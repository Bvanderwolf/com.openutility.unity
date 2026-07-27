using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableFloatEvent : ScriptableVariableEvent<float>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableFloat _variable;

        protected override UnityEvent<float> GetChangedEvent()
        {
            return (_variable != null ? _variable.ValueChanged : null);
        }

        protected override float GetValue()
        {
            return (_variable != null ? _variable.GetValue() : 0.0f);
        }
    }
}
