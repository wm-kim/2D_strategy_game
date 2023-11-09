using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.ScriptableObjects
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
                    var assets = Resources.LoadAll<T>("");
                    if (assets == null || assets.Length < 1)
                        throw new System.Exception(
                            "SingletonSO -> instance -> assets is null or length is less than 1 for type " +
                            typeof(T).ToString() + ".");
                    else if (assets.Length > 1)
                        Debug.LogWarning("SingletonSO -> instance -> assets length is greater than 1 for type " +
                                         typeof(T).ToString() + ".");
                    m_instance = assets[0];
                }

                return m_instance;
            }
        }
    }
}