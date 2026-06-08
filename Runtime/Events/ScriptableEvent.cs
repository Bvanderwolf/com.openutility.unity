using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Data.Events
{
    [CreateAssetMenu(fileName = "ScriptableEvent", menuName = "OpenUtility/Scriptable Event/Void")]
    public class ScriptableEvent : ScriptableObject
    {
        [SerializeField]
        private UnityEvent _event;

        public void Invoke() => _event?.Invoke();
        public void AddListener(UnityAction action) => _event.AddListener(action);
        public void RemoveListener(UnityAction action) => _event.RemoveListener(action);
    }
}
