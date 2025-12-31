using System.IO;
using UnityEditor;
using UnityEngine;

namespace OpenUtility.Data.Editor
{
    public class ScriptableVariableAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            string extension = Path.GetExtension(assetPath);
            if (extension != ".asset")
                return (AssetDeleteResult.DidNotDelete);

            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

            if (asset is ICanLoadValueFromPlayerPrefs loader)
            {
                Optional<string> preference = loader.PlayerPref;
                if (preference.HasValue)
                    PlayerPrefs.DeleteKey(preference.Value);
            }
            
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
