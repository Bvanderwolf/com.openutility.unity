using OpenUtility.Data;
using TMPro;

namespace OpenUtility.Samples.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(string), DisplayName = "Prefixed String Variable")]
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(string), DisplayName = "Prefixed String Variable")]
    public class PrefixedString : ScriptableString
    {
        public string prefix;
        
        public override void SetValue(string newValue)
        {
            base.SetValue($"{prefix} {newValue}");
        }
    }
}
