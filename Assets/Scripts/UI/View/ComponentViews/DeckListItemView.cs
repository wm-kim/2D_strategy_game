using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using TMPro;
using UnityEngine;

namespace Minimax
{
    public class DeckListItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_cardDataText;
        private CardBaseData m_cardData;
        
        public void SetData(CardBaseData cardData)
        {
            m_cardData = cardData;
            SetView(cardData.CardId);
        }
        
        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }
    }
}
