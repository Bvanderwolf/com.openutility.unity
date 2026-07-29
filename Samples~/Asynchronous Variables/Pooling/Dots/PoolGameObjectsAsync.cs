using System.Linq;
using OpenUtility.Data.Pooling;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoolGameObjectsAsync : MonoBehaviour
{
    [SerializeField]
    private AsyncScriptablePool _pool;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            _pool.Get().Then(OnRetrieved);

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            _pool.References.GetValue().FirstOrDefault()?.TryRelease();
    }

    private void OnRetrieved(PoolAsyncGameObject component)
    {
        component.transform.position = Random.insideUnitCircle * 2f;
    }
}
