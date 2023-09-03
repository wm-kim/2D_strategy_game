using Minimax.GamePlay.PlayerHand;

namespace Minimax.GamePlay.CommandSystem
{
    public class DrawACardCommand : Command
    {
        private int m_cardUID;
        private ClientPlayerHandManager m_clientPlayerHand;
        private ClientPlayerDeckManager m_clientPlayerDeck;
        
        public DrawACardCommand(int cardUID,
            ClientPlayerHandManager clientPlayerHand, 
            ClientPlayerDeckManager clientPlayerDeck)
        {
            m_cardUID = cardUID;
            m_clientPlayerHand = clientPlayerHand;
            m_clientPlayerDeck = clientPlayerDeck;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientPlayerDeck.RemoveCard(m_cardUID);
            m_clientPlayerHand.AddCard(m_cardUID);
        }
    }
}