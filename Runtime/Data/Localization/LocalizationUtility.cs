using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenUtility.DelayedExecution;
using TMPro;
using UnityEngine.Localization;

namespace OpenUtility.Data.Localization
{
    public static class LocalizationUtility
    {
        public static void AddLocalizedOptions(this TMP_Dropdown dropdown, int? indexToSelect, params LocalizedString[] strings)
        {
            var operations = strings.Select(s => s.GetLocalizedStringAsync()).ToArray();

            WaitFor.Operations(operations, result =>
            {
                var options = new List<string>(result.data);
                dropdown.AddOptions(options);

                if (indexToSelect.HasValue)
                    dropdown.SetValueWithoutNotify(indexToSelect.Value);
            });
        }
        
        public static IEnumerator AppendLocalizedString(this StringBuilder stringBuilder, LocalizedString localizedString)
        {
            yield return WaitFor.Localization(localizedString, localizedText =>
            {
                stringBuilder.Append(localizedText);
            });
        }

        public static IEnumerator AppendLocalizedString(this TMP_Text textElement, LocalizedString localizedString)
        {
            yield return WaitFor.Localization(localizedString, localizedText =>
            {
                textElement.text += localizedText;
            });
        }

        public static void SetLocalizedString(this TMP_Text textElement, LocalizedString localizedString) =>  WaitFor.Localization(localizedString, textElement.SetText);

        public static void AddLocalizedOption(this TMP_Dropdown dropdown, LocalizedString localizedString) => WaitFor.Localization(localizedString, localizedText => 
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
            dropdown.RefreshShownValue();
        });
    }
}