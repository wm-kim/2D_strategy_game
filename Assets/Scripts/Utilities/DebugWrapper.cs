using System;
using Unity.Netcode;

namespace Minimax.Utilities
{
    public class DebugWrapper : NetworkBehaviour
    {
        public static DebugWrapper Instance { get; private set; }

        private void Awake() => Instance = this;
        
        // [Conditional("UNITY_EDITOR")]
        public void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        // [Conditional("UNITY_EDITOR")]
        public void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        
        // [Conditional("UNITY_EDITOR")]
        public void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        // [Conditional("UNITY_EDITOR")]
        public void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
        
        [ClientRpc]
        public void LogClientRpc(string log, ClientRpcParams clientRpcParams = default) => Log(log);
    }
}
