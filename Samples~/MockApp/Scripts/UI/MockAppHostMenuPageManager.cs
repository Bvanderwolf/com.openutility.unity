using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public class MockAppHostMenuPageManager : MenuPageManager<MockAppHostMenuPage>
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _screen;

        private void Update()
        {
            ScreenService service = _screen.GetComponent<ScreenService>();
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            if (service.IsMainMenuOpen)
                service.CloseMainMenu();
            else
                service.OpenMainMenu();
        }
    }
}