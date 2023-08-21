using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Minimax.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DeckDtoCollectionSO")]
    public class DeckDtoCollectionSO : ScriptableObject
    {
        private Dictionary<int, DeckDTO> m_decks;

        public Dictionary<int, DeckDTO> Decks
        {
            get => m_decks;
            set 
            {
                m_decks?.Clear();
                m_decks = value; 
            }
        }

        /// <summary>
        /// 가장 최근에 수정되거나 생성된 덱의 Id를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public int GetRecentDeckId()
        {
            DeckDTO recentDeck = null;
            
            foreach (var deck in m_decks.Values)
            {
                if (recentDeck == null || deck.DateModified > recentDeck.DateModified)
                    recentDeck = deck;
            }
            
            if (recentDeck == null)
            {
                Debug.LogError("empty deck collection, so can not get recent deck id");
                return -1;
            }
            
            return recentDeck.Id;
        }
    }
}