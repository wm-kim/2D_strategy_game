using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using Unity.Services.CloudCode;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;

namespace Minimax
{
    public class ServerPlayerDeckManager : NetworkBehaviour
    {
        [SerializeField] private CardDBManager m_cardDBManager;
        
        private NetworkManager m_networkManager => NetworkManager.Singleton;
        private Dictionary<ulong, DeckDTO> m_playerDeckLists = new Dictionary<ulong, DeckDTO>();
        
        [SerializeField, Tooltip("Key: ClientId, Value: Deck")]
        private SerializedDictionary<ulong, List<CardBaseData>> m_playerDecks = new SerializedDictionary<ulong, List<CardBaseData>>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                SetupPlayerDecks();
            }
            
            base.OnNetworkSpawn();
        }
        
        private async void SetupPlayerDecks()
        {
            var isPlayerDeckListFetched = await FetchPlayerDeckListFromCloud();
            var isDBLoaded = await m_cardDBManager.LoadDBCardsAsync();
            
            if (isPlayerDeckListFetched && isDBLoaded)
            {
                // first shuffle the deck
                ShufflePlayerDeckLists();
                GeneratePlayerDecksFromLists();
            }
        }

        private void GeneratePlayerDecksFromLists()
        {
            for (int i = 0; i < m_playerDeckLists.Count; i++)
            {
                var deck = new List<CardBaseData>();
                foreach (var cardId in m_playerDeckLists[m_networkManager.ConnectedClientsIds[i]].CardIds)
                {
                    deck.Add(m_cardDBManager.GetCardData(cardId));
                }
                m_playerDecks.Add(m_networkManager.ConnectedClientsIds[i], deck);
            }
        }

        private async UniTask<bool> FetchPlayerDeckListFromCloud()
        {
            DebugWrapper.Log("Fetching player deck list from cloud...");
            
            // Get all connected player ids from session manager
            List<string> connectedPlayerIds = new List<string>();
            
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerId = SessionManager<SessionPlayerData>.Instance.GetPlayerId(clientId);
                connectedPlayerIds.Add(playerId);
            }

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

                for (int i = 0; i < connectedPlayerIds.Count; i++)
                {
                    m_playerDeckLists.Add(m_networkManager.ConnectedClientsIds[i], playerDeckLists[i]);
                }
                
                return true;
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
                return false;
            }
        }
        
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
        private int[] CopyPlayerDeckListAndShuffle(ulong clientId)
        {
            var deckList = m_playerDeckLists[clientId].CardIds;
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
