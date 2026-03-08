using OpenUtility.Data.Pooling;
using UnityEngine;

namespace OpenUtility.Samples.Data
{
    public static class ScriptablePoolExtensions
    {
        /// <summary>
        /// Returns a component of type T on retrieved game object from pool.
        /// </summary>
        public static T GetComponent<T>(this ScriptablePool pool)  where T : Component  => pool.Get().GetComponent<T>();
    }
}
