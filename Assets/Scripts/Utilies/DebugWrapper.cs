using System;
using System.Diagnostics;
using Unity.Netcode;

namespace Minimax
{
    public class DebugWrapper : NetworkBehaviour
    {
        public static DebugWrapper Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        [Conditional("UNITY_EDITOR")]
        public void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public void LogException(System.Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
        
        [Conditional("UNITY_EDITOR")]
        [ClientRpc]
        public void LogClientRpc(string message)
        {
            UnityEngine.Debug.Log($"[ClientRpc] {message}");
        }
    }
}
