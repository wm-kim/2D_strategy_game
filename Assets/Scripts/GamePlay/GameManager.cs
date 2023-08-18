using System.Collections;
using System.Collections.Generic;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class GameManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnManager m_turnManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Marks the current session as started, so from now on we keep the data of disconnected players.
                SessionManager<SessionPlayerData>.Instance.OnSessionStarted();
                DebugWrapper.Log("Session started");
                
                m_turnManager.StartGameServerRpc();
                
                m_networkManager.SceneManager.OnSceneEvent += GameManager_OnSceneEvent;
            }
            
            base.OnNetworkSpawn();
        }
        
        private void GameManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
        }
    }
}
