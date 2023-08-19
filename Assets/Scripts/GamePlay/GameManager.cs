using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using Unity.Services.Multiplay;
using UnityEngine;

namespace Minimax
{
    public class GameManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnManager m_turnManager;
        [SerializeField] private ProfileManager m_profileManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;

        private async void Start()
        {
#if DEDICATED_SERVER
            await MultiplayService.Instance.UnreadyServerAsync();
            // just for clearing logs
            Camera.main.enabled = false;
#endif
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Marks the current session as started, so from now on we keep the data of disconnected players.
                SessionManager<SessionPlayerData>.Instance.OnSessionStarted();
                DebugWrapper.Instance.Log("Session started");
                GlobalManagers.Instance.Connection.CacheClientRpcParams();
                
                m_turnManager.StartGameServerRpc();
                
                m_networkManager.SceneManager.OnSceneEvent += GameManager_OnSceneEvent;
            }
            
            base.OnNetworkSpawn();
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                m_networkManager.SceneManager.OnSceneEvent -= GameManager_OnSceneEvent;
            }
            
            base.OnNetworkDespawn();
        }
        
        // only executed on server
        private void GameManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
            
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
                if (!playerData.HasValue) return;
                var playerName = playerData.Value.PlayerName;
                m_profileManager.SetMyPlayerNameClientRpc(clientId, playerName);
            }
        }
    }
}
