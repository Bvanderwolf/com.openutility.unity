using OpenUtility.Data;
using TMPro;

namespace OpenUtility.Samples.Data
{
    [ScriptableVariableBinder(typeof(TMP_Text), typeof(int), typeof(EnvironmentSettingEnum), BindingGoal.DetermineValue, DisplayName = "Environment Setting")]
    public class EnvironmentSettingTextBinding : EnumTextEventBinding<EnvironementSetting>
    {
    }
}