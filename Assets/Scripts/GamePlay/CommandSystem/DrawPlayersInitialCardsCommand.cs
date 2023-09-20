using System.Collections.Generic;
using Minimax.GamePlay.PlayerHand;

namespace Minimax.GamePlay.CommandSystem
{
    public class DrawAllPlayerInitialCardsCmd : Command
    {
        private int[] m_myCardUIDs;
        private int[] m_opponentCardUIDs;
        private ClientMyHandManager m_clientMyHand;
        private ClientOpponentHandManager m_clientOpponentHand;
        private ClientMyDeckManager m_clientMyDeck;
        private ClientOpponentDeckManager m_clientOpponentDeck;
        
        public DrawAllPlayerInitialCardsCmd(int[] myCardUIDs,
            int[] opponentCardUIDs,
            ClientMyHandManager clientMyHand, 
            ClientOpponentHandManager clientOpponentHand,
            ClientMyDeckManager clientMyDeck,
            ClientOpponentDeckManager clientOpponentDeck)
        {
            m_myCardUIDs = myCardUIDs;
            m_opponentCardUIDs = opponentCardUIDs;
            m_clientMyHand = clientMyHand;
            m_clientOpponentHand = clientOpponentHand;
            m_clientMyDeck = clientMyDeck;
            m_clientOpponentDeck = clientOpponentDeck;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            foreach (var cardUID in m_myCardUIDs) m_clientMyDeck.RemoveCard(cardUID);
            foreach (var cardUID in m_opponentCardUIDs) m_clientOpponentDeck.RemoveCard(cardUID);
            
            m_clientMyHand.AddInitialCardsAndTween(m_myCardUIDs);
            m_clientOpponentHand.AddInitialCardsAndTween(m_opponentCardUIDs);
        }
    }
}