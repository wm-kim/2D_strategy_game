using System.Diagnostics;

namespace WMK
{
    public static class DebugStatic 
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
