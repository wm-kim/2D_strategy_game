using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using Unity.Services.CloudCode;
using UnityEngine;

namespace Minimax
{
    public class ServerPlayerDeckManager : NetworkBehaviour
    {
        [SerializeField] private CardDBManager m_cardDBManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        
        List<int>[] m_playerDeckLists = new List<int>[Define.MaxConnectedPlayers];

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                for (int i = 0; i < m_playerDeckLists.Length; i++) m_playerDeckLists[i] = new List<int>();
                FetchPlayerDeckListFromCloud();
            }
            
            base.OnNetworkSpawn();
        }

        private async void FetchPlayerDeckListFromCloud()
        {
            DebugWrapper.Instance.Log("Fetching player deck list from cloud...");
            
            // Get all connected player ids from session manager
            List<string> connectedPlayerIds = new List<string>();
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                connectedPlayerIds.Add(playerId);
            }
            
            var playerDecks = await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "GetPlayerDecks",
                new Dictionary<string, object>
                {
                    { "playerIds", connectedPlayerIds }
                });
            
            DebugWrapper.Instance.Log($"Fetched player deck list from cloud: {playerDecks}");
            
            m_playerDeckLists = JsonUtility.FromJson<List<int>[]>(playerDecks);
        }
    }
}
