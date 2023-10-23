namespace Minimax.GamePlay.CommandSystem
{
    public class DrawOpponentCardCmd : Command
    {
        private int m_cardUID;
        private ClientOpponentHandManager m_clientOpponentHand;
        private ClientOpponentDeckManager m_clientOpponentDeck;
        
        public DrawOpponentCardCmd(int cardUID,
            ClientOpponentHandManager clientOpponentHand, 
            ClientOpponentDeckManager clientOpponentDeck)
        {
            m_cardUID = cardUID;
            m_clientOpponentHand = clientOpponentHand;
            m_clientOpponentDeck = clientOpponentDeck;
        }
        
        public override void StartExecute()
        {
            base.StartExecute();
            m_clientOpponentDeck.RemoveCard(m_cardUID);
            m_clientOpponentHand.AddCardAndTween(m_cardUID);
        }
    }
}