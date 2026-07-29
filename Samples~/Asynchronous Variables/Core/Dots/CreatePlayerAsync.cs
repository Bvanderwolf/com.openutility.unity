using System.Collections;
using OpenUtility.Data;
using OpenUtility.Samples.Data;
using UnityEngine;

public class CreatePlayerAsync : MonoBehaviour
{
    [SerializeField]
    private ScriptableGameObject _player;

    [SerializeField]
    private bool _useAddressableApi = false;
    
    private IEnumerator Start()
    {
        Promised<GameObject> promise;
        if (_useAddressableApi)
        {
            promise = _player.CreateAddressableValueAsync();
        }
        else
        {
            promise = _player.CreateValueAsync();
        }
        
        yield return promise.Yield();

        if (!promise.HasError)
        {
            GameObject player = promise.Value;
            player.transform.position = Vector3.up;
        }
        
        promise.Release();
    }
}
