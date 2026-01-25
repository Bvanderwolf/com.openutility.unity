using OpenUtility.Data;
using TMPro;

namespace OpenUtility.Samples.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(int), typeof(PlayAreaTheme), BindingGoal.DetermineValue, DisplayName = "Play Area Theme Text")]
    public class PlayAreaThemeTextEventBinding : EnumTextEventBinding<Theme> 
    {
    }
}
