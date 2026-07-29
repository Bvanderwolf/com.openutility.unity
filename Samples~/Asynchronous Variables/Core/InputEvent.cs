#if ENABLE_INPUT_SYSTEM

using OpenUtility.Data.Events;
using UnityEngine;
using UnityEngine.InputSystem;


namespace OpenUtility.Samples.Data
{
    [CreateAssetMenu(fileName = "ScriptableEvent", menuName = "OpenUtility/Scriptable Event/Input")]
    public class InputEvent : ScriptableEvent
    {
        [Header("Settings")]
        [SerializeField]
        private InputAction _action;

        private void OnEnable()
        {
            _action.Enable();
            _action.started += OnActionStarted;
        }

        private void OnDisable()
        {
            _action.started -= OnActionStarted;
            _action.Disable();
        }

        private void OnActionStarted(InputAction.CallbackContext context)
        {
            Invoke();
        }
    }
}

#endif