using System;
using System.Collections;
using OpenUtility.Exceptions;
using UnityEngine;

namespace OpenUtility.DelayedExecution
{
    public class ActionExecutor : MonoBehaviour
    {
        public YieldInstruction ExecuteNextFrame(Action action)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunNextFrame()));
            
            IEnumerator RunNextFrame()
            {
                yield return null;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExecuteNextFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunNextFrame()));
            
            IEnumerator RunNextFrame()
            {
                yield return null;
                
                action.Invoke(parameter);
            }
        }

        public YieldInstruction ExecuteEndOfFrame(Action action)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunEndOfFrame()));
            
            IEnumerator RunEndOfFrame()
            {
                yield return WaitFor.EndOfFrame;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExecuteEndOfFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunEndOfFrame()));
            
            IEnumerator RunEndOfFrame()
            {
                yield return WaitFor.EndOfFrame;
                
                action.Invoke(parameter);
            }
        }
        
        public YieldInstruction ExecuteAfterFixedUpdate(Action action)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunFixedUpdate()));
            
            IEnumerator RunFixedUpdate()
            {
                yield return WaitFor.FixedUpdate;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExecuteAfterFixedUpdate<T>(Action<T> action, T parameter)
        {
            ThrowIf.SystemObjectNull(action);

            return (StartCoroutine(RunFixedUpdate()));
            
            IEnumerator RunFixedUpdate()
            {
                yield return WaitFor.FixedUpdate;
                
                action.Invoke(parameter);
            }
        }
        
        public YieldInstruction ExecuteAfterSeconds(Action action, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.Seconds(seconds);
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExecuteAfterSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.Seconds(seconds);
                
                action.Invoke(parameter);
            }
        }

        public YieldInstruction ExecuteAfterRealtimeSeconds(Action action, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.RealtimeSeconds(seconds);
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExecuteAfterRealtimeSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.RealtimeSeconds(seconds);
                
                action.Invoke(parameter);
            }
        }

        public YieldInstruction ExcecuteAfterFrames(Action action, int frameCount)
        {
            ThrowIf.SystemObjectNull(action);
            ThrowIf.SmallerThen(frameCount, 1);

            return (StartCoroutine(RunAfterFrames()));
            
            IEnumerator RunAfterFrames()
            {
                for (int i = 0; i < frameCount; i++)
                {
                    yield return null;
                }
                
                action.Invoke();
            }
        }
    }
}
