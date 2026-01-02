using OpenUtility.Exceptions;
using TMPro;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(float), DisplayName = "Default Float Binding")]
    public class DefaultInputFieldFloatBinding : InputFieldFloatBinding
    {
        public override void SetValue(string newValue)
        {
            ThrowIf.NotFloat(newValue, out float result);
            
            variable.SetValue(result);
        }
    }
}
