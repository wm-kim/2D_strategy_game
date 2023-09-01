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
        
        [SerializeField, Tooltip("Key: playerNumber, Value: Deck Card Data")]
        private SerializedDictionary<int, List<CardBaseData>> m_playersDeck = new SerializedDictionary<int, List<CardBaseData>>();

        public async UniTask SetupPlayersDeck()
        {
            var isPlayerDeckListFetched = await FetchPlayerDeckListFromCloud();
            var isDBLoaded = await m_cardDBManager.LoadDBCardsAsync();
            
            if (isPlayerDeckListFetched && isDBLoaded)
            {
                // first shuffle the deck (before shuffling, the deck is sorted by card id)
                ShufflePlayersDeckList();
                GetPlayersDeckDataFromDB();
                
                // send player deck list to clients
                var connectionManager = GlobalManagers.Instance.Connection;
                foreach (var clientId in m_networkManager.ConnectedClientsIds)
                {
                    var clientRpcParams = connectionManager.ClientRpcParams[clientId];
                    var playerNumber = connectionManager.GetPlayerNumber(clientId);
                    
                    // copy the deck list and shuffle it again for prevent player from knowing the deck order
                    var copiedCardIds = CopyPlayerDeckListAndShuffle(playerNumber);
                    m_clientPlayerDeckManager.SetupPlayerDeckClientRpc(copiedCardIds, clientRpcParams);
                }
            }
        }

        /// <summary>
        /// Get card data from DB using card id from deck list fetched from cloud
        /// </summary>
        private void GetPlayersDeckDataFromDB()
        {
            for (int i = 0; i < m_playersDeckList.Count; i++)
            {
                var deck = new List<CardBaseData>();
                foreach (var pair in m_playersDeckList)
                {
                    foreach (var cardId in pair.Value.CardIds)
                    {
                        deck.Add(m_cardDBManager.GetCardData(cardId));
                    }
                    m_playersDeck.Add(pair.Key, deck);
                }
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
        
        /// <summary>
        /// Copy the deck from the deck list and shuffle it
        /// This is used for client to view their deck
        /// </summary>
        private int[] CopyPlayerDeckListAndShuffle(int playerNumber)
        {
            var deckList = m_playersDeckList[playerNumber].CardIds;
            var deck = new int[deckList.Count];
            for (int i = 0; i < deckList.Count; i++)
            {
                deck[i] = deckList[i];
            }
            
            deck.Shuffle();
            return deck;
        }
        
        public List<CardBaseData> GetPlayerDeck(int playerNumber)
        {
            return m_playersDeck[playerNumber];
        }
        
        public void RemoveCardOfIndex(int playerNumber, int index)
        {
            m_playersDeck[playerNumber].RemoveAt(index);
        }
    }
}
