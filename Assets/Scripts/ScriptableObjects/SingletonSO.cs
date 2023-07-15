using UnityEngine;

namespace WMK
{
    public class SingletonSO<T> : ScriptableObject where T : ScriptableObject
    {
        private static T m_instance = null;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception("SingletonSO -> instance -> assets is null or length is less than 1 for type " + typeof(T).ToString() + ".");
                    }
                    else if (assets.Length > 1)
                    {
                        DebugStatic.LogWarning("SingletonSO -> instance -> assets length is greater than 1 for type " + typeof(T).ToString() + ".");
                    }
                    m_instance = assets[0];
                }
                return m_instance;
            }
        }
    }
}
