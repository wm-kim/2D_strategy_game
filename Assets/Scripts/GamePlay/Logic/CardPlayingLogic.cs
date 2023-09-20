using System;
using Minimax.CoreSystems;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.INetworkSerialize;
using Minimax.GamePlay.PlayerHand;
using Minimax.GamePlay.Unit;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for generating Card Playing Commands and sending them to the clients.
    /// </summary>
    public class CardPlayingLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private ServerPlayersHandManager m_serverPlayersHand;
        [SerializeField] private ServerMap m_serverMap;
        
        [Header("Client References")]
        [SerializeField] private ClientMyHandManager m_clientMyHand;
        [SerializeField] private ClientOpponentHandManager m_clientOpponentHand;
        [SerializeField] private ClientUnitManager m_clientUnitManager;
        
        [ServerRpc(RequireOwnership = false)]
        public void CommandPlayACardFromHandServerRpc(int cardUID, Vector2Int coord, ServerRpcParams serverRpcParams = default)
        {
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var sessionPlayers = SessionPlayerManager.Instance;
            var clientRpcParams = sessionPlayers.ClientRpcParams;
            
            var playerNumber = sessionPlayers.GetPlayerNumber(senderClientId);
            var opponentPlayerNumber = sessionPlayers.GetOpponentPlayerNumber(senderClientId);
            
            m_serverPlayersHand.RemoveCardFromHand(playerNumber, cardUID);
            var serverUnit = new ServerUnit(cardUID, coord);
            m_serverMap.PlaceUnitOnMap(serverUnit.UID, coord);
            
            var cardBaseData = ServerCard.CardsCreatedThisGame[cardUID].Data;
            switch (cardBaseData.GetCardType())
            {
                case CardType.Unit:
                    // prepare the unit data to send to opponent
                    var networkUnitData = new NetworkUnitCardData(cardBaseData as UnitBaseData);
                    PlayMyUnitCardFromHandClientRpc(serverUnit.UID, cardUID, coord, networkUnitData, clientRpcParams[senderClientId]);
                    PlayOpponentUnitCardFromHandClientRpc(serverUnit.UID, cardUID, coord, networkUnitData, clientRpcParams[opponentPlayerNumber]);
                    break;
                case CardType.Special:
                    break;
                case CardType.Undefined:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [ClientRpc]
        private void PlayMyUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, 
            NetworkUnitCardData unitData,
            ClientRpcParams clientRpcParams = default)
        {
            new PlayMyUnitCardFromHandCmd(unitUID, cardUID, coord, unitData, m_clientMyHand, m_clientUnitManager).AddToQueue();
        }

        // TODO : instead of passing in whole unit data, pass in only card id and let the client fetch the data from card db can be enough
        // But If there is change in card data on server, it will not be reflected on client so it is better to pass in the whole data
        [ClientRpc]
        private void PlayOpponentUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, 
            NetworkUnitCardData unitData,
            ClientRpcParams clientRpcParams = default)
        {
            new PlayerOpponentUnitCardFromHandCmd(unitUID, cardUID, coord, unitData, m_clientOpponentHand, m_clientUnitManager).AddToQueue();
        } 
    }
}