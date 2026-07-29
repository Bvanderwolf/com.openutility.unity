using System;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data.Events
{
    public abstract class TypedEventListener<T> : MonoBehaviour
    {
        [Serializable]
        private class Event : UnityEvent<T> {}
        
        [Header("Project References")]
        [SerializeField]
        private ScriptableEvent<T> _event;

        [SerializeField]
        private Event _onEvent;
        
        private void OnEnable()
        {
            _event.AddListener(OnEvent);
        }
        
        private void OnDisable()
        {
            _event.RemoveListener(OnEvent);
        }

        private void OnEvent(T value)
        {
            if (Application.exitCancellationToken.IsCancellationRequested)
                return;
            
            _onEvent?.Invoke(value);
        }
    }
    
    public class VoidEventListener : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField, FlexibleAssetCreation]
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
