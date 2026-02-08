using OpenUtility.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class ContentMenuManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableGameObject _addressables;
        
        [Header("Scene References")]
        [SerializeField]
        private Image _progressRenderer;

        [Header("Events")]
        [SerializeField]
        private UnityEvent _contentLoaded;

        public void LoadContent()
        {
            var service = _addressables.GetComponent<AddressableContentService>();
            service.LoadContent(OnDownloadStatusUpdate);
        }

        private void OnDownloadStatusUpdate(DownloadStatus status)
        {
            _progressRenderer.fillAmount = status.Percent;

            if (status.IsDone)
                _contentLoaded?.Invoke();
        }
    }
}
