using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;
using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class HighlightMovingPathCmd : Command
    {
        private int          m_unitUID;
        private Vector2Int[] m_pothCoords;
        private ClientMap    m_clientMap;

        public HighlightMovingPathCmd(int unitUID, Vector2Int[] pathCoords, ClientMap clientMap)
        {
            m_unitUID    = unitUID;
            m_pothCoords = pathCoords;
            m_clientMap  = clientMap;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMap.HighlightMovingPath(m_pothCoords);
            ExecutionComplete();
        }
    }
}