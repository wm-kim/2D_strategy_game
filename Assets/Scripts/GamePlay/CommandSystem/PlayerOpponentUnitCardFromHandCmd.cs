using Minimax.GamePlay.INetworkSerialize;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class PlayerOpponentUnitCardFromHandCmd : Command
    {
        private int                       m_unitUID;
        private int                       m_cardUID;
        private Vector2Int                m_coord;
        private NetworkUnitCardData       m_networkUnitData;
        private ClientOpponentHandManager m_clientOpponentHand;
        private ClientUnitManager         m_clientUnitManager;

        public PlayerOpponentUnitCardFromHandCmd(int unitUID, int cardUID, Vector2Int coord,
            NetworkUnitCardData unitData,
            ClientOpponentHandManager clientOpponentHand, ClientUnitManager clientUnitManager)
        {
            m_unitUID            = unitUID;
            m_cardUID            = cardUID;
            m_coord              = coord;
            m_networkUnitData    = unitData;
            m_clientOpponentHand = clientOpponentHand;
            m_clientUnitManager  = clientUnitManager;
        }

        public override void StartExecute()
        {
            base.StartExecute();

            // revealing the opponent card
            SetOpponentCardData();
            m_clientOpponentHand.PlayCardAndTween(m_cardUID);
            m_clientUnitManager.SpawnUnit(m_unitUID, m_cardUID, m_coord);
        }

        private void SetOpponentCardData()
        {
            var clientCard   = ClientCard.CardsCreatedThisGame[m_cardUID];
            var unitBaseData = UnitBaseData.CreateInstance(m_networkUnitData);
            clientCard.Data = unitBaseData;
        }
    }
}