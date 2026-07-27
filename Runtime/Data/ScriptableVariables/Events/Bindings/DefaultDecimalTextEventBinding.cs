using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(float), BindingGoal.DetermineValue, DisplayName = "Default Decimal")]
    public class DefaultDecimalTextEventBinding : DecimalTextEventBinding
    {
        [Header("Optional")]
        [SerializeField]
        private Optional<string> _floatFormat;
        
        [SerializeField]
        private Optional<string> _stringFormat;
        
        protected override string ConvertDecimalToText(float newValue)
        {
            string text = _floatFormat.HasValue
                ? newValue.ToString(_floatFormat.Value)
                : newValue.ToString(CultureInfo.InvariantCulture);

            try
            {
                return (_stringFormat.TryGetValue(out string format) ? string.Format(format, text) : text);
            }
            catch (Exception)
            {
                return (text);
            }
        }
    }
}
