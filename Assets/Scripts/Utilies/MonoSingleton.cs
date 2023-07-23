using UnityEngine;

namespace Minimax
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if (objs.Length > 0)
                        s_instance = objs[0];
                    if (objs.Length > 1)
                    {
                        DebugWrapper.Instance.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if (s_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        s_instance = obj.AddComponent<T>();
                    }
                }
                return s_instance;
            }
        }
    }
}