using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Samples.Data
{
    public class LoginMenuManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _authentication;

        [Header("Events")]
        [SerializeField]
        private UnityEvent _authenticationSuccess;

        [SerializeField]
        private UnityEvent _authenticationFailure;

        private void OnEnable()
        {
            var service = _authentication.GetComponent<AuthenticationService>();
            service.Authenticated += OnAuthenticated;
            service.AuthenticationFailed += OnAuthenticationFailed;
        }

        private void OnDisable()
        {
            if (!_authentication.TryGetComponent(out AuthenticationService service))
                return;
            
            service.Authenticated -= OnAuthenticated;
            service.AuthenticationFailed -= OnAuthenticationFailed;
        }

        public void Login()
        {
            Debug.Log("Logging in...");

            var service = _authentication.GetComponent<AuthenticationService>();
            Execute.AfterSeconds(service.Authenticate, 0.75f);
        }

        private void OnAuthenticated(UserInfo user)
        {
            Debug.Log("Authentication sucessfull");

            _authenticationSuccess?.Invoke();
        }

        private void OnAuthenticationFailed()
        {
            Debug.Log("Authentication Failed");

            _authenticationFailure?.Invoke();

        }
    }
}
