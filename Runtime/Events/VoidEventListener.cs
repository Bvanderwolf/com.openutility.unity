using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data.Events
{
    public class VoidEventListener : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptableEvent _event;

        [SerializeField]
        private UnityEvent _onEvent;
        
        private void OnEnable()
        {
            _event.AddListener(OnEvent);
        }
        
        private void OnDisable()
        {
            _event.RemoveListener(OnEvent);
        }

        private void OnEvent()
        {
            if (Application.exitCancellationToken.IsCancellationRequested)
                return;
            
            _onEvent?.Invoke();
        }
    }
}
