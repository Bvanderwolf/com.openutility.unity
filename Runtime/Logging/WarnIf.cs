using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.Logging
{
    public static class WarnIf
    {
        public static void SystemObjectNull(object obj, string message)
        {
            if (obj == null)
                Debug.LogWarning(message);
        }
        
        public static void SystemObjectNull(object obj, string message, Object context)
        {
            if (obj == null)
                Debug.LogWarning(message, context);
        }
    }
}
