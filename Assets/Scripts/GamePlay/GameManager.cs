using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay
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
            GlobalManagers.Instance.Connection.DeleteBackfillTicket();
            
            // just for clearing logs
            Camera.main.enabled = false;
#endif
        }

        public override void OnNetworkSpawn()
        {
            m_networkManager.SceneManager.OnSceneEvent += GameManager_OnSceneEvent;
            
            if (IsServer)
            {
                // Marks the current session as started, so from now on we keep the data of disconnected players.
                UnityGamingService.Multiplayer.SessionManager<SessionPlayerData>.Instance.OnSessionStarted();
                DebugWrapper.Log("Session started");
            }
            
            base.OnNetworkSpawn();
        }
        
        public override void OnNetworkDespawn()
        {
            m_networkManager.SceneManager.OnSceneEvent -= GameManager_OnSceneEvent;
            
            // clear the cached client rpc params
            GlobalManagers.Instance.Connection.ClientRpcParams.Clear();
            base.OnNetworkDespawn();
        }
        
        // only executed on server
        private void GameManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;

            if (IsServer)
            {
                SetPlayerNames();
                m_turnManager.StartGame();
            }
        }
        
        private void SetPlayerNames()
        {
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerData = UnityGamingService.Multiplayer.SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
                if (!playerData.HasValue) return;
                var playerName = playerData.Value.PlayerName;
                m_profileManager.SetMyPlayerNameClientRpc(clientId, playerName);
            }
        }
    }
}
