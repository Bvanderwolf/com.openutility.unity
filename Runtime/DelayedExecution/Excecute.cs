using System;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.DelayedExecution
{
    public static class Excecute
    {
        private static Optional<ActionExcecutor> _excecutor;

        /// <summary>
        /// Invokes the action on the next frame, after all Update calls have been made.
        /// </summary>
        public static YieldInstruction NextFrame(Action action)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteNextFrame(action));
        }

        /// <summary>
        /// Invokes the action on the next frame, after all Update calls have been made.
        /// </summary>
        public static YieldInstruction NextFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteNextFrame(action, parameter));
        }
        
        /// <summary>
        /// Invokes the action at the end of the current frame, after all rendering is complete.
        /// </summary>
        public static YieldInstruction EndOfFrame(Action action)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteEndOfFrame(action));
        }
        
        /// <summary>
        /// Invokes the action at the end of the current frame, after all rendering is complete.
        /// </summary>
        public static YieldInstruction EndOfFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteEndOfFrame(action, parameter));
        }
        
        /// <summary>
        /// Invokes the action after all FixedUpdate calls have been made.
        /// </summary>
        /// <param name="action"></param>
        public static YieldInstruction AfterFixedUpdate(Action action)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteAfterFixedUpdate(action));
        }
        
        /// <summary>
        /// Invokes the action after all FixedUpdate calls have been made.
        /// </summary>
        public static YieldInstruction AfterFixedUpdate<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);
            
            return (GetOrCreateExcecutor().ExcecuteAfterFixedUpdate(action, parameter));
        }
        
        /// <summary>
        /// Invokes the action after the specified number of seconds have passed.
        /// </summary>
        public static YieldInstruction AfterSeconds(Action action, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (GetOrCreateExcecutor().ExcecuteAfterSeconds(action, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number of seconds have passed.
        /// </summary>
        public static YieldInstruction AfterSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);
            
            return (GetOrCreateExcecutor().ExcecuteAfterSeconds(action, parameter, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number seconds have passed. Uses unscaled time.
        /// </summary>
        public static YieldInstruction AfterRealtimeSeconds(Action action, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (GetOrCreateExcecutor().ExcecuteAfterRealtimeSeconds(action, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number seconds have passed. Uses unscaled time.
        /// </summary>
        public static YieldInstruction AfterRealtimeSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);
            
            return (GetOrCreateExcecutor().ExcecuteAfterRealtimeSeconds(action, parameter, seconds));
        }

        private static ActionExcecutor GetOrCreateExcecutor()
        {
            if (_excecutor.HasValue)
                return (_excecutor.Value);
            
            GameObject instance = new GameObject("~Excecute.ActionExcecutor");
            ActionExcecutor excecutor = instance.AddComponent<ActionExcecutor>();
            Object.DontDestroyOnLoad(instance);

            _excecutor = excecutor;
            
            return (excecutor);
        }
    }
}
