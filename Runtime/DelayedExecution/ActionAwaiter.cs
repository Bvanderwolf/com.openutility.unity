using System;
using System.Collections;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Exceptions;
using OpenUtility.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
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
            return (StartCoroutine(SendWebRequest()));
            
            IEnumerator SendWebRequest()
            {
                yield return request.SendWebRequest();
                
                callback?.Invoke(request);
            }
        }

        public YieldInstruction WaitForOperation(AsyncOperation operation, Action<AsyncOperation> callback = null, Action<float> progress = null)
        { 
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                do
                {
                    progress?.Invoke(operation.progress);
                    
                    yield return null;
                    
                } while (!operation.isDone);
                
                callback?.Invoke(operation);
            }
        }

        public YieldInstruction WaitForOperation<T>(AsyncOperationBase<T> operation, Action<AsyncOperationBase<T>> callback = null)
        { 
            return (StartCoroutine(RunOperation()));
            
            IEnumerator RunOperation()
            {
                yield return operation;
                
                callback?.Invoke(operation);
            }
        }

        public YieldInstruction WaitForOperation(AsyncOperationHandle operation, Action<RequestResult> callback = null, Action<DownloadStatus> progress = null)
        {
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
            return (StartCoroutine(RunScroll()));
            
            IEnumerator RunScroll()
            {
                EasingFunction easingFunction = options.easingFunction ?? EasingFunctions.noEase;
    
                Vector2 start = scrollView.normalizedPosition;
                Vector2 end = options.position;
    
                float speed = options.speed <= 0f ? 1f : options.speed;
                float progress = 0f;

                while (progress < 1f)
                {
                    progress += Time.unscaledDeltaTime * speed;
        
                    float clampedProgress = Mathf.Min(progress, 1f);
        
                    float easedTime = easingFunction(clampedProgress);
        
                    scrollView.normalizedPosition = Vector2.LerpUnclamped(start, end, easedTime);

                    yield return null;
                }

                scrollView.normalizedPosition = end;
            }
        }

        public YieldInstruction WaitForFocus(TMP_InputField inputField, Action callback)
        {
            return StartCoroutine(RunWaitForFocus());

            IEnumerator RunWaitForFocus()
            {
                if (!inputField.gameObject.activeInHierarchy)
                    yield break;

                if (EventSystem.current == null)
                    yield break;

                EventSystem.current.SetSelectedGameObject(inputField.gameObject);

                // wait 1 frame (important for reliable focus across platforms)
                yield return null;

                inputField.ActivateInputField();
                inputField.MoveTextEnd(false);
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

        public YieldInstruction WaitForCondition<T>(T target, Func<T,bool> predicate, Action callback) where T : class
        {
            return (StartCoroutine(RunWaitForCondition()));
            
            IEnumerator RunWaitForCondition()
            {
                while (!predicate(target))
                {
                    yield return null;
                    
                    if (target is UnityEngine.Object unityObject && unityObject == null)
                        yield break;
                }
                
                callback?.Invoke();
            }
        }
    }
}
