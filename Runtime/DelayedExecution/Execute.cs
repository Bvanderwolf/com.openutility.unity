using System;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenUtility.DelayedExecution
{
    public static class Execute
    {
        private static Optional<ActionExecutor> _executor;

        /// <summary>
        /// Invokes the action on the next frame, after all Update calls have been made.
        /// </summary>
        public static YieldInstruction NextFrame(Action action)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteNextFrame(action));
        }

        /// <summary>
        /// Invokes the action on the next frame, after all Update calls have been made.
        /// </summary>
        public static YieldInstruction NextFrame<T>(Action<T> action, T argument)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteNextFrame(action, argument));
        }
        
        /// <summary>
        /// Invokes the action at the end of the current frame, after all rendering is complete.
        /// </summary>
        public static YieldInstruction EndOfFrame(Action action)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteEndOfFrame(action));
        }
        
        /// <summary>
        /// Invokes the action at the end of the current frame, after all rendering is complete.
        /// </summary>
        public static YieldInstruction EndOfFrame<T>(Action<T> action, T argument)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteEndOfFrame(action, argument));
        }
        
        /// <summary>
        /// Invokes the action after all FixedUpdate calls have been made.
        /// </summary>
        /// <param name="action"></param>
        public static YieldInstruction AfterFixedUpdate(Action action)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteAfterFixedUpdate(action));
        }
        
        /// <summary>
        /// Invokes the action after all FixedUpdate calls have been made.
        /// </summary>
        public static YieldInstruction AfterFixedUpdate<T>(Action<T> action, T parameter)
        {
            ThrowIf.SystemObjectNull(action);
            
            return (GetOrCreateExcecutor().ExecuteAfterFixedUpdate(action, parameter));
        }
        
        public static YieldInstruction AfterFrames(Action action, int frameCount)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(frameCount);
            
            return (GetOrCreateExcecutor().ExcecuteAfterFrames(action, frameCount));
        }
        
        /// <summary>
        /// Invokes the action after the specified number of seconds have passed.
        /// </summary>
        public static YieldInstruction AfterSeconds(Action action, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (GetOrCreateExcecutor().ExecuteAfterSeconds(action, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number of seconds have passed.
        /// </summary>
        public static YieldInstruction AfterSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);
            
            return (GetOrCreateExcecutor().ExecuteAfterSeconds(action, parameter, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number seconds have passed. Uses unscaled time.
        /// </summary>
        public static YieldInstruction AfterRealtimeSeconds(Action action, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (GetOrCreateExcecutor().ExecuteAfterRealtimeSeconds(action, seconds));
        }
        
        /// <summary>
        /// Invokes the action after the specified number seconds have passed. Uses unscaled time.
        /// </summary>
        public static YieldInstruction AfterRealtimeSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);
            
            return (GetOrCreateExcecutor().ExecuteAfterRealtimeSeconds(action, parameter, seconds));
        }

        private static ActionExecutor GetOrCreateExcecutor()
        {
            if (_executor.HasValue)
                return (_executor.Value);
            
            GameObject instance = new GameObject("~Excecute.ActionExecutor");
            ActionExecutor executor = instance.AddComponent<ActionExecutor>();
            Object.DontDestroyOnLoad(instance);

            _executor = executor;
            
            return (executor);
        }
    }
}
