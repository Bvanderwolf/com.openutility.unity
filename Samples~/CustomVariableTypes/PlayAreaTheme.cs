using OpenUtility.Data;
using TMPro;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public enum Theme
    {
        White,

        Dark
    }

    [ScriptableVariableBinder(typeof(TMP_Dropdown), typeof(int), typeof(PlayAreaTheme), BindingGoal.ReceiveValue)]
    [CreateAssetMenu(fileName = "PlayAreaTheme", menuName = "Scriptable Objects/PlayAreaTheme")]
    public class PlayAreaTheme : ScriptableEnum<Theme>
    {
    }
}
