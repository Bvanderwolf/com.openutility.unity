using System;
using OpenUtility.DelayedExecution;
using Random = UnityEngine.Random;

namespace OpenUtility.Samples.Data
{
    public class AuthenticationService : MockAppService
    {
        public bool IsAuthenticated { get; private set; }
        public UserInfo CurrentUser { get; private set; }

        public event Action<UserInfo> Authenticated;
        public event Action AuthenticationFailed;

        public void Authenticate()
        {
            float time = Random.Range(0.75f, 1.5f);
            Execute.AfterSeconds(CompleteAuthentication, time);
        }

        private void CompleteAuthentication()
        {
            bool success = Random.Range(0f, 1f) > 0.25f;
            if (success)
            {
                UserInfo user = new UserInfo
                {
                    username = "MockUser",
                    email = "mockuser@gmail.com"
                };

                IsAuthenticated = true;
                CurrentUser = user;
                Authenticated?.Invoke(user);
            }
            else
            {
                IsAuthenticated = false;
                AuthenticationFailed?.Invoke();
            }
        }
    }
}
