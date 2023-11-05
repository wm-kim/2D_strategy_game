using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class MoveUnitOneCellCmd : Command
    {
        private int               m_unitUID;
        private Vector2Int        m_destCoord;
        private ClientUnitManager m_clientUnitManager;

        public MoveUnitOneCellCmd(int unitUID, Vector2Int destCoord, ClientUnitManager clientUnitManager)
        {
            m_unitUID           = unitUID;
            m_destCoord         = destCoord;
            m_clientUnitManager = clientUnitManager;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientUnitManager.MoveUnitOneCell(m_unitUID, m_destCoord);
        }
    }
}