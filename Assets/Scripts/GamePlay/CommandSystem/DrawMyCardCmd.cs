using Minimax.GamePlay.PlayerDeck;
using Minimax.GamePlay.PlayerHand;

namespace Minimax.GamePlay.CommandSystem
{
    public class DrawMyCardCmd : Command
    {
        private int                 m_cardUID;
        private ClientMyHandManager _mClientMyHandData;
        private ClientMyDeckManager m_clientMyDeck;

        public DrawMyCardCmd(int cardUID,
            ClientMyHandManager clientMyHandData,
            ClientMyDeckManager clientMyDeck)
        {
            m_cardUID          = cardUID;
            _mClientMyHandData = clientMyHandData;
            m_clientMyDeck     = clientMyDeck;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMyDeck.RemoveCard(m_cardUID);
            _mClientMyHandData.AddCardAndTween(m_cardUID);
        }
    }
}