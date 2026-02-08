using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;

namespace OpenUtility.Samples.Data
{
   public class ConnectionMenuManager : MonoBehaviour
   {
      [Header("Project References")]
      [SerializeField]
       private ScriptableGameObject _connection;

      [SerializeField]
      private UnityEvent _connectionSuccess;

      [SerializeField]
      private UnityEvent _connectionFailure;

      private void OnEnable()
      {
         var service = _connection.GetComponent<ConnectionService>();
         service.ConnectionChanged += OnConnectionChanged;
      }

      private void OnDisable()
      {
         if (!_connection.TryGetComponent(out ConnectionService service))
            return;
         
         service.ConnectionChanged -= OnConnectionChanged;
      }

      public void CheckConnection()
      {
         Debug.Log("Checking connection...");

         var service = _connection.GetComponent<ConnectionService>();
         Execute.AfterSeconds(service.CheckConnection, 0.75f);
      }

      private void OnConnectionChanged(bool result)
      {
         if (result)
         {
            _connectionSuccess?.Invoke();
         }
         else
         {
            _connectionFailure?.Invoke();
         }
      }
   }

}