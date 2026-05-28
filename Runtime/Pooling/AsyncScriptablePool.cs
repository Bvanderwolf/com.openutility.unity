using System;
using OpenUtility.DelayedExecution;
using UnityEngine;

namespace OpenUtility.Data.Pooling
{
    [CreateAssetMenu(fileName = "AsyncGameObjectPool", menuName = "OpenUtility/Pooling/Async GameObject Pool", order = 1)]
    public class AsyncScriptablePool : AsyncScriptablePoolBase<PoolAsyncGameObject>
    {
        [Header("Project References")]
        [SerializeField]
        private GameObject _prefab;
        
        [SerializeField, Tooltip("An optional list variable for storing references to active instances.")]
        private Optional<PoolAsyncGameObjectList> _references;

        public PoolAsyncGameObjectList References => _references.GetValueOrDefault();
        
        public event Action<PoolAsyncGameObject> InstanceRetrieved;
        
        protected override Promised<PoolAsyncGameObject> OnCreatePromise()
        {
            AsyncInstantiateOperation<GameObject> operation = parent.HasValue 
                ? InstantiateAsync(_prefab, parent.Value) 
                : InstantiateAsync(_prefab);

            Promised<PoolAsyncGameObject> promise = new Promised<PoolAsyncGameObject>();

            WaitFor.Operation(operation, promise, OnComplete);
            
            return (promise);

            void OnComplete(AsyncOperation finishedOperation, object state)
            {
                GameObject gameObject = ((AsyncInstantiateOperation<GameObject>)finishedOperation).Result[0];
                Promised<PoolAsyncGameObject> promisedState = (Promised<PoolAsyncGameObject>)state;

                if (gameObject != null)
                {
                    if (!gameObject.TryGetComponent(out PoolAsyncGameObject instance))
                    {
                        Debug.Log($"[{name}] Could not find the {nameof(PoolAsyncGameObject)} component on prefab '{_prefab.name}'. It is best practice to add your pooling component beforehand to set serialized fields. Adding it manually now...");

                        instance = gameObject.AddComponent<PoolAsyncGameObject>();
                    }
                    
                    instance.OnCreatedByPromise(promisedState);

                    promisedState.Value = instance;
                }
                else
                {
                    string message = "Promised Instantiation failed. Result was null.";
                    promisedState.Error = message;
                }
            }
        }

        protected override void OnGetPromise(Promised<PoolAsyncGameObject> promise)
        {
            if (promise.HasValue)
            {
                Execute.NextFrame(CompletePromise, promise);
            }
            else
            {
                promise
                    .Then(instance => instance.gameObject.SetActive(true))
                    .Then(AddReferenceIfPossible)
                    .Then(instance => InstanceRetrieved?.Invoke(instance));
            }

            void CompletePromise(Promised<PoolAsyncGameObject> state)
            {
                PoolAsyncGameObject instance = state.Value;
                
                instance.gameObject.SetActive(true);
                
                state.Reset();
                state.Value = instance;
                
                AddReferenceIfPossible(instance);
                
                InstanceRetrieved?.Invoke(instance);
            }
        }

        protected override void OnReleasePromise(Promised<PoolAsyncGameObject> promise)
        {
            if (promise.HasValue)
            {
                PoolAsyncGameObject instance = promise.Value;
                
                instance.gameObject.SetActive(false);

                RemoveReferenceIfPossible(instance);
            }
            else
            {
                promise
                    .Then(instance => instance.gameObject.SetActive(false))
                    .Then(RemoveReferenceIfPossible);
            }
        }

        private void AddReferenceIfPossible(PoolAsyncGameObject instance)
        {
            if (_references.HasValue)
                _references.Value.Add(instance);
        }

        private void RemoveReferenceIfPossible(PoolAsyncGameObject instance)
        {
            if (_references.HasValue)
                _references.Value.Remove(instance);
        }
    }
}
