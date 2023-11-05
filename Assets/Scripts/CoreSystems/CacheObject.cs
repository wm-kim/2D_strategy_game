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
        [SerializeField] [Tooltip("캐시 데이터가 현재 로드되어 있는지 여부")]
        private bool m_isLoaded = false;

        // 캐시 데이터 업데이트 필요 여부를 나타냅니다.
        [SerializeField] [Tooltip("캐시 데이터를 업데이트해야 하는지 여부")]
        private bool m_needUpdate = false;

        /// <summary>
        /// 동기 캐시 데이터를 로드하는 경우 수행할 액션입니다.
        /// 처음 로드되는 경우 true, 업데이트가 필요하여 로드되는 경우 false를 인자로 받습니다.
        /// </summary>
        private Action<bool> m_onLoad = null;

        /// <summary>
        /// 비동기 캐시 데이터를 로드하는 경우 수행할 액션입니다.
        /// 처음 로드되는 경우 true, 업데이트가 필요하여 로드되는 경우 false를 인자로 받습니다.
        /// </summary>
        private Func<bool, UniTask> m_onLoadAsync = null;

        /// <summary>
        /// 캐시 데이터 로드가 완료된 경우 수행할 액션입니다.
        /// </summary>
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
        private CacheObject()
        {
        }

        /// <summary>
        /// 캐시 데이터를 로드하거나 로드가 완료된 경우 수행할 액션을 설정합니다.
        /// </summary>
        public CacheObject(Action<bool> onLoadAction, Action onLoadCompleted = null)
        {
            m_onLoad = onLoadAction ?? throw new ArgumentNullException(nameof(onLoadAction), "로드 액션은 null일 수 없습니다.");
            m_onLoadCompleted = onLoadCompleted;
        }

        /// <summary>
        /// 캐시 데이터를 로드하거나 로드가 완료된 경우 수행할 액션을 설정합니다.
        /// </summary>
        public CacheObject(Func<bool, UniTask> onLoadAsyncAction, Action onLoadCompleted = null)
        {
            m_onLoadAsync = onLoadAsyncAction ??
                            throw new ArgumentNullException(nameof(onLoadAsyncAction), "로드 액션은 null일 수 없습니다.");
            m_onLoadCompleted = onLoadCompleted;
        }

        /// <summary>
        /// 캐시 데이터를 업데이트해야 하는지 여부를 설정합니다.
        /// </summary>
        public void SetNeedUpdate()
        {
            // 처음 로드되지 않은 경우 업데이트가 필요하지 않으므로 예외를 발생시킵니다.
            if (!m_isLoaded) throw new InvalidOperationException("아직 캐시 데이터가 로드되지 않았으므로 업데이트가 필요하지 않습니다.");
            m_needUpdate = true;
        }

        /// <summary>
        /// 캐시 데이터를 로드합니다.
        /// </summary>
        public void RequestLoad()
        {
            Assert.IsTrue(!IsAsync, "비동기 로드 액션을 사용하는 경우 RequestLoadAsync()를 사용해야 합니다.");

            if (!m_isLoaded || m_needUpdate)
            {
                m_onLoadCompleted?.Invoke();

                m_isLoaded   = true;
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

                await m_onLoadAsync.Invoke(m_needUpdate);
                m_onLoadCompleted?.Invoke();

                m_isLoaded   = true;
                m_needUpdate = false;
            }
            else
            {
                m_onLoadCompleted?.Invoke();
            }
        }

        public void UpdateLoadAction(Action<bool> onLoadAction)
        {
            m_onLoad = onLoadAction ?? throw new ArgumentNullException(nameof(onLoadAction), "로드 액션은 null일 수 없습니다.");
        }

        public void UpdateLoadAction(Func<bool, UniTask> onLoadAsyncAction)
        {
            m_onLoadAsync = onLoadAsyncAction ??
                            throw new ArgumentNullException(nameof(onLoadAsyncAction), "로드 액션은 null일 수 없습니다.");
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
            m_isLoaded   = false;
            m_needUpdate = false;
        }
    }
}