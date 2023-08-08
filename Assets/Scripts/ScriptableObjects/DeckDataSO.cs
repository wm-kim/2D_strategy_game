using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Deck Data", menuName = "ScriptableObjects/DeckDataSO")]
    public class DeckDataSO : ScriptableObject
    {
        private Dictionary<int, CardBaseData> m_deckList  = new Dictionary<int, CardBaseData>();

        public bool ContainsCard(int cardId)
        {
            return m_deckList.ContainsKey(cardId);
        }

        public void AddCard(CardBaseData cardData)
        {
            if (!m_deckList.ContainsKey(cardData.CardId))
                m_deckList.Add(cardData.CardId, cardData);
            else DebugWrapper.LogError("DeckDataSO: " + name + " already contains card " + cardData.CardId + ".");
        }

        public void RemoveCard(int cardId)
        {
            m_deckList.Remove(cardId);
        }

        public Dictionary<int, CardBaseData> GetDeckList() => m_deckList;
        
    }
}
