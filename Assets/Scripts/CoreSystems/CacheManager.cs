using System;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Minimax.PropertyDrawer;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

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
        [SerializeField]
        [ReadOnly]
        private SerializedDictionary<string, CacheObject> cacheObjects = new();

        /// <summary>
        /// 캐시 오브젝트가 등록되어 있는지 여부를 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckHasKey(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "키는 null일 수 없습니다.");
            var hasKey = cacheObjects.ContainsKey(key);
            if (!hasKey) Debug.LogWarning($"캐시 오브젝트 키 {key}가 존재하지 않습니다.");
            return hasKey;
        }

        /// <summary>
        /// 동기 캐시 오브젝트를 등록합니다.
        /// </summary>
        /// <param name="key">캐시 오브젝트의 키</param>
        /// <param name="onLoadAction">캐시 오브젝트의 데이터 로드 액션</param>
        /// <param name="onLoadCompleted">캐시 오브젝트가 이미 로드되어 있는 경우 수행할 액션</param>
        public void Register(string key, Action<bool> onLoadAction, Action onLoadCompleted = null)
        {
            if (!cacheObjects.ContainsKey(key))
            {
                var cacheObject = new CacheObject(onLoadAction, onLoadCompleted);
                cacheObjects[key] = cacheObject;
            }
        }

        /// <summary>
        /// 비동기 캐시 오브젝트를 등록합니다.
        /// </summary>
        /// <param name="key">캐시 오브젝트의 키</param>
        /// <param name="onLoadAsyncAction">캐시 오브젝트의 데이터 비동기 로드 액션</param>
        /// <param name="onLoadCompleted">캐시 오브젝트가 이미 로드되어 있는 경우 수행할 액션</param>
        public void Register(string key, Func<bool, UniTask> onLoadAsyncAction, Action onLoadCompleted = null)
        {
            if (!cacheObjects.ContainsKey(key))
            {
                var cacheObject = new CacheObject(onLoadAsyncAction, onLoadCompleted);
                cacheObjects[key] = cacheObject;
            }
        }

        /// <summary>
        /// 등록된 캐시 오브젝트를 제거합니다.
        /// </summary>
        public void Unregister(string key)
        {
            if (CheckHasKey(key))
                cacheObjects.Remove(key);
        }

        /// <summary>
        /// 캐시 데이터를 로드하거나 로드가 완료된 경우 수행할 액션을 업데이트합니다.
        /// 보콩은 호출할 일이 없지만, multi-scene을 지원하기 위해 추가하였습니다.
        /// Action이 scene에 종속된 reference를 가지고 있을 경우, 해당 scene이 unload되면 Action이 무효화됩니다.
        /// 따라서 매번 scene이 load될 때마다 Action을 업데이트해야 합니다.
        /// </summary>
        public void UpdateLoadAction(string key, Action<bool> onLoadAction)
        {
            if (!CheckHasKey(key)) return;

            if (cacheObjects[key].IsLoaded)
                cacheObjects[key].UpdateLoadAction(onLoadAction);
        }

        public void UpdateLoadAction(string key, Func<bool, UniTask> onLoadAsyncAction)
        {
            if (!CheckHasKey(key)) return;

            if (cacheObjects[key].IsLoaded)
                cacheObjects[key].UpdateLoadAction(onLoadAsyncAction);
        }

        public void UpdateLoadCompletedAction(string key, Action onLoadCompleted)
        {
            if (!CheckHasKey(key)) return;

            if (cacheObjects[key].IsLoaded)
                cacheObjects[key].UpdateLoadCompletedAction(onLoadCompleted);
        }

        /// <summary>
        /// 특정 캐시 오브젝트를 로드합니다.
        /// </summary>
        public void RequestLoad(string key)
        {
            Debug.Log($"RequestLoad {key}");
            if (CheckHasKey(key))
            {
                if (cacheObjects[key].IsAsync) cacheObjects[key].RequestLoadAsync();
                else cacheObjects[key].RequestLoad();
            }
        }

        /// <summary>
        /// 특정 캐시 오브젝트의 업데이트 필요 상태를 표시합니다.
        /// </summary>
        public void SetNeedUpdate(string key)
        {
            if (CheckHasKey(key)) cacheObjects[key].SetNeedUpdate();
        }

        /// <summary>
        /// 특정 캐시 오브젝트의 상태를 초기화합니다.
        /// </summary>
        public void Reset(string key)
        {
            if (CheckHasKey(key)) cacheObjects[key].Reset();
        }

        /// <summary>
        /// 모든 캐시 오브젝트의 상태를 초기화합니다.
        /// </summary>
        public void ResetAll()
        {
            foreach (var cacheObject in cacheObjects.Values) cacheObject.Reset();
        }
    }
}