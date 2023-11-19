using System;
using System.Diagnostics;
using UnityEngine;

namespace Utilities
{
    public enum LogLevel
    {
        Debug,
        Normal
    }

    /// <summary>
    /// It overrides UnityEngine.Debug to mute debug messages completely on a platform-specific basis.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// 현재 로그 레벨. 이 레벨보다 낮은 메시지는 출력되지 않습니다.
        /// </summary>
        private static LogLevel m_currentLogLevel = LogLevel.Normal;

        public static bool IsDebugBuild => UnityEngine.Debug.isDebugBuild;

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEDICATED_SERVER_BUILD")]
        public static void Log(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.Log($"[Log] {message}");
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogWarning(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogWarning($"[Warning] {message}");
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogError(string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogError($"[Error] {message}");
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEDICATED_SERVER_BUILD")]
        public static void LogException(Exception exception, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return;
            UnityEngine.Debug.LogException(exception);
        }

        public static bool CheckIfTrueLog(bool condition, string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return false;
            if (!condition) Log(message, logLevel);
            return condition;
        }

        public static bool CheckIfTrueLogWarning(bool condition, string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return false;
            if (!condition) LogWarning(message, logLevel);
            return condition;
        }

        public static bool CheckIfTrueLogError(bool condition, string message, LogLevel logLevel = LogLevel.Normal)
        {
            if (m_currentLogLevel > logLevel) return false;
            if (!condition) LogError(message, logLevel);
            return condition;
        }

        public static TextMesh CreateText(string text, Transform parent = null, Vector3 localPosition = default,
            float scale = 1f, string sortingLayerName = "Default")
        {
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(parent);
            textObject.transform.localPosition                       = localPosition;
            textObject.transform.localScale                          = Vector3.one * scale;
            textObject.AddComponent<MeshRenderer>().sortingLayerName = sortingLayerName;
            var textComponent = textObject.AddComponent<TextMesh>();
            textComponent.alignment = TextAlignment.Center;
            textComponent.anchor    = TextAnchor.MiddleCenter;
            textComponent.text      = text;
            return textComponent;
        }
    }
}