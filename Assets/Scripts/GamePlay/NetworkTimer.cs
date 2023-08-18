using System.Collections;
using System.Collections.Generic;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using TMPro;

namespace Minimax
{
    public class NetworkTimer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnManager m_turnManager;
        [SerializeField] private TextMeshProUGUI m_timerText;

        [SerializeField, Range(0.0f, 10.0f)] private float m_timeBeforeGameStarts = 5.0f;
        [SerializeField, Range(0.0f, 60.0f)] private float m_timeForOneTurn = 15.0f;
        
        private NetworkVariable<bool> m_countdownStarted = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> m_hasGameStarted = new NetworkVariable<bool>(false);
        private bool m_clientStartCountdown;
        private bool m_clientGameStarted;
        private bool m_gameStarted = false;
        
        // only server can update this timeTillZero, responsible for client timer visual countdown
        private float m_timeTillZero = 0f;
        
        /// <summary>
        /// ServerRpc to start the timer for the game start
        /// </summary>
        [ServerRpc]
        public void StartTimerForGameStartServerRpc()
        {
            m_countdownStarted.Value = true;
            m_timeTillZero = m_timeBeforeGameStarts;
            DebugWrapper.Log($"Game Start in {m_timeBeforeGameStarts} seconds...");
        }
        
        [ServerRpc]
        public void StartTimerForNextTurnServerRpc()
        {
            m_countdownStarted.Value = true;
            m_timeTillZero = m_timeForOneTurn;
            SetReplicatedTimeTillZeroClientRPC(m_timeTillZero);
        }
        
        [ServerRpc]
        public void StopTimerServerRpc() => m_countdownStarted.Value = false;
        
        /// <summary>
        /// ClientRpc to set the timeTillZero on the client
        /// </summary>
        [ClientRpc]
        private void SetReplicatedTimeTillZeroClientRPC(float timeTillZero) 
            => m_timeTillZero = timeTillZero;

        private bool ShouldStartCountdown()
        {
            return IsServer ? m_countdownStarted.Value : m_clientStartCountdown;
        }
        
        private bool HasGameStarted()
        {
            return IsServer ? m_hasGameStarted.Value : m_clientGameStarted;
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient && !IsServer)
            {
                m_clientStartCountdown = false;
                m_clientGameStarted = false;
                m_countdownStarted.OnValueChanged += (oldValue, newValue) => { m_clientStartCountdown = newValue; };
                m_hasGameStarted.OnValueChanged += (oldValue, newValue) => { m_clientGameStarted = newValue; };
            }
            else if (IsServer)
            {
                m_hasGameStarted.Value = false;
                m_clientStartCountdown = false; 
            }
            
            base.OnNetworkSpawn();
        }
        

        private void Update()
        {
            UpdateTimer();
        }
        
        private void UpdateTimer()
        {
            if (!ShouldStartCountdown()) return;

            if (m_timeTillZero > 0.0f)
            {
                m_timeTillZero -= Time.deltaTime;
                
                if (IsServer && m_timeTillZero <= 0.0f)
                {
                    StopTimerServerRpc();

                    if (!m_hasGameStarted.Value)
                    {
                        DebugWrapper.Log("Game Started");
                        m_hasGameStarted.Value = true;
                        StartTimerForNextTurnServerRpc();
                    }
                    else
                    {
                        m_turnManager.TakeNextTurnServerRpc();
                    }
                }
                else if (IsClient)
                {
                    UpdateVisualTimer();
                }
            }
        }
        
        private void UpdateVisualTimer()
        {
            if (HasGameStarted() && m_timerText != null)
            {
                m_timerText.text = $"Time Left: {FormatTime(m_timeTillZero)}";
            }
        }
        
        private string FormatTime(float time)
        {
            int inSeconds = Mathf.RoundToInt(time);
            string minutes = (inSeconds / 60).ToString();
            string seconds = (inSeconds % 60).ToString();
            return $"{minutes}:{seconds}";
        }
    }
}
