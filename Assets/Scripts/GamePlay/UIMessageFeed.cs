using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// Handles the display of important in-game events or messages.
    /// </summary>
    public class UIMessageFeed : MonoBehaviour
    {
        private void OnEnable()
        {
            GlobalManagers.Instance.Connection.ConnectionEventChannel.Subscribe(OnConnectionEvent);
        }
        
        private void OnDisable()
        {
            if (!GlobalManagers.IsAvailable || GlobalManagers.Instance.Connection == null) return;
            GlobalManagers.Instance.Connection.ConnectionEventChannel.Unsubscribe(OnConnectionEvent);
        }
        
        private async void OnConnectionEvent(ConnectionEventMessage message)
        {
            if (message.ConnectStatus == ConnectStatus.Success)
            {
                PopupManager.Instance.HideCurrentPopup(Define.PlayerLostConnectionPopup);
            }
            
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
