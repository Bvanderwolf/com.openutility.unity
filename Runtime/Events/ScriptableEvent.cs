using System;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data.Events
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        [Serializable]
        private class Event : UnityEvent<T> {}

        [SerializeField]
        private Event _event;
        
        public void Invoke(T value) => _event?.Invoke(value);
        public void AddListener(UnityAction<T> action) => _event.AddListener(action);
        public void RemoveListener(UnityAction<T> action) => _event.RemoveListener(action);
    }
    
    [CreateAssetMenu(fileName = "ScriptableEvent", menuName = "OpenUtility/Scriptable Event/Void")]
    public class ScriptableEvent : ScriptableObject
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent _event;

        public void Invoke() => _event?.Invoke();
        public void AddListener(UnityAction action) => _event.AddListener(action);
        public void RemoveListener(UnityAction action) => _event.RemoveListener(action);
    }
}
