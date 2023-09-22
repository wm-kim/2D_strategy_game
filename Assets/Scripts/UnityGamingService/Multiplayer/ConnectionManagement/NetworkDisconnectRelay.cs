using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer.ConnectionManagement
{
    /// <summary>
    /// This is a relay class for disconnect clients driven by server intentionally.
    /// The primary reason for this class is to ensure server to know the reason for shutdown/disconnect
    /// because Netcode OnClientDisconnect callback doesn't passing on logical reason locally.
    /// </summary>
    public class NetworkDisconnectRelay
    {
        /// <summary>
        /// key is playerNumber, value is the reason for disconnect
        /// </summary>
        private Dictionary<int, ConnectStatus> m_disconnectReasons = new Dictionary<int, ConnectStatus>();
        
        /// <summary>
        /// This event will be invoked before disconnecting all clients.
        /// int is the playerNumber of the client who trigger the disconnect all.
        /// </summary>
        public event Action<int> BeforeDisconnectAll;

        public ConnectStatus GetDisconnectReason(int playerNumber)
        {
            return m_disconnectReasons.TryGetValue(playerNumber, out var reason) ? reason :
                // if this disconnect reason is not recorded (unintentional), return generic disconnect
                ConnectStatus.GenericDisconnect;
        }
        
        public void DisconnectAll(int loserPlayerNumber, ConnectStatus disconnectReason)
        {
            BeforeDisconnectAll?.Invoke(loserPlayerNumber);
            
            m_disconnectReasons.Clear();

            var netManager = NetworkManager.Singleton;
            for (var i = netManager.ConnectedClientsList.Count - 1; i >= 0; i--)
            {
                var clientId = netManager.ConnectedClientsList[i].ClientId;
                var playerNumber = SessionPlayerManager.Instance.GetPlayerNumber(clientId);
                m_disconnectReasons[playerNumber] = disconnectReason;
                
                var reason = JsonUtility.ToJson(disconnectReason);
                netManager.DisconnectClient(clientId, reason);
            }
        }
    }
}