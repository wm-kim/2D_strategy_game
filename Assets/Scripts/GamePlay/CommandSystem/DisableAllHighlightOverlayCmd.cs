using Minimax.GamePlay.GridSystem;

namespace Minimax.GamePlay.CommandSystem
{
    public class DisableAllHighlightOverlayCmd : Command
    {
        private ClientMap m_clientMap;

        public DisableAllHighlightOverlayCmd(ClientMap clientMap)
        {
            m_clientMap = clientMap;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMap.DisableHighlightCells();
            ExecutionComplete();
        }
    }
}