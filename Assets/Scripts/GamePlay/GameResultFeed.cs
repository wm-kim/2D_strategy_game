using System;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using Minimax.UnityGamingService.Multiplayer;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

namespace Minimax.GamePlay
{
    /// <summary>
    /// Handles the display of game result
    /// TODO : save game result to cloud storage
    /// </summary>
    public class GameResultFeed : NetworkBehaviour
    {
        private void OnEnable()
        {
            GlobalManagers.Instance.Connection.DisconnectRelay.BeforeDisconnectAll += DisplayGameResult;
        }

        private void OnDisable()
        {
            if (!GlobalManagers.IsAvailable) return;

            if (GlobalManagers.Instance.Connection != null)
                GlobalManagers.Instance.Connection.DisconnectRelay.BeforeDisconnectAll -= DisplayGameResult;
        }

        private void DisplayGameResult(int loserPlayerNumber)
        {
            var sessionPlayers = SessionPlayerManager.Instance;
            var playerNumbers  = sessionPlayers.GetAllPlayerNumbers();
            foreach (var playerNumber in playerNumbers)
            {
                var isWinner = playerNumber != loserPlayerNumber;
                DisplayGameResultClientRpc(isWinner, sessionPlayers.ClientRpcParams[playerNumber]);
            }
        }

        [ClientRpc]
        private void DisplayGameResultClientRpc(bool isWinner, ClientRpcParams clientRpcParams = default)
        {
            if (isWinner)
                PopupManager.Instance.RegisterPopupToQueue(PopupType.WinPopup,
                    PopupCommandType.Unique, PopupPriority.Critical);
            else
                PopupManager.Instance.RegisterPopupToQueue(PopupType.LosePopup,
                    PopupCommandType.Unique, PopupPriority.Critical);
        }
    }
}