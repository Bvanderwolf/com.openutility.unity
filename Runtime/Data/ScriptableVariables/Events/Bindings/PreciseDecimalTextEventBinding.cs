using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data
{
    public abstract class PreciseDecimalTextEventBinding : ScriptableVariableEvent<string>
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableDouble _variable;

        private readonly UnityEvent<string> _changedEvent = new UnityEvent<string>();

        protected override void OnEnable()
        {
            base.OnEnable();
            AddListener();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RemoveListener();
        }

        protected abstract string ConvertDecimalToText(double newValue);

        protected override UnityEvent<string> GetChangedEvent() => _changedEvent;

        protected override string GetValue()
        {
            if (_variable == null)
                return (null);
            
            double value = _variable.GetValue();
            return (ConvertDecimalToText(value));
        }

        private void AddListener()
        {
            if (_variable == null)
                return;
            
            _variable.ValueChanged.AddListener(OnValueChanged);
        }

        private void RemoveListener()
        {
            if (_variable == null)
                return;
            
            _variable.ValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(double newValue)
        {
            string converted = ConvertDecimalToText(newValue);
            _changedEvent?.Invoke(converted);
        }
    }
}
