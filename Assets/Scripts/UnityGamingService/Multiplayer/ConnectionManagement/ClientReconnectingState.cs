using System;
using System.Collections;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to a client attempting to reconnect to a server. It will try to reconnect a
    /// number of times defined by the ConnectionManager's NbReconnectAttempts property. If it succeeds, it will
    /// transition to the ClientConnected state. If not, it will transition to the Offline state. If given a disconnect
    /// reason first, depending on the reason given, may not try to reconnect again and transition directly to the
    /// Offline state.
    /// </summary>
    public class ClientReconnectingState : ClientConnectingState
    {
        public ClientReconnectingState(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        private Coroutine m_reconnectCoroutine;
        private int       m_attempts;

        public override void Enter()
        {
            m_attempts = 0;

            // TODO : Make Reconnecting Channel
            PopupManager.Instance.RegisterLoadingPopupToQueue(Define.ReconnectingPopup,
                "Lost connection to the server. Reconnecting...",
                PopupCommandType.Unique, PopupPriority.High);

            m_reconnectCoroutine = m_connectionManager.StartCoroutine(ReconnectCoroutine());
        }

        public override void Exit()
        {
            if (m_reconnectCoroutine != null)
            {
                m_connectionManager.StopCoroutine(m_reconnectCoroutine);
                m_reconnectCoroutine = null;
            }
        }

        public override void OnClientConnected(ulong _)
        {
            PopupManager.Instance.HideCurrentPopup(Define.ReconnectingPopup);
            m_connectionManager.ChangeState(m_connectionManager.ClientConnected);
        }

        /// <summary>
        /// Disconnect Reason에 따라 재연결을 시도할지 결정합니다.
        /// </summary>
        public override void OnClientDisconnect(ulong _)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            if (m_attempts < Define.MaxReconnectionAttempts)
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    m_reconnectCoroutine = m_connectionManager.StartCoroutine(ReconnectCoroutine());
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    switch (connectStatus)
                    {
                        case ConnectStatus.ServerEndedSession:
                        case ConnectStatus.UserRequestedDisconnect:
                        case ConnectStatus.ServerFull:
                            m_connectionManager.ChangeState(m_connectionManager.Offline);
                            break;
                        default:
                            m_reconnectCoroutine = m_connectionManager.StartCoroutine(ReconnectCoroutine());
                            break;
                    }
                }
            }
            else // 최대 재연결 시도 횟수를 초과한 경우
            {
                DebugWrapper.Log("Used up all reconnection attempts, giving up.");
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.GenericDisconnect);
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    m_connectionManager.ConnectStatusChannel.Publish(connectStatus);
                }

                // 오프라인 상태로 전환합니다. client입장에서는 네트워크 연결이 불안정한 것인지 
                // 서버가 죽은 것인지 알 수 가 없다.
                PopupManager.Instance.RegisterOneButtonPopupToQueue(Define.ServerDisconnectedPopup,
                    "Reconnecting to Server Failed.", "OK",
                    () =>
                    {
                        GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
                        PopupManager.Instance.HideCurrentPopup();
                    },
                    PopupCommandType.Unique, PopupPriority.Critical);

                m_connectionManager.ChangeState(m_connectionManager.Offline);
            }
        }

        private IEnumerator ReconnectCoroutine()
        {
            // 첫 번째 시도에서 성공하지 않은 경우, 다시 시도하기 전에 잠시 어느 정도의 시간을 기다립니다.
            // 이렇게 하면 연결 끊김의 원인이 일시적인 것인 경우, 다시 시도하기 전에 문제가 스스로 해결될 시간을 확보할 수 있습니다.
            if (m_attempts > 0) yield return new WaitForSeconds(Define.TimeBetweenReconnectionAttempts);

            // 연결을 시도합니다.
            DebugWrapper.Log("Attempting to reconnect to server...");

            m_connectionManager.NetworkManager.Shutdown();
            // wait until NetworkManager completes shutting down
            yield return new WaitWhile(() => m_connectionManager.NetworkManager.ShutdownInProgress);
            DebugWrapper.Log($"Reconnecting attempt {m_attempts + 1}/{Define.MaxReconnectionAttempts}...");

            // If first attempt, wait some time before attempting to reconnect to give time to services to update
            if (m_attempts == 0) yield return new WaitForSeconds(Define.TimeBeforeFirstAttempt);

            m_attempts++;

            // If this fails, the OnClientDisconnect callback will be invoked by Netcode
            ConnectClient();
        }
    }
}