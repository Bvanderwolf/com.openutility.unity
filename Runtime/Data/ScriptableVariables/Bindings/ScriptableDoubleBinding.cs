using UnityEngine;

namespace OpenUtility.Data
{
    /// <summary>
    /// Base class for binding a scriptable double variable to a component where TElementData is the type of data
    /// the component uses (e.g. float for Slider).
    /// </summary>
    /// <typeparam name="TElementData">The type of data the component uses (e.g. float for Slider).</typeparam>
    public abstract class ScriptableDoubleBinding<TElementData> : MonoBehaviour
    {
        [Header("Variable")]
        [SerializeField]
        private ScriptableDouble _variable;
        
        protected ScriptableDouble variable => _variable;
        
        public abstract void SetValue(TElementData newValue);
    }
}
