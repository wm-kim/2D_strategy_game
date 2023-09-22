using Minimax.GamePlay.INetworkSerialize;
using Minimax.GamePlay.PlayerHand;
using Minimax.ScriptableObjects.CardDatas;
using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class PlayMyUnitCardFromHandCmd : Command
    {
        private int m_unitUID;
        private int m_cardUID;
        private Vector2Int m_coord;
        private NetworkUnitCardData m_networkUnitData;
        private ClientMyHandManager m_clientMyHand;
        private ClientUnitManager m_clientUnitMananger;
        
        public PlayMyUnitCardFromHandCmd(int unitUID, int cardUID, Vector2Int coord, NetworkUnitCardData unitData,
            ClientMyHandManager clientMyHand, ClientUnitManager clientUnitManager)
        { 
            m_unitUID = unitUID;
            m_cardUID = cardUID;
            m_coord = coord;
            m_networkUnitData = unitData;
            m_clientMyHand = clientMyHand;
            m_clientUnitMananger = clientUnitManager;
        }
        
        // TODO : May be spawning unit animation can be separated out
        public override void StartExecute()
        {
            base.StartExecute();
            
            // set the updated card data 
            var clientCard = ClientCard.CardsCreatedThisGame[m_cardUID];
            var unitBaseData = UnitBaseData.CreateInstance(m_networkUnitData);
            clientCard.Data = unitBaseData;
            
            m_clientMyHand.PlayCardAndTween(m_cardUID);
            m_clientUnitMananger.SpawnUnit(m_unitUID, m_cardUID, m_coord);
        }
    }
}