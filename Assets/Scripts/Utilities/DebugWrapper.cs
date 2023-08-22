using System;
using System.Diagnostics;
using Minimax.CoreSystems;
using Unity.Netcode;

namespace Minimax.Utilities
{
    public enum LogLevel
    {
        Debug,
        Normal,
    }
    
    public static class DebugWrapper 
    {
        /// <summary>
        /// 현재 로그 레벨. 이 레벨보다 낮은 메시지는 출력되지 않습니다.
        /// </summary>
        private static LogLevel m_currentLogLevel = LogLevel.Normal;
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void Log(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.Log($"[Log] {message}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogWarning(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogWarning($"[Warning] {message}");
        }
       
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogError(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogError($"[Error] {message}");
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD"), Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogException(Exception exception, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogException(exception);
        }
    }
}
