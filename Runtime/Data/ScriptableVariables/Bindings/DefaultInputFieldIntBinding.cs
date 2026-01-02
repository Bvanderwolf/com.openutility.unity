using OpenUtility.Exceptions;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(int), DisplayName = "Default Int Binding")]
    public class DefaultInputFieldIntBinding : InputFieldIntBinding
    {
        public override void SetValue(string newValue)
        {
            ThrowIf.NotInt(newValue, out int result);
            
            variable.SetValue(result);
        }
    }
}
