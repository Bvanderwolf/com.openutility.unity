using Newtonsoft.Json.Linq;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenUtility.Samples.Data
{
    public class EntitySettingsManager : MonoBehaviour
    {
        [SerializeField]
        private ScriptableEntityData _entityData;

        [SerializeField]
        private EntityDisplaySettings _settings;

        public void SetRandomEntityData()
        {
            int min = 0;
            int max = 3;
            string url = $"https://csrng.net/csrng/csrng.php?min={min}&max={max}";
            WaitFor.GetRequest(url, OnRandomNumberRequestCompleted);
        }

        private void OnRandomNumberRequestCompleted(UnityWebRequest request)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching random number: {request.error}");
                return;
            }

            JArray array = JArray.Parse(request.downloadHandler.text);
            JObject json = (JObject)array[0];
            string status = json["status"]?.ToString();
            if (status != "success")
            {
                string reason = json["reason"]?.ToString() ?? "Unknown error";
                Debug.LogError($"Random number generator error: {reason}");
                return;
            }

            string randomString = json["random"].ToString();
            int randomId = int.Parse(randomString);

            EntityDisplayData displayData = _settings.GetValue(randomId);
            EntityData entityData = new EntityData
            {
                id = randomId,
                name = displayData.text,
            };

            _entityData.SetValue(entityData);
        }
    }

}