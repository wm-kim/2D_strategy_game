using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private ClientMyDeckManager m_clientMyDeckManager;
        [SerializeField] private ClientOpponentDeckManager m_clientOpponentDeckManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        
        /// <summary>
        /// Stores the deck list fetched from cloud. Key: playerNumber, Value: DeckDTO
        /// the reason why clientId is not used as key is because it can be changed when the player reconnects.
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
                
                var connectionManager = GlobalManagers.Instance.Connection;
                var clientIds = m_networkManager.ConnectedClientsIds;
                var copiedPlayersDeck = m_playersDeck.ToDictionary(playerDeck => playerDeck.Key,
                    playerDeck => new List<ServerCard>(playerDeck.Value));

                var copiedCardUniqueIds = new Dictionary<int, int[]>();
                var copiedCardIds = new Dictionary<int, int[]>();
                
                // shuffle the copied deck list for prevent player from knowing the deck order
                foreach (var playerDeck in copiedPlayersDeck)
                {
                    playerDeck.Value.Shuffle();
                    copiedCardUniqueIds.Add(playerDeck.Key, new int[playerDeck.Value.Count]);
                    copiedCardIds.Add(playerDeck.Key, new int[playerDeck.Value.Count]);
                    for (int i = 0; i < playerDeck.Value.Count; i++)
                    {
                        copiedCardUniqueIds[playerDeck.Key][i] = playerDeck.Value[i].UniqueCardID;
                        copiedCardIds[playerDeck.Key][i] = playerDeck.Value[i].Data.CardId;
                    }
                }
                
                foreach (var clientId in clientIds)
                {
                    var clientRpcParam = connectionManager.ClientRpcParams[clientId];
                    var playerNumber = connectionManager.GetPlayerNumber(clientId);
                    m_clientMyDeckManager.SetupMyDeckClientRpc(copiedCardUniqueIds[playerNumber],copiedCardIds[playerNumber], clientRpcParam);
                    
                    var opponentNumber = connectionManager.GetOpponentPlayerNumber(clientId);
                    m_clientOpponentDeckManager.SetupOpponentDeckClientRpc(copiedCardUniqueIds[opponentNumber], clientRpcParam);
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
        
        public List<ServerCard> GetPlayerDeck(ulong clientId)
        {
            var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(clientId);
            return m_playersDeck[playerNumber];
        }
    }
}
