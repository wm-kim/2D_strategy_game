using Minimax.GamePlay.Unit;

namespace Minimax.GamePlay.CommandSystem
{
    public class SetUnitMovableCmd : Command
    {
        private int m_unitUID;
        private bool m_isMovable;
        
        public SetUnitMovableCmd(int unitUID, bool isMovable)
        {
            m_unitUID = unitUID;
            m_isMovable = isMovable;
        }
        
        public override void StartExecute()
        {
            base.StartExecute();
            var unit = ClientUnit.UnitsCreatedThisGame[m_unitUID];
            unit.IsMovable = m_isMovable;
            Command.ExecutionComplete();
        }
    }
}