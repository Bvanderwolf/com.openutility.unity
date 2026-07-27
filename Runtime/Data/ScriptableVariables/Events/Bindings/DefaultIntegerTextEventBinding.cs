using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(int), BindingGoal.DetermineValue, DisplayName = "Default Integer")]
    public class DefaultIntegerTextEventBinding : IntegerTextEventBinding
    {
        [Header("Optional")]
        [SerializeField]
        private Optional<string> _intFormat;

        [SerializeField]
        private Optional<string> _stringFormat;

        protected override string ConvertIntegerToText(int newValue)
        {
            string text = _intFormat.HasValue
                ? newValue.ToString(_intFormat.Value)
                : newValue.ToString(CultureInfo.InvariantCulture);
            
            try
            {
                return (_stringFormat.HasValue ? string.Format(_stringFormat.Value, text) : text);
            }
            catch (Exception)
            {
                return (text);
            }
        }
    }
}
