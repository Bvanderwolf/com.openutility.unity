using System;
using System.Collections;
using OpenUtility.Exceptions;
using UnityEngine;

namespace OpenUtility.DelayedExecution
{
    public class ActionExcecutor : MonoBehaviour
    {
        public YieldInstruction ExcecuteNextFrame(Action action)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunNextFrame()));
            
            IEnumerator RunNextFrame()
            {
                yield return null;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExcecuteNextFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunNextFrame()));
            
            IEnumerator RunNextFrame()
            {
                yield return null;
                
                action.Invoke(parameter);
            }
        }

        public YieldInstruction ExcecuteEndOfFrame(Action action)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunEndOfFrame()));
            
            IEnumerator RunEndOfFrame()
            {
                yield return WaitFor.EndOfFrame;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExcecuteEndOfFrame<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunEndOfFrame()));
            
            IEnumerator RunEndOfFrame()
            {
                yield return WaitFor.EndOfFrame;
                
                action.Invoke(parameter);
            }
        }
        
        public YieldInstruction ExcecuteAfterFixedUpdate(Action action)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunFixedUpdate()));
            
            IEnumerator RunFixedUpdate()
            {
                yield return WaitFor.FixedUpdate;
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExcecuteAfterFixedUpdate<T>(Action<T> action, T parameter)
        {
            ThrowIf.Null(action);

            return (StartCoroutine(RunFixedUpdate()));
            
            IEnumerator RunFixedUpdate()
            {
                yield return WaitFor.FixedUpdate;
                
                action.Invoke(parameter);
            }
        }
        
        public YieldInstruction ExcecuteAfterSeconds(Action action, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.Seconds(seconds);
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExcecuteAfterSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.Seconds(seconds);
                
                action.Invoke(parameter);
            }
        }

        public YieldInstruction ExcecuteAfterRealtimeSeconds(Action action, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.RealtimeSeconds(seconds);
                
                action.Invoke();
            }
        }
        
        public YieldInstruction ExcecuteAfterRealtimeSeconds<T>(Action<T> action, T parameter, float seconds)
        {
            ThrowIf.Null(action);
            ThrowIf.Negative(seconds);

            return (StartCoroutine(RunAfterSeconds()));
            
            IEnumerator RunAfterSeconds()
            {
                yield return WaitFor.RealtimeSeconds(seconds);
                
                action.Invoke(parameter);
            }
        }
    }
}
