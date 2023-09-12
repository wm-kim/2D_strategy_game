using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.GamePlay;
using Minimax.UI.View.Popups;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// Handles the display of important in-game events or messages driven by the server
    /// </summary>
    public class UIServerMessageFeed : MonoBehaviour
    {
        private void OnEnable()
        {
            GlobalManagers.Instance.GameStatus.SubscribeToGameResult(OnGameResult);
            GlobalManagers.Instance.Connection.ConnectionEventChannel.Subscribe(OnConnectionEvent);
        }

        private void OnDisable()
        {
            if (!GlobalManagers.IsAvailable) return;
            
            if (GlobalManagers.Instance.GameStatus != null)
                GlobalManagers.Instance.GameStatus.UnsubscribeFromGameResult(OnGameResult);
            
            if (GlobalManagers.Instance.Connection != null)
                GlobalManagers.Instance.Connection.ConnectionEventChannel.Unsubscribe(OnConnectionEvent);
        }
        
        private void OnGameResult(GameResultMessage message)
        {
            var myPlayerNumber = SessionPlayerManager.Instance.GetMyPlayerNumber();
            if (myPlayerNumber == message.LoserPlayerNumber)
            {
                PopupManager.Instance.RegisterPopupToQueue(PopupType.LosePopup, 
                    PopupCommandType.Unique, PopupPriority.Critical);
            }
            else
            {
                PopupManager.Instance.RegisterPopupToQueue(PopupType.WinPopup, 
                    PopupCommandType.Unique, PopupPriority.Critical);
            }
        }
        
        private void OnConnectionEvent(ConnectionEventMessage message)
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
