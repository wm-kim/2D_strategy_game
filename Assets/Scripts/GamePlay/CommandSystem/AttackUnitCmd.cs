using Minimax.GamePlay.Unit;

namespace Minimax.GamePlay.CommandSystem
{
    public class AttackUnitCmd : Command
    {
        private int               m_attackerUID;
        private int               m_targetUnitUID;
        private ClientUnitManager m_clientUnitManager;

        public AttackUnitCmd(int attackerUID, int targetUnitUID, ClientUnitManager clientUnitManager)
        {
            m_attackerUID       = attackerUID;
            m_targetUnitUID     = targetUnitUID;
            m_clientUnitManager = clientUnitManager;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientUnitManager.AttackUnit(m_attackerUID, m_targetUnitUID);
            ExecutionComplete();
        }
    }
}