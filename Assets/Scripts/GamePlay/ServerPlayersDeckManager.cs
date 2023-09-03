using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.GamePlay.PlayerHand;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Minimax.GamePlay
{
    public class ServerPlayersDeckManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private ClientPlayerDeckManager m_clientPlayerDeckManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        
        /// <summary>
        /// Stores the deck list fetched from cloud. Key: playerNumber, Value: DeckDTO
        /// </summary>
        private Dictionary<int, DeckDTO> m_playersDeckList = new Dictionary<int, DeckDTO>();

        private Dictionary<int, List<ServerCard>> m_playersDeck = new Dictionary<int, List<ServerCard>>();

        public async UniTask SetupPlayersDeck()
        {
            var isPlayerDeckListFetched = await FetchPlayerDeckListFromCloud();
            if (isPlayerDeckListFetched)
            {
                // first shuffle the deck (before shuffling, the deck is sorted by card id)
                ShufflePlayersDeckList();
                GeneratePlayersDeck();
                
                // send player deck list to clients
                var connectionManager = GlobalManagers.Instance.Connection;
                foreach (var clientId in m_networkManager.ConnectedClientsIds)
                {
                    var clientRpcParams = connectionManager.ClientRpcParams[clientId];
                    var playerNumber = connectionManager.GetPlayerNumber(clientId);
                    
                    // copy the deck list and shuffle it again for prevent player from knowing the deck order
                    var copiedCardLogicList = new List<ServerCard>(m_playersDeck[playerNumber]);
                    copiedCardLogicList.Shuffle();
                    
                    int[] copiedCardUniqueIds = new int[copiedCardLogicList.Count];
                    int[] copiedCardIds = new int[copiedCardLogicList.Count];
                    
                    for (int i = 0; i < copiedCardLogicList.Count; i++)
                    {
                        copiedCardUniqueIds[i] = copiedCardLogicList[i].UID;
                        copiedCardIds[i] = copiedCardLogicList[i].Data.CardId;
                    }
                    
                    m_clientPlayerDeckManager.SetupPlayerDeckClientRpc(copiedCardUniqueIds,copiedCardIds, clientRpcParams);
                }
            }
        }

        /// <summary>
        /// Get card data from DB using card id from deck list fetched from cloud and generate players deck.
        /// </summary>
        private void GeneratePlayersDeck()
        {
            foreach (var pair in m_playersDeckList)
            {
                var deck = new List<ServerCard>();
                var playerNumber = pair.Key;
                
                foreach (var cardId in pair.Value.CardIds)
                {
                    // create instance of card data from card id
                    var cardData = Instantiate(m_cardDBManager.GetCardData(cardId));
                    var serverCardLogic = new ServerCard(cardData)
                    {
                        Owner = playerNumber
                    };
                    deck.Add(serverCardLogic);
                }
                m_playersDeck.Add(playerNumber, deck);
            }
        }

        private async UniTask<bool> FetchPlayerDeckListFromCloud()
        {
            DebugWrapper.Log("Fetching player deck list from cloud...");
            
            // Get all connected player ids from session manager, need for fetching deck list from cloud
            List<string> connectedPlayerIds = new List<string>();
            
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                connectedPlayerIds.Add(playerId);
            }

            // Fetch deck list from cloud
            try
            {
#if DEDICATED_SERVER
                var playerDeckLists =
                    await WebRequestManager.ServerRunCloudCodeModuleEndpointAsync<List<DeckDTO>>("Deck", "GetPlayerDecks", 
                        new Dictionary<string, object>
                        {
                            { "playerIds", connectedPlayerIds }
                        });
#elif UNITY_EDITOR
                var playerDecks = await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "GetPlayerDecks",
                    new Dictionary<string, object>
                    {
                        { "playerIds", connectedPlayerIds }
                    });
                DebugWrapper.Log($"Fetched player deck list from cloud: {playerDecks}");
                var playerDeckLists = JsonConvert.DeserializeObject<List<DeckDTO>>(playerDecks);
#endif

                // Store the deck list fetched from cloud
                var connectionManager = GlobalManagers.Instance.Connection;
                for (int i = 0; i < m_networkManager.ConnectedClientsIds.Count; i++)
                {
                    var playerNumber = connectionManager.GetPlayerNumber(m_networkManager.ConnectedClientsIds[i]);
                    m_playersDeckList.Add(playerNumber, playerDeckLists[i]);
                }
                
                return true;
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
                return false;
            }
        }

        /// <summary>
        /// 각 player들의 모든 덱을 섞는다.
        /// </summary>
        private void ShufflePlayersDeckList()
        {
            foreach (var deckList in m_playersDeckList)
            {
               for (int i = 0; i < deckList.Value.CardIds.Count; i++)
               {
                   deckList.Value.CardIds.Shuffle();
               }
            }
        }
        
        public List<ServerCard> GetPlayerDeck(int playerNumber) => m_playersDeck[playerNumber];
    }
}
