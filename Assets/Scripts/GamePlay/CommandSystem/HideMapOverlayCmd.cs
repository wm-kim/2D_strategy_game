using Minimax.GamePlay.GridSystem;

namespace Minimax.GamePlay.CommandSystem
{
    public class HideMapOverlayCmd : Command
    {
        private ClientMap m_clientMap;
        
        public HideMapOverlayCmd(ClientMap clientMap)
        {
            m_clientMap = clientMap;
        }
        
        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMap.DisableHighlightCells();
            Command.ExecutionComplete();
        }
    }
}