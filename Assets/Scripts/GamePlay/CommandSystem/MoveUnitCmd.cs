using Minimax.GamePlay.GridSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.GamePlay.CommandSystem
{
    public class MoveUnitCmd : CompositeCmd
    {
        private int m_unitUID;
        private Vector2Int[] m_path;
        private ClientUnitManager m_clientUnitManager;
        private ClientMap m_clientMap;
        
        public MoveUnitCmd(int unitUID, Vector2Int[] path, ClientUnitManager clientUnitManager, ClientMap clientMap)
        {
            m_unitUID = unitUID;
            m_path = path;
            m_clientUnitManager = clientUnitManager;
            m_clientMap = clientMap;
            
            var destCoord = m_path[^1];
            AddSubCommand(new SetUnitMovableCmd(m_unitUID, false));
            AddSubCommand(new DisableAllHighlightOverlayCmd(m_clientMap));
            AddSubCommand(new HighlightMovingPathCmd(m_unitUID, m_path, m_clientMap));
            for (int i = 0; i < m_path.Length; i++)
            {
                var moveUnitOneCellCmd = new MoveUnitOneCellCmd(m_unitUID, m_path[i], m_clientUnitManager);
                AddSubCommand(moveUnitOneCellCmd);
            }
            AddSubCommand(new DisableAllHighlightOverlayCmd(m_clientMap));
            AddSubCommand(new HighlightReachableCmd(m_unitUID, m_clientMap));
            AddSubCommand(new SetUnitMovableCmd(m_unitUID, true));
        }
        
        public override void StartExecute()
        {
            base.StartExecute();
            for (int i = 0; i < m_path.Length; i++)
            {
                var moveUnitOneCellCmd = new MoveUnitOneCellCmd(m_unitUID, m_path[i], m_clientUnitManager);
                AddSubCommand(moveUnitOneCellCmd);
            }
            Command.ExecutionComplete();
        }
    }
}