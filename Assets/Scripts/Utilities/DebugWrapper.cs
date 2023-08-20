using System;
using System.Diagnostics;
using Minimax.CoreSystems;
using Unity.Netcode;

namespace Minimax.Utilities
{
    public static class DebugWrapper 
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
    }
}
