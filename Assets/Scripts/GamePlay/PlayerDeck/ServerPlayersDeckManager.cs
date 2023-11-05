using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Minimax.DeckBuliding;
using Minimax.GamePlay.Card;
using Minimax.UnityGamingService.Multiplayer;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Services.CloudCode;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.PlayerDeck
{
    public class ServerPlayersDeckManager : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private CardDBManager m_cardDBManager;

        [SerializeField] private ClientMyDeckManager       m_clientMyDeckManager;
        [SerializeField] private ClientOpponentDeckManager m_clientOpponentDeckManager;

        private NetworkManager m_networkManager => NetworkManager.Singleton;

        /// <summary>
        /// Stores the deck list fetched from cloud. Key: playerNumber, Value: DeckDTO
        /// the reason why clientId is not used as key is because it can be changed when the player reconnects.
        /// </summary>
        private Dictionary<int, DeckDTO> m_playersDeckList = new();

        private Dictionary<int, List<int>> m_playersDeck = new();

        public async UniTask SetupPlayersDeck()
        {
            var isPlayerDeckListFetched = await FetchPlayerDeckListFromCloud();
            if (isPlayerDeckListFetched)
            {
                // first shuffle the deck (before shuffling, the deck is sorted by card id)
                ShufflePlayersDeckList();
                GeneratePlayersDeck();

                var clientIds = m_networkManager.ConnectedClientsIds;
                var copiedPlayersDeck = m_playersDeck.ToDictionary(playerDeck => playerDeck.Key,
                    playerDeck => new List<int>(playerDeck.Value));

                var copiedCardUIDs = new Dictionary<int, int[]>();
                var copiedCardIds  = new Dictionary<int, int[]>();

                // shuffle the copied deck list for prevent player from knowing the deck order
                foreach (var playerDeck in copiedPlayersDeck)
                {
                    playerDeck.Value.Shuffle();
                    copiedCardUIDs.Add(playerDeck.Key, new int[playerDeck.Value.Count]);
                    copiedCardIds.Add(playerDeck.Key, new int[playerDeck.Value.Count]);
                    for (var i = 0; i < playerDeck.Value.Count; i++)
                    {
                        copiedCardUIDs[playerDeck.Key][i] = playerDeck.Value[i];
                        copiedCardIds[playerDeck.Key][i] =
                            ServerCard.CardsCreatedThisGame[playerDeck.Value[i]].Data.CardId;
                    }
                }

                var sessionPlayers = SessionPlayerManager.Instance;
                foreach (var playerNumber in sessionPlayers.GetAllPlayerNumbers())
                {
                    var clientRpcParam = sessionPlayers.ClientRpcParams[playerNumber];
                    m_clientMyDeckManager.SetupMyDeckClientRpc(copiedCardUIDs[playerNumber],
                        copiedCardIds[playerNumber], clientRpcParam);
                    var opponentNumber = sessionPlayers.GetOpponentPlayerNumber(playerNumber);
                    m_clientOpponentDeckManager.SetupOpponentDeckClientRpc(copiedCardUIDs[opponentNumber],
                        clientRpcParam);
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
                var deck         = new List<int>();
                var playerNumber = pair.Key;

                foreach (var cardId in pair.Value.CardIds)
                {
                    // create instance of card data from card id
                    var cardData = Instantiate(m_cardDBManager.GetCardData(cardId));
                    var serverCard = new ServerCard(cardData)
                    {
                        Owner = playerNumber
                    };
                    deck.Add(serverCard.UID);
                }

                m_playersDeck.Add(playerNumber, deck);
            }
        }

        private async UniTask<bool> FetchPlayerDeckListFromCloud()
        {
            Debug.Log("Fetching player deck list from cloud...");

            // Get all connected player ids from session manager, need for fetching deck list from cloud
            var connectedPlayerIds = new List<string>();

            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerId = SessionPlayerManager.Instance.GetPlayerId(clientId);
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


#else
                var playerDecks = await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "GetPlayerDecks",
                    new Dictionary<string, object>
                    {
                        { "playerIds", connectedPlayerIds }
                    });

                Debug.Log($"Fetched player deck list from cloud: {playerDecks}");
                var playerDeckLists = JsonConvert.DeserializeObject<List<DeckDTO>>(playerDecks);
#endif

                // Store the deck list fetched from cloud
                for (var i = 0; i < m_networkManager.ConnectedClientsIds.Count; i++)
                {
                    var playerNumber =
                        SessionPlayerManager.Instance.GetPlayerNumber(m_networkManager.ConnectedClientsIds[i]);
                    m_playersDeckList.Add(playerNumber, playerDeckLists[i]);
                }

                return true;
            }
            catch (CloudCodeException exception)
            {
                Debug.LogError(exception.Message);
                return false;
            }
        }

        /// <summary>
        /// 각 player들의 모든 덱을 섞는다.
        /// </summary>
        private void ShufflePlayersDeckList()
        {
            foreach (var deckList in m_playersDeckList)
                for (var i = 0; i < deckList.Value.CardIds.Count; i++)
                    deckList.Value.CardIds.Shuffle();
        }

        public List<int> GetPlayerDeck(int playerNumber)
        {
            return m_playersDeck[playerNumber];
        }

        public bool IsCardLeftInDeck(int playerNumber)
        {
            return m_playersDeck[playerNumber].Count > 0;
        }
    }
}