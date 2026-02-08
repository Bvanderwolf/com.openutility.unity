using System.Collections;
using OpenUtility.Data;
using TMPro;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class ScreenService : MockAppService
    {
        [Header("Scene References")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TMP_Text _headerText;

        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _authentication;

        public bool IsMainMenuOpen { get; private set; }

        public void OpenMainMenu()
        {
            SetHeaderText();
            StartCoroutine(Fade(true));
        }

        public void CloseMainMenu()
        {
            StartCoroutine(Fade(false));
        }
        
        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        private void SetHeaderText()
        {
            var service = _authentication.GetComponent<AuthenticationService>();
            if (service.IsAuthenticated)
            {
                _headerText.text = $"Welcome, {service.CurrentUser.username}";
            }
            else
            {
                _headerText.text = "Welcome";
            }
        }

        private IEnumerator Fade(bool enable)
        {
            IsMainMenuOpen = enable;
            
            float duration = 0.75f;
            float current = 0f;
            
            while (current < duration)
            {
                yield return null;

                current += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(enable ? 0f : 1f, enable ? 1f : 0f, current / duration);
            }

            _canvasGroup.blocksRaycasts = enable;
        }
    }
}
