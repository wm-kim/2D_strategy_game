using Minimax.GamePlay.PlayerHand;
using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class PlayMyUnitCardFromHandCommand : Command
    {
        private int m_unitUID;
        private int m_cardUID;
        private Vector2Int m_coord;
        private ClientMyHandManager m_clientMyHand;
        private ClientUnitManager m_clientUnitMananger;
        
        public PlayMyUnitCardFromHandCommand(int unitUID, int cardUID, Vector2Int coord,
            ClientMyHandManager clientMyHand, ClientUnitManager clientUnitManager)
        { 
            m_unitUID = unitUID;
            m_cardUID = cardUID;
            m_coord = coord;
            m_clientMyHand = clientMyHand;
            m_clientUnitMananger = clientUnitManager;
        }
        
        // TODO : May be spawning unit animation can be separated out
        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMyHand.PlayCardAndTween(m_cardUID);
            m_clientUnitMananger.SpawnUnit(m_unitUID, m_cardUID, m_coord);
        }
    }
}