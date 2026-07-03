using OpenUtility.Exceptions;
using TMPro;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(double), BindingGoal.ReceiveValue, DisplayName = "Default Precise Decimal")]
    public class DefaultPreciseDecimalTextBinding : PreciseDecimalTextBinding
    {
        public override void SetValue(string newValue)
        {
            ThrowIf.NotDouble(newValue, out double result);
            
            variable.SetValue(result);
        }
    }
}
