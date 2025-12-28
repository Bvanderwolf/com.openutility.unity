using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenUtility.DelayedExecution;
using OpenUtility.Exceptions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OpenUtility.Data.Addressable
{
    /// <summary>
    /// Helps manage addressable content catalogs and downloading content at runtime.
    /// </summary>
    public static class AddressableContentManager
    {
        private struct SasToken
        {
            public Func<string> factoryMethod;
            public string[] baseUrls;
            
            public SasToken(Func<string> factoryMethod, string[] baseUrls)
            {
                this.factoryMethod = factoryMethod;
                this.baseUrls = baseUrls;
            }
        }

        /// <summary>
        /// Set if sas token usage is enabled to append sas tokens to web requests.
        /// </summary>
        private static SasToken? _sasToken;
        
        /// <summary>
        /// The list of loaded catalogs.
        /// </summary>
        private static readonly IList<IResourceLocator> _catalogs = new List<IResourceLocator>();

        /// <summary>
        /// Returns whether any content catalogs have been loaded.
        /// </summary>
        public static bool HasLoadedCatalogs => _catalogs.Count > 0;

        /// <summary>
        /// Enables the usage of SAS tokens for asset bundle requests. Provide your complete sas token and the base
        /// urls (e.g. "https://prodcontainer.blob.core.windows.net/" and https://devcontainer.blob.core.windows.net/).
        /// </summary>
        public static void EnableSasTokenUsage(string sasToken, params string[] baseUrls)
        {
            EnableSasTokenUsage(FactoryMethod, baseUrls);

            string FactoryMethod() => sasToken;
        }
        
        /// <summary>
        /// Enables the usage of SAS tokens for asset bundle requests. Provide your sas token factory method and the base
        /// urls (e.g. "https://prodcontainer.blob.core.windows.net/" and https://devcontainer.blob.core.windows.net/).
        /// </summary>
        public static void EnableSasTokenUsage(Func<string> sasTokenFactory, params string[] baseUrls)
        {
            ThrowIf.Null(sasTokenFactory);
            ThrowIf.EmptyArray(baseUrls);

            _sasToken = new SasToken(sasTokenFactory, baseUrls);
            
            Addressables.WebRequestOverride = WebRequestOverride;
        }
        
        /// <summary>
        /// Returns an enumeration of all keys in the loaded catalogs.
        /// </summary>
        public static IEnumerable GetCatalogKeys()
        {
            if (_catalogs.Count == 0)
                return (Enumerable.Empty<object>());

            return (_catalogs.SelectMany(catalog => catalog.Keys));
        }

        /// <summary>
        /// Returns an enumeration of all keys in the loaded catalogs. Use this overload if you want
        /// to filter which content to download before calling GetDownloadSize(keys) or DownloadContent(keys).
        /// </summary>
        public static IEnumerable GetCatalogKeys(string[] filter)
        {
            List<string> keys = new List<string>();
            foreach (object key in GetCatalogKeys())
            {
                if (!Array.Exists(filter, k => k.Equals(key)))
                    continue;
                
                keys.Add(key.ToString());
            }

            return (keys);
        }
        
        /// <summary>
        /// Returns an enumeration of all keys in the loaded catalogs. Use this overload if you want
        /// to filter which content to download before calling GetDownloadSize(keys) or DownloadContent(keys).
        /// </summary>
        public static IEnumerable GetCatalogKeys(Predicate<object> predicate)
        {
            List<object> keys = new List<object>();
            foreach (object key in GetCatalogKeys())
            {
                if (!predicate(key))
                    continue;
                
                keys.Add(key);
            }

            return (keys);
        }
        
        /// <summary>
        /// Donwloads the content catalog from the specified path. Returns false if the catalog is already loaded.
        /// </summary>
        public static bool DownloadContentCatalog(string catalogPath, Action<RequestResult> callback = null)
        {
            if (AddressableContent.IsContentCatalogLoaded(catalogPath))
            {
                Debug.LogWarning("Catalog already loaded");
                return (false);
            }

            AddressableContent.DownloadContentCatalog(catalogPath, OnComplete);

            return (true);

            void OnComplete(DataRequestResult<IResourceLocator> result)
            {
                if (result.success)
                {
                    _catalogs.Add(result.data);
                    Debug.Log($"Catalog downloaded. Id: {result.data.LocatorId}, keys: {string.Join(", ", result.data.Keys)}");
                    callback?.Invoke(RequestResult.CreateSuccess());
                }
                else
                {
                    Debug.LogWarning("Catalog download failed: " + result.error);
                    callback?.Invoke(RequestResult.CreateError(result.error));
                }
            }
        }

        /// <summary>
        /// Loads the content catalog from the specified path. Returns false if the cache does not exist.
        /// </summary>
        public static bool LoadContentCatalog(string catalogPath, Action<RequestResult> callback)
        {
            if (!AddressableContent.CacheExists())
            {
                Debug.LogWarning("Cache does not exist");
                return (false);
            }

            AddressableContent.LoadContentCatalog(catalogPath, OnComplete);

            return (true);

            void OnComplete(DataRequestResult<IResourceLocator> result)
            {
                if (result.success)
                {
                    _catalogs.Add(result.data);
                    Debug.Log($"Catalog loaded. Id: {result.data.LocatorId}, keys: {string.Join(", ", result.data.Keys)}");
                    callback?.Invoke(RequestResult.CreateSuccess());
                }
                else
                {
                    Debug.LogWarning("Failed: " + result.error);
                    callback?.Invoke(RequestResult.CreateError(result.error));
                }
            }
        }

        /// <summary>
        /// Retrieves the download size for all content in the loaded catalogs. Returns false if no catalogs are loaded.
        /// </summary>
        public static bool GetDownloadSize(Action<DataRequestResult<long?>> callback)
        {
            if (_catalogs.Count == 0)
            {
                Debug.LogWarning("Catalogs should be loaded before getting download size");
                return (false);
            }

            AddressableContent.GetDownloadSize(_catalogs, OnComplete);

            return (true);

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

                    callback?.Invoke(DataRequestResult<long?>.CreateSuccess(result.data));
                }
                else
                {
                    Debug.Log("Failed: " + result.error);
                    callback?.Invoke(DataRequestResult<long?>.CreateError(result.error));
                }
            }
        }

        /// <summary>
        /// Downloads all content in the loaded catalogs. Returns false if no catalogs are loaded.
        /// </summary>
        public static bool DownloadContent(Action<RequestResult> resultCallback, Action<DownloadStatus> statusCallback = null)
        {
            if (_catalogs.Count == 0)
            {
                Debug.LogWarning("Catalog should be loaded before downloading content");
                return (false);
            }

            AsyncOperationHandle operation = AddressableContent.DownloadContent(_catalogs);
            WaitFor.Operation(operation, resultCallback, statusCallback);

            return (true);
        }
        
        /// <summary>
        /// Downloads content for the specified keys. Use the GetLoadedCatalogKeys() method to get all keys in the loaded catalogs.
        /// Returns false if no catalogs are loaded.
        /// </summary>
        public static bool DownloadContent(IEnumerable keys, Action<RequestResult> resultCallback, Action<DownloadStatus> statusCallback = null)
        {
            if (_catalogs.Count == 0)
            {
                Debug.LogWarning("Catalog should be loaded before downloading content");
                return (false);
            }
        
            AsyncOperationHandle operation = AddressableContent.DownloadContent(keys);
            WaitFor.Operation(operation, resultCallback, statusCallback);

            return (true);
        }

        /// <summary>
        /// Retrieves the list of catalogs that require and update. Returns false if no catalogs are loaded.
        /// </summary>
        public static bool GetUpdatedCatalogs(Action<DataRequestResult<string[]>> callback)
        {
            if (_catalogs.Count == 0)
            {
                Debug.LogWarning("Catalog should be loaded before checking for updates");
                return (false);
            }

            AddressableContent.GetUpdatedCatalogs(OnComplete);

            return (true);

            void OnComplete(DataRequestResult<string[]> result)
            {
                if (result.success)
                {
                    if (result.data.Length != 0)
                        Debug.Log($"Found {result.data.Length} updated catalogs");

                    callback?.Invoke(DataRequestResult<string[]>.CreateSuccess(result.data));
                }
                else
                {
                    Debug.LogError($"Retrieval of updated catalogs failed: {result.error}");

                    callback?.Invoke(DataRequestResult<string[]>.CreateError(result.error));
                }
            }
        }

        /// <summary>
        /// Downloads the updated remote catalogs at the given paths. Use this method after receiving updated catalogs
        /// from the <see cref="GetUpdatedCatalogs"/> method after logging in the second time. Returns false if no catalogs are loaded.
        /// </summary>
        public static bool DownloadUpdatedCatalogs(string[] catalogPaths, Action<RequestResult> callback)
        {
            if (_catalogs.Count == 0)
            {
                Debug.LogWarning("Catalog should be loaded before updating");
                return (false);
            }

            AddressableContent.DownloadUpdatedContentCatalogs(catalogPaths, OnComplete);

            return (true);

            void OnComplete(DataRequestResult<IResourceLocator[]> result)
            {
                if (result.success)
                {
                    for (int i = 0; i < _catalogs.Count; i++)
                    {
                        string locatorId = _catalogs[i].LocatorId;
                        int indexOfUpdatedCatalog = Array.FindIndex(result.data, (catalog) => catalog.LocatorId == locatorId);
                        if (indexOfUpdatedCatalog == -1)
                            continue;

                        _catalogs[i] = result.data[indexOfUpdatedCatalog];
                    }

                    Debug.Log($"Download of updated catalogs was a success. Updated {_catalogs.Count} catalogs");

                    callback?.Invoke(RequestResult.CreateSuccess());
                }
                else
                {
                    Debug.LogError($"Download of updated catalogs failed: {result.error}");

                    callback?.Invoke(RequestResult.CreateError(result.error));
                }
            }
        }

        /// <summary>
        /// Returns whether cache exists for all loaded catalogs.
        /// </summary>
        public static bool CacheExistsForLoadedCatalogs()
        {
            if (_catalogs.Count == 0)
                return (false);

            foreach (IResourceLocator catalog in _catalogs)
                if (!AddressableContent.CacheExists((ResourceLocationMap)catalog))
                    return (false);

            return (true);
        }

        /// <summary>
        /// Clears the runtime cache by removing all loaded catalogs.
        /// </summary>
        public static void ClearRuntimeCache()
        {
            for (int i = _catalogs.Count - 1; i >= 0; i--)
            {
                Addressables.RemoveResourceLocator(_catalogs[i]);
                _catalogs.RemoveAt(i);
            }
        }
        
        /// <summary>
        /// Appends the SAS token to the web request if the request url starts with one of the base urls.
        /// </summary>
        private static void WebRequestOverride(UnityWebRequest request)
        {
            if (!_sasToken.HasValue)
                return;
            
            SasToken token = _sasToken.Value;
            if (!token.baseUrls.Any(url => request.url.StartsWith(url)))
                return;
                
            string sasToken = token.factoryMethod.Invoke();
            request.url += sasToken;
        }
    }
}