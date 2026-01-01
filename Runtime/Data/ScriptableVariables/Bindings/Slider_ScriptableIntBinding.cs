using OpenUtility.Data;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility
{
    [ScriptableVariableBinder(typeof(Slider), typeof(int), DisplayName = "Default Int Binding")]
    public class Slider_ScriptableIntBinding : MonoBehaviour
    {
        [SerializeField]
        private ScriptableInt _variable;

        public void SetValue(float newValue)
        {
            var casted = (int)newValue;
            
            _variable.SetValue(casted);
        }
    }
}
