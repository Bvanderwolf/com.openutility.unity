using System;
using System.Collections;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using OpenUtility.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using EasingFunction = OpenUtility.Data.EasingFunction;

namespace OpenUtility.DelayedExecution
{
    public class ActionAwaiter : MonoBehaviour
    {
        public YieldInstruction WaitForLocalization(LocalizedString localizedString, Action<string> callback = null)
        {
            ThrowIf.Null(localizedString);
            
            return (StartCoroutine(RunLocalization()));
            
            IEnumerator RunLocalization()
            {
                AsyncOperationHandle<string> operation = localizedString.GetLocalizedStringAsync();
                
                if (!operation.IsDone)
                    yield return operation; 
                
                callback?.Invoke(operation.Result);
            }
        }

        public YieldInstruction WaitForWebRequest(UnityWebRequest request, Action<UnityWebRequest> callback = null)
        {
            ThrowIf.Null(request);
            
            return (StartCoroutine(SendWebRequest()));
            
            IEnumerator SendWebRequest()
            {
                yield return request.SendWebRequest();
                
                callback?.Invoke(request);
            }
        }

        public YieldInstruction WaitForOperation(AsyncOperation operation, Action<AsyncOperation> callback = null)
        {
            ThrowIf.Null(operation);
            
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                yield return operation;
                
                callback?.Invoke(operation);
            }
        }

        public YieldInstruction WaitForOperation<T>(AsyncOperationBase<T> operation, Action<AsyncOperationBase<T>> callback = null)
        {
            ThrowIf.Null(operation);
            
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                yield return operation;
                
                callback?.Invoke(operation);
            }
        }

        public YieldInstruction WaitForOperation(AsyncOperationHandle operation, Action<RequestResult> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operation);

            operation.Completed += OnCompletion;
            
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                while (!operation.IsDone)
                {
                    progress?.Invoke(operation.GetDownloadStatus());
                    yield return null;
                }
            }

            void OnCompletion(AsyncOperationHandle handle)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(RequestResult.CreateSuccess());
                }
                else
                {
                    callback?.Invoke(RequestResult.CreateError(handle.OperationException.Message));
                }
            }
        }

        public YieldInstruction WaitForOperation<T>(AsyncOperationHandle<T> operation, Action<DataRequestResult<T>> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operation);

            operation.Completed += OnCompletion;
            
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                while (!operation.IsDone)
                {
                    progress?.Invoke(operation.GetDownloadStatus());
                    yield return null;
                }
            }

            void OnCompletion(AsyncOperationHandle<T> handle)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(DataRequestResult<T>.CreateSuccess(handle.Result));
                }
                else
                {
                    callback?.Invoke(DataRequestResult<T>.CreateError(handle.OperationException.Message));
                }
            }
        }

        public YieldInstruction WaitForOperations<T>(AsyncOperationHandle<T>[] operations, Action<DataRequestResult<T[]>> callback = null, Action<DownloadStatus> progress = null)
        {
            ThrowIf.Null(operations);

            return (StartCoroutine(RunOperations()));
            
            IEnumerator RunOperations()
            {
                while (true)
                {
                    bool isDone = true;
                    long totalBytes = 0;
                    long downloadedBytes = 0;

                    for (int i = 0; i < operations.Length; i++)
                    {
                        var operation = operations[i];
                        if (!operation.IsDone)
                            isDone = false;

                        var status = operation.GetDownloadStatus();
                        totalBytes += status.TotalBytes;
                        downloadedBytes += status.DownloadedBytes;
                    }

                    if (totalBytes > 0)
                        progress?.Invoke(new DownloadStatus { TotalBytes = totalBytes, DownloadedBytes = downloadedBytes });

                    if (isDone)
                        break;

                    yield return null;
                }
                
                bool success = true;
                T[] results = new T[operations.Length];
                for (int i = 0; i < operations.Length; i++)
                {
                    var operation = operations[i];
                    if (operation.Status == AsyncOperationStatus.Succeeded)
                    {
                        results[i] = operation.Result;
                    }
                    else
                    {
                        success = false;
                    }
                    
                }
                
                var result = success
                    ? DataRequestResult<T[]>.CreateSuccess(results)
                    : DataRequestResult<T[]>.CreateError(results, "One or more operations failed.");

                callback?.Invoke(result);
            }
        }

        public YieldInstruction WaitForScroll(ScrollRect scrollView, ScrollOptions options = default, Action callback = null)
        {
            ThrowIf.Null(scrollView);

            return (StartCoroutine(RunScroll()));
            
            IEnumerator RunScroll()
            {
                // 1. Fallback naar noEase als er niets is ingevuld
                EasingFunction easingFunction = options.easingFunction ?? EasingFunctions.noEase;
    
                Vector2 start = scrollView.normalizedPosition;
                Vector2 end = options.position;
    
                // Gebruik een multiplier voor snelheid in plaats van een harde '1 seconde'
                float speed = options.speed <= 0f ? 1f : options.speed;
                float progress = 0f;

                while (progress < 1f)
                {
                    // Verhoog progressie op basis van tijd en snelheid
                    progress += Time.unscaledDeltaTime * speed;
        
                    // Zorg dat we nooit boven de 1.0 uitkomen voor de easing functie
                    float clampedProgress = Mathf.Min(progress, 1f);
        
                    // Pas de easing toe
                    float easedTime = easingFunction(clampedProgress);
        
                    // Pas de positie aan
                    scrollView.normalizedPosition = Vector2.LerpUnclamped(start, end, easedTime);

                    yield return null;
                }

                // Voor de zekerheid de exacte eindpositie zetten
                scrollView.normalizedPosition = end;
            }
        }

        public YieldInstruction WaitForConnection(UnityWebRequest request, Action<RequestResult> callback)
        {
            return (StartCoroutine(RunConnectionCheck()));

            IEnumerator RunConnectionCheck()
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(RequestResult.CreateSuccess());
                }
                else
                {
                    string message = string.IsNullOrEmpty(request.error) ? request.downloadHandler.text : request.error;
                    callback?.Invoke(RequestResult.CreateError(message));
                }
                
                request.Dispose();
            }
        }
    }
}
