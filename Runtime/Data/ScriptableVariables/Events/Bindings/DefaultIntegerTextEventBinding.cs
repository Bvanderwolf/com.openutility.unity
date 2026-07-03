using TMPro;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(int), BindingGoal.DetermineValue, DisplayName = "Default Integer")]
    public class DefaultIntegerTextEventBinding : IntegerTextEventBinding
    {
        protected override string ConvertIntegerToText(int newValue) => newValue.ToString();
    }
}
