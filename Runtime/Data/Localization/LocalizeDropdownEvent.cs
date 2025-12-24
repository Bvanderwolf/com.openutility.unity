using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenUtility.DelayedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OpenUtility.Data.Localization
{
    public class LocalizeDropdownEvent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TMP_Dropdown _dropdown;

        [SerializeField]
        private LocalizedString[] _options;

        [Header("Settings")]
        [SerializeField, Tooltip("If true, will convert the localized string to Title Case.")]
        private bool _useTitleCasing = false;

        public LocalizedString[] Options => _options;

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            
            LocalizeDropdown();
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        private void OnSelectedLocaleChanged(Locale locale)
        {
            if (gameObject.activeInHierarchy)
                LocalizeDropdown();
        }

        private void LocalizeDropdown()
        {
            if (_options.Length == 0)
                return;

            AsyncOperationHandle<string>[] operations = _options
                .Select(option => option.GetLocalizedStringAsync())
                .ToArray();

            WaitFor.Operations(operations, OnOptionsLocalized);
        }

        private void OnOptionsLocalized(DataRequestResult<string[]> result)
        {
            if (!result.success)
            {
                Debug.LogError(result.error);
                return;
            }

            var options = new List<string>(result.data);
            if (_useTitleCasing)
            {
                for (int i = 0; i < result.data.Length; i++)
                    options[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result.data[i].ToLower());
            }
            
            int dropdownValue = _dropdown.value;
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
            _dropdown.SetValueWithoutNotify(dropdownValue);
        }
    }
}