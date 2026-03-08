using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenUtility.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace OpenUtility.Samples.Data
{
    public struct AddFromFileOptions
    {
        /// <summary>
        /// Etags can be used to determine if a cached file is up to date with the server version.
        /// Enabling this will cause the system to make a web request for each file added from file to check if the cached version is still valid.
        /// If the server version has changed since the last download, the new version will be downloaded and cached.
        /// If the server version is unchanged, the cached version will be used without making an additional web request to download the file.
        /// </summary>
        public bool useETags;
        
        /// <summary>
        /// Caches downloaded files to the local file system for future use. When enabled,
        /// the function will check for a cached version of the file before downloading.
        /// This value is ignored if useETags is true, as etags require a cached file to compare against.
        /// Default file cache directory is {Application.persistantDataPath}/images.
        /// </summary>
        public bool useFileCache;
        
        /// <summary>
        /// Caches downloaded textures in memory for the duration of the application.
        /// When enabled, the system will check for a cached version of the texture before downloading or loading from file.
        /// </summary>
        public bool useRuntimeCache;

        public bool ShouldUseFileCache => useFileCache || useETags;

        public static AddFromFileOptions Default => new AddFromFileOptions
        {
            useETags = false,
            useFileCache = true,
            useRuntimeCache = true
        };
    }
    
    [CreateAssetMenu(fileName = "TextureList", menuName = "Scriptable Objects/TextureList")]
    public class TextureList : ScriptableList<Texture2D>
    {
        [Header("File Settings")]
        [SerializeField, Tooltip("An optional sas token to use when your textures are retreived from a secure location (e.g. azure blob storage).")]
        private Optional<StringReference> _sasToken;

        [SerializeField, Tooltip("An optional domain from which your textures should be retreived when use the 'AddFromFile' function.")]
        private Optional<StringReference> _domain;

        [SerializeField, Tooltip("An optional directory the texture cache is placed in inside the 'Application.persistentDataPath'. Default is 'images'.")]
        private Optional<StringReference> _cacheDirectory;
        
        private readonly string[] _extensions = new[] { ".jpg", ".png", ".tga", ".exr" };

        public static string DefaultCacheDirectory => Path.Combine(Application.persistentDataPath, "images");

        public Texture2D AddFromData(byte[] data) => AddFromData(data, null);

        public Texture2D AddFromData(byte[] data, string textureName)
        {
            var texture = TextureManager.BytesToTexture(data, textureName);
            
            Add(texture);

            return (texture);
        }

        public Texture2D AddFromSprite(Sprite sprite)
        {
            var texture = TextureManager.SpriteToTexture(sprite);

            Add(texture);

            return (texture);
        }

        public Sprite GetSprite(int index)
        {
            var texture = GetValue(index);
            var sprite = TextureManager.TextureToSprite(texture);

            return (sprite);
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR
        public Promised<Texture2D> AddFromFile(string fileName, AddFromFileOptions? options = null)
        {
            string nameWithExtension = Path.GetFileName(fileName);
            string url = _domain.HasValue ? $"{_domain.Value.Value}/{fileName}" : fileName;

            if (_sasToken.HasValue)
                url += _sasToken.Value;

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

            Promised<Texture2D> promise = Promised<Texture2D>.FromWebRequest(request, Selector).Then(Add);
            
            return (promise);
            
            Texture2D Selector(byte[] bytes) => TextureManager.BytesToTexture(bytes, nameWithExtension);
        }
#else
        public Promised<Texture2D> AddFromFile(string fileName, AddFromFileOptions? options = null)
        {
            options ??= AddFromFileOptions.Default;
            
            Promised<Texture2D> promise;
            
            string nameWithExtension = Path.GetFileName(fileName);
            string basePath = Application.persistentDataPath;
            string cacheDirectory = _cacheDirectory.HasValue ? Path.Combine(basePath, _cacheDirectory.Value) : DefaultCacheDirectory;
            string fullPath = Path.Combine(cacheDirectory, nameWithExtension);
            if (options.Value.ShouldUseFileCache && File.Exists(fullPath))
            {
                if (options.Value.useRuntimeCache && TextureManager.TryGetTexture(nameWithExtension, out Texture2D texture))
                {
#if UNITY_EDITOR
                    Debug.Log($"Retrieving texture {texture.name} from memory.");
#endif
                    promise = Promised<Texture2D>.FromResult(texture);
                    
                    Add(texture);
                    
                    return (promise);
                }

                if (!options.Value.useETags)
                {
#if UNITY_EDITOR
                    Debug.Log($"Retrieving texture from cache at path: {fullPath}");
#endif
                    Task<byte[]> task = File.ReadAllBytesAsync(fullPath);

                    promise = Promised<Texture2D>.FromTask(task, Selector).Then(Add);
                        
                    return (promise);
                }
            }

            string url = _domain.HasValue ? $"{_domain.Value.Value}/{fileName}" : fileName;

#if UNITY_EDITOR
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                Debug.Log($"Retrieving texture from URL: {url}");
            else
                Debug.LogWarning($"Detected '{url}' not to be well formed.");
#endif

            if (_sasToken.HasValue)
                url += _sasToken.Value;

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

            if (options.Value.useETags)
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                string etagPath = Path.Combine(cacheDirectory, $"{nameWithoutExtension}.etag");
                if (File.Exists(etagPath))
                {
                    string etag = File.ReadAllText(etagPath);
                    
                    request.SetRequestHeader("If-None-Match", etag);
                }

                UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                operation.completed += OnWebRequestOperationCompleted;

                promise = Promised<Texture2D>.FromWebRequest(operation, Selector)
                    .Then(Add)
                    .Catch(ReadNotModifiedFromCache);

                return (promise);

                void OnWebRequestOperationCompleted(AsyncOperation finishedOperation)
                {
                    UnityWebRequest finishedRequest = ((UnityWebRequestAsyncOperation)finishedOperation).webRequest;
                    
                    if (finishedRequest.responseCode == (long)HttpStatusCode.NotModified)
                    {
#if UNITY_EDITOR
                        Debug.Log($"Cached version of texture '{nameWithExtension}' is up to date. Using cached version without downloading.");
#endif
                        return;
                    }
                    
                    if (finishedRequest.result != UnityWebRequest.Result.Success)
                        return;
                    
#if UNITY_EDITOR
                    Debug.Log($"Cached version of texture '{nameWithExtension}' is not up to date. Using and caching new downloaded version.");
#endif
                    
                    string etag = finishedRequest.GetResponseHeader("ETag");

                    Directory.CreateDirectory(cacheDirectory);
                    File.WriteAllBytes(fullPath, finishedRequest.downloadHandler.data);
                    File.WriteAllText(etagPath, etag);
                }
                
                void ReadNotModifiedFromCache(Promised<Texture2D> erroredPromise)
                {
                    if (!long.TryParse(erroredPromise.Error, out long code) || (HttpStatusCode)code != HttpStatusCode.NotModified)
                        return;

                    Task<byte[]> task = File.ReadAllBytesAsync(fullPath);

                    erroredPromise.Extend().WithTask(task, Selector);
                }
            }

            promise = Promised<Texture2D>.FromWebRequest(request, Selector).Then(Add).Then(WriteToFileAsync);
            
            return (promise);
            Texture2D Selector(byte[] bytes) => TextureManager.BytesToTexture(bytes, nameWithExtension);
        }
#endif

        public void ClearFileCache()
        {
            string basePath = Application.persistentDataPath;
            string cacheDirectory = _cacheDirectory.HasValue ? Path.Combine(basePath, _cacheDirectory.Value) : DefaultCacheDirectory;

            Directory.Delete(cacheDirectory, true);
            
#if UNITY_EDITOR
            Debug.Log($"Cleared texture cache at path: {cacheDirectory}");
#endif
        }
        
        private async Task WriteToFileAsync(Texture2D texture)
        {
            if (texture == null)
                return;
            
            string fileName = texture.name;
            string basePath = Application.persistentDataPath;
            string cacheDirectory = _cacheDirectory.HasValue ? Path.Combine(basePath, _cacheDirectory.Value) : DefaultCacheDirectory;
            string fullPath = Path.Combine(cacheDirectory, fileName);

            Directory.CreateDirectory(cacheDirectory);

            byte[] data;
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".png":
                    data = texture.EncodeToPNG();
                    break;
                
                case ".jpg":
                    data = texture.EncodeToJPG();
                    break;
                
                case ".tga":
                    data = texture.EncodeToTGA();
                    break;
                
                case ".exr":
                    data = texture.EncodeToEXR();
                    break;
                
                default:
                    Debug.LogWarning($"Unsupported file extension '{extension}' for texture '{fileName}'. Defaulting to PNG format.");
                    data = texture.EncodeToPNG();
                    break;
            }
            
#if UNITY_EDITOR
            Debug.Log($"Writing texture to cache at path: {fullPath}");
#endif
            
            await File.WriteAllBytesAsync(fullPath, data).ConfigureAwait(false);
        }
    }
}