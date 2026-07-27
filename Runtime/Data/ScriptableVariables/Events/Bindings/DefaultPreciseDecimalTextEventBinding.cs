using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(double), BindingGoal.DetermineValue, DisplayName = "Default Precise Decimal")]
    public class DefaultPreciseDecimalTextEventBinding : PreciseDecimalTextEventBinding
    {
        [Header("Optional")]
        [SerializeField]
        private Optional<string> _doubleFormat;
        
        [SerializeField]
        private Optional<string> _stringFormat;
        
        protected override string ConvertDecimalToText(double newValue)
        {
            string text = _doubleFormat.HasValue
                ? newValue.ToString(_doubleFormat.Value)
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
