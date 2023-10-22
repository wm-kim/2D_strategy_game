using System.Collections.Generic;
using Minimax.GamePlay.PlayerHand;

namespace Minimax.GamePlay.CommandSystem
{
    public class DrawAllPlayerInitialCardsCmd : Command
    {
        private int[] m_myCardUIDs;
        private int[] m_opponentCardUIDs;
        private ClientMyHandDataManager _mClientMyHandData;
        private ClientOpponentHandDataManager m_clientOpponentHand;
        private ClientMyDeckManager m_clientMyDeck;
        private ClientOpponentDeckManager m_clientOpponentDeck;
        
        public DrawAllPlayerInitialCardsCmd(int[] myCardUIDs,
            int[] opponentCardUIDs,
            ClientMyHandDataManager clientMyHandData, 
            ClientOpponentHandDataManager clientOpponentHand,
            ClientMyDeckManager clientMyDeck,
            ClientOpponentDeckManager clientOpponentDeck)
        {
            m_myCardUIDs = myCardUIDs;
            m_opponentCardUIDs = opponentCardUIDs;
            _mClientMyHandData = clientMyHandData;
            m_clientOpponentHand = clientOpponentHand;
            m_clientMyDeck = clientMyDeck;
            m_clientOpponentDeck = clientOpponentDeck;
        }

        public override void StartExecute()
        {
            base.StartExecute();
            foreach (var cardUID in m_myCardUIDs) m_clientMyDeck.RemoveCard(cardUID);
            foreach (var cardUID in m_opponentCardUIDs) m_clientOpponentDeck.RemoveCard(cardUID);
            
            _mClientMyHandData.AddInitialCardsAndTween(m_myCardUIDs);
            m_clientOpponentHand.AddInitialCardsAndTween(m_opponentCardUIDs);
        }
    }
}