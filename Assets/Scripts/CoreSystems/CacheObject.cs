using System;
using UnityEngine;
using UnityEngine.Serialization;

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

        // 실제 데이터 로드를 수행하는 액션. 외부에서 초기화할 수 있도록 NonSerialized로 설정.
        private Action m_onLoad = null;

        // 로드 상태 접근자
        public bool IsLoaded => m_isLoaded;

        // 업데이트 필요 상태 접근자
        public bool NeedUpdate => m_needUpdate;

        // 초기화 메서드. 데이터 로드 액션을 설정합니다.
        public void Initialize(Action onLoadAction)
        {
            m_onLoad = onLoadAction ?? throw new ArgumentNullException(nameof(onLoadAction), "로드 액션은 null일 수 없습니다.");
        }

        // 캐시 데이터 업데이트를 표시합니다.
        public void SetNeedUpdate() => m_needUpdate = true;

        // 캐시 데이터를 로드합니다.
        // 이미 로드된 데이터나 업데이트가 필요하지 않은 데이터는 재로드되지 않습니다.
        public void Load()
        {
            if (!m_isLoaded || m_needUpdate)
            {
                m_onLoad?.Invoke();

                m_isLoaded = true;
                m_needUpdate = false;
            }
        }

        // 캐시 상태를 초기화합니다.
        public void Reset()
        {
            m_isLoaded = false;
            m_needUpdate = false;
        }
    }
}
