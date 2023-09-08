using System;
using Minimax.CoreSystems;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.INetworkSerialize;
using Minimax.GamePlay.PlayerHand;
using Minimax.GamePlay.Unit;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Logic
{
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
            var clientRpcParams = GlobalManagers.Instance.Connection.ClientRpcParams;
            
            var playerNumber = GlobalManagers.Instance.Connection.GetPlayerNumber(senderClientId);
            var opponentPlayerNumber = GlobalManagers.Instance.Connection.GetOpponentPlayerNumber(senderClientId);
            
            m_serverPlayersHand.RemoveCardFromHand(playerNumber, cardUID);
            var serverUnit = new ServerUnit(cardUID);
            m_serverMap.PlaceUnitOnMap(serverUnit.UID, coord);
            
            var data = ServerCard.CardsCreatedThisGame[cardUID].Data;
            switch (data.GetCardType())
            {
                case CardType.Unit:
                    // prepare the unit data to send to opponent
                    var networkUnitData = new NetworkUnitCardData(data as UnitBaseData);
                    PlayMyUnitCardFromHandClientRpc(serverUnit.UID, cardUID, coord, clientRpcParams[senderClientId]);
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
        private void PlayMyUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, ClientRpcParams clientRpcParams = default)
        {
            new PlayMyUnitCardFromHandCommand(unitUID, cardUID, coord, m_clientMyHand, m_clientUnitManager).AddToQueue();
        }

        [ClientRpc]
        private void PlayOpponentUnitCardFromHandClientRpc(int unitUID, int cardUID, Vector2Int coord, NetworkUnitCardData unitData,
            ClientRpcParams clientRpcParams = default)
        {
            new PlayerOpponentUnitCardFromHandCommand(unitUID, cardUID, coord, unitData, m_clientOpponentHand, m_clientUnitManager).AddToQueue();
        } 
    }
}