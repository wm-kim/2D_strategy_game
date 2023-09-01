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
    public class ServerPlayerDeckManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private ClientPlayerDeckManager m_clientPlayerDeckManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        
        /// <summary>
        /// Stores the deck list fetched from cloud. Key: playerNumber, Value: DeckDTO
        /// </summary>
        private Dictionary<int, DeckDTO> m_playerDeckLists = new Dictionary<int, DeckDTO>();
        
        [SerializeField, Tooltip("Key: playerNumber, Value: Deck Card Data")]
        private SerializedDictionary<int, List<CardBaseData>> m_playerDecks = new SerializedDictionary<int, List<CardBaseData>>();

        public async UniTask SetupPlayerDecks()
        {
            var isPlayerDeckListFetched = await FetchPlayerDeckListFromCloud();
            var isDBLoaded = await m_cardDBManager.LoadDBCardsAsync();
            
            if (isPlayerDeckListFetched && isDBLoaded)
            {
                // first shuffle the deck (before shuffling, the deck is sorted by card id)
                ShufflePlayerDeckLists();
                GetPlayerDecksCardDataFromDB();
                
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
        private void GetPlayerDecksCardDataFromDB()
        {
            for (int i = 0; i < m_playerDeckLists.Count; i++)
            {
                var deck = new List<CardBaseData>();
                foreach (var pair in m_playerDeckLists)
                {
                    foreach (var cardId in pair.Value.CardIds)
                    {
                        deck.Add(m_cardDBManager.GetCardData(cardId));
                    }
                    m_playerDecks.Add(pair.Key, deck);
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
                    m_playerDeckLists.Add(playerNumber, playerDeckLists[i]);
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
        private void ShufflePlayerDeckLists()
        {
            foreach (var deckList in m_playerDeckLists)
            {
               for (int i = 0; i < deckList.Value.CardIds.Count; i++)
               {
                   ShuffleList(deckList.Value.CardIds);
               }
            }
        }
        
        /// <summary>
        /// Copy the deck from the deck list and shuffle it
        /// This is used for client to view their deck
        /// </summary>
        private int[] CopyPlayerDeckListAndShuffle(int playerNumber)
        {
            var deckList = m_playerDeckLists[playerNumber].CardIds;
            var deck = new int[deckList.Count];
            for (int i = 0; i < deckList.Count; i++)
            {
                deck[i] = deckList[i];
            }
            ShuffleList(deck);
            return deck;
        }
        
        private void ShuffleList(List<int> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int rnd = Random.Range(0, i + 1);
                (list[rnd], list[i]) = (list[i], list[rnd]);
            }
        }
        
        private void ShuffleList(int[] list)
        {
            int n = list.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int rnd = Random.Range(0, i + 1);
                (list[rnd], list[i]) = (list[i], list[rnd]);
            }
        }
    }
}
