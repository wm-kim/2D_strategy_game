using System;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Minimax.CoreSystems
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheObject
    {
        // 캐시 데이터의 로드 상태를 나타냅니다.
        [SerializeField, Tooltip("캐시 데이터가 현재 로드되어 있는지 여부")]
        private bool m_isLoaded = false;

        // 캐시 데이터 업데이트 필요 여부를 나타냅니다.
        [SerializeField, Tooltip("캐시 데이터를 업데이트해야 하는지 여부")]
        private bool m_needUpdate = false;

        private Action m_onLoad = null;
        private Func<UniTask> m_onLoadAsync = null;
        private Action m_onLoadCompleted = null;

        /// <summary>
        /// 캐시 데이터가 로드되어 있는지 여부를 반환합니다.
        /// </summary>
        public bool IsLoaded => m_isLoaded;
        
        /// <summary>
        /// 동기 캐시 캐시 데이터인지 혹은 비동기 캐시 데이터인지 여부를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public bool IsAsync => m_onLoadAsync != null;

        // 아무 작업도 수행하지 않는 기본 생성자를 private으로 설정하여 외부에서 호출할 수 없도록 합니다.
        private CacheObject() { } 
        
        /// <summary>
        /// 캐시 데이터를 로드하거나 로드가 완료된 경우 수행할 액션을 설정합니다.
        /// </summary>
        public CacheObject(Action onLoadAction, Action onLoadCompleted = null)
        {
            m_onLoad = onLoadAction ?? throw new ArgumentNullException(nameof(onLoadAction), "로드 액션은 null일 수 없습니다.");
            m_onLoadCompleted = onLoadCompleted;
        }
        
        /// <summary>
        /// 캐시 데이터를 로드하거나 로드가 완료된 경우 수행할 액션을 설정합니다.
        /// </summary>
        public CacheObject(Func<UniTask> onLoadAsyncAction, Action onLoadCompleted = null)
        {
            m_onLoadAsync = onLoadAsyncAction ?? throw new ArgumentNullException(nameof(onLoadAsyncAction), "로드 액션은 null일 수 없습니다.");
            m_onLoadCompleted = onLoadCompleted;
        }

        /// <summary>
        /// 캐시 데이터를 업데이트해야 하는지 여부를 설정합니다.
        /// </summary>
        public void SetNeedUpdate() => m_needUpdate = true;

        /// <summary>
        /// 캐시 데이터를 로드합니다.
        /// </summary>
        public void RequestLoad()
        {
            Assert.IsTrue(!IsAsync, "비동기 로드 액션을 사용하는 경우 RequestLoadAsync()를 사용해야 합니다.");
            
            if (!m_isLoaded || m_needUpdate)
            {
                m_onLoad?.Invoke();
                m_onLoadCompleted?.Invoke();
            
                m_isLoaded = true;
                m_needUpdate = false;
            }
            else
            {
                m_onLoadCompleted?.Invoke();
            }
        }
        
        /// <summary>
        /// 캐시 데이터를 비동기 로드합니다.
        /// </summary>
        public async UniTask RequestLoadAsync()
        {
            if (!m_isLoaded || m_needUpdate)
            {
                Assert.IsTrue(IsAsync, "동기 로드 액션을 사용하는 경우 RequestLoad()를 사용해야 합니다.");
              
                await m_onLoadAsync.Invoke();
                m_onLoadCompleted?.Invoke();
                
                m_isLoaded = true;
                m_needUpdate = false;
            }
            else
            {
                m_onLoadCompleted?.Invoke();
            }
        }
        
        public void UpdateLoadAction(Action onLoadAction)
        {
            m_onLoad = onLoadAction ?? throw new ArgumentNullException(nameof(onLoadAction), "로드 액션은 null일 수 없습니다.");
        }
        
        public void UpdateLoadAction(Func<UniTask> onLoadAsyncAction)
        {
            m_onLoadAsync = onLoadAsyncAction ?? throw new ArgumentNullException(nameof(onLoadAsyncAction), "로드 액션은 null일 수 없습니다.");
        }
        
        public void UpdateLoadCompletedAction(Action onLoadCompletedAction)
        {
            m_onLoadCompleted = onLoadCompletedAction;
        }

        /// <summary>
        /// 캐시 데이터의 상태를 초기화합니다.
        /// </summary>
        public void Reset()
        {
            m_isLoaded = false;
            m_needUpdate = false;
        }
    }
}
