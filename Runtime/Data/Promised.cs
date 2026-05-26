using System;
using System.Net;
using System.Threading.Tasks;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace OpenUtility.Data
{
    public static class PromisePool<T>
    {
        /// <summary>
        /// The pool of promised objects used for type T. This pool is created lazily when the first promise is requested.
        /// </summary>
        private static IObjectPool<Promised<T>> _pool;

        /// <summary>
        /// Returns a promise of type T from the pool.
        /// </summary>
        /// <returns></returns>
        public static Promised<T> Get() => (_pool ??= new ObjectPool<Promised<T>>(() => new Promised<T>(), actionOnRelease: (p) => p.Reset())).Get();
        
        /// <summary>
        /// Outputs a promise of type T from the pool. Use the return value with the 'using' statement o release the promise upon completion.
        /// </summary>
        public static PooledObject<Promised<T>> Get(out Promised<T> promise) => (_pool ??= new ObjectPool<Promised<T>>(() => new Promised<T>())).Get(out promise);

        /// <summary>
        /// Releases the given promise, returning it back to the pool.
        /// </summary>
        public static void Release(Promised<T> promise) => _pool?.Release(promise);
    }
    
    [Serializable]
    public class Promised<T>
    {
        public readonly struct Extension
        {
            private readonly Promised<T> _promise;
            
            public Extension(Promised<T> promise)
            {
                _promise = promise;
                _promise.Error = null;
            }
            
            /// <summary>
            /// Extends the promise with a task, completing the promise after the given task has completed.
            /// </summary>
            public Promised<T> WithTask(Task<T> task)
            {
                task.ContinueWith(OnComplete, _promise);

                return (_promise);

                void OnComplete(Task<T> completedTask, object state)
                {
                    Promised<T> promisedState = (Promised<T>)state;
                
                    if (completedTask.IsCompletedSuccessfully)
                    {
                        T value = completedTask.Result;
                        promisedState.Value = value;
                    }
                    else
                    {
                        string message = completedTask.Exception?.ToString();
                        promisedState.Error = message;
                    }
                }
            }
            
            /// <summary>
            /// Extends the promise with a task, completing the promise after the given task has completed.
            /// </summary>
            public Promised<T> WithTask<TResult>(Task<TResult> task, Func<TResult, T> selector)
            {
                task.ContinueWith(OnComplete, _promise);

                return (_promise);

                async Task OnComplete(Task<TResult> completedTask, object state)
                {
                    Promised<T> promisedState = (Promised<T>)state;
                
                    if (completedTask.IsCompletedSuccessfully)
                    {
                        try
                        {
                            await Awaitable.MainThreadAsync();
                        
                            TResult value = completedTask.Result;
                            promisedState.Value = selector(value);
                        }
                        catch (Exception e)
                        {
                            string message = e.ToString();
                            promisedState.Error = message;
                        }
                    }
                    else
                    {
                        string message = completedTask.Exception?.ToString();
                        promisedState.Error = message;
                    }
                }
            }
        }
        
        public class YieldInstruction : CustomYieldInstruction
        {
            public override bool keepWaiting => !_completed;

            private bool _completed;
            
            public YieldInstruction(Promised<T> promise)
            {
                promise.Then(OnValueReceived);
                promise.Catch(OnErrorReceived);
            }

            private void OnValueReceived(T result)
            {
                _completed = true;
            }

            private void OnErrorReceived(Promised<T> promise)
            {
                _completed = true;
            }
        }
        
        public class ValueReceivedEvent : UnityEvent<T> {}
        
        [SerializeField]
        private ValueReceivedEvent _valueReceived;
        
        private Action<Promised<T>> _errorReceived;
        private Optional<T> _value;
        private string _error;
        private bool _releaseOnCompletion;

        /// <summary>
        /// Is this promise completed and does it have a value (it is fullfilled).
        /// </summary>
        public bool HasValue => _value.HasValue;
        
        /// <summary>
        /// Is this promise completed and does it have an error (it failed).
        /// </summary>
        public bool HasError => Error != null;

        /// <summary>
        /// The error message the promise failed with.
        /// </summary>
        public string Error
        {
            get => _error;
            set
            {
                bool previouslyHadError = _error != null;
                
                _error = value;
                
                if (!previouslyHadError)
                {
                    _errorReceived?.Invoke(this);
                    
                    if (_releaseOnCompletion)
                        Release();
                }
            }
        }
        
        /// <summary>
        /// The value of this promise. Setting it the first time will fullfill it,
        /// triggering the 'Then' callbacks.
        /// </summary>
        public T Value
        {
            get => _value.GetValueOrDefault();
            set
            {
                bool previouslyHadValue = _value.HasValue;

                _value = value;
                
                if (!previouslyHadValue)
                {
                    _valueReceived?.Invoke(value);
                    
                    if (_releaseOnCompletion)
                        Release();
                }
            }
        }

        /// <summary>
        /// Return a yield instruction to use to wait for completion in coroutines.
        /// </summary>
        /// <returns></returns>
        public YieldInstruction Yield() => new YieldInstruction(this);

        /// <summary>
        /// Adds a callback when the promise is fullfilled.
        /// </summary>
        public Promised<T> Then(UnityAction<T> callback)
        {
            (_valueReceived ??= new ValueReceivedEvent()).AddListener(callback);
            return (this);
        }

        /// <summary>
        /// Adds a callback when the promise is fullfilled.
        /// </summary>
        public Promised<T> Then(Func<T, Task> callback)
        {
            (_valueReceived ??= new ValueReceivedEvent()).AddListener(OnValueReceived);

            void OnValueReceived(T value)
            {
                Task task = callback(value);
                task.ContinueWith(OnComplete);

                void OnComplete(Task completedTask)
                {
                    if (completedTask.IsCompletedSuccessfully)
                        return;
                    
                    string message = completedTask.Exception?.ToString();
                    Error = message;
                }
            }

            return (this);
        }

        /// <summary>
        /// Adds a callback when the promise has failed.
        /// </summary>
        public Promised<T> Catch(Action<Promised<T>> onErrorReceived)
        {
            _errorReceived += onErrorReceived;
            return (this);
        }

        /// <summary>
        /// Returns this promise to the promise pool when it has been completed.
        /// Always try to use this if you are not caching the promise yourself.
        /// </summary>
        public Promised<T> ReleaseOnCompletion()
        {
            _releaseOnCompletion = true;
            return (this);
        }

        /// <summary>
        /// Extends this promise, providing you with an interface to continue with
        /// other tasks without fullfilling the promise.
        /// </summary>
        /// <returns></returns>
        public Extension Extend() => new Extension(this);
        
        public override string ToString() => _value.HasValue ? _value.ToString() : "(Pending)";

        public static implicit operator T(Promised<T> promise) => promise.Value;

        /// <summary>
        /// Returns a fullfilled promise with given result as a value.
        /// </summary>
        public static Promised<T> FromResult(T result)
        {
            Promised<T> promise = PromisePool<T>.Get();

            promise._value = result;

            return (promise);
        } 

        /// <summary>
        /// Returns a promise that is fullfilled if given task is completed.
        /// </summary>
        public static Promised<T> FromTask(Task<T> task)
        {
            Promised<T> promise = PromisePool<T>.Get();
            Extension extension = new Extension(promise);
            
            return (extension.WithTask(task));
        }
        
        /// <summary>
        /// Returns a promise that is fullfilled if given task is completed.
        /// </summary>
        public static Promised<T> FromTask<TResult>(Task<TResult> task, Func<TResult, T> selector)
        {
            Promised<T> promise = PromisePool<T>.Get();
            Extension extension = new Extension(promise);
            
            return (extension.WithTask(task, selector));
        }

        /// <summary>
        /// Returns a promise that is fullfilled if given webrequest is completed.
        /// </summary>
        public static Promised<T> FromWebRequest(UnityWebRequest request, Func<byte[], T> selector)
        {
            Promised<T> promise = PromisePool<T>.Get();

            WaitFor.WebRequest(request, promise, OnComplete);

            return (promise);

            void OnComplete(UnityWebRequest finishedRequest, object state)
            {
                Promised<T> promisedState = (Promised<T>)state;
                
                if (finishedRequest.result == UnityWebRequest.Result.Success)
                {
                    byte[] bytes = finishedRequest.downloadHandler.data;
                    promisedState.Value = selector(bytes);
                }
                else if ((HttpStatusCode)finishedRequest.responseCode == HttpStatusCode.NotModified)
                {
                    string message = ((long)HttpStatusCode.NotModified).ToString();
                    promisedState.Error = message;
                }
                else
                {
                    string message = finishedRequest.error ?? finishedRequest.downloadHandler.text;
                    promisedState.Error = message;
                }
            }
        }
        
        /// <summary>
        /// Returns a promise that is fullfilled if given webrequest operaration is completed.
        /// </summary>
        public static Promised<T> FromWebRequest(UnityWebRequestAsyncOperation operation, Func<byte[], T> selector)
        {
            Promised<T> promise = PromisePool<T>.Get();

            WaitFor.Operation(operation, promise, OnComplete);

            return (promise);

            void OnComplete(AsyncOperation finishedOperation, object state)
            {
                UnityWebRequest request = ((UnityWebRequestAsyncOperation)finishedOperation).webRequest;
                Promised<T> promisedState = (Promised<T>)state;
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] bytes = request.downloadHandler.data;
                    promisedState.Value = selector(bytes);
                }
                else if ((HttpStatusCode)request.responseCode == HttpStatusCode.NotModified)
                {
                    string message = ((long)HttpStatusCode.NotModified).ToString();
                    promisedState.Error = message;
                }
                else
                {
                    string message = request.error ?? request.downloadHandler.text;
                    promisedState.Error = message;
                }
            }
        }
        
        /// <summary>
        /// Returns a promise that is fullfilled if given instantiation operation is completed.
        /// </summary>
        public static Promised<T> FromInstantiation(AsyncInstantiateOperation<T> operation)
        {
            Promised<T> promise = PromisePool<T>.Get();

            WaitFor.Operation(operation, promise, OnComplete);
            
            return (promise);

            void OnComplete(AsyncOperation finishedOperation, object state)
            {
                T result = ((AsyncInstantiateOperation<T>)finishedOperation).Result[0];
                Promised<T> promisedState = (Promised<T>)state;

                if (result != null)
                {
                    promisedState.Value = result;
                }
                else
                {
                    string message = "Promised Instantiation failed. Result was null.";
                    promisedState.Error = message;
                }
            }
        }
        
        /// <summary>
        /// Returns a promise that is fullfilled if given scene load operation is completed.
        /// </summary>
        public static Promised<Scene> FromSceneLoad(AsyncOperation operation, Func<Scene> selector, Action<float> progress = null)
        {
            Promised<Scene> promise = PromisePool<Scene>.Get();
            
            WaitFor.Operation(operation, OnComplete, progress);

            return (promise);

            void OnComplete(AsyncOperation finishedOperation)
            {
                Scene result = selector();
                if (result.IsValid())
                {
                    promise.Value = result;
                }
                else
                {
                    string message = "Promised Scene Load failed. Result was invalid.";
                    promise.Error = message;
                }
            }
        }

        /// <summary>
        /// Returns a failed promise with given error message.
        /// </summary>
        public static Promised<T> FromError(string message)
        {
            Promised<T> promise = PromisePool<T>.Get();
            
            promise.Error = message;

            return (promise);
        }

        /// <summary>
        /// Resets this promise. Use if you are caching a reusing this promise value. If not, release the promise
        /// using the 'Release' function.
        /// </summary>
        public void Reset()
        {
            _value = Optional<T>.None();
            _valueReceived = null;
            _errorReceived = null;
            _error = null;
        }
        
        /// <summary>
        /// Releases this promise, returning it back to the pool.
        /// </summary>
        public void Release() => PromisePool<T>.Release(this);
    }
}