using UnityEngine;

namespace OpenUtility.Data.Addressable
{
    [CreateAssetMenu(fileName = "AddressableContentSettings", menuName = "OpenUtility/AddressableContentSettings")]
    public class AddressableContentSettings : ScriptableObject
    {
        [SerializeField]
        private ScriptableString _storageName;

        [SerializeField]
        private ScriptableString _storageUrl;

        public string StorageName => _storageName.GetValue();
        
        public string StorageUrl => _storageUrl.GetValue();
    }
}
