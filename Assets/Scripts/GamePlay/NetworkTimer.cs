using System;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

namespace Minimax
{
    public class NetworkTimer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_timerText;

        private NetworkVariable<float> m_time = new NetworkVariable<float>(0f);
        // 타이머가 완료되었는지 여부를 추적하는 변수
        private bool m_isTimerFinished = false;

        private float m_duration = 0f;
        private Action m_onServerTimerComplete;
        private Action m_onClientTimerComplete;
        
        public void ConFig(float duration, Action onServerTimerComplete, Action onClientTimerComplete = null)
        {
            Assert.IsTrue(duration > 0f);
            
            m_duration = duration;
            m_onServerTimerComplete = onServerTimerComplete;
            m_onClientTimerComplete = onClientTimerComplete;
        }
        
        public void ClearConfig()
        {
            m_duration = 0f;
            m_onServerTimerComplete = null;
            m_onClientTimerComplete = null;
        }

        void Update()
        {
            if (IsServer && m_time.Value > 0f && !m_isTimerFinished)
            {
                m_time.Value -= Time.deltaTime;
                // Update timer text
                if (m_time.Value <= 0f) TimeFinished();
            }
            
            if (IsClient)
            {
                // Update timer text
                m_timerText.text = m_time.Value.ToString("F1");
            }
        }
        
        private void TimeFinished()
        {
            m_time.Value = 0f;
            m_isTimerFinished = true;
            
            DebugWrapper.Log("Time Finished");
            m_onServerTimerComplete?.Invoke();
            TimerFinishedClientRpc();
        }

        public void StartTimer()
        {
            if (!IsServer) return;
            
            DebugWrapper.Log($"StartTimer, duration: {m_duration}");
            m_isTimerFinished = false;
            m_time.Value = m_duration;
        }
        
        [ClientRpc]
        private void TimerFinishedClientRpc()
        {
            m_onClientTimerComplete?.Invoke();
        }
    }
}
