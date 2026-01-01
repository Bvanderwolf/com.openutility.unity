using OpenUtility.Exceptions;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(float), DisplayName = "Default Float Binding")]
    public class TMP_InputField_ScriptableFloatBinding : MonoBehaviour
    {
        [SerializeField]
        private ScriptableFloat _variable;

        public void SetValue(string newValue)
        {
            ThrowIf.NotFloat(newValue, out float result);
            
            _variable.SetValue(result);
        }
        
        public void SetValueWithoutNotify(string newValue)
        {
            ThrowIf.NotFloat(newValue, out float result);
            
            _variable.SetValueWithoutNotify(result);
        }
    }
}
