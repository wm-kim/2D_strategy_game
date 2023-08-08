using System;
using UnityEngine;

namespace Minimax.Utilities
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static readonly object instanceLock = new object();
        private static T s_instance;

        public static T Instance
        {
            get
            {
                if (s_instance == null && Time.timeScale != 0)
                {
                    CreateInstance();
                }
                return s_instance;
            }
        }

        private static void CreateInstance()
        {
            lock (instanceLock)
            {
                if (s_instance != null) return;

                s_instance = FindExistingInstance() ?? CreateNewInstance();

                if (s_instance == null)
                {
                    Debug.LogError("Failed to create an instance of " + typeof(T).Name);
                }
            }
        }

        private static T FindExistingInstance()
        {
            T[] existingInstances = FindObjectsOfType<T>();

            if (existingInstances.Length > 1)
            {
                Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
            }

            return existingInstances.Length > 0 ? existingInstances[0] : null;
        }

        private static T CreateNewInstance()
        {
            GameObject singletonObject = new GameObject { name = typeof(T).Name };
            return singletonObject.AddComponent<T>();
        }
        
        public static bool IsInitialized => s_instance != null;

        private void OnApplicationQuit()
        {
            Time.timeScale = 0;
        }
    }
}