using System;
using Minimax.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace Minimax.GamePlay
{
    public enum TimerType
    {
        Player0Turn,
        Player1Turn,
    }
    
    public class NetworkTimer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_timerText;

        private NetworkVariable<float> m_time = new NetworkVariable<float>(0f);
        // 타이머가 완료되었는지 여부를 추적하는 변수
        private bool m_isTimerFinished = false;

        private float m_duration = 0f;
        private System.Action m_onServerTimerComplete;
        private System.Action m_onClientTimerComplete;
        
        /// <summary>
        /// 타이머가 파괴되었는지 여부를 추적하는 변수, 이미 파괴된 타이머는 더 이상 업데이트하지 않습니다.
        /// </summary>
        private bool m_isDestroyed = false;
        
        public void ConFig(float duration, System.Action onServerTimerComplete, System.Action onClientTimerComplete = null)
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
            if (m_isDestroyed) return; 
            
            if (IsServer)
            {
                if (m_time.Value > 0f && !m_isTimerFinished)
                {
                    m_time.Value -= Time.deltaTime;
                    // Update timer text
                    if (m_time.Value <= 0f) TimeFinished();
                }
            }
            
            if (IsClient)
            {
                // Update timer text
                m_timerText.text = m_time.Value.ToString("F1");
            }
        }
        
        private void TimeFinished()
        {
            if (!IsServer) return;
            
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
        
        public void EndTimerImmediately()
        {
            if (!IsServer) return;
            
            DebugWrapper.Log("EndTimerImmediately");
            TimeFinished();
        }
        
        [ClientRpc]
        private void TimerFinishedClientRpc()
        {
            m_onClientTimerComplete?.Invoke();
        }
        
        public override void OnNetworkDespawn()
        {
            m_isDestroyed = true;
            base.OnNetworkDespawn();
        }
    }
}
