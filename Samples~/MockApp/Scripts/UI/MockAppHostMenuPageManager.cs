using System.Collections;
using OpenUtility.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenUtility.Samples.Data
{
    public class MockAppHostMenuPageManager : MenuPageManager<MockAppHostMenuPage>
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _screen;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            StartCoroutine(OnEscapeButtonPressedRoutine());
        }

        private IEnumerator OnEscapeButtonPressedRoutine()
        {
            if (!_screen.TryGetComponent(out ScreenService service))
                yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);

            yield return null;
            
            service ??= _screen.GetComponent<ScreenService>();

            if (service.IsMainMenuOpen)
                service.CloseMainMenu();
            else
                service.OpenMainMenu();
        }
    }
}