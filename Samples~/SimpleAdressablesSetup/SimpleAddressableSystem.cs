using OpenUtility.Data;
using OpenUtility.Data.Addressable;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace OpenUtility.Samples.Addressables
{
    public class SimpleAddressableSystem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string _catalogPath;

        [Header("UI Elements")]
        [SerializeField]
        private Slider _loadStatusSlider;

        [SerializeField]
        private TMP_Text _feedback;

        [SerializeField]
        private TMP_Text _status;
        
        public void LoadContentCatalog()
        {
            bool started = AddressableContentManager.DownloadContentCatalog(_catalogPath);
            if (started)
            {
                UpdateStatus("Loading catalog...");
            }
            else
            {
                UpdateFeedback("Could not start catalog load. Check logs for details.");
            }
            
            AddressableContentManager.EnableSasTokenUsage("?sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupitfx&se=2050-07-24T19:39:00Z&st=2023-07-23T22:00:00Z&spr=https&sig=Rbm%2BUtjZeSdWWmzFPiUap%2BgXcXnV0jUNpfcaFh0EFpM%3D", "https://echovirtualcontentdev.blob.core.windows.net/");
        }
        
        public void DownloadContent()
        {
            bool started = AddressableContentManager.DownloadContent(OnComplete, DisplayDownloadStatus);
            if (started)
            {
                UpdateStatus("Downloading content...");
            }
            else
            {
                UpdateFeedback("Could not start download. Check logs for details.");
            }

            void OnComplete(RequestResult result)
            {
                Debug.Log(result.success ? "Download was a success" : "Download failed");
                
                UpdateStatus(result.success ? "Content downloaded successfully" : $"Download failed: {result.error}");
            }
        }
        
        public void ClearCache()
        {
            if (!AddressableContent.CacheExists())
            {
                Debug.LogWarning("No cache to clear");
                return;
            }

            AddressableContent.DeleteCacheFiles();
        }
        public void CheckCache()
        {
            bool cacheExists = AddressableContentManager.CacheExistsForLoadedCatalogs();
            Debug.Log(cacheExists ? "Cache exists" : "Cache does not exist");
        }
        
        private void DisplayDownloadStatus(DownloadStatus status)
        {
            _loadStatusSlider.value = status.Percent;
        }

        private void UpdateStatus(string text)
        {
            _status.text = text;
        }
        
        private void UpdateFeedback(string text)
        {
            _feedback.text = text;
        }
    }
}