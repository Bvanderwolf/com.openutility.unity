using OpenUtility.Data;
using TMPro;

namespace OpenUtility.Samples.Data
{
    public enum EnvironementSetting
    {
        Development,
        Staging,
        Production
    }
    
    [ScriptableVariableBinder(typeof(TMP_Dropdown), typeof(int), DisplayName = "Environment Setting")]
    public class EnvironmentSettingEnum : ScriptableEnum<EnvironementSetting>
    {
    }
}
