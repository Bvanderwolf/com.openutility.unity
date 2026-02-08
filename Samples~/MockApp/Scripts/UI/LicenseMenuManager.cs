using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Samples.Data
{
    public class LicenseMenuManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private GameObjectGroup _services;

        [Header("Events")]
        [SerializeField]
        private UnityEvent _licenseCheckSuccess;

        [SerializeField]
        private UnityEvent _licenseCheckFailure;

        private void OnEnable()
        {
            var licensing = _services.GetComponent<LicenseService>();
            licensing.CheckSuccess += OnLicenseCheckSuccess;
            licensing.CheckFailed += OnLicenseCheckFailed;
        }

        private void OnDisable()
        {
            if (!_services.TryGetComponent(out LicenseService licensing))
                return;
            
            licensing.CheckSuccess -= OnLicenseCheckSuccess;
            licensing.CheckFailed -= OnLicenseCheckFailed;
        }

        public void CheckLicense()
        {
            Debug.Log("Checking license...");

            Execute.AfterSeconds(RunLicenseCheck, 0.75f);

            void RunLicenseCheck()
            {
                var authentication = _services.GetComponent<AuthenticationService>();
                var licensing = _services.GetComponent<LicenseService>();
                UserInfo user = authentication.CurrentUser;
                licensing.CheckLicenseForUser(user);
            }
        }

        private void OnLicenseCheckSuccess(LicenseInfo license)
        {
            Debug.Log("License check sucessfull");

            _licenseCheckSuccess?.Invoke();
        }

        private void OnLicenseCheckFailed()
        {
            Debug.Log("License check Failed");

            _licenseCheckFailure?.Invoke();
        }
    }
}