using System;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.INetworkSerialize;
using Minimax.GamePlay.PlayerHand;
using Minimax.GamePlay.Unit;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.UnityGamingService.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.Logic
{
    // Encapsulate data related to card playing
    public struct CardPlayData
    {
        public int CardUID { get; set; }
        public int UnitUID { get; set; }
        public int PlayerNumber { get; set; }
        public Vector2Int Coord { get; set; }
    }
    
    /// <summary>
    /// Responsible for generating Card Playing Commands and sending them to the clients.
    /// </summary>
    public class CardPlayingLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private ServerPlayersHandManager m_serverPlayersHand;
        [SerializeField] private ServerMap m_serverMap;
        [SerializeField] private ServerManaManager m_serverManaManager; 
        [SerializeField] private TurnManager m_turnManager;
        
        [Header("Client References")]
        [SerializeField] private ClientMyHandDataManager m_clientMyHand;
        [SerializeField] private ClientOpponentHandDataManager m_clientOpponentHand;
        [SerializeField] private ClientUnitManager m_clientUnitManager;
        
        [ServerRpc(RequireOwnership = false)]
        public void CommandPlayACardFromHandServerRpc(int cardUID, Vector2Int coord, ServerRpcParams serverRpcParams = default)
        {
            var sessionPlayers = SessionPlayerManager.Instance;
            
            var cardPlayData = new CardPlayData
            {
                CardUID = cardUID,
                Coord = coord,
                PlayerNumber = sessionPlayers.GetPlayerNumber(serverRpcParams.Receive.SenderClientId)
            };
            
            if (!ServerCheckIsCardPlayable(cardPlayData, cardUID)) return;
            ProcessCardPlayOnServer(cardPlayData);
        }
        
        private bool ServerCheckIsCardPlayable(CardPlayData cardPlayData, int cardUID)
        {
            // 플레이어의 턴인지 확인합니다.
            m_turnManager.CheckIfPlayerTurn(cardPlayData.PlayerNumber);

            // 셀이 플레이어에 의해 배치 가능한지 확인합니다.
            if (!IsCellPlaceableByPlayer(cardPlayData.Coord, cardPlayData.PlayerNumber)) return false;
            
            // 마나 비용을 확인하고 사용합니다.
            var manaCost = ServerCard.CardsCreatedThisGame[cardUID].Data.Cost;
            if (!m_serverManaManager.TryConsumeMana(cardPlayData.PlayerNumber, manaCost)) return false;
            
            return true;
        }

        private bool IsCellPlaceableByPlayer(Vector2Int coord, int playerNumber)
        {
            var serverCell = m_serverMap[coord];
            return serverCell.CheckIfPlaceableBy(playerNumber);
        }

        private void ProcessCardPlayOnServer(CardPlayData cardPlayData)
        {
            m_serverPlayersHand.RemoveCardFromHand(cardPlayData.PlayerNumber, cardPlayData.CardUID);
            var serverUnit = new ServerUnit(cardPlayData.CardUID, cardPlayData.Coord);
            m_serverMap.PlaceUnitOnMap(serverUnit.UID, cardPlayData.Coord);
            
            // Assign the generated UID to our data object
            cardPlayData.UnitUID = serverUnit.UID;

            CommandBasedOnCardType(cardPlayData);
        }
        
        private void CommandBasedOnCardType(CardPlayData cardPlayData)
        {
            var clientRpcParams = SessionPlayerManager.Instance.ClientRpcParams;
            var cardBaseData = ServerCard.CardsCreatedThisGame[cardPlayData.CardUID].Data;
            var opponentPlayerNumber = SessionPlayerManager.Instance.GetOpponentPlayerNumber(cardPlayData.PlayerNumber);
            
            switch (cardBaseData.GetCardType())
            {
                case CardType.Unit:
                    // prepare the unit data to send to opponent
                    var networkUnitData = new NetworkUnitCardData(cardBaseData as UnitBaseData);
                    PlayMyUnitCardFromHandClientRpc(cardPlayData.UnitUID, cardPlayData.CardUID, cardPlayData.Coord, networkUnitData, clientRpcParams[cardPlayData.PlayerNumber]);
                    PlayOpponentUnitCardFromHandClientRpc(cardPlayData.UnitUID, cardPlayData.CardUID, cardPlayData.Coord, networkUnitData, clientRpcParams[opponentPlayerNumber]);
                    break;
                case CardType.Special:
                    break;
                case CardType.Undefined:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [ClientRpc]
        private void PlayMyUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, NetworkUnitCardData unitData, ClientRpcParams clientRpcParams = default)
        {
            new PlayMyUnitCardFromHandCmd(unitUID, cardUID, coord, unitData, m_clientMyHand, m_clientUnitManager).AddToQueue();
        }

        // TODO : instead of passing in whole unit data, pass in only card id and let the client fetch the data from card db can be enough
        // But If there is change in card data on server, it will not be reflected on client so it is better to pass in the whole data
        [ClientRpc]
        private void PlayOpponentUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, NetworkUnitCardData unitData, ClientRpcParams clientRpcParams = default)
        {
            new PlayerOpponentUnitCardFromHandCmd(unitUID, cardUID, coord, unitData, m_clientOpponentHand, m_clientUnitManager).AddToQueue();
        } 
    }
}