using System.Collections;
using OpenUtility.Data;
using OpenUtility.Data.Addressable;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace OpenUtility.Samples.Addressables
{
    public class AdvancedAddressablesSystem : MonoBehaviour
    {
        [Header("Required")]
        [SerializeField]
        private string _catalogPath;

        [Header("Optional")]
        [SerializeField]
        private string[] _filteredKeys;

        [SerializeField]
        private Optional<string[]> _baseUrls;

        [SerializeField]
        private Optional<string> _sasToken;
        
        [Header("UI Elements")]
        [SerializeField]
        private Slider _loadStatusSlider;

        [SerializeField]
        private TMP_Text _feedback;

        [SerializeField]
        private TMP_Text _status;

        private void Awake()
        {
            if (_sasToken.HasValue && _baseUrls.HasValue)
                AddressableContentManager.EnableSasTokenUsage(_sasToken.Value, _baseUrls.Value);
        }

        [ContextMenu("Load Catalog")]
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
        }

        [ContextMenu("Get Download Size")]
        public void GetDownloadSize()
        {
            AddressableContentManager.GetDownloadSize(OnComplete);

            void OnComplete(DataRequestResult<long?> result)
            {
                if (result.success)
                {
                    if (result.data.HasValue)
                    {
                        Debug.Log($"Success: content ready ({result.data.Value} MB) ");
                    }
                    else
                    {
                        Debug.Log("Success: content loaded");
                    }
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
        }

        [ContextMenu("Get Filtered Download Size")]
        public void GetFilteredDownloadSize()
        {
            IEnumerable keys = AddressableContentManager.GetCatalogKeys(_filteredKeys);
            AddressableContent.GetDownloadSize(keys, OnComplete);

            void OnComplete(DataRequestResult<long?> result)
            {
                if (result.success)
                {
                    if (result.data.HasValue)
                    {
                        Debug.Log($"Success: content ready ({result.data.Value} MB) ");
                    }
                    else
                    {
                        Debug.Log("Success: content loaded");
                    }
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
        }

        [ContextMenu("Download Content")]
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

        [ContextMenu("Download Filtered Content")]
        public void DownloadFilteredContent()
        {
            IEnumerable keys = AddressableContentManager.GetCatalogKeys(_filteredKeys);
            bool started = AddressableContentManager.DownloadContent(keys, OnComplete, DisplayDownloadStatus);
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

        [ContextMenu("Clear Cache")]
        public void ClearCache()
        {
            if (!AddressableContent.CacheExists())
            {
                Debug.LogWarning("No cache to clear");
                return;
            }

            AddressableContent.DeleteCacheFiles();
        }

        [ContextMenu("Check Cache")]
        public void CheckCache()
        {
            bool cacheExists = AddressableContentManager.CacheExistsForLoadedCatalogs();
            Debug.Log(cacheExists ? "Cache exists" : "Cache does not exist");
        }

        [ContextMenu("Load Catalog and Download Content")]
        public void LoadCatalogAndDownloadContent()
        {
            AddressableContentManager.DownloadContentCatalog(_catalogPath, OnComplete);

            void OnComplete(RequestResult result)
            {
                if (result.success)
                {
                    Debug.Log("Catalog loaded");
                    DownloadContent();
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
        }

        [ContextMenu("Check For Catalog Updates")]
        public void CheckForCatalogUpdates()
        {
            AddressableContentManager.GetUpdatedCatalogs(OnComplete);

            void OnComplete(DataRequestResult<string[]> result)
            {
                if (result.success)
                {
                    Debug.Log("Catalogs with updates: " + string.Join(", ", result.data));
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
        }

        [ContextMenu("Check and Update Catalogs")]
        public void UpdateCatalogs()
        {
            AddressableContentManager.GetUpdatedCatalogs(OnComplete);

            void OnComplete(DataRequestResult<string[]> result)
            {
                if (result.success)
                {
                    if (result.data.Length == 0)
                    {
                        Debug.Log("No catalogs to update");
                        return;
                    }

                    Debug.Log("Catalogs with updates: " + string.Join(", ", result.data));
                    AddressableContentManager.DownloadUpdatedCatalogs(result.data, OnUpdateComplete);
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }

            void OnUpdateComplete(RequestResult result)
            {
                if (result.success)
                {
                    Debug.Log("Catalogs updated");
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
        }

        [ContextMenu("Check, Update And GetSize Of Catalog")]
        public void CheckUpdateAndGetSizeOfCatalog()
        {
            AddressableContentManager.GetUpdatedCatalogs(OnComplete);

            void OnComplete(DataRequestResult<string[]> result)
            {
                if (result.success)
                {
                    if (result.data.Length == 0)
                    {
                        Debug.Log("No catalogs to update");
                        return;
                    }

                    Debug.Log("Catalogs with updates: " + string.Join(", ", result.data));

                    AddressableContentManager.DownloadUpdatedCatalogs(result.data, OnUpdateComplete);
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }

            void OnUpdateComplete(RequestResult result)
            {
                if (result.success)
                {
                    Debug.Log("Catalogs updated");

                    GetDownloadSize();
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                }
            }
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
