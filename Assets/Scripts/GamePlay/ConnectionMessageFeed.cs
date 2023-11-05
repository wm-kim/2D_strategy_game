using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.UI.View.Popups;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.GamePlay
{
    /// <summary>
    /// Handles the display of players connection message
    /// </summary>
    public class ConnectionMessageFeed : MonoBehaviour
    {
        private void OnEnable()
        {
            GlobalManagers.Instance.Connection.ConnectionEventChannel.Subscribe(OnConnectionEvent);
        }

        private void OnDisable()
        {
            if (!GlobalManagers.IsAvailable) return;

            if (GlobalManagers.Instance.Connection != null)
                GlobalManagers.Instance.Connection.ConnectionEventChannel.Unsubscribe(OnConnectionEvent);
        }

        private void OnConnectionEvent(ConnectionEventMessage message)
        {
            if (message.ConnectStatus == ConnectStatus.Success)
                PopupManager.Instance.HideCurrentPopup(Define.PlayerLostConnectionPopup);

            if (message.ConnectStatus == ConnectStatus.GenericDisconnect)
            {
                DebugWrapper.Log($"Generic disconnect: {message.PlayerName}");
                PopupManager.Instance.RegisterLoadingPopupToQueue(Define.PlayerLostConnectionPopup,
                    $"{message.PlayerName} lost connection. Waiting for reconnect...",
                    PopupCommandType.Unique, PopupPriority.Normal);
            }
        }
    }
}