using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace OpenUtility.Data.Addressable.Editor
{
    /*
     * Properties can be referenced in the addressable profile using the following syntax:
     * [OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.buildTarget]
     * [OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.storageUrl]
     * [OpenUtility.Data.Addressable.Editor.AddressableBuildProperties.storageName]
     */
    
    /// <summary>
    /// Holds properties used for the addressable profile to build the asset bundles with the correct url's.
    /// </summary>
    public static class AddressableBuildProperties
    {
        private static readonly AddressableContentSettings _settings;

        /// <summary>
        /// The build target to use for the addressable profile but in lowercase as that is
        /// compatible with url's.
        /// </summary>
        public static string buildTarget => EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();

        /// <summary>
        /// The name of the storage folder used for the addressable bundles.
        /// </summary>
        public static string storageName
        {
            get
            {
                if (_settings == null)
                {
                    Debug.LogError("No settings found. Please create a login in the project: Create > OpenUtility > AddressableContentSettings");
                    return string.Empty;
                }
                
                string value = _settings.StorageName;
                if (string.IsNullOrEmpty(value))
                {
                    Debug.LogError("No storage name found. Please check your variable set for storage name in the settings.");
                    return string.Empty;
                }
                
                return (value);
            }
        }

        /// <summary>
        /// The url to the storage folder used for the addressable bundles.
        /// </summary>
        public static string storageUrl
        {
            get
            {
                if (_settings == null)
                {
                    Debug.LogError("No settings found. Please create a login in the project: Create > OpenUtility > AddressableContentSettings");
                    return string.Empty;
                }
                
                string value = _settings.StorageUrl;
                if (string.IsNullOrEmpty(value))
                {
                    Debug.LogError("No storage url found. Please check your variable set for storage url in the settings.");
                    return string.Empty;
                }
                
                return (value);
            }
        }
        
        static AddressableBuildProperties()
        {
            var assets = AssetDatabase.FindAssets("t:AddressableContentSettings");
            if (assets.Length == 0)
                return;

            _settings = assets
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<AddressableContentSettings>)
                .FirstOrDefault();
        }
    }
}

#endif