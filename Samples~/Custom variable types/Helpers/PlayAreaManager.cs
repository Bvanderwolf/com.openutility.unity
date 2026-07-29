using System;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class PlayAreaManager : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField]
        private PlayAreaTheme _theme;
        
        [SerializeField]
        private RectTransform _playingArea;
        
        private void OnEnable()
        {
            _theme.EnumValueChanged.AddListener(OnThemeChanged);
        }

        private void OnDisable()
        {
            _theme.EnumValueChanged.RemoveListener(OnThemeChanged);
        }
        
        private void OnThemeChanged(Theme newTheme)
        {
            Image background = _playingArea.GetComponent<Image>();
            switch (newTheme)
            {
                case Theme.White:
                    background.color = Color.white;
                    break;
                
                case Theme.Dark:
                    background.color = new Color(95f / 255f, 95f / 255f, 95f / 255f);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(newTheme), newTheme, null);
            }
        }
    }
}