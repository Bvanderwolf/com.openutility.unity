using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public class ScriptableDoubleEvent : ScriptableVariableEvent<double>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableDouble _variable;
        
        protected override UnityEvent<double> GetChangedEvent() => _variable.ValueChanged;
        protected override double GetValue() => _variable.GetValue();
    }
}
