using System;
using OpenUtility.DelayedExecution;
using Random = UnityEngine.Random;

namespace OpenUtility.Samples.Data
{
    public class LicenseService : MockAppService
    {
        public bool IsLicensed { get; private set; }
        public LicenseInfo CurrentLicense { get; private set; }

        public event Action<LicenseInfo> CheckSuccess;
        public event Action CheckFailed;

        public void CheckLicenseForUser(UserInfo user)
        {
            float time = Random.Range(0.75f, 1.5f);
            Execute.AfterSeconds(CompleteLicenseCheck, time);
        }

        private void CompleteLicenseCheck()
        {
            bool success = Random.Range(0f, 1f) > 0.25f;
            if (success)
            {
                LicenseInfo license = new LicenseInfo
                {
                    id = Guid.NewGuid().ToString(),
                    orderStoreID = "MOCK-ORDER-12345",
                    validityPeriod = DateTime.UtcNow.AddYears(1).ToString("g")
                };

                IsLicensed = true;
                CurrentLicense = license;
                CheckSuccess?.Invoke(license);
            }
            else
            {
                IsLicensed = false;
                CheckFailed?.Invoke();
            }
        }
    }
}
