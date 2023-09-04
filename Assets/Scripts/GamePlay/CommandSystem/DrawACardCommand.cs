using Minimax.GamePlay.PlayerHand;

namespace Minimax.GamePlay.CommandSystem
{
    public class DrawACardCommand : Command
    {
        private int m_cardUID;
        private ClientMyHandManager m_clientMyHand;
        private ClientMyDeckManager m_clientMyDeck;
        
        public DrawACardCommand(int cardUID,
            ClientMyHandManager clientMyHand, 
            ClientMyDeckManager clientMyDeck)
        {
            m_cardUID = cardUID;
            m_clientMyHand = clientMyHand;
            m_clientMyDeck = clientMyDeck;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            m_clientMyDeck.RemoveCard(m_cardUID);
            m_clientMyHand.AddCard(m_cardUID);
        }
    }
}