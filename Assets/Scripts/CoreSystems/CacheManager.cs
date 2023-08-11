using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Minimax.CoreSystems
{
    /// <summary>
    /// 어플리케이션이 실행될 동안 유지되는 캐시 오브젝트를 관리합니다.
    /// </summary>
    public class CacheManager : MonoBehaviour
    {
        // 싱글톤 패턴 구현
        public static CacheManager Instance { get; private set; }

        // 각 캐시 오브젝트를 관리하기 위한 딕셔너리
        [SerializeField, ReadOnly]
        private SerializedDictionary<string, CacheObject> cacheObjects = new SerializedDictionary<string, CacheObject>();

        /// <summary>
        /// 캐시 오브젝트를 등록하고 초기화합니다.
        /// </summary>
        /// <param name="key">캐시 오브젝트의 키</param>
        /// <param name="onLoadAction">캐시 오브젝트의 데이터 로드 액션</param>
        public void RegisterCacheObject(string key, Action onLoadAction)
        {
            if (!cacheObjects.TryGetValue(key, out CacheObject cacheObject))
            {
                cacheObject = new CacheObject();
                cacheObject.Initialize(onLoadAction);
                cacheObjects[key] = cacheObject;
            }
            else
            {
                Debug.LogWarning($"{key} cache object already exists.");
            }
        }
        
        /// <summary>
        /// 등록된 캐시 오브젝트를 제거합니다.
        /// </summary>
        /// <param name="key">제거할 캐시 오브젝트의 키</param>
        private void UnregisterCacheObject(string key)
        {
            if (cacheObjects.TryGetValue(key, out CacheObject cacheObject))
                cacheObjects.Remove(key);
            else
            {
                Debug.LogWarning($"cannot find {key} cache object.");
            }
        }

        /// <summary>
        /// 특정 캐시 오브젝트를 로드합니다.
        /// </summary>
        /// <param name="key">로드할 캐시 오브젝트의 키</param>
        public void LoadCacheObject(string key)
        {
            if (cacheObjects.TryGetValue(key, out CacheObject cacheObject))
            {
                cacheObject.Load();
            }
            else
            {
                Debug.LogWarning($"cannot find {key} cache object.");
            }
        }

        /// <summary>
        /// 모든 캐시 오브젝트를 로드합니다.
        /// </summary>
        public void LoadAllCacheObjects()
        {
            foreach (var cacheObject in cacheObjects.Values)
            {
                cacheObject.Load();
            }
        }
        
        /// <summary>
        /// 특정 캐시 오브젝트의 업데이트 필요 상태를 표시합니다.
        /// </summary>
        /// <param name="key">업데이트 필요 상태를 표시할 캐시 오브젝트의 키</param>
        public void SetNeedUpdate(string key)
        {
            if (cacheObjects.TryGetValue(key, out CacheObject cacheObject))
            {
                cacheObject.SetNeedUpdate();
            }
            else
            {
                Debug.LogWarning($"cannot find {key} cache object.");
            }
        }

        /// <summary>
        /// 특정 캐시 오브젝트의 상태를 초기화합니다.
        /// </summary>
        /// <param name="key">초기화할 캐시 오브젝트의 키</param>
        public void ResetCacheObject(string key)
        {
            if (cacheObjects.TryGetValue(key, out CacheObject cacheObject))
            {
                cacheObject.Reset();
            }
            else
            {
                Debug.LogWarning($"cannot find {key} cache object.");
            }
        }

        /// <summary>
        /// 모든 캐시 오브젝트의 상태를 초기화합니다.
        /// </summary>
        public void ResetAllCacheObjects()
        {
            foreach (var cacheObject in cacheObjects.Values)
            {
                cacheObject.Reset();
            }
        }
    }
}
