using OpenUtility.Exceptions;
using TMPro;
using UnityEngine;

namespace OpenUtility.Data
{
    [ScriptableVariableBinder(typeof(TMP_InputField), typeof(int), DisplayName = "Default Int Binding")]
    public class TMP_InputField_ScriptableIntBinding : MonoBehaviour
    {
        [SerializeField]
        private ScriptableInt _variable;

        public void SetValue(string newValue)
        {
            ThrowIf.NotInt(newValue, out int result);
            
            _variable.SetValue(result);
        }
    }
}
