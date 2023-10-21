using System;
using Minimax.CoreSystems;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.GamePlay.Unit;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
#if DEDICATED_SERVER
using Unity.Services.Multiplay;
using UnityEngine.Serialization;
#endif

namespace Minimax.GamePlay
{
    /// <summary>
    /// 게임
    /// 게임을 시작하기 전에 필요한 설정들을 하고, 모든 플레이어가 준비되면 게임을 시작합니다.
    /// </summary>
    public class GameInitializer : NetworkBehaviour
    {
#region Fields and Properties
        [Header("References")]
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private ServerPlayersDeckManager m_serverPlayersDeckManager;
        [SerializeField] private TurnManager m_turnManager;
        [SerializeField] private ProfileManager m_profileManager;
        [SerializeField] private ServerManaManager m_manaManager;
        [SerializeField] private ServerMap m_serverMap;

        [Header("Game Logics")] [SerializeField]
        private CardDrawingLogic m_cardDrawingLogic;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
#endregion

#region Unity Lifecycle Methods
        private async void Start()
        {
            HandleStartForDedicatedServer();
        }
        
        public override void OnNetworkSpawn()
        {
            HandleOnNetworkSpawn();
        }
        
        public override void OnNetworkDespawn()
        {
            HandleOnNetworkDespawn();
        }
#endregion

#region Event Handlers

        private async void GameManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            HandleSceneEvent(sceneEvent);
        }

#endregion

#region Private Helper Methods

        /// <summary>
        /// 서버 설정 시작
        /// </summary>
        private async void HandleStartForDedicatedServer()
        {
#if DEDICATED_SERVER
            await MultiplayService.Instance.UnreadyServerAsync();
            Camera.main.enabled = false;  // just for clearing logs
#endif
        }
        
        private void HandleOnNetworkSpawn()
        {
            m_networkManager.SceneManager.OnSceneEvent += GameManager_OnSceneEvent;

            if (IsServer)
            {
                SessionPlayerManager.Instance.OnSessionStarted();
                DebugWrapper.Log("Session started");
            }

            base.OnNetworkSpawn();
        }
        
        private void HandleOnNetworkDespawn()
        {
            m_networkManager.SceneManager.OnSceneEvent -= GameManager_OnSceneEvent;
            ClearGameData();
            base.OnNetworkDespawn();
        }
        
        private void ClearGameData()
        {
            if (IsServer)
            {
                IDFactory.ResetIDs();
                SessionPlayerManager.Instance.ClientRpcParams.Clear();
                ServerCard.CardsCreatedThisGame.Clear();
                ServerUnit.UnitsCreatedThisGame.Clear();
            }

            if (IsClient)
            {
                ClientCard.CardsCreatedThisGame.Clear();
                ClientUnit.UnitsCreatedThisGame.Clear();
            }
        }

        // only executed on server
        private async void HandleSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted) return;
            
            // load card data from DB (server & client)
            await m_cardDBManager.LoadDBCardsAsync();
            
            if (IsServer)
            {
                InitializeServerGameData();
            }
        }
        
        private async void InitializeServerGameData()
        {
            await m_serverPlayersDeckManager.SetupPlayersDeck();
            m_serverMap.GenerateMap(Define.MapSize);
            m_serverMap.SetPlayersInitialPlaceableArea();
            m_profileManager.SetPlayersName();
            m_manaManager.InitPlayersMana();
            m_cardDrawingLogic.CommandDrawAllPlayerInitialCards();
            m_turnManager.StartInitialTurn();

            var connection = GlobalManagers.Instance.Connection;
            connection.ChangeState(connection.GameStarted);
        }
#endregion
        
    }
}
