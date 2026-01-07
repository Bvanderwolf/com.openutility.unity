using OpenUtility.Data;
using UnityEngine.UI;

namespace OpenUtility
{
    [ScriptableVariableBinder(typeof(Slider), typeof(int), DisplayName = "Default Slider Int Binding")]
    public class DefaultSliderIntBinding : SliderIntBinding
    {
        public override void SetValue(float newValue)
        {
            var casted = (int)newValue;
            
            variable.SetValue(casted);
        }
    }
}
