using System;
using System.Net;
using System.Threading.Tasks;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenUtility.Samples.Data
{
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
                        promisedState.OnError(message);
                    }
                }
            }
            
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
                            promisedState.OnError(message);
                        }
                    }
                    else
                    {
                        string message = completedTask.Exception?.ToString();
                        promisedState.OnError(message);
                    }
                }
            }
        }
        
        public class YieldInstruction : CustomYieldInstruction
        {
            public override bool keepWaiting => !_promise.HasValue;

            private readonly Promised<T> _promise;
            
            public YieldInstruction(Promised<T> promise)
            {
                _promise = promise;
            }
        }
        
        public class ValueReceivedEvent : UnityEvent<T> {}
        
        [SerializeField]
        private ValueReceivedEvent _valueReceived;
        
        private Action<Promised<T>> _errorReceived;

        public bool HasValue => _value.HasValue;
        public bool HasError => Error != null;
        public string Error { get; private set; }

        public T Value
        {
            get => _value.GetValueOrDefault();
            set
            {
                bool previouslyHadValue = _value.HasValue;

                _value = value;
                
                if (!previouslyHadValue)
                    _valueReceived?.Invoke(value);
            }
        }
        
        public YieldInstruction Yield() => new YieldInstruction(this);
        
        public Promised<T> Then(UnityAction<T> callback)
        {
            (_valueReceived ??= new ValueReceivedEvent()).AddListener(callback);
            return (this);
        }

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
                    OnError(message);
                }
            }

            return (this);
        }
        
        public Promised<T> Catch(Action<Promised<T>> onErrorReceived)
        {
            _errorReceived += onErrorReceived;
            return (this);
        }
        
        public Extension Extend(params UnityAction<T>[] callbacksToRemove) => new Extension(this);
        
        private Optional<T> _value;

        private void OnError(string message)
        {
            Error = message;
            _errorReceived?.Invoke(this);
        }
        
        public override string ToString() => _value.HasValue ? _value.ToString() : "(Pending)";

        public static implicit operator T(Promised<T> promise) => promise.Value;

        public static Promised<T> FromResult(T result) => (new Promised<T>
        {
            _value = result
        });

        public static Promised<T> FromTask(Task<T> task)
        {
            Promised<T> promise = new Promised<T>();
            Extension extension = new Extension(promise);
            
            return (extension.WithTask(task));
        }
        
        public static Promised<T> FromTask<TResult>(Task<TResult> task, Func<TResult, T> selector)
        {
            Promised<T> promise = new Promised<T>();
            Extension extension = new Extension(promise);
            
            return (extension.WithTask(task, selector));
        }

        public static Promised<T> FromWebRequest(UnityWebRequest request, Func<byte[], T> selector)
        {
            Promised<T> promise = new Promised<T>();

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
                    promisedState.OnError(message);
                }
                else
                {
                    string message = finishedRequest.error ?? finishedRequest.downloadHandler.text;
                    promisedState.OnError(message);
                }
            }
        }
        
        public static Promised<T> FromWebRequest(UnityWebRequestAsyncOperation operation, Func<byte[], T> selector)
        {
            Promised<T> promise = new Promised<T>();

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
                    promisedState.OnError(message);
                }
                else
                {
                    string message = request.error ?? request.downloadHandler.text;
                    promisedState.OnError(message);
                }
            }
        }
    }
}