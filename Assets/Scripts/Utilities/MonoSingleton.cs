using System;
using UnityEngine;

namespace Minimax.Utilities
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        // 싱글턴 인스턴스
        private static T s_instance;

        // 멀티쓰레딩 환경에서 안정성을 위한 락 객체
        private static readonly object instanceLock = new object();

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    // 락을 걸어서 한 번에 하나의 쓰레드만 인스턴스를 생성하도록 한다.
                    lock (instanceLock)
                    {
                        // 락을 획득한 후에도 다시 한번 null 체크를 한다.
                        if (s_instance == null)
                        {
                            CreateInstance();
                        }
                    }
                }
                return s_instance;
            }
        }

        // 싱글턴 인스턴스 생성
        private static void CreateInstance()
        {
            if (s_instance != null) return;

            s_instance = FindExistingInstance() ?? CreateNewInstance();

            if (s_instance == null)
            {
                Debug.LogError("Failed to create an instance of " + typeof(T).Name);
            }
        }

        // 존재하는 인스턴스 찾기
        private static T FindExistingInstance()
        {
            T[] existingInstances = FindObjectsOfType<T>();

            // 둘 이상의 인스턴스가 존재할 경우 에러 로깅
            if (existingInstances.Length > 1)
            {
                Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                // 여기서 게임을 중단하거나 추가 조치를 취할 수 있습니다.
            }

            return existingInstances.Length > 0 ? existingInstances[0] : null;
        }

        // 새 인스턴스 생성
        private static T CreateNewInstance()
        {
            GameObject singletonObject = new GameObject { name = typeof(T).Name };
            return singletonObject.AddComponent<T>();
        }
        
        // 인스턴스가 초기화되었는지 확인
        public static bool IsInitialized => s_instance != null;
    }
}