using Minimax.CoreSystems;
using Minimax.SceneManagement;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    public class ClientConnectedState : OnlineState
    {
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager) { }
        
        public override void Enter() { }

        public override void Exit() { }

        public override void OnUserRequestedShutdown()
        {
            m_connectionManager.RequestShutdownServerRpc();
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            var disconnectReason = m_connectionManager.NetworkManager.DisconnectReason;
            DebugWrapper.Log($"Client {clientId} disconnected");
            
            if (string.IsNullOrEmpty(disconnectReason))
            {
                m_connectionManager.ConnectStatusChannel.Publish(ConnectStatus.Reconnecting);
                m_connectionManager.ChangeState(m_connectionManager.ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                m_connectionManager.ConnectStatusChannel.Publish(connectStatus);
                
                DebugWrapper.Log($"Disconnected reason: {connectStatus.ToString()}");
                m_connectionManager.ChangeState(m_connectionManager.Offline);

                // if (clientId == m_connectionManager.NetworkManager.LocalClientId)
                // {
                //     PopupManager.Instance.RegisterPopupToQueue(PopupType.LosePopup);
                // }
                // else
                // {
                //     PopupManager.Instance.RegisterPopupToQueue(PopupType.WinPopup);
                // }                    
                // GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
            }
        }
    }
   
}