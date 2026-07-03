using System.Globalization;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(double), BindingGoal.DetermineValue, DisplayName = "Default Precise Decimal")]
    public class DefaultPreciseDecimalTextEventBinding : PreciseDecimalTextEventBinding
    {
        [SerializeField]
        private Optional<string> _format;
        
        protected override string ConvertDecimalToText(double newValue)
        {
            return (_format.HasValue ? newValue.ToString(_format.Value) : newValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}
