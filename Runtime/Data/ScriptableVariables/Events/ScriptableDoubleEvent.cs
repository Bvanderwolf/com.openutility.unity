using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableDoubleEvent : ScriptableVariableEvent<double>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableDouble _variable;

        protected override UnityEvent<double> GetChangedEvent()
        {
            return (_variable != null ? _variable.ValueChanged : null);
        }

        protected override double GetValue()
        {
            return (_variable != null ? _variable.GetValue() : 0.0d);
        }
    }
}
