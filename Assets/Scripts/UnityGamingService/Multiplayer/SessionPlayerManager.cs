using System;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.UnityGamingService.Multiplayer
{
    public class SessionPlayerManager : SessionManager<SessionPlayerData>
    {
        private SessionPlayerManager() : base() { }
        
        public ClientRpcParamManager ClientRpcParams { get; private set; } = new ClientRpcParamManager();
        
        public new static SessionPlayerManager Instance { get; } = new SessionPlayerManager();
        
        public int GetAvailablePlayerNumber()
        {
            for (var i = 0; i < Define.MaxConnectedPlayers; i++)
            {
                if (IsPlayerNumberAvailable(i))
                {
                    return i;
                }
            }
            
            DebugWrapper.LogError("Server is full, There is no available player number");
            return -1;
        }

        private bool IsPlayerNumberAvailable(int playerNumber)
        {
            var connectedClientIds = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (var clientId in connectedClientIds)
            {
                var playerData = GetPlayerData(clientId);
                if (playerData.HasValue)
                {
                    if (playerData.Value.PlayerNumber == playerNumber)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        public int GetMyPlayerNumber() => GetPlayerNumber(NetworkManager.Singleton.LocalClientId);
        
        public int GetPlayerNumber(ulong clientId)
        {
            var playerData = GetPlayerData(clientId);
            if (playerData.HasValue)
            {
                return playerData.Value.PlayerNumber;
            }
            
            // this should never happen, throw an exception
            throw new Exception($"Player Number data not found for client {clientId}");
        }
        
        public int GetOpponentPlayerNumber(ulong clientId)
        {
            var playerNumber = GetPlayerNumber(clientId);
            
            int connectedClientCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            if (connectedClientCount == 2)
            {
                // if there are two players, return the opponent player number
                return playerNumber == 0 ? 1 : 0;
            }
            else
            {
                // throw an exception
                throw new Exception($"Cannot get opponent player number for client {clientId} " +
                                    $"because there are {connectedClientCount} players connected");
            }
        }
    }
}