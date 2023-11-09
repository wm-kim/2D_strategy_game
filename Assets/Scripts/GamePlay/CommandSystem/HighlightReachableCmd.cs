using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;

namespace Minimax.GamePlay.CommandSystem
{
    public class HighlightReachableCmd : Command
    {
        private int       m_unitUID;
        private ClientMap m_clientMap;

        public HighlightReachableCmd(int unitUID, ClientMap clientMap)
        {
            m_unitUID   = unitUID;
            m_clientMap = clientMap;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            var clientUnit = ClientUnit.UnitsCreatedThisGame[m_unitUID];
            var unitCell   = m_clientMap[clientUnit.Coord];
            m_clientMap.HighlightMovableCells(unitCell, clientUnit.MoveRange);
            ExecutionComplete();
        }
    }
}