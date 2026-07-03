using UnityEngine;

namespace OpenUtility
{
    public interface IProcessScriptableValue<T>
    {
        T Process(T value);
    }
    
    public abstract class ScriptableValueProcessor<T> : ScriptableObject, IProcessScriptableValue<T>
    {
        public abstract T Process(T value);
    }
}
