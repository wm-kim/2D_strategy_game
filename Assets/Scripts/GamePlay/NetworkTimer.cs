using System;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using TMPro;

namespace Minimax
{
    public class NetworkTimer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_timerText;

        private NetworkVariable<float> m_time = new NetworkVariable<float>(0f);
        
        /// <summary>
        /// 타이머가 종료되었을 때 호출되는 이벤트입니다. 서버에서만 호출됩니다.
        /// </summary>
        public event Action ServerOnTimerFinished;
        
        /// <summary>
        /// 타이머가 종료되었을 때 호출되는 이벤트입니다. 클라이언트에서만 호출됩니다.
        /// </summary>
        public event Action ClientOnTimerFinished;
        
        void Update()
        {
            if (IsServer && m_time.Value >= 0f)
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
            ServerOnTimerFinished?.Invoke();
            TimerFinishedClientRpc();
        }

        public void SetTimer(float time)
        {
            DebugWrapper.Log($"SetTimerServerRpc: {time}");
            m_time.Value = time;
        }
        
        [ClientRpc]
        private void TimerFinishedClientRpc()
        {
            // Handle timer finished event on clients
            ClientOnTimerFinished?.Invoke();
        }
    }
}
